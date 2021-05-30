using UnityEngine;

namespace ECS
{
    [System.Serializable, UnityProxy.ProxyComponent(UnityProxy.ProxyComponenentStateOverride.Persistent)]
    public struct GameObjectProxyComponenet : IComponent
    {
        public string Name;
        public string Tag;
        public int Layer;
        public bool IsActive;
        
        void IComponent.Register(in int world, in int entity) => Core.ComponentMap<GameObjectProxyComponenet>.TryAddOrSet(world, entity, this);

        public static GameObjectProxyComponenet Create(GameObject gameObject) =>
            new GameObjectProxyComponenet()
            {
                Name = gameObject.name,
                Tag = gameObject.tag,
                Layer = gameObject.layer,
                IsActive = gameObject.activeSelf,
            };
    }
}
