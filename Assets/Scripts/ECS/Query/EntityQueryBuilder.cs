using System.Collections.Generic;

namespace ECS
{
    public class EntityQueryBuilder : IQueryBuilder
    {
        readonly EntityQueryBuilderGroup m_IncludedAny;
        readonly EntityQueryBuilderGroup m_IncludedAll;
        readonly EntityQueryBuilderGroup m_Excluded;

        public EntityQueryBuilder()
        {
            m_IncludedAny = new EntityQueryBuilderGroup(this);
            m_IncludedAll = new EntityQueryBuilderGroup(this);
            m_Excluded = new EntityQueryBuilderGroup(this);
        }

        public IQueryGroupModificator Any() => m_IncludedAny;
        public IQueryGroupModificator All() => m_IncludedAll;
        public IQueryGroupModificator None() => m_Excluded;

        public EntityQuery ToQuery() => new EntityQuery(m_IncludedAny.Componenents, m_IncludedAll.Componenents, m_Excluded.Componenents);

        public void Recycle()
        {
            m_IncludedAny.Clear();
            m_IncludedAll.Clear();
            m_Excluded.Clear();
        }

        public class EntityQueryBuilderGroup : IQueryGroupModificator
        {
            readonly EntityQueryBuilder m_Builder;

            public readonly HashSet<int> Componenents;

            public EntityQueryBuilderGroup(in EntityQueryBuilder builder)
            {
                m_Builder = builder;
                Componenents = new HashSet<int>();
            }

            public IQueryGroupModificator Of<TComponent>() where TComponent : struct, IComponent
            {
                Componenents.Add(Core.ComponentTypeUtility.HashCodeOf<TComponent>());
                return this;
            }

            public IQueryBuilder BackToBuilder() => m_Builder;

            public EntityQuery ToQuery() => m_Builder.ToQuery();

            public void Clear() => Componenents.Clear();
        }
    }

    public interface IQueryBuilder
    {
        IQueryGroupModificator All();
        IQueryGroupModificator Any();
        IQueryGroupModificator None();
        EntityQuery ToQuery();
    }

    public interface IQueryGroupModificator
    {
        IQueryGroupModificator Of<TComponent>() where TComponent : struct, IComponent;
        IQueryBuilder BackToBuilder();
        EntityQuery ToQuery();
    }
}
