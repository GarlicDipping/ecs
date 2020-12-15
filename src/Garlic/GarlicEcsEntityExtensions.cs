using System;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        public static int GetComponentsOfBaseType<T> (in this EcsEntity entity, ref T[] list) {
            ref var entityData = ref entity.Owner.GetEntityData (entity);
#if DEBUG
            if (entityData.Gen != entity.Gen) { throw new Exception ("Cant touch destroyed entity."); }
#endif
            var itemsCount = entityData.ComponentsCountX2 >> 1;
            if (list == null || list.Length < itemsCount) {
                list = new T[itemsCount];
            }

            int foundItems = 0;
            for (int i = 0, iMax = entityData.ComponentsCountX2; i < iMax; i += 2) {
                var component = entity.Owner.ComponentPools[entityData.Components[i]].GetItem (entityData.Components[i + 1]);
                if (component is T)
                {
                    list[foundItems] = (T) component;
                    foundItems++;
                }
            }
            Array.Resize(ref list, foundItems);
            return itemsCount;
        }

        public static object Get(in this EcsEntity entity, Type t)
        {
            var name = nameof(EcsEntityExtensions.Get);
            MethodInfo method = typeof(EcsEntityExtensions).GetMethod(name, 
                BindingFlags.Public | BindingFlags.Static);
            MethodInfo generic = method.MakeGenericMethod(t);
            return generic.Invoke(entity, new object[]{entity});
        }
    }
}