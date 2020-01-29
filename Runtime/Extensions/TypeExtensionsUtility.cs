using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utility
{
    public class TypeExtensionsUtility
    {
        public static bool IsConstructedGenericType(Type type)
        {
           return type.IsGenericType && !type.IsGenericTypeDefinition; 
        }

        public static bool CheckGenericType (Type type, Type toCompare)
        {
            if (type.IsGenericTypeDefinition)
                if (type.GetGenericTypeDefinition () == toCompare)
                    return true;

            if (IsConstructedGenericType(type))
                if (type.GetGenericTypeDefinition () == toCompare)
                    return true;

            if (type.BaseType != null)
                return CheckGenericType (type.BaseType, toCompare);

            return false;
        }

        public static List<Type> GetAllTypes()
        {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> foundTypes = new List<Type>();
            int assembliesLength = assemblies.Length;

            for (int i = 0; i < assembliesLength; i++)
            {
                if (assemblies[i].IsDynamic)
                    continue;

                try
                {
                    foundTypes.AddRange(assemblies[i].GetExportedTypes());
                }
                catch
                {
                }
            }

            return foundTypes;
        }
    }
}