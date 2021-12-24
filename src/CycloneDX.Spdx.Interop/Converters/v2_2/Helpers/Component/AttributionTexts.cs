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
using CycloneDX.Models.v1_3;

namespace CycloneDX.Spdx.Interop.Helpers
{
    public static class AttributionTexts
    {
        public static List<string> GetSpdxAttributionTexts(this Component component)
        {
            if (component.Evidence?.Copyright != null)
            {
                var texts = new List<string>();
                foreach (var copyright in component.Evidence.Copyright)
                {
                    texts.Add(copyright.Text);
                }
                return texts;
            }
            else
            {
                return null;
            }
        }

        public static void AddSpdxAttributionTexts(this Component component, List<string> attributionTexts)
        {
            if (attributionTexts != null)
            {
                if (component.Evidence == null) { component.Evidence = new Evidence(); }
                if (component.Evidence.Copyright == null) { component.Evidence.Copyright = new List<EvidenceCopyright>(); }
                foreach (var attribution in attributionTexts)
                {
                    component.Evidence.Copyright.Add(new EvidenceCopyright
                    {
                        Text = attribution,
                    });
                }
            }
        }    
    }
}
