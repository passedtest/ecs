namespace ECS.Core
{
    public struct RemoveComponentCommand<TComponent> : ICommand where TComponent : struct, IComponent
    {
        readonly int m_World;
        readonly int m_Entity;

        public RemoveComponentCommand(in int world, in int entity)
        {
            m_World = world;
            m_Entity = entity;
        }

        void ICommand.Execute() => ComponentMap<TComponent>.Remove(m_World, m_Entity);
    }
}

