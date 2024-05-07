// <copyright file="Reflector.Get.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides functionality for working with reflection.
    /// </summary>
    public static partial class Reflector
    {
        /// <summary>
        /// Gets a value by the provided path.
        /// </summary>
        /// <typeparam name="TReturn">A type of a value to be set with the target value.</typeparam>
        /// <param name="instance">A root object.</param>
        /// <param name="type">A type of a root object.
        /// If <paramref name="instance"/> is not <c>null</c>, then its type is used instead.
        /// </param>
        /// <param name="memberPath">A path to a member.</param>
        /// <param name="value">A gotten value.</param>
        /// <param name="bindingFlags">Binding flags to find members.</param>
        /// <exception cref="TargetException">
        /// Target thrown an exception during execution. See inner exception for details.
        /// </exception>
        /// <returns>
        /// <c>true</c> in case of success, otherwise <c>false</c> if
        /// <paramref name="memberPath"/> is <c>null</c> or empty
        /// or
        /// <paramref name="instance"/> is <c>null</c> and <paramref name="type"/> is <c>null</c>
        /// or
        /// a target member or one of intermediate members was not found
        /// or
        /// a member is not static and its container is null
        /// or
        /// a target member or an intermediate member is neither <see cref="PropertyInfo"/> nor <see cref="FieldInfo"/>
        /// or
        /// a target value is not of type <typeparamref name="TReturn"/>.
        /// </returns>
        /// <example>
        /// var success = Reflector.Get(DateTime.Now, null, "TimeOfDay.Hours", out int value);
        ///
        /// // success == true;
        /// // value == DateTime.Now.TimeOfDay.Hours;
        ///
        /// or
        ///
        /// var success = Reflector.Get(typeof(int), null, "CustomAttributes[0].AttributeType", out int value);
        ///
        /// // success == true;
        /// // value == typeof(int).CustomAttributes.First().AttributeType;
        ///
        /// or
        ///
        /// var success = Reflector.Get(typeof(int), null, "CustomAttributes[0]", out int value);
        ///
        /// // success == true;
        /// // value == typeof(int).CustomAttributes.First().
        /// </example>
        public static bool Get<TReturn>(
            object instance,
            Type type,
            string memberPath,
            out TReturn value,
            BindingFlags bindingFlags = DefaultBindingFlags)
        {
            if ((instance == null && type == null) || string.IsNullOrEmpty(memberPath))
            {
                value = default;

                return false;
            }

            var container = instance;
            var containerType = container?.GetType() ?? type;

            while (true)
            {
                var dot = memberPath.IndexOf('.');
                var size = (dot == -1) ? memberPath.Length : dot;
                var prop = memberPath.Substring(0, size);

                object index = null;
                var indexerEnd = prop.LastIndexOf(']');

                if (indexerEnd != -1)
                {
                    var indexerStart = prop.IndexOf('[');

                    var indexValue = prop.Substring(indexerStart + 1, indexerEnd - indexerStart - 1);

                    index = int.TryParse(indexValue, out int result) ? result : indexValue;

                    prop = prop.Substring(0, indexerStart);
                }

                var memberInfos = containerType
                    .GetTypeInfo()
                    .GetMember(prop, MemberTypes.Field | MemberTypes.Property, bindingFlags);

                var found = GetNextContainer(memberInfos, ref container, ref containerType);

                if (!found)
                {
                    value = default;

                    return false;
                }

                if (index != null)
                {
                    var isElementFound = GetElementAt(container, index, out container);

                    if (!isElementFound)
                    {
                        value = default;

                        return false;
                    }

                    if (container == null)
                    {
                        if (dot != -1)
                        {
                            value = default;

                            return false;
                        }
                    }
                    else
                    {
                        containerType = container.GetType();
                    }
                }

                if (dot == -1)
                {
                    break;
                }

                memberPath = memberPath.Substring(dot + 1);
            }

            if (container == null)
            {
                value = default;

                return true;
            }
            else if (container is TReturn)
            {
                value = (TReturn)container;

                return true;
            }
            else
            {
                value = default;

                return false;
            }
        }
    }
}
