// This file is part of CycloneDX Library for .NET
//
// Licensed under the Apache License, Version 2.0 (the “License”);
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an “AS IS” BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// SPDX-License-Identifier: Apache-2.0
// Copyright (c) OWASP Foundation. All Rights Reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CycloneDX;

namespace CycloneDX.Core.Benchmark
{
    public class Serialization
    {
        private readonly CycloneDX.Models.v1_3.Bom _bom;
        private readonly string _xmlBom;
        private readonly string _jsonBom;
        private readonly byte[] _protobufBom;
        
        public Serialization()
        {
            _xmlBom = File.ReadAllText(Path.Join("Resources", "valid-bom-1.3.xml"));
            _bom = Xml.Deserializer.Deserialize(_xmlBom);
            _jsonBom = Json.Serializer.Serialize(_bom);
            _protobufBom = Protobuf.Serializer.Serialize(_bom);
        }

        [Benchmark]
        public string SerializeJson() => Json.Serializer.Serialize(_bom);
        
        [Benchmark]
        public CycloneDX.Models.v1_3.Bom DeserializeJson() => Json.Deserializer.Deserialize(_jsonBom);
        
        [Benchmark]
        public string SerializeXml() => Xml.Serializer.Serialize(_bom);
        
        [Benchmark]
        public CycloneDX.Models.v1_3.Bom DeserializeXml() => Xml.Deserializer.Deserialize(_xmlBom);
        
        [Benchmark]
        public byte[] SerializeProtobuf() => Protobuf.Serializer.Serialize(_bom);
        
        [Benchmark]
        public CycloneDX.Models.v1_3.Bom DeserializeProtobuf() => Protobuf.Deserializer.Deserialize(_protobufBom);
    }

    public class Validation
    {
        private readonly string _xmlBom;
        private readonly string _jsonBom;
        private readonly byte[] _protobufBom;
        
        public Validation()
        {
            _xmlBom = File.ReadAllText(Path.Join("Resources", "valid-bom-1.3.xml"));
            var bom = Xml.Deserializer.Deserialize(_xmlBom);
            _jsonBom = Json.Serializer.Serialize(bom);
            _protobufBom = Protobuf.Serializer.Serialize(bom);
        }

        [Benchmark]
        public async Task<CycloneDX.Models.ValidationResult> ValidateJson() => await Json.Validator.Validate(_jsonBom, SchemaVersion.v1_3);
        
        [Benchmark]
        public async Task<CycloneDX.Models.ValidationResult> ValidateXml() => await Xml.Validator.Validate(_xmlBom, SchemaVersion.v1_3);
        
        // [Benchmark]
        // public async Task<CycloneDX.Models.ValidationResult> ValidateProtobuf() => await Protobuf.Validator.Validate(_jsonBom, SchemaVersion.v1_3);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Serialization>();
            BenchmarkRunner.Run<Validation>();
        }
    }
}
