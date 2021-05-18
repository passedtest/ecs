using System;

namespace ECS
{
    public abstract class ExtendedSystem : BaseSystem
    {
        public delegate LoopIterationResult IterationDelegate(int entity, int counter);

        protected void ForEachEntityParallel(ECSWorld world, System.Action<int> entityProcessor) =>
             System.Threading.Tasks.Parallel.ForEach(world.Entities, entityProcessor);

        protected void ForEachEntity(ECSWorld world, IterationDelegate processor)
        {
            var cnt = 0;
            foreach (var targetEntity in world.Entities)
            {
                var resutl = processor(targetEntity, cnt);
                cnt++;
                if (resutl == LoopIterationResult.Brake)
                    break;
            }
        }

        public delegate LoopIterationResult T0_Processor<T0>(int entity, int index, T0 component);
        public delegate LoopIterationResult T0_ProcessorRef<T0>(int entity, int index, ref T0 component);

        protected void ForEachEntityWith<T0>(ECSWorld world, T0_Processor<T0> processor) 
            where T0 : struct, IComponent
        {
            var T0Components = world.GetComponentsWithEntityReadOnly<T0>();

            ForEachEntity(world, (entity, index) =>
            {
                var hasT0Component = T0Components.TryGetComponent(entity, out var T0Component);

                var result = LoopIterationResult.None;
                if (hasT0Component)
                    result = processor(entity, index, T0Component);

                return result;
            });
        }

        protected void ForEachEntityWithRef<T0>(ECSWorld world, T0_ProcessorRef<T0> processor)
            where T0 : struct, IComponent
        {
            var T0Components = world.GetComponentsWithEntity<T0>();

            ForEachEntity(world, (entity, index) =>
            {
                var hasT0Component = T0Components.TryGetComponent(entity, out var T0Component);

                var result = LoopIterationResult.None;
                if (hasT0Component)
                {
                    result = processor(entity, index, ref T0Component);
                    T0Components.TrySetComponent(entity, T0Component);
                }

                return result;
            });
        }

        protected void ForEachEntityWith<T0, T1>(ECSWorld world, System.Func<int, int, T0, T1, LoopIterationResult> processor) 
            where T0 : struct, IComponent
            where T1 : struct, IComponent
        {
            var T0Components = world.GetComponentsWithEntityReadOnly<T0>();
            var T1Components = world.GetComponentsWithEntityReadOnly<T1>();

            ForEachEntity(world, (entity, index) =>
            {
                var hasT0Component = T0Components.TryGetComponent(entity, out var T0Component);
                var hasT1Component = T1Components.TryGetComponent(entity, out var T1Component);

                var result = LoopIterationResult.None;
                if (hasT0Component && hasT1Component)
                    result = processor(entity, index, T0Component, T1Component);

                return result;
            });
        }

        protected void ForEachEntityWith<T0, T1, T2>(ECSWorld world, System.Func<int, int, T0, T1, T2, LoopIterationResult> processor)
            where T0 : struct, IComponent
            where T1 : struct, IComponent
            where T2 : struct, IComponent
        {
            var T0Components = world.GetComponentsWithEntityReadOnly<T0>();
            var T1Components = world.GetComponentsWithEntityReadOnly<T1>();
            var T2Components = world.GetComponentsWithEntityReadOnly<T2>();

            ForEachEntity(world, (entity, index) =>
            {
                var hasT0Component = T0Components.TryGetComponent(entity, out var T0Component);
                var hasT1Component = T1Components.TryGetComponent(entity, out var T1Component);
                var hasT2Component = T2Components.TryGetComponent(entity, out var T2Component);

                var result = LoopIterationResult.None;
                if (hasT0Component && hasT1Component && hasT2Component)
                    result = processor(entity, index, T0Component, T1Component, T2Component);

                return result;
            });
        }

        protected void ForEachEntityWith<T0, T1, T2, T3>(ECSWorld world, System.Func<int, int, T0, T1, T2, T3, LoopIterationResult> processor)
            where T0 : struct, IComponent
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
        {
            var T0Components = world.GetComponentsWithEntityReadOnly<T0>();
            var T1Components = world.GetComponentsWithEntityReadOnly<T1>();
            var T2Components = world.GetComponentsWithEntityReadOnly<T2>();
            var T3Components = world.GetComponentsWithEntityReadOnly<T3>();

            ForEachEntity(world, (entity, index) =>
            {
                var hasT0Component = T0Components.TryGetComponent(entity, out var T0Component);
                var hasT1Component = T1Components.TryGetComponent(entity, out var T1Component);
                var hasT2Component = T2Components.TryGetComponent(entity, out var T2Component);
                var hasT3Component = T3Components.TryGetComponent(entity, out var T3Component);

                var result = LoopIterationResult.None;
                if (hasT0Component && hasT1Component && hasT2Component && hasT3Component)
                    result = processor(entity, index, T0Component, T1Component, T2Component, T3Component);

                return result;
            });
        }

        protected void ForEachEntityWith<T0, T1, T2, T3, T4>(ECSWorld world, System.Func<int, int, T0, T1, T2, T3, T4, LoopIterationResult> processor)
            where T0 : struct, IComponent
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
        {
            var T0Components = world.GetComponentsWithEntityReadOnly<T0>();
            var T1Components = world.GetComponentsWithEntityReadOnly<T1>();
            var T2Components = world.GetComponentsWithEntityReadOnly<T2>();
            var T3Components = world.GetComponentsWithEntityReadOnly<T3>();
            var T4Components = world.GetComponentsWithEntityReadOnly<T4>();

            ForEachEntity(world, (entity, index) =>
            {
                var hasT0Component = T0Components.TryGetComponent(entity, out var T0Component);
                var hasT1Component = T1Components.TryGetComponent(entity, out var T1Component);
                var hasT2Component = T2Components.TryGetComponent(entity, out var T2Component);
                var hasT3Component = T3Components.TryGetComponent(entity, out var T3Component);
                var hasT4Component = T4Components.TryGetComponent(entity, out var T4Component);

                var result = LoopIterationResult.None;
                if (hasT0Component && hasT1Component && hasT2Component && hasT3Component && hasT4Component)
                    result = processor(entity, index, T0Component, T1Component, T2Component, T3Component, T4Component);

                return result;
            });
        }

        public enum LoopIterationResult
        {
            None,
            Brake,
        }
    }

    public class EntityIterator
    {
        public delegate void ComponentProcessor<TComponent>(TComponent componennt) where TComponent : struct, IComponent;
        public delegate void ComponentReferenceProcessor<TComponent>(ref TComponent componennt) where TComponent : struct, IComponent;

        readonly System.Collections.Generic.List<IProcessor> m_Sorters;

        public EntityIterator() 
        {
            m_Sorters = new System.Collections.Generic.List<IProcessor>();
        }

        public void With<TComponent>(ComponentProcessor<TComponent> processor) where TComponent : struct, IComponent =>
            m_Sorters.Add(new WithProcessor<TComponent>(processor));

        public void WithRef<TComponent>(ComponentReferenceProcessor<TComponent> processor) where TComponent : struct, IComponent =>
          m_Sorters.Add(new WithRefProcessor<TComponent>(processor));

        public void Execute(ECSWorld world)
        {
            foreach (var s in m_Sorters)
                s.PrepareForWorld(world);

            foreach (var targetEntity in world.Entities)
                foreach (var s in m_Sorters)
                    s.Process(targetEntity);
        }

        public interface IProcessor
        {
            void PrepareForWorld(ECSWorld world);
            void Process(int entity);
        }

        class WithProcessor<TComponent> : IProcessor where TComponent : struct, IComponent
        {
            readonly ComponentProcessor<TComponent> m_Processor;
            IReadOnlyComponentCollection<TComponent> m_CurrentComponentMap;

            public WithProcessor(ComponentProcessor<TComponent> processor)
            {
                m_Processor = processor ?? throw new ArgumentNullException(nameof(processor));
            }

            void IProcessor.PrepareForWorld(ECSWorld world)
            {
                m_CurrentComponentMap = world.GetComponentsWithEntityReadOnly<TComponent>();
            }

            void IProcessor.Process(int entity)
            {
                var hasComonenent = m_CurrentComponentMap.TryGetComponent(entity, out var componennt);
                if (hasComonenent)
                    m_Processor(componennt);
            }
        }

        class WithRefProcessor<TComponent> : IProcessor where TComponent : struct, IComponent
        {
            readonly ComponentReferenceProcessor<TComponent> m_Processor;
            IComponentCollection<TComponent> m_CurrentComponentMap;

            public WithRefProcessor(ComponentReferenceProcessor<TComponent> processor)
            {
                m_Processor = processor ?? throw new ArgumentNullException(nameof(processor));
            }

            void IProcessor.PrepareForWorld(ECSWorld world)
            {
                m_CurrentComponentMap = world.GetComponentsWithEntity<TComponent>();
            }

            void IProcessor.Process(int entity)
            {
                var hasComonenent = m_CurrentComponentMap.TryGetComponent(entity, out var componennt);
                if (hasComonenent)
                {
                    m_Processor(ref componennt);
                    m_CurrentComponentMap.TrySetComponent(entity, componennt);
                }
            }
        }
    }
}
