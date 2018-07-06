using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indril.TypeToolkit
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
        /// A safe version of <see cref="Type.GetGenericArguments"/> which returns an empty array
        /// if the type is not generic.
        /// </summary>
        /// <param name="type">The type.</param>
        public static Type[] GetGenericArgumentsSafe(this Type type)
            => (type.IsGenericType || type.IsGenericTypeDefinition) ? type.GetGenericArguments() : new Type[0];

        /// <summary>
        /// A safe version of <see cref="Type.GetGenericTypeDefinition"/> which returns the type
        /// itself if <paramref name="type"/> isn't a generic type.
        /// </summary>
        /// <param name="type">The type.</param>
        public static Type GetGenericTypeDefinitionSafe(this Type type)
            => type.IsGenericType ? type.GetGenericTypeDefinition() : type;
    }
}
