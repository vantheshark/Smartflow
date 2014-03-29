using System;

namespace Smartflow.Core
{
    internal static class TypeExtensions
    {
        public static bool Is<T>(this Type type)
        {
            return Is(type, typeof(T));
        }

        public static bool Is(this Type type, Type parentType)
        {
            return parentType.IsAssignableFrom(type) || IsSubclassOfRawGeneric(parentType, type);
        }

        //http://stackoverflow.com/questions/457676/c-sharp-reflection-check-if-a-class-is-derived-from-a-generic-class
        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
        
        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || type.Equals(typeof(string));
        }
    }
}
