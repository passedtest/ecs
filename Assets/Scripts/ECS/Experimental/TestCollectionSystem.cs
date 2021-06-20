namespace ECS.Core.Experimental
{
    public class TestCollectionSystem : BaseSystem
    {
        protected override void OnExecuteUpdate(in ECSWorld world, in float deltaTime)
        {
            base.OnExecuteUpdate(world, deltaTime);

            var collectionComponenents = world.GetComponentsWithEntityReadOnly<ComponentWithReferences>();

            foreach (var targetEntity in world.Entities)
            {
                if (!collectionComponenents.TryGetComponent(targetEntity, out var collectionComponenent))
                    continue;

                //for (var i = 0; i < collectionComponenent.IntCollection.Length; i++)
                //    UnityEngine.Debug.LogError(collectionComponenent.IntCollection[i]);
            }
        }
    }
}
