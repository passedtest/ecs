using UnityEngine;

namespace ECS
{
    [System.Serializable, UnityProxy.ProxyComponentAttribute(UnityProxy.ProxyComponenentStateOverride.Persistent)]
    public struct TransformComponent : IComponent
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        void IComponent.Register(int world, int entity) => Core.ComponentMap<TransformComponent>.TryAddOrSet(world, entity, this);
    }
}
