using System;

namespace ECS.UnityProxy
{
    public enum ProxyComponenentStateOverride
    {
        None,
        Persistent,
        Exclude,
    }

    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class ProxyComponentAttribute : Attribute
    {
        public readonly ProxyComponenentStateOverride State;
        public ProxyComponentAttribute(ProxyComponenentStateOverride state)
        {
            State = state;
        }
    }
}
