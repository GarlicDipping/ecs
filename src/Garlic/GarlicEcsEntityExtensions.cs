using System.IO;
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
        ///     Serialize all serializable components of this entity.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
#if ENABLE_IL2CPP
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeserializeEntity(in this EcsEntity entity, BinaryReader reader)
        {
            ref var entityData = ref entity.Owner.GetEntityData(entity);
            for (int i = 0, iiMax = entityData.ComponentsCountX2; i < iiMax; i += 2)
            {
                var attatchedComponentIdx = entityData.Components[i];
                if (entity.Owner.ComponentPools[attatchedComponentIdx].IsSerializable())
                {
                    var itemIndex = entityData.Components[i + 1];
                    entity.Owner.ComponentPools[attatchedComponentIdx].InvokeDeserialize(itemIndex, reader);
                }
            }
        }
    }
}