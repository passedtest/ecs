using System.Collections.Generic;

namespace ECS
{
    public sealed class MasterSystem : BaseSystem
    {
        readonly List<Core.ISystem> m_Systems;

        public MasterSystem(params Core.ISystem[] systems)
        {
            m_Systems = new List<Core.ISystem>(systems);
        }

        protected override void OnExecuteUpdate(in ECSWorld world, in float deltaTime)
        {
            base.OnExecuteUpdate(world, deltaTime);
            foreach (var system in m_Systems)
                system.ExecuteUpdate(world, deltaTime);
        }

        protected override void OnExecuteFixedUpdate(in ECSWorld world, in float deltaTime)
        {
            base.OnExecuteFixedUpdate(world, deltaTime);
            foreach (var system in m_Systems)
                system.ExecuteFixedUpdate(world, deltaTime);
        }

        protected override void OnExecuteGizmoUpdate(in ECSWorld world, in float deltaTime)
        {
            base.OnExecuteGizmoUpdate(world, deltaTime);
            foreach (var system in m_Systems)
                system.ExecuteGizmoUpdate(world, deltaTime);
        }
    }
}
