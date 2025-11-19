## Useful Files For Your Repository: .NET Edition 

This post is a follow-up to my earlier post, [Useful Metadata Files For Your Repository](#). While that post tried its best to be language and tooling-agnostic, this edition specialises in lesser known utilities for .NET developers. 

## global.json

For .NET Core 3.1 SDK and later, the global.json file contains a definition for which SDK versions should be used when using the `dotnet` CLI command. 

As the name implies, the file contains json data (specifically, JSON with comments), which must follow a specific schema. 

The below example dictates that within the directories `src/` and `test/`, the .NET SDK used must be version 8.0.300 exactly. 

```json
{
  "sdk": {
    "version": "8.0.300",
    "rollForward": "disable",
    "allowPrerelease": false,
    "paths": ["src/", "test/"],
    "errorMessage": "A suitable .NET SDK could not be found"
  }
}
```

## Directory.Build.props, Directory.Build.targets

`Directory.Build.props` and `Directory.Build.targets` are XML files, much like a `.csproj` file, which contain additional information for C# projects in a given directory and its subdirectories. In these files, you can specify just about anything you'd specify in a csproj file. 

The difference between them is the time at which they're loaded: `Directory.Build.props` is loaded early, and its settings can be overwritten by a `.csproj` file, while `Directory.Build.targets` is loaded later, and overwrites rules in a `.csproj`. One could think of `.props` more as suggestions, and `.targets` as strict rules. 

## MSBuild.rsp, Directory.Build.rsp

These response files contain CLI arguments to be passed automatically to MSBuild when it's called. 

When `MSBuild.exe` is run, it will automatically load in `MSBuild.rsp` from the current directory. Since MSBuild version 15.6, it will also search for `Directory.Build.rsp` in parent directories, which follows the same format. 

For example, to always show the most verbose logs possible, but disable the startup banner and copyright message, one might use:

```shell
-verbosity:diagnostic
-nologo
```

To ignore these response files when running MSBuild, the `-noAutoResponse` (or `-noautorsp`) flag can be passed. 

## Directory.Pakages.props 

`Directory.Pakages.props` is an alternative to using `<PackageReference />` in a `.csproj` file (or `packages.config` for older projects). Since NuGet 6.2, dependeniees can be managed centrally using a single file. 

In truth, this file applies to all MSBuild projects which can use a `<PackageReference />`, even retrospectively. So long as compatible tooling is used, one could use this file with legacy `.csproj` files, VB or F# projects, and more.

This file can be created automatically form a template using `dotnet new packagesprops`. The contents should look something like this: 

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.1" />
  </ItemGroup>
</Project>
```

It's important to note that this file's rules don't cascade. Once one `Directory.Pakages.props` file is found, no more in higher directories are considered. 

It's also worth mentioning that this file is often largely redundant for smaller repositories. Well-architected small solutions often don't require multiple projects to share references in this way. Try not to use this file just for the sake of it. 

## nuget.config 

The `nuget.config` file is an XML file that contains configuration information for the NuGet package manager. In its most basic form, it looks like this: 

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="repositorypath" value="packages" />
  </config>
  <!-- other things here -->
</configuration>
```

This file can define custom package sources, to save developers the effort of setting up new custom sources after cloning the repository for the first time. 

```xml
  <packageSources>
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
    <add key="Custom" value="https://my.custom.nuget.feed.xyz/v3/index.json" />
  </packageSources>
```

Additionally, `<clear />` can be used to clear any system-wide package sources, ensuring that there are no conflicts, and saving developers the trouble of having to sign into, disable, or skip unused sources. 

```xml
  <packageSources>
    <clear />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
```

Similarly, it can defend against the user disabling sources that are explicitly included in the file. 

```xml
  <disabledPackageSources>
    <clear />
  </disabledPackageSources>
```

Package source mapping can be used to hint as to which packages should be retrieved from which sources. This can help resolve conflicts and reduce redundant requests. 

```xml
  <packageSourceMapping>
    <packageSource key="nuget">
      <package pattern="*" />
    </packageSource>
    <packageSource key="Custom">
      <package pattern="My.Custom.Package" />
    </packageSource>
  </packageSourceMapping>
```

## Honourable Mention: Dockerfile, .dockerignore

While Docker isn't strictly to do with the .NET ecosystem, it can still be a powerful tool to avoid the "it works on my machine" problem, as is one of the benefits of many of the files listed above. 

While it won't enable MacOS and Linux users to run projects that rely on Windows-specific functionality (because Windows containers can only be run on Windows machines), it can still help to manage versions and environments, and it's worth considering especially for web and API projects. 

## Further Reading 

For global.json, consider the [global.json overview](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json) for more information about the schema. 

For Directory.Build files, [Customize the build by folder](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory) contains significantly more information than the brief overview in this article. 

For autoresponse files, [MSBuild response files](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-response-files) describes the function of the files, while [MSBuild command-line reference](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-command-line-reference) contains an exhasustive list of the options available. 

For Directory.Packages.Props, [Central Package Management (CPM)](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management) also explains some more in-depth options not touched on by this post. 

For nuget.config, the [nuget.config reference](https://learn.microsoft.com/en-us/nuget/reference/nuget-config-file) describes a variety of options beyond just configuring sources. 

Finally, for those wanting to try Docker, [Tutorial: Containerize a .NET app](https://learn.microsoft.com/en-us/dotnet/core/docker/build-container) is a good starting point. 