using System;
using System.Runtime.CompilerServices;

namespace Leopotam.Ecs.Garlic
{
    public static class EcsReflectionHelper
    {
        public static int GetComponentTypeIndex(System.Type componentType)
        {
            int typeIndex = EcsComponentTypeIndexMap.GetComponentTypeIndex(componentType);
            //이 타입이 아직 등록되지 않은 상황이면 리플렉션으로 Static Constructor 호출
            if (typeIndex == -1)
            {
                CallGenericStaticConstructor(typeof(EcsComponentType<>), componentType);
            }

            return typeIndex;
        }

        public static void CallGenericStaticConstructor(System.Type constructorClassType, System.Type genericType)
        {
            if (constructorClassType.IsClass && constructorClassType.IsAbstract && constructorClassType.IsSealed)
            {
                var genericClass = constructorClassType.MakeGenericType(genericType);
                RuntimeHelpers.RunClassConstructor(genericClass.TypeHandle);
            }
            else
            {
                throw new Exception($"이 클래스는 Generic Static Class가 아닙니다!: {constructorClassType.ToString()}");
            }
        }
    }
}