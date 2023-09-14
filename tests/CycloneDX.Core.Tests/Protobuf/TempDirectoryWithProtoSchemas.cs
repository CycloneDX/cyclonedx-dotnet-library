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
using System.Reflection;

namespace CycloneDX.Core.Tests.Protobuf
{
    public class TempDirectoryWithProtoSchemas : TempDirectory
    {
        public TempDirectoryWithProtoSchemas()
        {
            var assembly = typeof(CycloneDX.Protobuf.Serializer).GetTypeInfo().Assembly;
            foreach (var versionString in new List<string> { "1.3", "1.4", "1.5" })
            {
                using (var schemaStream = assembly.GetManifestResourceStream($"CycloneDX.Core.Schemas.bom-{versionString}.proto"))
                {
                    using (var fileStream = File.Create(Path.Join(DirectoryPath, $"bom-{versionString}.proto")))
                    {
                        schemaStream.Seek(0, SeekOrigin.Begin);
                        schemaStream.CopyTo(fileStream);
                    }
                }
            }
        }
    }
}
