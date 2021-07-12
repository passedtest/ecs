namespace ECS.Core.Ptr
{
    public struct ComponentPtr<TComponenent> : IPtr where TComponenent : struct, IComponent
    {
        public bool IsValid => ComponentMap<TComponenent>.ComponentExists(m_EntityPtr.World, m_EntityPtr.Entity);

        readonly EntityPtr m_EntityPtr;

        ComponentPtr(in EntityPtr entityPtr)
        {
            m_EntityPtr = entityPtr;
        }

        public TComponenent Get()
        {
            if (!TryGet(out var component))
                throw new System.InvalidOperationException();

            return component;
        }

        public bool TryGet(out TComponenent component) =>
            ComponentMap<TComponenent>.TryGet(m_EntityPtr.World, m_EntityPtr.Entity, out component);

        public void Set(in TComponenent component) =>
            ComponentMap<TComponenent>.TryAddOrSet(m_EntityPtr.World, m_EntityPtr.Entity, component);

        public static ComponentPtr<TComponenent> New(EntityPtr entity) =>
            new ComponentPtr<TComponenent>(entity);

        public static implicit operator TComponenent(ComponentPtr<TComponenent> componentPtr) =>
            componentPtr.Get();

        public static implicit operator EntityPtr(ComponentPtr<TComponenent> componentPtr) =>
            componentPtr.m_EntityPtr;
    }
}
