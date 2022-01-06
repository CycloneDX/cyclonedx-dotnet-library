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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CycloneDX.Models;

namespace CycloneDX.Core.Benchmark
{
    public class Serialization
    {
        private readonly Bom _bom;
        private readonly string _xmlBom;
        private readonly string _jsonBom;
        private readonly byte[] _protobufBom;
        
        public Serialization()
        {
            _xmlBom = File.ReadAllText(Path.Join("Resources", "valid-bom-1.4.xml"));
            _jsonBom = Json.Serializer.Serialize(_bom);
            _protobufBom = Protobuf.Serializer.Serialize(_bom);

            _bom = Xml.Serializer.Deserialize(_xmlBom);
        }

        [Benchmark]
        public string SerializeJson() => Json.Serializer.Serialize(_bom);
        
        [Benchmark]
        public Bom DeserializeJson() => Json.Serializer.Deserialize(_jsonBom);
        
        [Benchmark]
        public string SerializeXml() => Xml.Serializer.Serialize(_bom);
        
        [Benchmark]
        public Bom DeserializeXml() => Xml.Serializer.Deserialize(_xmlBom);
        
        [Benchmark]
        public byte[] SerializeProtobuf() => Protobuf.Serializer.Serialize(_bom);
        
        [Benchmark]
        public Bom DeserializeProtobuf() => Protobuf.Serializer.Deserialize(_protobufBom);
    }

    public class Validation
    {
        private readonly Dictionary<SpecificationVersion, string> _xmlBoms = new Dictionary<SpecificationVersion, string>();
        private readonly Dictionary<SpecificationVersion, string> _jsonBoms = new Dictionary<SpecificationVersion, string>();
        
        public Validation()
        {
            var bomXml = File.ReadAllText(Path.Join("Resources", "valid-bom-1.4.xml"));
            var bom = Xml.Serializer.Deserialize(bomXml);
            _xmlBoms[SpecificationVersion.v1_4] = Xml.Serializer.Serialize(bom);
            _jsonBoms[SpecificationVersion.v1_4] = Json.Serializer.Serialize(bom);
            bom.SpecVersion = SpecificationVersion.v1_3;
            _xmlBoms[SpecificationVersion.v1_3] = Xml.Serializer.Serialize(bom);
            _jsonBoms[SpecificationVersion.v1_3] = Json.Serializer.Serialize(bom);
            bom.SpecVersion = SpecificationVersion.v1_2;
            _xmlBoms[SpecificationVersion.v1_2] = Xml.Serializer.Serialize(bom);
            _jsonBoms[SpecificationVersion.v1_2] = Json.Serializer.Serialize(bom);
        }

        [Benchmark]
        public ValidationResult ValidateJson14() => Json.Validator.Validate(_jsonBoms[SpecificationVersion.v1_4], SpecificationVersion.v1_4);
        [Benchmark]
        public ValidationResult ValidateJson13() => Json.Validator.Validate(_jsonBoms[SpecificationVersion.v1_3], SpecificationVersion.v1_3);
        [Benchmark]
        public ValidationResult ValidateJson12() => Json.Validator.Validate(_jsonBoms[SpecificationVersion.v1_2], SpecificationVersion.v1_2);
        
        [Benchmark]
        public ValidationResult ValidateXml14() => Xml.Validator.Validate(_xmlBoms[SpecificationVersion.v1_4], SpecificationVersion.v1_4);
        [Benchmark]
        public ValidationResult ValidateXml13() => Xml.Validator.Validate(_xmlBoms[SpecificationVersion.v1_3], SpecificationVersion.v1_3);
        [Benchmark]
        public ValidationResult ValidateXml12() => Xml.Validator.Validate(_xmlBoms[SpecificationVersion.v1_2], SpecificationVersion.v1_2);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // BenchmarkRunner.Run<Serialization>();
            BenchmarkRunner.Run<Validation>();
        }
    }
}
