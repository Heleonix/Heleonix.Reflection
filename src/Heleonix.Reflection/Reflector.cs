// <copyright file="Reflector.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection
{
    using System;
    using System.Collections;
    using System.Reflection;

    /// <summary>
    /// Provides functionality for working with reflection.
    /// </summary>
    public static partial class Reflector
    {
        /// <summary>
        /// The default binding flags.
        /// </summary>
        public const BindingFlags DefaultBindingFlags
            = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        /// <summary>
        /// The property OR field flags to determine member types if they're property or field.
        /// </summary>
        private const MemberTypes PropertyOrFieldMemberTypes = MemberTypes.Property | MemberTypes.Field;

        /// <summary>
        /// The empty member information.
        /// </summary>
        private static readonly MemberInfo[] EmptyMemberInfo = Array.Empty<MemberInfo>();

        /// <summary>
        /// Determines whether the specified property is static by its getter (if it is defined) or by its setter (if it is defined).
        /// </summary>
        /// <param name="info">The property information.</param>
        /// <returns><c>true</c> if the specified property is static; otherwise, <c>false</c>.</returns>
        public static bool IsStatic(PropertyInfo info)
            => info != null && ((info.CanRead && info.GetMethod.IsStatic) || (info.CanWrite && info.SetMethod.IsStatic));

        /// <summary>
        /// Checks if parameters have corresponding types.
        /// </summary>
        /// <param name="paramInfos">The parameter info.</param>
        /// <param name="paramTypes">The parameter types.</param>
        /// <returns><c>true</c> if parameters match, otherwise <c>false</c>.</returns>
        private static bool ParameterTypesMatch(ParameterInfo[] paramInfos, Type[] paramTypes)
        {
            if (paramTypes == null)
            {
                return true;
            }

            if (paramTypes.Length != paramInfos.Length)
            {
                return false;
            }

            for (var i = paramTypes.Length - 1; i >= 0; i--)
            {
                if (paramInfos[i].ParameterType != paramTypes[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets an element at the specified <paramref name="index"/> in the specified <paramref name="container"/>.
        /// </summary>
        /// <param name="container">A container to get an element from.</param>
        /// <param name="index">An index to get an elementa at.</param>
        /// <returns>An element by the specified <paramref name="index"/>.</returns>
        private static object GetElementAt(object container, object index)
        {
            if (container is IDictionary dictionary)
            {
                foreach (var key in dictionary.Keys)
                {
                    if (key.Equals(index) || Convert.ToString(key) == Convert.ToString(index))
                    {
                        return dictionary[key];
                    }
                }

                return null;
            }

            if (container is IList list)
            {
                if ((int)index >= list.Count)
                {
                    return null;
                }

                return list[(int)index];
            }

            if (container is IEnumerable enumerable)
            {
                var enumerator = enumerable.GetEnumerator();
                var intIndex = (int)index;

                while (enumerator.MoveNext())
                {
                    if (intIndex == 0)
                    {
                        return enumerator.Current;
                    }

                    intIndex--;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets next container and its type.
        /// </summary>
        /// <param name="memberInfos">Member info to find a next container.</param>
        /// <param name="container">Current container.</param>
        /// <param name="containerType">Current container's type.</param>
        /// <returns><c>true</c> if a next container is found, otherwise <c>false</c>.</returns>
        private static bool GetNextContainer(MemberInfo[] memberInfos, ref object container, ref Type containerType)
        {
            var memberInfo = memberInfos.Length > 0 ? memberInfos[0] : null;

            if (memberInfo is PropertyInfo propertyInfo
                    && (container != null || IsStatic(propertyInfo)) && propertyInfo.CanRead)
            {
                container = propertyInfo.GetValue(container);
                containerType = container?.GetType() ?? propertyInfo.PropertyType;

                return true;
            }
            else if (memberInfo is FieldInfo fieldInfo && (container != null || fieldInfo.IsStatic))
            {
                container = fieldInfo.GetValue(container);
                containerType = container?.GetType() ?? fieldInfo.FieldType;

                return true;
            }

            container = null;

            containerType = null;

            return false;
        }
    }
}