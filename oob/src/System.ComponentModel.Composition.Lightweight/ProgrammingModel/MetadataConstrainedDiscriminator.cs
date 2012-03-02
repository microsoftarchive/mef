// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Util;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Composition.Lightweight.ProgrammingModel
{
    /// <summary>
    /// Represents a discriminator that is augmented with one or more export metadata values.
    /// </summary>
    /// <remarks>This class could probably perform better using a list of ordered key-value pairs
    /// for metadata, and could cache hash code values.</remarks>
    public sealed class MetadataConstrainedDiscriminator
    {
        readonly object _innerDiscriminator;
        readonly IDictionary<string, object> _metadata;

        /// <summary>
        /// Construct an <see cref="MetadataConstrainedDiscriminator"/>.
        /// </summary>
        /// <param name="innerDiscriminator"></param>
        /// <param name="importMetadata"></param>
        public MetadataConstrainedDiscriminator(IDictionary<string, object> importMetadata, object innerDiscriminator = null)
        {
            if (importMetadata == null) throw new ArgumentNullException("importMetadata");

            _innerDiscriminator = innerDiscriminator;
            _metadata = importMetadata;
        }

        /// <summary>
        /// The remaining (non-metadata) aspect of the discriminator.
        /// </summary>
        public object InnerDiscriminator { get { return _innerDiscriminator; } }

        /// <summary>
        /// The metadata associated with the discriminator.
        /// </summary>
        public IDictionary<string, object> Metadata { get { return _metadata; } }

        /// <summary>
        /// Retrieve a metadata value from the discriminator, returning a new discriminator
        /// without that value.
        /// </summary>
        /// <typeparam name="T">The type of the metadata value to unwrap.</typeparam>
        /// <param name="discriminator">The discriminator to unwrap.</param>
        /// <param name="importMetadataName">The metadata key to unwrap.</param>
        /// <param name="metadataValue">The unwrapped value.</param>
        /// <param name="unwrappedDiscriminator">The unwrapped discriminator.</param>
        /// <returns>True if the value was present and could be unwapped, otherwise false.</returns>
        public static bool Unwrap<T>(object discriminator, string importMetadataName,
                                     out T metadataValue, out object unwrappedDiscriminator)
        {
            metadataValue = default(T);
            unwrappedDiscriminator = null;

            if (discriminator == null)
                return false;

            var imcd = discriminator as MetadataConstrainedDiscriminator;
            if (imcd == null)
                return false;

            object value;
            if (!imcd.Metadata.TryGetValue(importMetadataName, out value))
                return false;

            if (!(value is T))
                return false;

            metadataValue = (T)value;
            if (imcd.Metadata.Count == 1)
            {
                unwrappedDiscriminator = imcd.InnerDiscriminator;
            }
            else
            {
                var newMetadata = new Dictionary<string, object>(imcd.Metadata);
                newMetadata.Remove(importMetadataName);
                unwrappedDiscriminator = new MetadataConstrainedDiscriminator(newMetadata, imcd.InnerDiscriminator);
            }

            return true;
        }

        /// <summary>
        /// Checks whether two discriminators are equivalent.
        /// </summary>
        /// <param name="obj">The other discriminator to check.</param>
        /// <returns>True if equivalent; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
                
            var imcd = obj as MetadataConstrainedDiscriminator;
            if (imcd == null)
                return false;

            return _innerDiscriminator == null ?
                (imcd._innerDiscriminator == null && MetadataEqual(_metadata, imcd._metadata)) :
                (_innerDiscriminator.Equals(imcd._innerDiscriminator) && MetadataEqual(_metadata, imcd._metadata));
        }

        /// <summary>
        /// Gets a hash code for the discriminator.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return _innerDiscriminator == null ?
                MetadataHashCode(_metadata) :
                MetadataHashCode(_metadata) ^ _innerDiscriminator.GetHashCode();
        }

        /// <summary>
        /// Gets a string representation of the discriminator.
        /// </summary>
        /// <returns>A string representing the discriminator.</returns>
        public override string ToString()
        {
            var result = string.Format("{{ {0} }}",
                string.Join(", ", _metadata.Select(kv => string.Format("{0} = {1}", kv.Key, Formatters.Format(kv.Value)))));

            if (_innerDiscriminator != null)
                result = Formatters.Format(_innerDiscriminator) + " " + result;

            return result;
        }

        internal static bool MetadataEqual(IDictionary<string, object> first, IDictionary<string, object> second)
        {
            if (first.Count != second.Count)
                return false;

            foreach (var firstItem in first)
            {
                object secondValue;
                if (!second.TryGetValue(firstItem.Key, out secondValue))
                    return false;

                if (firstItem.Value == null)
                {
                    if (secondValue != null)
                        return false;
                }
                else
                {
                    IEnumerable<object> firstEnumerable = firstItem.Value as IEnumerable<object>, secondEnumerable = secondValue as IEnumerable<object>;
                    if (firstEnumerable != null)
                    {
                        if (!Enumerable.SequenceEqual(firstEnumerable, secondEnumerable))
                            return false;
                    }
                    else if (!firstItem.Value.Equals(secondValue))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static int MetadataHashCode(IDictionary<string, object> metadata)
        {
            var result = -1;
            foreach (var kv in metadata)
            {
                result ^= kv.Key.GetHashCode();
                if (kv.Value != null)
                {
                    var enumerableValue = kv.Value as IEnumerable<object>;
                    if (enumerableValue != null)
                    {
                        foreach (var ev in enumerableValue)
                            if (ev != null)
                                result ^= ev.GetHashCode();
                    }
                    else
                    {
                        result ^= kv.Value.GetHashCode();
                    }
                }
            }

            return result;
        }
    }
}
