using System;
using System.Collections.Generic;

namespace ECS.Core
{
    public struct TypeDefinition
    {
        public readonly Type Type;
        public readonly int HashCode;

        public TypeDefinition(Type type)
        {
            Type = type;
            HashCode = ComponentTypeUtility.HashCodeOf(type);
        }
    }

    public static class ComponentTypeUtility
    {
        readonly static Type s_ComponenentInterfaceType;

        readonly static Dictionary<int, int> s_HashCodeLookup;
        readonly static Dictionary<int, Type> s_TypeLookup;

        public static IReadOnlyDictionary<int, Type> Types => s_TypeLookup;

        static ComponentTypeUtility()
        {
            s_ComponenentInterfaceType = typeof(IComponent);

            s_HashCodeLookup = new Dictionary<int, int>();
            s_TypeLookup = new Dictionary<int, Type>();

            RebuildTypeLookups();
        }

        public static int HashCodeOf<TComponent>() where TComponent : struct, IComponent =>
            HashCodeOf(typeof(TComponent));

        public static int HashCodeOf(in Type type)
        {
            if (!type.IsComponenentType())
                throw new InvalidOperationException($"{type.FullName} doesn't implements {s_ComponenentInterfaceType.FullName}");

            var currentCode = type.GetHashCode();
            if (!s_HashCodeLookup.TryGetValue(currentCode, out var actualHashCode))
                s_HashCodeLookup.Add(currentCode, actualHashCode = ProduseHashCode(type));

            if (!s_TypeLookup.ContainsKey(actualHashCode))
                s_TypeLookup.Add(actualHashCode, type);

            return actualHashCode;
        }

        public static bool TryGetType(in string fullName, out Type type) => s_TypeLookup.TryGetValue(ProduseHashCode(fullName), out type);
        public static bool TryGetType(in int hashCode, out Type type) => s_TypeLookup.TryGetValue(hashCode, out type);

        static int ProduseHashCode(in Type type) => ProduseHashCode(type.FullName);
        static int ProduseHashCode(in string typeFullName) => typeFullName.GetHashCode();

        static void RebuildTypeLookups()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    if (type.IsComponenentType())
                    {
                        HashCodeOf(type);

                        if (Attribute.GetCustomAttribute(type, typeof(RequireGenericComponenentTypeAttribute)) is RequireGenericComponenentTypeAttribute genericComponenentRequireAttribute)
                        {
                            foreach (var genericComponenentType in genericComponenentRequireAttribute.Types)
                            {
                                if (!genericComponenentType.IsGenericType)
                                    throw new InvalidOperationException();

                                if (!genericComponenentType.IsComponenentType())
                                    throw new InvalidOperationException($"{genericComponenentType.FullName} is not a componenent type");

                                var genericArguments = genericComponenentType.GetGenericArguments();
                                foreach (var genericArgument in genericArguments)
                                    if (!genericArgument.IsValueType)
                                        throw new InvalidOperationException();

                                HashCodeOf(genericComponenentType);
                            }
                        }
                    }
        }

        public static bool IsComponenentType(this Type type) =>
            s_ComponenentInterfaceType.IsAssignableFrom(type) && !type.Equals(s_ComponenentInterfaceType);

        public static IReadOnlyCollection<Type> GetComponenentTypes() => s_TypeLookup.Values;
    }

    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
    public class RequireGenericComponenentTypeAttribute : Attribute
    {
        public readonly Type[] Types;

        public RequireGenericComponenentTypeAttribute(params Type[] targetTypes)
        {
            Types = targetTypes;
        }
    }
}
