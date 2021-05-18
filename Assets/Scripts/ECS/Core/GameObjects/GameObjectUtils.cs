namespace ECS.Core
{
    public static class GameObjectUtils
    {
        public const int InvalidReferenceId = 0;
        public static bool IsReferenceIdValid(in int instanceId) => instanceId > 0;
    }
}
