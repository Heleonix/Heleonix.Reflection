// <copyright file="Reflector.Set.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Provides functionality for working with reflection.
    /// </summary>
    public static partial class Reflector
    {
        /// <summary>
        /// Sets a provided value by the provided path.
        /// </summary>
        /// <param name="instance">A root object.</param>
        /// <param name="type">A type of a root object.
        /// If <paramref name="instance"/> is not <c>null</c>, then its type is used instead.
        /// </param>
        /// <param name="memberPath">A path to a member.</param>
        /// <param name="value">A value to be set.</param>
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
        /// a target member or an intermediate member is neither <see cref="PropertyInfo"/> nor <see cref="FieldInfo"/>.
        /// </returns>
        /// <example>
        /// public class Root
        /// {
        ///     public Child Child { get; set; } = new Child();
        ///     public Child[] Children { get; set; } = new Child[] { new Child(), new Child() };
        /// }
        ///
        /// public class Child { public int Value { get; set; } }
        ///
        /// var root = new Root();
        ///
        /// var success1 = Reflector.Set(root, null, "Child.Value", 111);
        /// var success2 = Reflector.Set(root, null, "Children[0].Value", 222);
        /// var success3 = Reflector.Set(root, null, "Children[1]", new Child() { Value = 333 });
        ///
        /// // success1 == true;
        /// // success2 == true;
        /// // success3 == true;
        ///
        /// // root.Child.Value == 111;
        /// // root.Children[0].Value == 222;
        /// // root.Children[1].Value == 333.
        /// </example>
        public static bool Set(
            object instance,
            Type type,
            string memberPath,
            object value,
            BindingFlags bindingFlags = DefaultBindingFlags)
            => Set(instance, type, memberPath, value, false, bindingFlags);

        /// <summary>
        /// Sets a provided value by the provided path with coercion into the target member type.
        /// </summary>
        /// <param name="instance">A root object.</param>
        /// <param name="type">A type of a root object.
        /// If <paramref name="instance"/> is not <c>null</c>, then its type is used instead.
        /// </param>
        /// <param name="memberPath">A path to a member.</param>
        /// <param name="value">A value to be set.</param>
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
        /// a target member or an intermediate member is neither <see cref="PropertyInfo"/> nor <see cref="FieldInfo"/>.
        /// </returns>
        /// <example>
        /// public class Root
        /// {
        ///     public Child Child { get; set; } = new Child();
        ///     public Child[] Children { get; set; } = new Child[] { new Child(), new Child() };
        /// }
        ///
        /// public class Child { public int Value { get; set; } }
        ///
        /// var root = new Root();
        ///
        /// var success1 = Reflector.Set(root, null, "Child.Value", 111);
        /// var success2 = Reflector.Set(root, null, "Children[0].Value", 222);
        /// var success3 = Reflector.Set(root, null, "Children[1]", new Child() { Value = 333 });
        ///
        /// // success1 == true;
        /// // success2 == true;
        /// // success3 == true;
        ///
        /// // root.Child.Value == 111;
        /// // root.Children[0].Value == 222;
        /// // root.Children[1].Value == 333.
        /// </example>
        public static bool SetCoerced(
            object instance,
            Type type,
            string memberPath,
            object value,
            BindingFlags bindingFlags = DefaultBindingFlags)
            => Set(instance, type, memberPath, value, true, bindingFlags);

        /// <summary>
        /// Sets a provided value by the provided path.
        /// </summary>
        /// <param name="instance">A root object.</param>
        /// <param name="type">A type of a root object.
        /// If <paramref name="instance"/> is not <c>null</c>, then its type is used instead.
        /// </param>
        /// <param name="memberPath">A path to a member.</param>
        /// <param name="value">A value to be set.</param>
        /// <param name="coerce">
        /// Specifies if the <paramref name="value"/> needs to be coerced into the target type or assigned as is.
        /// </param>
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
        /// a target member or an intermediate member is neither <see cref="PropertyInfo"/> nor <see cref="FieldInfo"/>.
        /// </returns>
        /// <example>
        /// public class Root
        /// {
        ///     public Child Child { get; set; } = new Child();
        ///     public Child[] Children { get; set; } = new Child[] { new Child(), new Child() };
        /// }
        ///
        /// public class Child { public int Value { get; set; } }
        ///
        /// var root = new Root();
        ///
        /// var success1 = Reflector.Set(root, null, "Child.Value", 111);
        /// var success2 = Reflector.Set(root, null, "Children[0].Value", 222);
        /// var success3 = Reflector.Set(root, null, "Children[1]", new Child() { Value = 333 });
        ///
        /// // success1 == true;
        /// // success2 == true;
        /// // success3 == true;
        ///
        /// // root.Child.Value == 111;
        /// // root.Children[0].Value == 222;
        /// // root.Children[1].Value == 333.
        /// </example>
        private static bool Set(
            object instance,
            Type type,
            string memberPath,
            object value,
            bool coerce,
            BindingFlags bindingFlags)
        {
            if ((instance == null && type == null) || string.IsNullOrEmpty(memberPath))
            {
                return false;
            }

            var container = instance;
            var containerType = container?.GetType() ?? type;
            int index;
            MemberInfo memberInfo;

            while (true)
            {
                var dot = memberPath.IndexOf('.');
                var size = (dot == -1) ? memberPath.Length : dot;
                var prop = memberPath.Substring(0, size);

                var indexerEnd = prop.LastIndexOf(']');

                index = -1;

                if (indexerEnd != -1)
                {
                    var indexerStart = prop.IndexOf('[');

                    index = int.Parse(
                        prop.Substring(indexerStart + 1, indexerEnd - indexerStart - 1),
                        CultureInfo.CurrentCulture);

                    prop = prop.Substring(0, indexerStart);
                }

                var memberInfos = containerType
                    .GetTypeInfo()
                    .GetMember(prop, MemberTypes.Field | MemberTypes.Property, bindingFlags);

                if (dot == -1 && index == -1)
                {
                    memberInfo = memberInfos.Length > 0 ? memberInfos[0] : null;

                    break;
                }

                var found = GetNextContainer(memberInfos, ref container, ref containerType);

                if (!found)
                {
                    return false;
                }

                if (index != -1)
                {
                    if (dot != -1)
                    {
                        container = GetElementAt(container, index);

                        if (container == null)
                        {
                            return false;
                        }

                        containerType = container.GetType();
                    }
                    else
                    {
                        if (container is IList)
                        {
                            containerType = typeof(IList);
                        }

                        memberInfo = containerType.GetTypeInfo().GetProperty("Item", bindingFlags);

                        break;
                    }
                }

                memberPath = memberPath.Substring(dot + 1);
            }

            if (memberInfo is PropertyInfo pi && (container != null || IsStatic(pi)) && pi.CanWrite)
            {
                if (coerce)
                {
                    value = CoerceValue(pi.PropertyType, value);
                }

                pi.SetValue(container, value, index != -1 ? new object[] { index } : null);

                return true;
            }
            else if (memberInfo is FieldInfo fi && (container != null || fi.IsStatic))
            {
                if (coerce)
                {
                    value = CoerceValue(fi.FieldType, value);
                }

                fi.SetValue(container, value);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Dynamically converts <paramref name="value"/> into the specified <paramref name="memberType"/>.
        /// </summary>
        /// <param name="memberType">A type to convert the value to.</param>
        /// <param name="value">A value to be dynamically converted.</param>
        /// <returns>The converted value.</returns>
        private static object CoerceValue(Type memberType, object value)
        {
            if (value is null)
            {
                return null;
            }

            if (memberType.GetTypeInfo().IsInstanceOfType(value))
            {
                return value;
            }

            var converter = TypeDescriptor.GetConverter(memberType);

            if (!converter.CanConvertFrom(value.GetType()))
            {
                return value;
            }

            return converter.ConvertFrom(value);
        }
    }
}
