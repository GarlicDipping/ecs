using System;

namespace Leopotam.Ecs.Garlic
{
    public static class GarlicEcsEntityExtensions
    {
        /// <summary>
        /// Gets types of all attached components.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="list">List to put results in it. if null - will be created. If not enough space - will be resized.</param>
        /// <returns>Amount of components in list.</returns>
        public static int GetNetworkComponents (in this EcsEntity entity, ref IEcsNetworkComponent[] list) {
            ref var entityData = ref entity.Owner.GetEntityData (entity);
#if DEBUG
            if (entityData.Gen != entity.Gen) { throw new Exception ("Cant touch destroyed entity."); }
#endif
            var itemsCount = entityData.ComponentsCountX2 >> 1;
            if (list == null || list.Length < itemsCount) {
                list = new IEcsNetworkComponent[itemsCount];
            }
            for (int i = 0, j = 0, iMax = entityData.ComponentsCountX2; i < iMax; i += 2, j++) {
                list[j] = entity.Owner.ComponentPools[entityData.Components[i]].GetItem (entityData.Components[i + 1]) as IEcsNetworkComponent;
            }
            return itemsCount;
        }
    }
}