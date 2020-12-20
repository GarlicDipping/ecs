using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Leopotam.Ecs.Garlic
{
    public static class GarlicEcsEntityExtensions
    {
        /// <summary>
        ///     Serialize all serializable components of this entity.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
#if ENABLE_IL2CPP
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SerializeEntity(in this EcsEntity entity, BinaryWriter writer)
        {
            var serializableComponentsCount = 0;
            var seekIndex = (int) writer.BaseStream.Length;
            writer.Write(serializableComponentsCount);
            ref var entityData = ref entity.Owner.GetEntityData(entity);
            for (int i = 0, iiMax = entityData.ComponentsCountX2; i < iiMax; i += 2)
            {
                var attatchedComponentIdx = entityData.Components[i];
                if (entity.Owner.ComponentPools[attatchedComponentIdx].IsSerializable())
                {
                    serializableComponentsCount++;
                    var itemIndex = entityData.Components[i + 1];
                    entity.Owner.ComponentPools[attatchedComponentIdx].InvokeSerialize(itemIndex, writer);
                }
            }

            writer.Seek(seekIndex, SeekOrigin.Begin);
            writer.Write(serializableComponentsCount);
            writer.Seek(0, SeekOrigin.End);
        }
        
        /// <summary>
        ///     Deserialize all serializable components of this entity.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
#if ENABLE_IL2CPP
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeserializeComponent(in this EcsEntity entity, System.Type componentType, byte[] data)
        {
            var name = nameof(InternalDeserialize);
            var method = typeof(GarlicEcsEntityExtensions).GetMethod(name, 
                BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(componentType);
            generic.Invoke(null, new object[] {entity, data});
        }
        
        /// <summary>
        /// Returns exist component on entity or adds new one otherwise.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
#if ENABLE_IL2CPP
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        static void InternalDeserialize<T> (in this EcsEntity entity, byte[] data) where T : struct {
            ref var entityData = ref entity.Owner.GetEntityData (entity);
#if DEBUG
            if (entityData.Gen != entity.Gen) { throw new Exception ("Cant add component to destroyed entity."); }
#endif
            var typeIdx = EcsComponentType<T>.TypeIndex;
            // check already attached components.
            for (int i = 0, iiMax = entityData.ComponentsCountX2; i < iiMax; i += 2) {
                if (entityData.Components[i] == typeIdx)
                {
                    int itemIndex = entityData.Components[i + 1];
                    using (MemoryStream stream = new MemoryStream(data))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            entity.Owner.ComponentPools[typeIdx].InvokeDeserialize(itemIndex, reader);
                        }   
                    }
                    return;
                }
            }
            // attach new component.
            if (entityData.Components.Length == entityData.ComponentsCountX2) {
                Array.Resize (ref entityData.Components, entityData.ComponentsCountX2 << 1);
            }
            entityData.Components[entityData.ComponentsCountX2++] = typeIdx;

            var pool = entity.Owner.GetPool<T> ();

            var idx = pool.New ();
            entityData.Components[entityData.ComponentsCountX2++] = idx;
#if DEBUG
            for (var ii = 0; ii < entity.Owner.DebugListeners.Count; ii++) {
                entity.Owner.DebugListeners[ii].OnComponentListChanged (entity);
            }
#endif
            entity.Owner.UpdateFilters (typeIdx, entity, entityData);
            
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    pool.InvokeDeserialize(idx, reader);
                }   
            }
        }

        // public static object Get(in this EcsEntity entity, System.Type type)
        // {
        //     bool isStruct = type.IsValueType && !type.IsPrimitive;
        //     if (isStruct == false)
        //     {
        //         throw new Exception("Only struct type allowed!");                
        //     }
        //     var getMethod = typeof(EcsEntityExtensions).GetMethod(nameof(EcsEntityExtensions.Get))
        //         .MakeGenericMethod(type);
        //     return getMethod.Invoke(null, new object[] { entity });
        // }
        //
        // public static bool Has(in this EcsEntity entity, System.Type type)
        // {
        //     ref var entityData = ref entity.Owner.GetEntityData(entity);
        //     for (int i = 0, iiMax = entityData.ComponentsCountX2; i < iiMax; i += 2)
        //     {
        //         var typeIndexInternal = entityData.Components[i];
        //         if (entity.Owner.ComponentPools[typeIndexInternal].ItemType == type)
        //         {
        //             return true;
        //         }
        //     }
        //     
        //     return false;
        // }
        //
        // static int InvokeDeserializeComponent(in this EcsEntity entity, System.Type componentType, byte[] data)
        // {
        //     ref var entityData = ref entity.Owner.GetEntityData(entity);
        //
        //     for (int i = 0, iiMax = entityData.ComponentsCountX2; i < iiMax; i += 2)
        //     {
        //         var typeIndexInternal = entityData.Components[i];
        //         if (entity.Owner.ComponentPools[typeIndexInternal].ItemType == componentType)
        //         {
        //             var itemIndex = entityData.Components[i + 1];
        //             using (MemoryStream stream = new MemoryStream(data))
        //             {
        //                 using (BinaryReader reader = new BinaryReader(stream))
        //                 {
        //                     entity.Owner.ComponentPools[typeIndexInternal].InvokeDeserialize(itemIndex, reader);
        //                 }
        //             }
        //
        //             return typeIndexInternal;
        //         }
        //     }
        //
        //     return -1;
        // }
    }
}