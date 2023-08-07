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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;

namespace CycloneDX.Models
{
    [Serializable]
    public class BomEntityConflictException : Exception
    {
        public BomEntityConflictException()
            : base(String.Format("Unresolvable conflict in Bom entities"))
        { }

        public BomEntityConflictException(Type type)
            : base(String.Format("Unresolvable conflict in Bom entities of type {0}", type.ToString()))
        { }

        public BomEntityConflictException(string msg)
            : base(String.Format("Unresolvable conflict in Bom entities: {0}", msg))
        { }

        public BomEntityConflictException(string msg, Type type)
            : base(String.Format("Unresolvable conflict in Bom entities of type {0}: {1}", type.ToString(), msg))
        { }
    }

    [Serializable]
    public class BomEntityIncompatibleException : Exception
    {
        public BomEntityIncompatibleException()
            : base(String.Format("Comparing incompatible Bom entities"))
        { }

        public BomEntityIncompatibleException(Type type1, Type type2)
            : base(String.Format("Comparing incompatible Bom entities of types {0} and {1}", type1.ToString(), type2.ToString()))
        { }

        public BomEntityIncompatibleException(string msg)
            : base(String.Format("Comparing incompatible Bom entities: {0}", msg))
        { }

        public BomEntityIncompatibleException(string msg, Type type1, Type type2)
            : base(String.Format("Comparing incompatible Bom entities of types {0} and {1}: {2}", type1.ToString(), type2.ToString(), msg))
        { }
    }

    /// <summary>
    /// BomEntity is intended as a base class for other classes in CycloneDX.Models,
    /// which in turn encapsulate different concepts and data types described by
    /// the specification. It allows them to share certain behaviors such as the
    /// ability to determine "equivalent but not equal" objects (e.g. two instances
    /// of a Component with the same "bom-ref" but different in some properties),
    /// and to define the logic for merge-ability of such objects while coding much
    /// of the logical scaffolding only once.
    /// </summary>
    public class BomEntity : IEquatable<BomEntity>
    {
        protected BomEntity()
        {
            // a bad alternative to private is to: throw new NotImplementedException("The BomEntity class directly should not be instantiated");
        }

        public bool Equals(BomEntity other)
        {
            if (other is null || this.GetType() != other.GetType()) return false;
            return CycloneDX.Json.Serializer.Serialize(this) == CycloneDX.Json.Serializer.Serialize(other);
        }
    
        public override int GetHashCode()
        {
            return CycloneDX.Json.Serializer.Serialize(this).GetHashCode();
        }

        /// <summary>
        /// Do this and other objects describe the same real-life entity?
        /// Override this in sub-classes that have a more detailed definition of
        /// equivalence (e.g. that certain fields are equal even if whole contents
        /// are not).
        /// </summary>
        /// <param name="other">Another object of same type</param>
        /// <returns>True if two data objects are considered to represent
        /// the same real-life entity, False otherwise.</returns>
        public bool Equivalent(BomEntity other)
        {
            return (!(other is null) && (this.GetType() == other.GetType()) && this.Equals(other));
        }

        /// <summary>
        /// Default implementation just "agrees" that Equals()==true objects
        /// are already merged (returns true), and that Equivalent()==false
        /// objects are not (returns false), and for others (equivalent but
        /// not equal, or different types) raises an exception.
        /// Treats a null "other" object as a success (it is effectively a
        /// no-op merge, which keeps "this" object as is).
        /// </summary>
        /// <param name="other">Another object of same type whose additional
        /// non-conflicting data we try to squash into this object.</param>
        /// <returns>True if merge was successful, False if it these objects
        /// are not equivalent, or throws if merge can not be done (including
        /// lack of merge logic or unresolvable conflicts in data points).
        /// </returns>
        /// <exception cref="BomEntityConflictException">Source data problem: two entities with conflicting information</exception>
        /// <exception cref="BomEntityIncompatibleException">Caller error: somehow merging different entity types</exception>
        public bool MergeWith(BomEntity other)
        {
            if (other is null) return true;
            if (this.GetType() != other.GetType())
            {
                // Note: potentially descendent classes can catch this
                // to adapt their behavior... if some two different
                // classes would ever describe something comparable
                // in real life.
                throw new BomEntityIncompatibleException(this.GetType(), other.GetType());
            }

            if (this.Equals(other)) return true;
            if (!this.Equivalent(other)) return false;

            // Normal mode of operation: descendant classes catch this
            // exception to use their custom non-trivial merging logic.
            throw new BomEntityConflictException(
                "Base-method implementation treats equivalent but not equal entities as conflicting",
                this.GetType());
        }
    }
}
