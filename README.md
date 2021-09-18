[![Build Status](https://github.com/CycloneDX/cyclonedx-dotnet-library/workflows/.NET%20Core%20CI/badge.svg)](https://github.com/CycloneDX/cyclonedx-dotnet-library/actions?workflow=.NET+Core+CI)
[![License](https://img.shields.io/badge/license-Apache%202.0-brightgreen.svg)][License]
[![NuGet Version](https://img.shields.io/nuget/v/CycloneDX.Core.svg)](https://www.nuget.org/packages/CycloneDX.Core/)
![Nuget](https://img.shields.io/nuget/dt/CycloneDX.Models.svg)
[![Website](https://img.shields.io/badge/https://-cyclonedx.org-blue.svg)](https://cyclonedx.org/)
[![Slack Invite](https://img.shields.io/badge/Slack-Join-blue?logo=slack&labelColor=393939)](https://cyclonedx.org/slack/invite)
[![Group Discussion](https://img.shields.io/badge/discussion-groups.io-blue.svg)](https://groups.io/g/CycloneDX)
[![Twitter](https://img.shields.io/twitter/url/http/shields.io.svg?style=social&label=Follow)](https://twitter.com/CycloneDX_Spec)

# CycloneDX libraries for .NET


The CycloneDX libraries for .NET support programmatically consuming and producing CycloneDX bill-of-materials. CycloneDX is a lightweight BOM specification that is easily created, human readable, and simple to parse.

The libraries support .NET Standard 2.0.

[![Open in Gitpod](https://gitpod.io/button/open-in-gitpod.svg)](https://gitpod.io/#https://github.com/CycloneDX/cyclonedx-dotnet-library)

## Getting Started

To add data models, serialization, deserialization, and validation to your project

```
dotnet add package CycloneDX.Core
```

To add additional utility methods to your project

```
dotnet add package CycloneDX.Utils
```

## License

Permission to modify and redistribute is granted under the terms of the Apache 2.0 license. See the [LICENSE] file for the full license.

[License]: https://github.com/CycloneDX/cyclonedx-dotnet-library/blob/master/LICENSE

## Contributing

Pull requests are welcome. But please read the
[CycloneDX contributing guidelines](https://github.com/CycloneDX/.github/blob/master/CONTRIBUTING.md) first.

To build and test the solution locally standard commands like `dotnet build` and `dotnet test` work.

Documentation is generated using [DocFX](https://dotnet.github.io/docfx/index.html).

It is generally expected that pull requests will include relevant tests.
Tests are automatically run on Windows, MacOS and Linux for every pull request.
And build warnings will break the build.

If you are having trouble debugging a test that is failing for a platform you
don't have access to please us know.
