---
title: Useful Metadata Files For Your Repository 
---

There are many more files than README.md to provide valuable metadata about a repository, but a surprising number of developers and organisations never consider them. This post is walkthrough of more niche language-agnostic metadata files which might prove valuable in your repos. 

## .gitattributes 

Alright, I know I *just* said that this was going to be about more niche files, but you'd be astounded at the number of career developers I hear lament about problems that could easily be solved with a good .gitattributes file. 

The .gitattributes file tells git how to treat files when creating and committing. 

Probably the most widely used function of the .gitattributes file is to manage line endings in the repository. Many developers have known the pains of managing line endings across multiple systems. The wrong kind of line endings can not only cause massive diffs where you've only changed a few lines, it can also lead to unexpected behaviours when interpreted by some programs. 

To instruct git on handling line endings for a file, add a line in .gitattributes with a file pattern, and one or more macros or key-value pairs. For example, let's tell git to treat all .plist files (iOS app manifest files) as text files with unix-style line endings: 

```
*.plist text eol=lf
```

This is just the beginning of the power of .gitattributes files - they can also hint at how to treat diffs and merges, and be configured to manage git LFS, GitHub Linguist, ignore files during export, and more. 

A warning against treating all files the same: changing line endings in binary files, such as images, can corrupt them in ways that are difficult to fix. Use the binary macro on assets and other binaries to be sure that this never happens. 

A deep dive into .gitattributes will be published at a later date. 

## CODEOWNERS 

Stored at either `/CODEOWNERS` or `/docs/CODEOWNERS`, this file declares 0 or more accounts or email addresses who "own" each file in a repository. This can mean different things depending on project and organisation, but is generally used to require approval from a certain number of codeowners for each file changed in a pull request. 

Its format is simple: a file pattern, followed by a github username (prefixed with @) or email address. 

```
docs/*.md someone@example.com
src/frontend someone.else@example.com
```

CODEOWNERS integrates well with GitHub, and has a convenient extension for VSCode, though it has limited support in other editors and ecosystems. 

## CITATION.{yml,cff}

The CITATION file is useful for indicating how to properly cite your codebase (which is subtly different from crediting). It's a yaml file containing information about software, as well as the author(s). It's primarily intended for researchers and publishers of research software. 

Its format is the Citation File Format, a yaml subset: 
```
cff-version: 1.2.0
message: "If you use this software, please cite it as below."
authors:
- family-names: "Lisa"
  given-names: "Mona"
  orcid: "https://orcid.org/0000-0000-0000-0000"
- family-names: "Bot"
  given-names: "Hew"
  orcid: "https://orcid.org/0000-0000-0000-0000"
title: "My Research Software"
version: 2.0.4
doi: 10.5281/zenodo.1234
date-released: 2017-12-18
url: "https://github.com/github-linguist/linguist"
```

This file has support in GitHub and will show up on your repository, along the right hand side under the "About" section alongside the readme, license, and code of conduct. 

## .mailmap

Many might think that the mailmap file is a relic of a forgotten age, but it still has its merits to this day. 

In its simplest form, the mailmap file is a list of email addresses. 

```
John Smith <john.smith@example.com>
Jane Doe <jane.doe@example.com>
```

In this form, it can help with contact and crediting should the source code be distributed without the git history for any reason, as well as serving as a convenient lookup for those who are less familiar with git. 

Additionally, it can help clarify when part of a committer's identity (their chosen name and/or email address) changes. Say John and Jane got married: 

```
John Doe <john.doe@example.com> John Smith <john.smith@example.com> 
Jane Doe <jane.doe@example.com>
```

## .editorconfig

The editorconfig file does pretty much what it says on the tin: it configures the editor, or, at least, provides a series of suggestions that an editor may choose to follow. 

It should be noted that an editorconfig should not be used as a tool to enforce code style (this is better done in a CI pipeline) or to treat warnings as errors (which can be configured in more reliable ways for most languages). The editorconfig is more a set of suggestions to nudge developers towards confirming to the standards of a repository. 

Additionally, not all editors and extensions respect all editorconfig preferences, and not all editor options are configurable in editorconfig. 
