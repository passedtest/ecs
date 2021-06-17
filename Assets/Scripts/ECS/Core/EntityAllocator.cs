namespace ECS.Core
{
    public class EntityAllocator
    {
        public int CurrentUID { get; private set; }
        public int AllocateUID => ++CurrentUID;
        public int NextEntityUID => CurrentUID + 1;

        public EntityAllocator() : this(0) { }
        public EntityAllocator(int currentUID)
        {
            CurrentUID = currentUID;
        }
    }
}
