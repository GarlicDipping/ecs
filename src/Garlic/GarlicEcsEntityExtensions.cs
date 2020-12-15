using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Leopotam.Ecs.Garlic
{
    public static class GarlicEcsEntityExtensions
    {
        /// <summary>
        /// Gets all component of base type.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
#if ENABLE_IL2CPP
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static int GetComponentsOfBaseType<T>(in this EcsEntity entity, ref T[] list)
        {
            ref var entityData = ref entity.Owner.GetEntityData(entity);
#if DEBUG
            if (entityData.Gen != entity.Gen) throw new Exception("Cant touch destroyed entity.");
#endif
            var itemsCount = entityData.ComponentsCountX2 >> 1;
            if (list == null || list.Length < itemsCount) list = new T[itemsCount];

            var foundItems = 0;
            for (int i = 0, iMax = entityData.ComponentsCountX2; i < iMax; i += 2)
            {
                var component = entity.Owner.ComponentPools[entityData.Components[i]]
                    .GetItem(entityData.Components[i + 1]);
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
            var method = typeof(EcsEntityExtensions).GetMethod(name,
                BindingFlags.Public | BindingFlags.Static);
            var generic = method.MakeGenericMethod(t);
            return generic.Invoke(entity, new object[] {entity});
        }
    }
}