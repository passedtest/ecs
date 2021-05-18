namespace ECS.Core
{
    public interface ISystem
    {
        void ExecuteUpdate(in ECSWorld world, in float deltaTime);
        void ExecuteFixedUpdate(in ECSWorld world, in float deltaTime);
        void ExecuteGizmoUpdate(ECSWorld world, in float deltaTime);
    }
}
