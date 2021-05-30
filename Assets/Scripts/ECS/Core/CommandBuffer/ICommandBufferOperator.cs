namespace ECS.Core
{
    public interface ICommandBufferOperator : ICommandBufferCommonModificator, ICommandBufferConcatenator
    {
        void TryRemoveComponent<TComponenent>(in int world, in int entity) where TComponenent : struct, IComponent;
        void TryRemoveEntity(in int world, in int entity);
        void ClearForWorld(in int world);
    }

    public interface ICommandBufferConcatenator
    {
        void TryAddOrSetComponent<TComponenent>(in int world, in int entity, in TComponenent component) where TComponenent : struct, IComponent;
        void TryAddOrSetComponent(in int world, in int entity, in IComponent component);
    }

    public interface ICommandBufferCommonModificator
    {
        void ScheduleCommand(ICommand command);
    }
}
