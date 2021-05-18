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
            var currentCode = type.GetHashCode();
            if (!s_HashCodeLookup.TryGetValue(currentCode, out var actualHashCode))
            {
                if (!type.IsComponenentType())
                    throw new InvalidOperationException($"{type.FullName} doesn't implements {s_ComponenentInterfaceType.FullName}");

                s_HashCodeLookup.Add(currentCode, actualHashCode = ProduseHashCode(type));
            }

            return actualHashCode;
        }

        public static bool TryGetType(in int hashCode, out Type type) => s_TypeLookup.TryGetValue(hashCode, out type);

        static int ProduseHashCode(in Type type) => type.FullName.GetHashCode();

        static void RebuildTypeLookups()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) 
                foreach(var type in assembly.GetTypes())
                    if (type.IsComponenentType())
                    {
                        var produsedHashCode = ProduseHashCode(type);
                        s_HashCodeLookup.Add(type.GetHashCode(), produsedHashCode);
                        s_TypeLookup.Add(produsedHashCode, type);
                    }
        }

        public static bool IsComponenentType(this Type type) =>
            s_ComponenentInterfaceType.IsAssignableFrom(type);
    }
}
