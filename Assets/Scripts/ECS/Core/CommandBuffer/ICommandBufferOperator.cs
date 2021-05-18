namespace ECS.Core
{
    public interface ICommandBufferOperator
    {
        void TryAddOrSetComponent<TComponenent>(in int world, in int entity, in TComponenent component) where TComponenent : struct, IComponent;
        void TryRemoveComponent<TComponenent>(in int world, in int entity) where TComponenent : struct, IComponent;
        void TryRemoveEntity(in int world, in int entity);
        void ClearForWorld(in int world);
    }
}
