using System.IO;

namespace Leopotam.Ecs.Garlic
{
    public interface IEcsSerializable<T> where T : struct
    {
        /// <summary>
        ///     컴포넌트를 검색하기 위한 유니크 Id
        ///     ex) TransformComponent의 Id는 서버/클라에서 모두 0이라 가정
        ///     만약 어떤 엔티티가 0번 컴포넌트를 붙이려 한다면 해당 Id를 가진 컴포넌트의 타입을 찾아 붙여줘야함
        /// </summary>
        /// <returns></returns>
        int GetComponentTypeId();

        void Serialize(ref T component, BinaryWriter serializer);
        void Deserialize(ref T component, BinaryReader deserializer);
    }
}