namespace ECS.Core
{
    [System.Serializable]
    public struct EntityAllocatorComponenent : IComponent
    {
        public int CurrentUID => m_CurrentUID;

        [UnityEngine.SerializeField]
        int m_CurrentUID;
        public int AllocateUID => ++m_CurrentUID;
        public int NextEntityUID => m_CurrentUID + 1;

        void IComponent.Register(in int world, in int entity) =>
            ComponentMap<EntityAllocatorComponenent>.TryAddOrSet(world, entity, this);

        public static int Allocate(in int world)
        {
            Single<EntityAllocatorComponenent>.TryGet(world, out var allocator);
            var result = allocator.AllocateUID;
            Single<EntityAllocatorComponenent>.TryAddOrSet(world, allocator);
            return result;
        }

        public static EntityAllocatorComponenent Get(in int world)
        {
            Single<EntityAllocatorComponenent>.TryGet(world, out var allocator);
            return allocator;
        }

        public static void Set(in int world, in EntityAllocatorComponenent allocator) =>
            Single<EntityAllocatorComponenent>.TryAddOrSet(world, allocator);
    }
}
