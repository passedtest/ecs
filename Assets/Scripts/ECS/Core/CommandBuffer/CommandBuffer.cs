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
            ScheduleCommand(new AddComponentCommand(world, entity, component));

        public void TryAddOrSetComponent(in int world, in int entity, in IComponent component) =>
            ScheduleCommand(new AddComponentCommand(world, entity, component));

        public void TryRemoveComponent<TComponenent>(in int world, in int entity) where TComponenent : struct, IComponent =>
            ScheduleCommand(new RemoveComponentCommand<TComponenent>(world, entity));

        public void TryRemoveEntity(in int world, in int entity) =>
            ScheduleCommand(new RemoveEntityCommand(world, entity));

        public void ClearForWorld(in int world) =>
            ScheduleCommand(new RemoveWorldEntitiesCommand(world));

        public void ScheduleCommand(ICommand command) => m_Commands.Enqueue(command);
    }
}
