using ECS.Core;
using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// Allocates new entity;
    /// </summary>
    public struct EntityBuilder
    {
        readonly int m_WorldUID;
        readonly int m_EntityUID;

        EntityBuilder(ECSWorld world)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            m_WorldUID = world.UID;
            m_EntityUID = world.AllocateEntityUID;
        }

        public void AddComponenet<TComponent>(TComponent component) where TComponent : struct, IComponent =>
            ComponentMap<TComponent>.TryAdd(m_WorldUID, m_EntityUID, component);

        public static EntityBuilder New(ECSWorld world) => new EntityBuilder(world);
    }

    /// <summary>
    /// Allocates new entity;
    /// </summary>
    public struct EntityLazyBuilder
    {
        readonly int m_WorldUID;
        readonly int m_EntityUID;

        readonly List<IComponent> m_Components;

        bool m_IsValid;

        EntityLazyBuilder(ECSWorld world)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            m_WorldUID = world.UID;
            m_EntityUID = world.AllocateEntityUID;
            m_Components = new List<IComponent>();

            m_IsValid = true;
        }

        public EntityLazyBuilder AddComponents(IEnumerable<IComponent> componenetns)
        {
            if (!m_IsValid)
                throw new InvalidOperationException($"Entity '{m_EntityUID}' for world '{m_WorldUID}' is invalid");

            m_Components.AddRange(componenetns);
            return this;
        }

        public EntityLazyBuilder AddComponent(IComponent component)
        {
            if (!m_IsValid)
                throw new InvalidOperationException($"Entity '{m_EntityUID}' for world '{m_WorldUID}' is invalid");

            m_Components.Add(component);
            return this;
        }

        public void Build()
        {
            if (!m_IsValid)
                throw new InvalidOperationException($"Entity '{m_EntityUID}' for world '{m_WorldUID}' is invalid");

            if (m_Components.Count == 0)
                throw new InvalidOperationException($"Unable to build entity with zero components");

            foreach (var c in m_Components)
                c.Register(m_WorldUID, m_EntityUID);

            m_Components.Clear();
            m_IsValid = false;
        }

        public static EntityLazyBuilder New(ECSWorld world) => new EntityLazyBuilder(world);

        public static EntityLazyBuilder operator +(EntityLazyBuilder builder, IComponent component)
        {
            if (!builder.m_IsValid)
                throw new InvalidOperationException($"Invalid builder");

            return builder.AddComponent(component);
        }
    }
}
