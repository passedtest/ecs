using ECS.Core;

namespace ECS.Experimental.Collections
{
    [System.Serializable]
    public struct CollectionPtr<TElement> where TElement : struct
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
            var targetIndex = ValidateIndexRange(index);
            ComponentMap<CollectionElementComponent<TElement>>.TryAddOrSet(m_Elements.World, targetIndex, CollectionElementComponent<TElement>.WithValue(value));
        }

        int ValidateIndexRange(int index)
        {
            var targetIndex = index - m_Elements.Start;
            if (targetIndex < 0 || targetIndex >= Length)
                throw new System.IndexOutOfRangeException();

            return targetIndex;
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

        public static implicit operator EntityRangePtr (CollectionPtr<TElement> collection) => 
            collection.m_Elements;

        public static CollectionPtr<TElement> New(EntityRangePtr entityRange) => 
            new CollectionPtr<TElement>(entityRange);
    }
}
