namespace ECS.Core
{
    public struct DirectCommandBufferConcatenator : ICommandBufferConcatenator
    {
        void ICommandBufferConcatenator.TryAddOrSetComponent<TComponenent>(in int world, in int entity, in TComponenent component) =>
            component.Register(world, entity);

        void ICommandBufferConcatenator.TryAddOrSetComponent(in int world, in int entity, in IComponent component) =>
            component.Register(world, entity);
    }
}
