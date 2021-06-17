namespace ECS.Experimental.Collections
{
    [System.Serializable]
    public struct CollectionElementComponent<TElement> : IComponent where TElement : struct
    {
        public bool IsValid => m_IsValid;
        [UnityEngine.SerializeField]
        bool m_IsValid;

        public TElement Value => m_Value;
        [UnityEngine.SerializeField]
        TElement m_Value;

        public void Set(TElement element) =>
            SetInternal(element, true);

        public void Reset() =>
            SetInternal(default, false);

        void SetInternal(TElement value, bool isValid)
        {
            m_Value = value;
            m_IsValid = isValid;
        }

        void IComponent.Register(in int world, in int entity) =>
            Core.ComponentMap<CollectionElementComponent<TElement>>.TryAddOrSet(world, entity, this);

        public static CollectionElementComponent<TElement> WithValue(TElement value)
        {
            var component = new CollectionElementComponent<TElement>();
            component.Set(value);
            return component;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Struct, AllowMultiple = false)]
    public class RequireCollectionOfTypeAttribute : Core.RequireGenericComponenentTypeAttribute
    {
        public RequireCollectionOfTypeAttribute(params System.Type[] collectionTypes) : base(ToCollectionComponentTypes(collectionTypes)) { }

        static System.Type[] ToCollectionComponentTypes(System.Type[] collectionTypes)
        {
            var result = new System.Type[collectionTypes.Length];
            for(var i = 0; i < result.Length; i++)
                result[i] = typeof(CollectionElementComponent<>).MakeGenericType(collectionTypes[i]);

            return result;
        }
    }
}
