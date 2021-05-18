namespace ECS.Core
{
    public struct RemoveEntityCommand : ICommand
    {
        readonly int m_World;
        readonly int m_Entity;

        public RemoveEntityCommand(in int world, in int entity)
        {
            m_World = world;
            m_Entity = entity;
        }

        void ICommand.Execute() => SharedComponentMap.ClearForEntity(m_World, m_Entity);
    }
}

