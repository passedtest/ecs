using System.Collections.Generic;

namespace ECS
{
    public abstract class BaseSystem : Core.ISystem
    {
        static List<int> s_EntityIterationBuffer;

        public Core.ICommandBufferOperator UpdateCommandBuffer => m_UpdateCommandBuffer;
        readonly Core.CommandBuffer m_UpdateCommandBuffer;

        public Core.ICommandBufferOperator FixedUpdateCommandBuffer => m_FixedUpdateCommandBuffer;
        readonly Core.CommandBuffer m_FixedUpdateCommandBuffer;

        protected BaseSystem()
        {
            m_UpdateCommandBuffer = new Core.CommandBuffer();
            m_FixedUpdateCommandBuffer = new Core.CommandBuffer();
        }

        protected IReadOnlyList<int> GetEntitySafeBuffer(in ECSWorld world)
        {
            if(s_EntityIterationBuffer == null)
                s_EntityIterationBuffer = new List<int>(1000);

            s_EntityIterationBuffer.Clear();
            s_EntityIterationBuffer.AddRange(world.Entities);

            return s_EntityIterationBuffer;
        }

        void Core.ISystem.ExecuteUpdate(in ECSWorld world, in float deltaTime)
        {
            m_UpdateCommandBuffer.Execute();
            OnExecuteUpdate(world, deltaTime);
        }

        void Core.ISystem.ExecuteFixedUpdate(in ECSWorld world, in float deltaTime)
        {
            m_FixedUpdateCommandBuffer.Execute();
            OnExecuteFixedUpdate(world, deltaTime);
        }

        void Core.ISystem.ExecuteGizmoUpdate(ECSWorld world, in float deltaTime) =>
            OnExecuteGizmoUpdate(world, deltaTime);

        protected virtual void OnExecuteUpdate(in ECSWorld world, in float deltaTime) { }
        protected virtual void OnExecuteFixedUpdate(in ECSWorld world, in float deltaTime) { }
        protected virtual void OnExecuteGizmoUpdate(in ECSWorld world, in float deltaTime) { }
    }
}
