--- 
title: The complete guide to hosting Blazor WebAssembly apps on GitHub Pages 
---

Blazor WASM is probably one of my favourite technologies of all time. As a developer, it's great to be able to use C# and HTML, rather than JavaScript or XAML; from a UX and accessibility perspective, I love the browser as a widely available and frictionless platform for application delivery; and finally, as a cheapskate, it's great to be able to host my apps for free. 

That said, it's not so simple as pointing GitHub pages at your repo and calling it a day. There are several steps required to get a Blazor WASM app fully functional when hosting on GitHub pages. 

We'll start under the assumption that you already have a Blazor WASM app and a GitHub repo, and that your repo is configured for Pages to be deployed via pipeline. Setting that up, as well as setting up custom DNS for GitHub Pages, is beyond the scope of this post. 

## 1. Modify your &lt;base /&gt; tag (for \*.github.io/\* only)

If you're hosting your site at a path that isn't the root domain - for example, on the default GitHub Pages URI of https://yourname.github.io/your-project, you'll need to update the `<base />` tag in `wwwroot/index.html`. This tag informs the browser how to resolve URIs which don't start with `/` or `./`. 

In the `<head>` section of `index.html`, add or modify the following line. Note that the "/" at the end of the string is critically important. 
```html
<base href="/your-project-name/" />
```

NOTE: If you do this, it will break your website when running locally. You might want to consider writing a custom compiler target. 

## 2. Modify other hrefs to work with the &lt;base /&gt; tag (for \*.github.io/\* only)

After changing the base URI to something other than just the domain, you'll need to ensure that all hrefs are built to compensate for the fact that they're now different. 

* Relative paths should be phrased as `href="./path-to-some-page"`
* Absolute paths should be phrased as `href="path-to-some-page"`
* Paths beginning with `/` should generally be avoided. 

## 3. Add rafgraph's Blazor WASM routing fix 

Blazor WASM is made for single-page applications. Navigation is handled internally, and navigating directly to specific pages isn't supported. This is true of running locally as well, using the Kestrel server that's launched when running the project. If your app is truly a SPA, you're fine to skip this step, but the majority of sites probably want this feature. 

GitHub user rafgraph kindly shares their solution in [a repository](https://github.com/rafgraph/spa-github-pages) shared under the MIT license. 

First, add the following script into `wwwroot/404.html`. If you're using a GitHub pages URI, set pathSegmentsToKeep to 1. If using a custom domain, set it to 0. 

```js
var pathSegmentsToKeep = 1;

var l = window.location;

l.replace(
l.protocol + '//' + l.hostname + (l.port ? ':' + l.port : '') +
l.pathname.split('/').slice(0, 1 + pathSegmentsToKeep).join('/') + '/?/' +
l.pathname.slice(1).split('/').slice(pathSegmentsToKeep).join('/').replace(/&/g, '~and~') +
(l.search ? '&' + l.search.slice(1).replace(/&/g, '~and~') : '') +
l.hash);
```

Then, add the following script to `wwwroot/index.html`. 

```js
(function(l) {
if (l.search[1] === '/' ) {
var decoded = l.search.slice(1).split('&').map(function(s) { 
return s.replace(/~and~/g, '&')
}).join('?');
window.history.replaceState(null, null,
    l.pathname.slice(0, -1) + decoded + l.hash
    );
}
}(window.location))
```

## 4. Add robots.txt and sitemap.txt

In 2023, Google updated their crawlers. Previously, they would follow `404.html`'s redirect and index the page at which they arrived and index the page as if they'd received a HTTP 301 redirect. Now, they recognise the HTTP 404 status code sent by the web server, and don't bother crawling the sent page or waiting for the redirect. 

To get around this, you'll need to add the following line to `wwwroot/robots.txt` to point crawlers to the sitemap. 

```
Sitemap: https://yourname.github.io/your-project/sitemap.txt
```

Then, add routes you'd like indexed to `wwwroot/sitemap.txt`. This can be  time-consuming to do by hand, so you might want to automate it, e.g. by running the following snippet after compilation, which will add all non-parameterised routes which were declared using `@page` directives. 

```bash
grep '@page\s+"[^{]*"' | awk '{print $2}' >> publish/wwwroot/sitemap.txt
```

## 5. Add a pipeline to deploy to pages

You'll need to define a new workflow in `.github/workflows/deploy.yml` (you can call the file whatever you like, so long as it's a `.yml` file in `.github/workflows/`).

First, some setup. We'll need at least one trigger. If working in the "gitflow" style, this is usually any push to the `main` branch: 

```yml
on:
  push:
    branches:
      - main
```

We'll also need to declare our intent to publish to Pages. 
```yml
permissions:
  contents: read
  pages: write
  id-token: write
```

And finally, we'll need to add a concurrency group. This essentially prevents us from deploying twice to the same site at the same time. 
```yml
concurrency:
  group: "pages"
  cancel-in-progress: false
```

Now, on with the job! In this post, we'll use the `windows-latest` environment, because we rely on `xcopy` later down the line. Feel free to use a different image if you are comfortable adjusting subsequent steps. 

```yml
jobs:
  build:
    runs-on: windows-latest
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
```

Now for our steps. These should be a child of build, per the snippet shown below, but we'll skip the top part in subsequent steps to minimise repeating ourselves. 

```yml
jobs:
  build:
    runs-on: windows-latest
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}

    steps:
    - name: Checkout code
      uses: actions/checkout@v4

    # ...
```

We'll need to check out the codebase and set up .NET. I'm using .NET 10.0.100 (10.x) - you might need to change this depending on the version you're using. 

```yml
    steps:
    - name: Checkout code
      uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 10.x
```

This next step is optional, but recommended. The `wasm-tools` workload adds more aggressive tree-shaking, which reduces the size of your deployed artifacts and makes delivery to the end user faster. 

```yml
    - name: Use wasm-tools for treeshake
      run: |
        dotnet workload install wasm-tools
```

Next, we'll need to publish the site. We'll put the output in a folder called `publish`. Remember to modify the path to point to your WASM project. 

```yml
    - name: Publish project
      run: |
        dotnet publish src/YourSite/YourSite.csproj -c Release -o publish
```

Finally, we'll copy and publish the output to GitHub pages. 

```yml
    - name: Copy wwwroot to expected output folder
      run: |
        mkdir _site
        xcopy publish\wwwroot _site /E /I /Y

    - name: Upload artifact for GitHub Pages
      uses: actions/upload-pages-artifact@v3
      with:
        path: _site

    - name: Deploy to GitHub Pages
      id: deployment
      uses: actions/deploy-pages@v4
```