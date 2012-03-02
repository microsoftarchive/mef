using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Reflection.Context
{
    internal static class CollectionServices
    {
        public static T[] Empty<T>()
        {
            return new T[0];
        }

        public static bool CompareArrays<T>(T[] left, T[] right)
        {
            if (left.Length != right.Length)
                return false;

            for (int i = 0; i < left.Length; i++)
            {
                if (!left[i].Equals(right[i]))
                    return false;
            }

            return true;
        }

        public static int GetArrayHashCode<T>(T[] array)
        {
            int hashcode = 0;
            foreach (T t in array)
                hashcode ^= t.GetHashCode();

            return hashcode;
        }

        public static object[] ConvertListToArray(List<object> list, Type arrayType)
        {
            // Mimic the behavior of GetCustomAttributes in runtime reflection.
            if (arrayType.HasElementType || arrayType.IsValueType || arrayType.ContainsGenericParameters)
                return list.ToArray();

            // Converts results to typed array.
            Array typedArray = Array.CreateInstance(arrayType, list.Count);

            list.CopyTo((object[])typedArray);

            return (object[])typedArray;
        }

        public static object[] IEnumerableToArray(IEnumerable<object> enumerable, Type arrayType)
        {
            List<object> list = new List<object>(enumerable);

            return ConvertListToArray(list, arrayType);
        }
    }
}