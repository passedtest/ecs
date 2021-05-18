namespace ECS.Core
{
    public struct RemoveWorldEntitiesCommand : ICommand
    {
        readonly int m_World;

        public RemoveWorldEntitiesCommand(in int world)
        {
            m_World = world;
        }

        void ICommand.Execute() => SharedComponentMap.ClearForWorld(m_World);
    }
}

