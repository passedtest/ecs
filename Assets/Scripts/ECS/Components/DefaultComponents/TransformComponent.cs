using UnityEngine;

namespace ECS
{
    [System.Serializable, UnityProxy.ProxyComponent(UnityProxy.ProxyComponenentStateOverride.Persistent)]
    public struct TransformComponent : IComponent
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        void IComponent.Register(in int world, in int entity) => Core.ComponentMap<TransformComponent>.TryAddOrSet(world, entity, this);

        public static TransformComponent Create(Transform transform) =>
            new TransformComponent()
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.localScale,
            };
    }
}
