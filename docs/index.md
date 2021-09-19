# CycloneDX .NET Library Documentation

The CycloneDX libraries for .NET support programmatically consuming and
producing CycloneDX bill-of-materials. CycloneDX is a lightweight BOM
specification that is easily created, human readable, and simple to parse.

The libraries support .NET Standard 2.0.

## Getting Started

To add data models, serialization, deserialization, and validation to your project

```shell
dotnet add package CycloneDX.Core
```

To add additional utility methods to your project

```shell
dotnet add package CycloneDX.Utils
```

For code examples check out the [Getting Started Article](articles/getting-started.md)

## License

Permission to modify and redistribute is granted under the terms of the
Apache 2.0 license. See the [LICENSE] file for the full license.

[License]: https://github.com/CycloneDX/cyclonedx-dotnet-library/blob/master/LICENSE

## Contributing

Pull requests are welcome. But please read the
[CycloneDX contributing guidelines](https://github.com/CycloneDX/.github/blob/master/CONTRIBUTING.md) first.

To build and test the solution locally standard commands like `dotnet build`
and `dotnet test` work.

Documentation is generated using [DocFX](https://dotnet.github.io/docfx/index.html).

It is generally expected that pull requests will include relevant tests.
Tests are automatically run on Windows, MacOS and Linux for every pull request.
And build warnings will break the build.

If you are having trouble debugging a test that is failing for a platform you
don't have access to please us know.