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

## Getting Started

For help getting started using the CycloneDX .NET Library refer to the [documentation](https://cyclonedx.github.io/cyclonedx-dotnet-library/).

## SPDX Interop

The `CycloneDX.Spdx.Interop` library includes methods for converting between
CycloneDX and SPDX formats. (Currently only SPDX v2.3 JSON format is supported.)

### High level overview of information lost during conversion:

This is a high level overview of information that will be lost during
conversion. This is current state only, some features are yet to be
implemented as indicated below.

If you are familiar with both formats, and would like to contribute to
minimising data loss during conversion, pull requests are welcome :)

#### SPDX -> CycloneDX

| Feature | Notes |
| --- | --- |
| Relationship Information | Implementation pending, related to CycloneDX component assemblies, dependency graph, and composition. |
| License information in files | Needs review, the way SPDX and CycloneDX handle license information evidence is slightly different. |
| Snippet Information | Snippets are not currently supported by CycloneDX |
| Non-SPDX licenses | Implementation pending |
| Package URL for Component Identity | SPDX supports multiple PURLs for a package. But doesn't support specifying if any are a component identifier. The first one is used as component purl.|
| CPE for Component Identity | SPDX supports multiple CPEs for a package. But doesn't support specifying if any are a component identifier. The first one is used as component CPE.|

#### CycloneDX -> SPDX

| Feature | Notes |
| --- | --- |
| Component assemblies | Implementation pending - related to SPDX Relationship Information |
| SWID Tags | SPDX doesn't support SWID tags. |
| CPE and Package URL for Component Identity | SPDX supports multiple CPEs and PURLs for a package. But doesn't support specifying if any are a component identifier. |
| Device & Hardware Components | SPDX does not support devices or hardware as components. |
| Composition | Implementation pending - related to SPDX Relationship Information |
| Dependency Graph | Implementation pending - related to SPDX Relationship Information |
| External References | External references are handled differently between the two formats. |
| Non-SPDX licenses | Implementation pending |

## License

Permission to modify and redistribute is granted under the terms of the Apache 2.0 license. See the [LICENSE] file for the full license.

[License]: https://github.com/CycloneDX/cyclonedx-dotnet-library/blob/master/LICENSE

## Contributing

Pull requests are welcome. But please read the
[CycloneDX contributing guidelines](https://github.com/CycloneDX/.github/blob/master/CONTRIBUTING.md) first.

To build and test the solution locally standard commands like `dotnet build` and `dotnet test` work.

The protocol buffer tests require the protocol buffer compiler executable `protoc` to be available in your path.

Documentation is generated using [DocFX](https://dotnet.github.io/docfx/index.html).

It is generally expected that pull requests will include relevant tests.
Tests are automatically run on Windows, MacOS and Linux for every pull request.
And build warnings will break the build.

If you are having trouble debugging a test that is failing for a platform you
don't have access to please us know.
