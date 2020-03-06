using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nordril.TypeToolkit
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the type's base name, i.e. its name without any generic parameters.
        /// </summary>
        /// <param name="type">The type.</param>
        public static string GetBaseName(this Type type) => type.Name.Split('`')[0];

        /// <summary>
        /// Gets the type's base full name, i.e. its full name (namespace-qualified) name without any generic parameters.
        /// </summary>
        /// <param name="type">The type.</param>
        public static string GetBaseFullName(this Type type) => type.FullName.Split('`')[0];

        /// <summary>
        /// Gets the type's generic name, which is equal to its base name for non-generic types,
        /// and equal to the angle-bracket-syntax in C#.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="useFullName">If true, the type's full (namespace-qualified) name will be used, otherwise the type's name.</param>
        public static string GetGenericName(this Type type, bool useFullName = false)
        {
            var ret = new StringBuilder();

            ret.Append(useFullName ? type.GetBaseFullName() : type.GetBaseName());

            if (type.IsGenericType)
            {
                ret.Append('<');
                ret.Append(string.Join(", ", type.GetGenericArguments().Select(a => a.GetGenericName(useFullName))));
                ret.Append('>');
            }

            return ret.ToString();
        }

        /// <summary>
        /// A safe version of <see cref="Type.GetGenericArguments"/> which returns an empty array if the type is not generic.
        /// </summary>
        /// <param name="type">The type.</param>
        public static Type[] GetGenericArgumentsSafe(this Type type)
            => (type.IsGenericType || type.IsGenericTypeDefinition) ? type.GetGenericArguments() : new Type[0];

        /// <summary>
        /// A safe version of <see cref="MethodInfo.GetGenericArguments"/> which returns an empty array if the type is not generic.
        /// </summary>
        /// <param name="type">The type.</param>
        public static Type[] GetGenericArgumentsSafe(this MethodInfo type)
            => (type.IsGenericMethod || type.IsGenericMethodDefinition) ? type.GetGenericArguments() : new Type[0];

        /// <summary>
        /// A safe version of <see cref="Type.GetGenericTypeDefinition"/> which returns the type
        /// itself if <paramref name="type"/> isn't a generic type.
        /// </summary>
        /// <param name="type">The type.</param>
        public static Type GetGenericTypeDefinitionSafe(this Type type)
            => type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        public static IList<Type> GetSupertypes(this Type type)
        {
            var ret = new List<Type>();

            foreach (var intf in type.GetInterfaces())
            {
                if (intf.IsGenericType && intf.GetGenericArguments().All(a => a.IsGenericParameter))
                    ret.Add(intf.GetGenericTypeDefinition());
                else
                    ret.Add(intf);
            }

            while (!(type is null) && type != typeof(object))
            {
                type = type.BaseType;
                if (type != null)
                {
                    if (type.IsGenericType && type.GetGenericArguments().All(a => a.IsGenericParameter))
                        ret.Add(type.GetGenericTypeDefinition());
                    else
                        ret.Add(type);
                }
            }

            return ret;
        }

        /// <summary>
        /// A looser form of <see cref="Type.IsAssignableFrom(Type)"/> which also returns true if the open type <paramref name="type"/> implements the open interface or open base type <paramref name="that"/>.
        /// </summary>
        /// <param name="type">The first type which should be assignable to the second.</param>
        /// <param name="that">The second type which should be assignable from the first.</param>
        /// <param name="instantations">If <paramref name="type"/>is an open generic type and <paramref name="that"/> is closed, this list contains the types to which the type-parameters of <paramref name="type"/> have to be instantiated. Otherwise, this is null.</param>
        /// <returns></returns>
        public static bool UnifiableWith(this Type type, Type that, out IList<Type> instantations)
        {
            //rules:
            //Un(t<>, t<>) -> true (equality)
            //Un(t<X>, t<Y>) -> All[x in X] Un(x,y) (args-recursion)
            //Un(t<>, c<> : ... b<> ...) -> Unifiable(t,b) (open/supertype)
            //Un(t<X>, c<Y> : ... b<X> ...) -> Unifiable(t,b) (closed/supertype)
            //otherwise: false

            var typeGeneric = type.GetGenericTypeDefinitionSafe();
            var typeArgs = type.GetGenericArgumentsSafe();
            var thatGeneric = that.GetGenericTypeDefinitionSafe();
            var thatArgs = that.GetGenericArgumentsSafe();

            instantations = null;

            //Case 1: base types match and are both open generic types
            if (typeGeneric == thatGeneric && type.IsGenericTypeDefinition && that.IsGenericTypeDefinition)
                return true;

            //Case 2: the left type is a closed type and the right type is open
            if (typeGeneric == thatGeneric && !type.IsGenericTypeDefinition && that.IsGenericTypeDefinition)
            {
                instantations = type.GetGenericArgumentsSafe();
                return true;
            }

            //Case 2: base types match and are both constructed generic types
            if (typeGeneric == thatGeneric && typeArgs.Length == thatArgs.Length && typeArgs.Zip(thatArgs, (x, y) => x.UnifiableWith(y, out var _)).All(x => x))
                return true;

            //Case 3: the right type matches a base type of the right type (open)
            var thatSupertypes = GetSupertypes(thatGeneric);
            var anyMatches = thatSupertypes.Any(t => UnifiableWith(typeGeneric, t, out var _));

            if (anyMatches)
                return true;

            //Case 4: the right type matches a base type of the right type (open)
            thatSupertypes = GetSupertypes(that);
            anyMatches = thatSupertypes.Any(t => UnifiableWith(type, t, out var _));

            if (anyMatches)
                return true;

            //Otherwise: false;
            return false;
        }

        /*public static TypeLike ToTreeRepresentation(this Type type)
        {

        }*/
    }
}
