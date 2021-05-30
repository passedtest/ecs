using System.Collections.Generic;

namespace ECS
{
    public static class Pool<T> where T : class, new()
    {
        readonly static Stack<T> s_Pool;

        static Pool()
        {
            s_Pool = new Stack<T>();
            s_Pool.Push(new T());
        }

        public static T Provide() =>
            s_Pool.Count > 0 ?
            s_Pool.Pop() :
            new T();

        public static void Push(T item) => 
            s_Pool.Push(item);
    }
}
