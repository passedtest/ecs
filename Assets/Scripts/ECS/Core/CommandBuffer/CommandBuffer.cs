using System.Collections.Generic;

namespace ECS.Core
{
    public sealed class CommandBuffer : ICommandBufferOperator
    {
        readonly Queue<ICommand> m_Commands;

        public CommandBuffer() 
        {
            m_Commands = new Queue<ICommand>();
        }

        public void Execute()
        {
            while (m_Commands.Count > 0)
                m_Commands
                    .Dequeue()
                    .Execute();
        }

        public void TryAddOrSetComponent<TComponenent>(in int world, in int entity, in TComponenent component) where TComponenent : struct, IComponent =>
            m_Commands.Enqueue(new AddComponentCommand<TComponenent>(world, entity, component));

        public void TryRemoveComponent<TComponenent>(in int world, in int entity) where TComponenent : struct, IComponent =>
            m_Commands.Enqueue(new RemoveComponentCommand<TComponenent>(world, entity));

        public void TryRemoveEntity(in int world, in int entity) =>
            m_Commands.Enqueue(new RemoveEntityCommand(world, entity));
        public void ClearForWorld(in int world) =>
            m_Commands.Enqueue(new RemoveWorldEntitiesCommand(world));
    }
}
