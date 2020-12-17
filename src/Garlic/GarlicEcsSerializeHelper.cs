using System.IO;

namespace Leopotam.Ecs.Garlic
{
    internal static class GarlicEcsSerializeHelper
    {
        public static void SerializeComponent<T>(ref T component, BinaryWriter writer)
            where T : struct, IEcsSerializable<T>
        {
            //Write Component TypeId
            var typeId = component.GetComponentTypeId();
            writer.Write(typeId);
            //Write Temporary Component Size
            var componentSize = 0;
            writer.Write(componentSize);
            var lengthBefore = writer.BaseStream.Length;
            component.Serialize(ref component, writer);
            var lengthAfter = writer.BaseStream.Length;
            componentSize = (int) (lengthAfter - lengthBefore);
            writer.Seek(-componentSize - sizeof(int), SeekOrigin.Current);
            writer.Write(componentSize);
            writer.Seek(0, SeekOrigin.End);
        }
    }
}