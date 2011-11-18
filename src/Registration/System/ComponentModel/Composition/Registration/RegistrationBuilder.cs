using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Registration
{
    public class RegistrationBuilder : System.Reflection.Context.CustomReflectionContext
    {
#if FEATURE_INTERNAL_REFLECTIONCONTEXT
        internal class InnerRC : System.Reflection.ReflectionContext {
            public override Type MapType(Type t) { return t; }
            public override Assembly MapAssembly(Assembly a) { return a; }
        }
#else
        internal class InnerRC : System.Reflection.ReflectionContext
        {
            public override TypeInfo MapType(TypeInfo t) { return t; }
            public override Assembly MapAssembly(Assembly a) { return a; }
        }

#endif

        private static readonly ReflectionContext _inner = new InnerRC();
        private static readonly List<object> EmptyList = new List<object>();

        private Lock _lock = new Lock();
        private List<PartBuilder> _conventions = new List<PartBuilder>();

        private Dictionary<MemberInfo, List<Attribute>> _memberInfos = new Dictionary<MemberInfo, List<Attribute>>();
        private Dictionary<ParameterInfo, List<Attribute>> _parameters = new Dictionary<ParameterInfo, List<Attribute>>();

        public RegistrationBuilder() : base(_inner)
        {
        }

        public PartBuilder<T> ForTypesDerivedFrom<T>()
        {
            var partBuilder = new PartBuilder<T>((t) => typeof(T) != t && typeof(T).IsAssignableFrom(t));
            this._conventions.Add(partBuilder);
            return partBuilder;
        }

        public PartBuilder ForTypesDerivedFrom(Type type)
        {
            Requires.NotNull(type, "type");

            var partBuilder = new PartBuilder((t) => type != t && type.IsAssignableFrom(t));
            this._conventions.Add(partBuilder);
            return partBuilder;
        }

        public PartBuilder<T> ForType<T>()
        {
            var partBuilder = new PartBuilder<T>((t) => t == typeof(T));
            this._conventions.Add(partBuilder);
            return partBuilder;
        }

        public PartBuilder ForType(Type type)
        {
            Requires.NotNull(type, "type");

            var partBuilder = new PartBuilder((t) => t == type);
            this._conventions.Add(partBuilder);
            return partBuilder;
        }

        public PartBuilder<T> ForTypesMatching<T>(Predicate<Type> typeFilter)
        {
            Requires.NotNull(typeFilter, "typeFilter");

            var partBuilder = new PartBuilder<T>(typeFilter);
            this._conventions.Add(partBuilder);
            return partBuilder;
        }

        public PartBuilder ForTypesMatching(Predicate<Type> typeFilter)
        {
            Requires.NotNull(typeFilter, "typeFilter");

            var partBuilder = new PartBuilder(typeFilter);
            this._conventions.Add(partBuilder);
            return partBuilder;
        }

        private IEnumerable<Tuple<object, List<Attribute>>> EvaluateThisTypeAgainstTheConvention(Type type)
        {
            List<Tuple<object, List<Attribute>>> results = new List<Tuple<object, List<Attribute>>>();
            List<Attribute> attributes = new List<Attribute>();

            var configuredMembers = new List<Tuple<object, List<Attribute>>>();
            bool specifiedConstructor = false;
            bool matchedConvention = false;
            foreach (var builder in this._conventions.Where(c => c.SelectType(type.UnderlyingSystemType)))
            {
                attributes.AddRange(builder.BuildTypeAttributes(type));

                specifiedConstructor |= builder.BuildConstructorAttributes(type, ref configuredMembers);
                builder.BuildPropertyAttributes(type, ref configuredMembers);
                matchedConvention = true;
            }
            if (matchedConvention && !specifiedConstructor)
            {
                // DefaultConstructor
                PartBuilder.BuildDefaultConstructorAttributes(type, ref configuredMembers);
            }
            configuredMembers.Add(Tuple.Create((object)type, attributes));
            return configuredMembers;
        }

        // Handle Type Exports and Parts
        protected override IEnumerable<object> GetCustomAttributes(System.Reflection.MemberInfo member, IEnumerable<object> declaredAttributes)
        {
            var attributes = base.GetCustomAttributes(member, declaredAttributes);

            // Now edit the attributes returned from the base type
            List<Attribute> cachedAttributes = null;
            if (member.MemberType == MemberTypes.TypeInfo || member.MemberType == MemberTypes.NestedType)
            {
                MemberInfo underlyingMemberType = ((Type)member).UnderlyingSystemType;
                using (new ReadLock(this._lock))
                {
                    this._memberInfos.TryGetValue(underlyingMemberType, out cachedAttributes);
                }
                if (cachedAttributes == null)
                {
                    using (new WriteLock(this._lock))
                    {
                        //Double check locking another thread may have inserted one while we were away.
                        if (!this._memberInfos.TryGetValue(underlyingMemberType, out cachedAttributes))
                        {
                            List<Attribute> attributeList;
                            foreach (var element in EvaluateThisTypeAgainstTheConvention((Type)member))
                            {
                                attributeList = element.Item2;
                                if (attributeList != null)
                                {
                                    if (element.Item1 is MemberInfo)
                                    {
                                        List<Attribute> memberAttributes;
                                        switch (((MemberInfo)element.Item1).MemberType)
                                        {
                                            case MemberTypes.Constructor:
                                                if (!this._memberInfos.TryGetValue((MemberInfo)element.Item1, out memberAttributes))
                                                {
                                                    this._memberInfos.Add((MemberInfo)element.Item1, element.Item2);
                                                }
                                                else
                                                {
                                                    memberAttributes.AddRange(attributeList);
                                                }
                                                break;
                                            case MemberTypes.TypeInfo:
                                            case MemberTypes.NestedType:
                                            case MemberTypes.Property:
                                                if (!this._memberInfos.TryGetValue((MemberInfo)element.Item1, out memberAttributes))
                                                {
                                                    this._memberInfos.Add((MemberInfo)element.Item1, element.Item2);
                                                }
                                                else
                                                {
                                                    memberAttributes.AddRange(attributeList);
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        Assumes.IsTrue(element.Item1 is ParameterInfo);
                                        List<Attribute> parameterAttributes;
                                        // Item contains as Constructor parameter to configure
                                        if (!this._parameters.TryGetValue((ParameterInfo)element.Item1, out parameterAttributes))
                                        {
                                            this._parameters.Add((ParameterInfo)element.Item1, element.Item2);
                                        }
                                        else
                                        {
                                            parameterAttributes.AddRange(cachedAttributes);
                                        }
                                    }
                                }
                            }
                        }

                        // We will have updated all of the MemberInfos by now so lets reload cachedAttributes wiuth the current store
                        this._memberInfos.TryGetValue(underlyingMemberType, out cachedAttributes);
                    }
                }
            }
            else if (member.MemberType == System.Reflection.MemberTypes.Constructor || member.MemberType == System.Reflection.MemberTypes.Property)
            {
                cachedAttributes = ReadMemberCustomAttributes(member);
            }
            return cachedAttributes == null ? attributes : attributes.Concat(cachedAttributes);
        }

        //This is where ParameterImports will be handled
        protected override IEnumerable<object> GetCustomAttributes(System.Reflection.ParameterInfo parameter, IEnumerable<object> declaredAttributes)
        {
            var attributes = base.GetCustomAttributes(parameter, declaredAttributes);
            List<Attribute> cachedAttributes = ReadParameterCustomAttributes(parameter);
            return cachedAttributes == null ? attributes : attributes.Concat(cachedAttributes);
        }

        private List<Attribute> ReadMemberCustomAttributes(System.Reflection.MemberInfo member)
        {
            List<Attribute> cachedAttributes = null;
            bool getMemberAttributes = false;

            // Now edit the attributes returned from the base type
            using (new ReadLock(this._lock))
            {
                if (!this._memberInfos.TryGetValue(member, out cachedAttributes))
                {
                    // If there is nothing for this member Cache any attributes for the DeclaringType
                    if (!this._memberInfos.TryGetValue(member.DeclaringType.UnderlyingSystemType, out cachedAttributes))
                    {
                        // If there is nothing for this parameter look to see if the declaring Member has been cached yet?
                        // need to do it outside of the lock, so set the flag we'll check it in a bit
                        getMemberAttributes = true;
                    }
                    cachedAttributes = null;
                }
            }

            if (getMemberAttributes)
            {
                GetCustomAttributes(member.DeclaringType, EmptyList);

                // We should have run the rules for the enclosing parameter so we can again
                using (new ReadLock(this._lock))
                {
                    this._memberInfos.TryGetValue(member, out cachedAttributes);
                }
            }

            return cachedAttributes;
        }


        private List<Attribute> ReadParameterCustomAttributes(System.Reflection.ParameterInfo parameter)
        {
            List<Attribute> cachedAttributes = null;
            bool getMemberAttributes = false;

            // Now edit the attributes returned from the base type
            using (new ReadLock(this._lock))
            {
                if (!this._parameters.TryGetValue(parameter, out cachedAttributes))
                {
                    // If there is nothing for this parameter Cache any attributes for the DeclaringType
                    if (!this._memberInfos.TryGetValue(parameter.Member.DeclaringType, out cachedAttributes))
                    {
                        // If there is nothing for this parameter look to see if the declaring Member has been cached yet?
                        // need to do it outside of the lock, so set the flag we'll check it in a bit
                        getMemberAttributes = true;
                    }
                    cachedAttributes = null;
                }
            }

            if (getMemberAttributes)
            {
                GetCustomAttributes(parameter.Member.DeclaringType, EmptyList);

                // We should have run the rules for the enclosing parameter so we can again
                using (new ReadLock(this._lock))
                {
                    this._parameters.TryGetValue(parameter, out cachedAttributes);
                }
            }

            return cachedAttributes;
        }
    }
}
