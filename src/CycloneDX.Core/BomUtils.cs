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
using System.Linq;
using System.Security.Claims;
using CycloneDX.Core.Models;
using CycloneDX.Models;
using CycloneDX.Models.Vulnerabilities;
using static CycloneDX.Models.EvidenceIdentity;

namespace CycloneDX
{
    public static class BomUtils
    {
        internal static DateTime? UtcifyDateTime(DateTime? value)
        {
            if (value is null)
            {
                return null;
            }
            else if (value.Value.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
            }
            else if (value.Value.Kind == DateTimeKind.Local)
            {
                return value.Value.ToUniversalTime();
            }
            else
            {
                return value;
            }
        }

        internal static Bom GetBomForSerialization(Bom bom)
        {
            if (bom.SpecVersion == SpecificationVersionHelpers.CurrentVersion)
            {
                return bom;
            }
            else
            {
                var downgradedBom = CopyBomAndDowngrade(bom);
                return downgradedBom;
            }
        }

        internal static Bom CopyBomAndDowngrade(Bom bom)
        {
            var bomCopy = bom.Copy();

            // The protobuf deep copy does not preserve Signature/XmlSignature properties.
            // Signatories require either a "signature" or "organization"+"externalReference"
            // per the JSON schema oneOf constraint. Remove any that are now invalid.
            if (bomCopy.Declarations?.Affirmation?.Signatories != null)
            {
                bomCopy.Declarations.Affirmation.Signatories.RemoveAll(s =>
                    s.Signature == null && (s.Organization == null || s.ExternalReference == null));
            }

            // we downgrade stuff starting with lowest spec first
            // this will remove entire classes of things and will save unnecessary processing further down
            if (bomCopy.SpecVersion < SpecificationVersion.v1_1)
            {
                bomCopy.SerialNumber = null;
                bomCopy.ExternalReferences = null;

                EnumerateAllComponents(bomCopy, (component) =>
                {
                    component.BomRef = null;
                    component.Pedigree = null;
                    component.ExternalReferences = null;
                });
            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_2)
            {
                bomCopy.Metadata = null;
                bomCopy.Dependencies = null;
                bomCopy.Services = null;

                EnumerateAllComponents(bomCopy, (component) =>
                {
                    #pragma warning disable 618
                    component.Author = null;
                    #pragma warning restore 618
                    component.MimeType = null;
                    component.Supplier = null;
                    component.Swid = null;

                    if (component.Pedigree != null)
                    {
                        component.Pedigree.Patches = null;
                    }
                });
            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_3)
            {
                bomCopy.Compositions = null;
                if (bomCopy.Metadata != null)
                {
                    bomCopy.Metadata.Licenses = null;
                    bomCopy.Metadata.Properties = null;
                }
                EnumerateAllComponents(bomCopy, (component) =>
                {
                    component.Properties = null;
                    component.Evidence = null;
                    if (component.ExternalReferences != null)
                    {
                        foreach (var extRef in component.ExternalReferences)
                        {
                            extRef.Hashes = null;
                        }
                    }
                });
                EnumerateAllServices(bomCopy, (service) =>
                {
                    service.Properties = null;
                    if (service.ExternalReferences != null)
                    {
                        foreach (var extRef in service.ExternalReferences)
                        {
                            extRef.Hashes = null;
                        }
                    }
                });
            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_4)
            {
                EnumerateAllComponents(bomCopy, (component) =>
                {
                    component.ReleaseNotes = null;
                    if (component.Version == null)
                    {
                        component.Version = "0.0.0";
                    }
                });
                EnumerateAllServices(bomCopy, (service) =>
                {
                    service.ReleaseNotes = null;
                });
                bomCopy.Vulnerabilities = null;
            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_5)
            {
                bomCopy.Annotations = null;
                bomCopy.Properties = null;
                bomCopy.Formulation = null;

                if (bomCopy.Metadata != null)
                {
                    bomCopy.Metadata.Lifecycles = null;
                }

                if (bomCopy.Compositions != null)
                {
                    foreach (var composition in bomCopy.Compositions)
                    {
                        composition.BomRef = null;
                        composition.Vulnerabilities = null;
                    }
                }

                EnumerateAllToolChoices(bomCopy, (toolchoice) =>
                {
                    toolchoice.Components = null;
                    toolchoice.Services = null;
                });

                EnumerateAllComponents(bomCopy, (component) =>
                {
                    component.ModelCard = null;
                    component.Data = null;
                    if ((int)component.Type > 8)
                    {
                        component.Type = Component.Classification.Library;
                    }
                });

                EnumerateAllServices(bomCopy, (service) =>
                {
                    service.TrustZone = null;
                    if (service.Data != null)
                    {
                        foreach (var data in service.Data)
                        {
                            data.Name = null;
                            data.Description = null;
                            data.Governance = null;
                            data.Source = null;
                            data.Destination = null;
                        }
                    }
                });

                EnumerateAllVulnerabilities(bomCopy, (vulnerability) =>
                {
                    vulnerability.Rejected = null;
                    vulnerability.ProofOfConcept = null;
                    vulnerability.Workaround = null;
                    if (vulnerability.Analysis != null)
                    {
                        vulnerability.Analysis.FirstIssued = null;
                        vulnerability.Analysis.LastUpdated = null;
                    }

                    if (vulnerability.Ratings != null)
                    {
                        var i = 0;
                        while (i < vulnerability.Ratings.Count)
                        {
                            if (vulnerability.Ratings[i].Method == ScoreMethod.CVSSV4 ||
                                vulnerability.Ratings[i].Method == ScoreMethod.SSVC)
                            {
                                vulnerability.Ratings.RemoveAt(i);
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                });

                EnumerateAllEvidence(bomCopy, (evidence) =>
                {
                    evidence.Identity = null;
                    evidence.Occurrences = null;
                    evidence.Callstack = null;
                });

                EnumerateAllLicenseChoices(bomCopy, (licenseChoice) =>
                {
                    licenseChoice.BomRef = null;
                });

                EnumerateAllLicenses(bomCopy, (license) =>
                {
                    license.BomRef = null;
                    license.Licensing = null;
                    license.Properties = null;
                });

                EnumerateAllOrganizationalEntity(bomCopy, (orgEntity) =>
                {
                    orgEntity.BomRef = null;
                });

                EnumerateAllOrganizationalContact(bomCopy, (orgContact) =>
                {
                    orgContact.BomRef = null;
                });
            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_6)
            {
                bomCopy.Declarations = null;
                bomCopy.Definitions = null;

                EnumerateAllComponents(bomCopy, (component) =>
                {
                    component.CryptoProperties = null;
                    if (component.Type == Component.Classification.Cryptographic_Asset)
                    {
                        component.Type = Component.Classification.Library;
                    }
                    component.Tags = null;
                    component.OmniborId = null;
                    component.Swhid = null;
                    component.Authors = null;
                    component.Manufacturer = null;

                    if (component.ModelCard?.Considerations != null)
                    {
                        component.ModelCard.Considerations.EnvironmentalConsiderations = null;
                    }
                });

                EnumerateAllOrganizationalEntity(bomCopy, (oe) =>
                {
                    oe.Address = null;
                });

                EnumerateAllServices(bomCopy, (service) =>
                {
                    service.Tags = null;
                });

                if (bomCopy.Metadata != null)
                {
                    bomCopy.Metadata.Manufacturer = null;
                }

                EnumerateAllDependencies(bomCopy, (dependency) =>
                {
                    dependency.Provides = null;
                });

                EnumerateAllEvidence(bomCopy, (evidence) =>
                {
                    if (evidence?.Identity?.Count > 1)
                    {
                        evidence.Identity.RemoveRange(1, evidence.Identity.Count - 1);
                    }
                    if (evidence.Identity?.Count == 1 &&
                        (evidence.Identity[0].Field == EvidenceFieldType.OmniborId
                        || evidence.Identity[0].Field == EvidenceFieldType.Swhid))
                    {
                        evidence.Identity.Clear();
                    }
                    if (evidence.Identity?.Count == 1)
                    {
                        evidence.Identity[0].ConcludedValue = null;
                    }
                });

                EnumerateAllLicenseChoices(bomCopy, (licenseChoice) =>
                {
                    if (licenseChoice.License != null)
                    {
                        licenseChoice.License.Acknowledgement = null;
                    }
                    licenseChoice.Acknowledgement = null;
                });

                EnumerateAllExternalReferences(bomCopy, (externalReference) =>
                {
                    if (externalReference != null)
                    {
                        if (externalReference.Type == ExternalReference.ExternalReferenceType.Source_Distribution
                            || externalReference.Type == ExternalReference.ExternalReferenceType.Electronic_Signature
                            || externalReference.Type == ExternalReference.ExternalReferenceType.Digital_Signature
                            || externalReference.Type == ExternalReference.ExternalReferenceType.Rfc_9116)
                        {
                            externalReference.Type = ExternalReference.ExternalReferenceType.Other;
                        }
                    }
                });

            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_7)
            {
                bomCopy.Citations = null;

                if (bomCopy.Metadata != null)
                {
                    bomCopy.Metadata.DistributionConstraints = null;
                }

                if (bomCopy.Definitions != null)
                {
                    bomCopy.Definitions.Patents = null;
                }

                EnumerateAllComponents(bomCopy, (component) =>
                {
                    component.VersionRange = null;
                    component.IsExternal = null;
                    component.PatentAssertions = null;

                    if (component.CryptoProperties?.CertificateProperties != null)
                    {
                        var certProps = component.CryptoProperties.CertificateProperties;
                        certProps.SerialNumber = null;
                        certProps.CertificateFileExtension = null;
                        certProps.Fingerprint = null;
                        certProps.CertificateStates = null;
                        certProps.CreationDate = null;
                        certProps.ActivationDate = null;
                        certProps.DeactivationDate = null;
                        certProps.RevocationDate = null;
                        certProps.DestructionDate = null;
                        certProps.CertificateExtensions = null;
                        certProps.RelatedCryptographicAssets = null;
                    }

                    if (component.CryptoProperties?.RelatedCryptoMaterialProperties != null)
                    {
                        var rcmProps = component.CryptoProperties.RelatedCryptoMaterialProperties;
                        rcmProps.Fingerprint = null;
                        rcmProps.RelatedCryptographicAssets = null;
                    }

                    if (component.CryptoProperties?.ProtocolProperties != null)
                    {
                        var protoProps = component.CryptoProperties.ProtocolProperties;
                        protoProps.RelatedCryptographicAssets = null;
                        // Downgrade detailed IKEv2 transform types to null (v1.6 doesn't support the detailed format)
                        if (protoProps.Ikev2TransformTypes != null)
                        {
                            protoProps.Ikev2TransformTypes = null;
                        }
                    }
                });

                EnumerateAllServices(bomCopy, (service) =>
                {
                    service.PatentAssertions = null;
                });

                EnumerateAllLicenseChoices(bomCopy, (licenseChoice) =>
                {
                    licenseChoice.ExpressionDetails = null;
                    licenseChoice.Licensing = null;
                    licenseChoice.Properties = null;
                });

                // v1.6 xs:choice only allows EITHER licenses OR an expression, not both.
                // v1.7 allows unbounded mixing. Strip expressions when mixed with licenses.
                DowngradeMixedLicenseChoiceLists(bomCopy);

                EnumerateAllExternalReferences(bomCopy, (externalReference) =>
                {
                    if (externalReference != null)
                    {
                        if (externalReference.Type == ExternalReference.ExternalReferenceType.Patent
                            || externalReference.Type == ExternalReference.ExternalReferenceType.Patent_Family
                            || externalReference.Type == ExternalReference.ExternalReferenceType.Patent_Assertion
                            || externalReference.Type == ExternalReference.ExternalReferenceType.Citation)
                        {
                            externalReference.Type = ExternalReference.ExternalReferenceType.Other;
                        }
                    }
                });
            }

            // triggers a bunch of stuff, don't remove unless you know what you are doing
            bomCopy.SpecVersion = bomCopy.SpecVersion;

            return bomCopy;
        }

        public static Bom Copy(this Bom bom)
        {
            var protoBom = Protobuf.Serializer.SerializeForDeepCopy(bom);
            var bomCopy = Protobuf.Serializer.Deserialize(protoBom);
            return bomCopy;
        }

        public static void EnqueueMany<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            if (items != null)
            {
                foreach (var item in items.Where(item => item != null))
                {
                    queue.Enqueue(item);
                }
            }
        }

        public static void EnumerateAllComponents(Bom bom, Action<Component> callback)
        {
            var q = new Queue<Component>();

            q.Enqueue(bom.Metadata?.Component);
            q.EnqueueMany(bom.Metadata?.Tools?.Components);
            q.EnqueueMany(bom.Components);
            q.EnqueueMany(bom.Annotations?.Select(an => an.Annotator).Where(anor => anor.Component != null).Select(anor => anor.Component) ?? new List<Component>());
            q.EnqueueMany(bom.Declarations?.Targets?.Components);
            q.EnqueueMany(bom.Formulation?.Where(f => f.Components != null).SelectMany(f => f.Components));
            q.EnqueueMany(bom.Vulnerabilities?.Where(v => v.Tools?.Components != null).SelectMany(v => v.Tools.Components));

            while (q.Count > 0)
            {
                var currentComponent = q.Dequeue();
                if (currentComponent != null)
                {
                    callback(currentComponent);

                    q.EnqueueMany(currentComponent.Components);
                    q.EnqueueMany(currentComponent.Pedigree?.Ancestors);
                    q.EnqueueMany(currentComponent.Pedigree?.Descendants);
                    q.EnqueueMany(currentComponent.Pedigree?.Variants);
                }
            }
        }

        public static void EnumerateAllServices(Bom bom, Action<Service> callback)
        {
            var q = new Queue<Service>();

            q.EnqueueMany(bom.Metadata?.Tools?.Services);
            q.EnqueueMany(bom.Services);
            q.EnqueueMany(bom.Annotations?.Select(an => an.Annotator).Where(anor => anor.Service != null).Select(anor => anor.Service) ?? new List<Service>());
            q.EnqueueMany(bom.Declarations?.Targets?.Services);
            q.EnqueueMany(bom.Formulation?.Where(f => f.Services != null).SelectMany(f => f.Services));
            q.EnqueueMany(bom.Vulnerabilities?.Where(v => v.Tools?.Services != null).SelectMany(v => v.Tools.Services));

            while (q.Count > 0)
            {
                var currentService = q.Dequeue();
                if (currentService != null)
                {
                    callback(currentService);

                    q.EnqueueMany(currentService.Services);
                }
            }
        }

        public static void EnumerateAllVulnerabilities(Bom bom, Action<Vulnerability> callback)
        {
            var q = new Queue<Vulnerability>();

            if (bom.Vulnerabilities != null)
            {
                foreach (var vulnerability in bom.Vulnerabilities)
                {
                    q.Enqueue(vulnerability);
                }
            }

            while (q.Count > 0)
            {
                var currentVulnerability = q.Dequeue();

                callback(currentVulnerability);
            }
        }
        public static void EnumerateAllEvidence(Bom bom, Action<Evidence> callback)
        {
            EnumerateAllComponents(bom, (component) =>
            {
                if (component.Evidence != null)
                {
                    callback(component.Evidence);
                }
            });
        }

        public static void EnumerateAllLicenses(Bom bom, Action<License> callback)
        {
            EnumerateAllLicenseChoices(bom, (licenseChoice) =>
            {
                if (licenseChoice.License != null)
                {
                    callback(licenseChoice.License);
                }
            });
        }

        private static void DowngradeMixedLicenseChoiceList(List<LicenseChoice> licenses)
        {
            if (licenses == null || licenses.Count <= 1) return;
            // v1.6 xs:choice only allows EITHER multiple licenses OR one expression.
            // When mixed, keep licenses and drop expressions.
            // When multiple expressions, keep only the first.
            bool hasLicense = false;
            bool hasExpression = false;
            foreach (var lc in licenses)
            {
                if (lc.License != null) hasLicense = true;
                if (lc.Expression != null) hasExpression = true;
            }
            if (hasLicense && hasExpression)
            {
                licenses.RemoveAll(lc => lc.Expression != null && lc.License == null);
            }
            else if (hasExpression)
            {
                // Keep only the first expression
                bool first = true;
                licenses.RemoveAll(lc =>
                {
                    if (lc.Expression != null)
                    {
                        if (first) { first = false; return false; }
                        return true;
                    }
                    return false;
                });
            }
        }

        private static void DowngradeMixedLicenseChoiceLists(Bom bom)
        {
            if (bom.Metadata?.Licenses != null)
                DowngradeMixedLicenseChoiceList(bom.Metadata.Licenses);
            EnumerateAllComponents(bom, (component) =>
            {
                DowngradeMixedLicenseChoiceList(component.Licenses);
            });
            EnumerateAllServices(bom, (service) =>
            {
                DowngradeMixedLicenseChoiceList(service.Licenses);
            });
        }

        public static void EnumerateAllLicenseChoices(Bom bom, Action<LicenseChoice> callback)
        {
            if (bom.Metadata?.Licenses != null)
            {
                foreach (var license in bom.Metadata.Licenses)
                {
                    callback(license);
                }

            }
            EnumerateAllComponents(bom, (component) =>
            {
                if (component.Licenses != null)
                {
                    foreach (var license in component.Licenses)
                    {
                        callback(license);
                    }
                }
            });

            EnumerateAllServices(bom, (service) =>
            {
                if (service.Licenses != null)
                {
                    foreach (var license in service.Licenses)
                    {
                        callback(license);
                    }
                }
            });

            EnumerateAllEvidence(bom, (evidence) =>
            {
                if (evidence.Licenses != null)
                {
                    foreach (var license in evidence.Licenses)
                    {
                        callback(license);
                    }
                }
            });
        }

        public static void EnumerateAllOrganizationalEntity(Bom bom, Action<OrganizationalEntity> callback)
        {
            #pragma warning disable 618
            if (bom.Metadata?.Manufacture != null)
            {
                callback(bom.Metadata.Manufacture);
            }
            #pragma warning restore 618
            if (bom.Metadata?.Supplier != null) callback(bom.Metadata.Supplier);

            if (bom.Annotations != null)
            {
                foreach (var annotation in bom.Annotations)
                {
                    if (annotation.Annotator?.Organization != null)
                    {
                        callback(annotation.Annotator.Organization);
                    }
                }
            }

            bom.Declarations?.Targets?.Organizations?.ForEach(callback);

            EnumerateAllVulnerabilities(bom, (vulnerability) =>
            {
                if (vulnerability.Credits?.Organizations != null)
                {
                    foreach (var org in vulnerability.Credits.Organizations) callback(org);
                }
            });
            EnumerateAllComponents(bom, (component) =>
            {
                if (component.Supplier != null)
                {
                    callback(component.Supplier);
                }


                component.ModelCard?.Considerations?.EnvironmentalConsiderations?.EnergyConsumptions?
                    .ForEach(energyConsumption =>
                        energyConsumption?.EnergyProviders?
                            .ForEach(energyProvider =>
                            {
                                if (energyProvider?.Organization != null)
                                {
                                    callback(energyProvider.Organization);
                                }
                            }));


            });
            EnumerateAllServices(bom, (service) =>
            {
                if (service.Provider != null)
                {
                    callback(service.Provider);
                }
            });
        }

        public static void EnumerateAllOrganizationalContact(Bom bom, Action<OrganizationalContact> callback)
        {
            EnumerateAllOrganizationalEntity(bom, (orgEntity) =>
            {
                if (orgEntity.Contact != null)
                {
                    foreach (var contact in orgEntity.Contact)
                    {
                        callback(contact);
                    }
                }
            });

            EnumerateAllVulnerabilities(bom, (vulnerability) =>
            {
                if (vulnerability.Credits?.Individuals != null)
                {
                    foreach (var individual in vulnerability.Credits.Individuals)
                    {
                        callback(individual);
                    }
                }
            });

            if (bom.Declarations?.Evidence != null)
            {
                foreach (var item in bom.Declarations?.Evidence?.Select(x => x.Author))
                {
                    callback(item);
                }
                foreach (var item in bom.Declarations?.Evidence?.Select(x => x.Reviewer))
                {
                    callback(item);
                }
            }
        }

        public static void EnumerateAllToolChoices(Bom bom, Action<ToolChoices> callback)
        {
            if (bom.Metadata?.Tools != null)
                callback(bom.Metadata.Tools);
            EnumerateAllVulnerabilities(bom, (vuln) =>
            {
                if (vuln.Tools != null)
                {
                    callback(vuln.Tools);
                }
            });
        }

        public static void EnumerateAllDependencies(Bom bom, Action<Dependency> callback)
        {
            var q = new Queue<Dependency>();


            q.EnqueueMany(bom.Dependencies);


            while (q.Count > 0)
            {
                var currentDependency = q.Dequeue();
                if (currentDependency != null)
                {
                    callback(currentDependency);

                    q.EnqueueMany(currentDependency.Dependencies);
                }
            }
        }

        public static void EnumerateAllDatasetChoices(Bom bom, Action<DatasetChoices> callback)
        {
            EnumerateAllComponents(bom, (component) =>
            {
                if (component?.ModelCard?.ModelParameters?.Datasets != null)
                {
                    callback(component.ModelCard.ModelParameters.Datasets);
                }
            });
        }

        public static void EnumerateAllExternalReferences(Bom bom, Action<ExternalReference> callback)
        {
            if (bom.ExternalReferences != null)
            {
                foreach (var item in bom.ExternalReferences)
                {
                    callback(item);
                }
            }

            EnumerateAllComponents(bom, (component) =>
            {
                if (component?.ExternalReferences != null)
                {
                    foreach (var item in component.ExternalReferences)
                    {
                        callback(item);
                    }
                }
                if (component?.ModelCard?.Considerations?.EnvironmentalConsiderations?.EnergyConsumptions != null)
                {
                    foreach (var energyConsumption in component.ModelCard.Considerations.EnvironmentalConsiderations.EnergyConsumptions)
                    {
                        if (energyConsumption?.EnergyProviders != null)
                        {
                            foreach (var energyProvider in energyConsumption.EnergyProviders)
                            {
                                if (energyProvider?.ExternalReferences != null)
                                {
                                    foreach (var item in energyProvider.ExternalReferences)
                                    {
                                        callback(item);
                                    }
                                }
                            }
                        }
                    }
                }
            });

            EnumerateAllServices(bom, (service) =>
            {
                if (service?.ExternalReferences != null)
                {
                    foreach (var item in service.ExternalReferences)
                    {
                        callback(item);
                    }
                }
            });


            EnumerateAllToolChoices(bom, (toolsChoice) =>
            {
                if (toolsChoice?.Tools != null)
                {
                    foreach (var tool in toolsChoice.Tools)
                    {
                        if (tool.ExternalReferences != null)
                        {
                            foreach (var item in tool.ExternalReferences)
                            {
                                callback(item);
                            }
                        }
                    }
                }
            });

            if (bom.Declarations?.Claims != null)
            {
                foreach (var claim in bom.Declarations.Claims)
                {
                    if (claim?.ExternalReferences != null)
                    {
                        foreach (var item in claim.ExternalReferences)
                        {
                            callback(item);
                        }
                    }
                }
            }

            if (bom.Declarations?.Affirmation?.Signatories != null)
            {
                foreach (var signatory in bom.Declarations?.Affirmation?.Signatories)
                {
                    if (signatory?.ExternalReference != null)
                    {
                        callback(signatory.ExternalReference);
                    }
                }
            }

            if (bom.Definitions?.Standards != null)
            {
                foreach (var standard in bom.Definitions.Standards)
                {
                    if (standard?.ExternalReferences != null)
                    {
                        foreach (var item in standard.ExternalReferences)
                        {
                            callback(item);
                        }
                    }
                }
            }

            if (bom.Definitions?.Patents != null)
            {
                foreach (var patentOrFamily in bom.Definitions.Patents)
                {
                    if (patentOrFamily?.Patent?.ExternalReferences != null)
                    {
                        foreach (var item in patentOrFamily.Patent.ExternalReferences)
                        {
                            callback(item);
                        }
                    }
                    if (patentOrFamily?.PatentFamily?.ExternalReferences != null)
                    {
                        foreach (var item in patentOrFamily.PatentFamily.ExternalReferences)
                        {
                            callback(item);
                        }
                    }
                }
            }

            EnumerateAllResourceReferenceChoices(bom, (resoureReferenceChoice) =>
            {
                if (resoureReferenceChoice?.ExternalReference != null)
                {
                    callback(resoureReferenceChoice.ExternalReference);
                }
            });

        }

        public static void EnumerateAllWorkflows(Bom bom, Action<Workflow> callback)
        {
            if (bom.Formulation != null)
            {
                foreach (var formulation in bom.Formulation)
                {
                    if (formulation?.Workflows != null)
                    {
                        foreach (var workflow in formulation.Workflows)
                        {
                            callback(workflow);
                        }
                    }
                }
            }
        }

        public static void EnumerateAllResourceReferenceChoices(Bom bom, Action<ResourceReferenceChoice> callback)
        {
            EnumerateAllWorkflows(bom, (workflow) =>
            {
                if (workflow?.ResourceReferences != null)
                {
                    foreach (var resourceReference in workflow.ResourceReferences)
                    {
                        callback(resourceReference);
                    }
                }
                if (workflow?.Inputs != null)
                {
                    foreach (var input in workflow.Inputs)
                    {
                        if (input.Resource != null) { callback(input.Resource); }
                        if (input.Source != null) { callback(input.Source); }
                        if (input.Target != null) { callback(input.Target); }
                    }
                }
                if (workflow?.Outputs != null)
                {
                    foreach (var output in workflow.Outputs)
                    {
                        if (output.Resource != null) { callback(output.Resource); }
                        if (output.Source != null) { callback(output.Source); }
                        if (output.Target != null) { callback(output.Target); }
                    }
                }
                if (workflow?.Trigger?.Event != null)
                {
                    if (workflow.Trigger.Event.Source != null) { callback(workflow.Trigger.Event.Source); }
                    if (workflow.Trigger.Event.Target != null) { callback(workflow.Trigger.Event.Target); }
                }

                foreach (var task in workflow.Tasks)
                {
                    if (task?.ResourceReferences != null)
                    {
                        foreach (var resourceReference in task.ResourceReferences)
                        {
                            callback(resourceReference);
                        }
                    }
                    if (task?.Inputs != null)
                    {
                        foreach (var input in task.Inputs)
                        {
                            if (input.Resource != null) { callback(input.Resource); }
                            if (input.Source != null) { callback(input.Source); }
                            if (input.Target != null) { callback(input.Target); }
                        }
                    }
                    if (task?.Outputs != null)
                    {
                        foreach (var output in task.Outputs)
                        {
                            if (output.Resource != null) { callback(output.Resource); }
                            if (output.Source != null) { callback(output.Source); }
                            if (output.Target != null) { callback(output.Target); }
                        }
                    }
                    if (task?.Trigger?.Event != null)
                    {
                        if (task.Trigger.Event.Source != null) { callback(task.Trigger.Event.Source); }
                        if (task.Trigger.Event.Target != null) { callback(task.Trigger.Event.Target); }
                    }
                }
            });
        }
    }
}