namespace ECS.Core
{
    public struct AddComponentCommand<TComponent> : ICommand where TComponent : struct, IComponent
    {
        readonly int m_World;
        readonly int m_Entity;
        readonly TComponent m_Component;

        public AddComponentCommand(in int world, in int entity, in TComponent componenent)
        {
            m_World = world;
            m_Entity = entity;
            m_Component = componenent;
        }

        void ICommand.Execute() => m_Component.Register(m_World, m_Entity);
    }

    public struct AddComponentCommand : ICommand
    {
        readonly int m_World;
        readonly int m_Entity;
        readonly IComponent m_Component;

        public AddComponentCommand(in int world, in int entity, in IComponent componenent)
        {
            m_World = world;
            m_Entity = entity;
            m_Component = componenent;
        }

        void ICommand.Execute() => m_Component.Register(m_World, m_Entity);
    }
}
