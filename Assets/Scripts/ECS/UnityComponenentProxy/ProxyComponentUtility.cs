using System;
using System.Collections.Generic;

namespace ECS.UnityProxy
{
    public static class ProxyComponentUtility
    {
        readonly static HashSet<int> s_AllProxyComponenents;
        readonly static HashSet<int> s_PersistentComponenentTypes;

        static ProxyComponentUtility()
        {
            s_AllProxyComponenents = new HashSet<int>();
            s_PersistentComponenentTypes = new HashSet<int>();

            foreach (var type in Core.ComponentTypeUtility.GetComponenentTypes())
            {
                var typeHashCode = Core.ComponentTypeUtility.HashCodeOf(type);

                if (Attribute.GetCustomAttribute(type, typeof(ProxyComponentAttribute)) is ProxyComponentAttribute attribute)
                {
                    if (attribute.State == ProxyComponenentStateOverride.Exclude)
                        continue;

                    s_AllProxyComponenents.Add(typeHashCode);
                    if (attribute.State == ProxyComponenentStateOverride.Persistent)
                        s_PersistentComponenentTypes.Add(typeHashCode);
                }
                else
                    s_AllProxyComponenents.Add(typeHashCode);
            }
        }

        public static IReadOnlyCollection<int> GetComponenentTypeHashCodes() => s_AllProxyComponenents;
        public static IReadOnlyCollection<int> GetPersistentComponenentTypeHashCodes() => s_PersistentComponenentTypes;

        public static bool IsPersistentProxyComponent(this Type type) =>
            s_PersistentComponenentTypes.Contains(Core.ComponentTypeUtility.HashCodeOf(type));
    }
}