using System.Collections.Generic;

namespace ECS
{
    public abstract class BaseSystem : Core.ISystem
    {
        public Core.ICommandBufferOperator BeforeUpdateCommandBuffer => m_BeforeUpdateCommandBuffer;
        readonly Core.CommandBuffer m_BeforeUpdateCommandBuffer;

        public Core.ICommandBufferOperator AfterUpdateCommandBuffer => m_AfterUpdateCommandBuffer;
        readonly Core.CommandBuffer m_AfterUpdateCommandBuffer;

        public Core.ICommandBufferOperator FixedUpdateCommandBuffer => m_FixedUpdateCommandBuffer;
        readonly Core.CommandBuffer m_FixedUpdateCommandBuffer;

        protected BaseSystem()
        {
            m_BeforeUpdateCommandBuffer = new Core.CommandBuffer();
            m_AfterUpdateCommandBuffer = new Core.CommandBuffer();

            m_FixedUpdateCommandBuffer = new Core.CommandBuffer();
        }

        void Core.ISystem.ExecuteUpdate(in ECSWorld world, in float deltaTime)
        {
            m_BeforeUpdateCommandBuffer.Execute();
            OnExecuteUpdate(world, deltaTime);
            m_AfterUpdateCommandBuffer.Execute();
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
