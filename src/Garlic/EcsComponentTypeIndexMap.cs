using System;
using System.Collections.Generic;

namespace Leopotam.Ecs.Garlic
{
    internal static class EcsComponentTypeIndexMap
    {
        private static readonly Dictionary<int, Type> IdTypeDict;
        private static readonly Dictionary<Type, int> TypeIdDict;

        static EcsComponentTypeIndexMap()
        {
            TypeIdDict = new Dictionary<Type, int>();
            IdTypeDict = new Dictionary<int, Type>();
        }

        public static void Register(Type type, int typdIndex)
        {
            IdTypeDict.Add(typdIndex, type);
            TypeIdDict.Add(type, typdIndex);
        }

        public static int GetComponentTypeIndex(Type type)
        {
            int typeIndex = -1;
            TypeIdDict.TryGetValue(type, out typeIndex);
            return typeIndex;
        }

        public static Type GetComponentType(int typeIndex)
        {
            IdTypeDict.TryGetValue(typeIndex, out var type);
            return type;
        }
    }
}