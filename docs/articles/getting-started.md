# Getting Started

All the following examples require the `CycloneDX.Core` library.

Add to your project with the command `dotnet add package CycloneDX.Core`.


## Serialization, Deserialization & Validation


### JSON Examples

```csharp
using CycloneDX;
using CycloneDX.Json;

...

// deserializing from a string
var bom = Serializer.Deserialize(jsonString);
// deserializing from a stream
var bom = await Serializer.DeserializeAsync(jsonStream);


// serializing to a string
var jsonString = Serializer.Serialize(bom);
// serializing to a stream
await Serializer.SerializeAsync(bom, stream);


// validating a string or stream
var result = Validator.Validate(jsonString, SpecificationVersion.v1_3);
// or
var result = await Validator.ValidateAsync(jsonStream, SpecificationVersion.v1_3);

if (result.Valid)
{
    Console.WriteLine("Valid CycloneDX v1.3 JSON document");
}
else
{
    Console.WriteLine("Not a valid CycloneDX v1.3 JSON document");
    Console.WriteLine("Validation errors:");
    foreach (var message in result.Messages)
    {
        Console.WriteLine(message);
    }
}
```


### XML Examples

```csharp
using CycloneDX;
using CycloneDX.Xml;

...

// deserializing from a string
var bom = Serializer.Deserialize(xmlString);
// deserializing from a stream
var bom = Serializer.Deserialize(xmlStream);


// serializing to a string
var xmlString = Serializer.Serialize(bom);
// serializing to a stream
Serializer.Serialize(bom, stream);


// validating a string or stream
var result = Validator.Validate(xmlString, SpecificationVersion.v1_3);
// or
var result = Validator.Validate(xmlStream, SpecificationVersion.v1_3);

if (result.Valid)
{
    Console.WriteLine("Valid CycloneDX v1.3 XML document");
}
else
{
    Console.WriteLine("Not a valid CycloneDX v1.3 XML document");
    Console.WriteLine("Validation errors:");
    foreach (var message in result.Messages)
    {
        Console.WriteLine(message);
    }
}
```


### Protobuf Examples

```csharp
using CycloneDX;
using CycloneDX.Protobuf;

...

// deserializing from a byte array
var bom = Serializer.Deserialize(protobufBytes);
// deserializing from a stream
var bom = Serializer.Deserialize(protobufStream);


// serializing to a byte array
var bytes = Serializer.Serialize(bom);
// serializing to a stream
Serializer.Serialize(bom, stream);
```


## Converting between BOM formats

```csharp
using CycloneDX;

...

using (var inputFile = File.OpenRead("bom.json"))
using (var outputFile = File.OpenWrite("bom.xml"))
{
    var bom = await Json.Serializer.DeserializeAsync(inputFile);
    Xml.Serializer.Serialize(bom, outputFile);
}
```

## Downgrading/Upgrading BOM formats

```csharp
using CycloneDX;

...

using (var inputFile = File.OpenRead("bom-1.2.json"))
using (var outputFile = File.OpenWrite("bom-1.3.json"))
{
    var bom = await Json.Serializer.DeserializeAsync(inputFile);
    // set the SpecVersion to whatever version you want
    bom.SpecVersion = SpecificationVersion.v1_3;
    Json.Serializer.Serialize(bom, outputFile);
}
```
