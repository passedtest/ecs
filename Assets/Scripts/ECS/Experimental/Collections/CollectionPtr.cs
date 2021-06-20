namespace ECS.Core.Experimental.Collections
{
    [System.Serializable]
    public struct CollectionPtr<TElement> : System.IDisposable where TElement : struct
    {
        public delegate void ForEachProcessorDelegate(ref TElement element, in int index);

        readonly EntityRangePtr m_Elements;

        CollectionPtr(in EntityRangePtr elements)
        {
            m_Elements = elements;
        }

        public int Length => m_Elements.Length;

        public TElement this[in int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        TElement Get(in int index)
        {
            var targetIndex = ValidateIndexRange(index);

            if (!ComponentMap<CollectionElementComponent<TElement>>.TryGet(m_Elements.World, targetIndex, out var component))
                throw new System.InvalidOperationException();

            if (!component.IsValid)
                throw new System.InvalidOperationException();

            return component.Value;
        }

        void Set(in int index, in TElement value)
        {
            var entity = ValidateIndexRange(index);
            ComponentMap<CollectionElementComponent<TElement>>.TryAddOrSet(m_Elements.World, entity, CollectionElementComponent<TElement>.WithValue(value));
        }

        int ValidateIndexRange(int index)
        {
            if (index < 0 || index >= Length)
                throw new System.IndexOutOfRangeException();

            var entity = m_Elements.IndexToEntity(index);
            return entity;
        }

        public void ForEach(ForEachProcessorDelegate processor)
        {
            for(var i = 0; i < Length; i++)
            {
                var element = Get(i);
                processor(ref element, i);
                Set(i, element);
            }
        }

        public TElement[] ToArray()
        {
            var result = new TElement[Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = Get(i);

            return result;
        }

        public void CopyTo(ref CollectionPtr<TElement> other, in int offset = 0)
        {
            for(var i = System.Math.Max(0, offset); i < System.Math.Min(Length, other.Length); i++)
                other[i] = Get(i);
        }

        public CollectionPtr<TElement> CloneWithSize(in int length)
        {
            var newCollection = Allocate(m_Elements.World, length);
            CopyTo(ref newCollection);
            return newCollection;
        }

        public CollectionPtr<TElement> CloneWith(in CollectionPtr<TElement> other)
        {
            var newCollection = CloneWithSize(Length + other.Length);
            other.CopyTo(ref newCollection, Length);

            return newCollection;
        }

        public static implicit operator EntityRangePtr(CollectionPtr<TElement> collection) => 
            collection.m_Elements;

        public static CollectionPtr<TElement> New(EntityRangePtr entityRange) => 
            new CollectionPtr<TElement>(entityRange);

        public static CollectionPtr<TElement> Allocate(in int world, in int length) =>
            AllocateViaCommandBuffer(world, length, new DirectCommandBufferConcatenator());

        public static CollectionPtr<TElement> AllocateViaCommandBuffer(in int world, in int length, in ICommandBufferConcatenator commandBuffer)
        {
            var allocator = EntityAllocatorComponenent.Get(world);

            var entityRangePtp = new EntityRangePtr(world, allocator.NextEntityUID, length);
            for (var i = 0; i < length; i++)
                commandBuffer.TryAddOrSetComponent(world, allocator.AllocateUID, new CollectionElementComponent<TElement>());

            EntityAllocatorComponenent.Set(world, allocator);

            return New(entityRangePtp);
        }

        public void Dispose()
        {
            for (var entity = m_Elements.Start; entity < m_Elements.Start + m_Elements.Length; entity++)
                ComponentMap<CollectionElementComponent<TElement>>.Remove(m_Elements.World, entity);
        }
    }
}
