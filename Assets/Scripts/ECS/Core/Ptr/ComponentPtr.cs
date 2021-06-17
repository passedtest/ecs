namespace ECS.Core.Ptr
{
    public struct ComponentPtr<TComponenent> where TComponenent : struct, IComponent
    {
        public bool IsValid => ComponentMap<TComponenent>.TryGet(m_EntityPtr.World, m_EntityPtr.Entity, out _);

        readonly EntityPtr m_EntityPtr;

        ComponentPtr(in EntityPtr entityPtr)
        {
            m_EntityPtr = entityPtr;
        }

        public TComponenent Get()
        {
            if (!ComponentMap<TComponenent>.TryGet(m_EntityPtr.World, m_EntityPtr.Entity, out var componenent))
                throw new System.InvalidOperationException();

            return componenent;
        }

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
