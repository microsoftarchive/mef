// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition
{
    /// <summary>
    /// Marks a part as being constrainted to sharing within the named boundary.
    /// </summary>
    /// <example>
    /// [Export,
    ///  Shared("HttpRequest")]
    /// public class HttpResponseWriter { }
    /// </example>
    /// <seealso cref="SharingBoundaryAttribute"/>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SharedAttribute : Attribute, IPartMetadataAttribute
    {
        readonly string _sharingBoundaryName;

        /// <summary>
        /// Mark a part as globally shared.
        /// </summary>
        public SharedAttribute()
        {
        }

        /// <summary>
        /// Construct a <see cref="SharedAttribute"/> for the specified
        /// boundary name.
        /// </summary>
        /// <param name="sharingBoundaryName">The boundary outside of which this part is inaccessible.</param>
        public SharedAttribute(string sharingBoundaryName)
        {
            if (sharingBoundaryName == null) throw new ArgumentNullException("boundary");

            _sharingBoundaryName = sharingBoundaryName;
        }

        /// <summary>
        /// The boundary outside of which this part is inaccessible.
        /// </summary>
        public string SharingBoundaryName { get { return _sharingBoundaryName; } }

        IDictionary<string, object> IPartMetadataAttribute.Metadata 
        {
            get 
            { 
                var result = new Dictionary<string, object> {
                    { Constants.CreationPolicyPartMetadataName, CreationPolicy.Shared } };

                if (_sharingBoundaryName != null)
                    result.Add(Constants.SharedPartMetadataName, _sharingBoundaryName);

                return result;
            }
        }
    }
}
