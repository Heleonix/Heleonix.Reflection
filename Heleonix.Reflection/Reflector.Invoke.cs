// <copyright file="Reflector.Invoke.cs" company="Heleonix - Hennadii Lutsyshyn">
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
        /// Invokes a method or constructor by the provided path.
        /// Use "ctor" to invoke constructors, i.e."Item.SubItem.ctor".
        /// </summary>
        /// <typeparam name="TReturn">A type of a value to be returned.</typeparam>
        /// <param name="instance">A root object.</param>
        /// <param name="type">A type of a root object.
        /// If <paramref name="instance"/> is not <c>null</c>, then its runtime type is used instead.
        /// </param>
        /// <param name="memberPath">A path to a member to invoke.</param>
        /// <param name="parameterTypes">
        /// Types of parameters to find a method by.
        /// Pass <c>null</c> to ignore parameters, or an empty array for parameterless methods.
        /// </param>
        /// <param name="returnValue">A value to be returned if a member is not void.</param>
        /// <param name="bindingFlags">Binding flags to find members.</param>
        /// <param name="arguments">Arguments to be passed into a member to invoke.</param>
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
        /// an intermediate member is neither <see cref="PropertyInfo"/> nor <see cref="FieldInfo"/>
        /// or
        /// an intermediate member is not static and its container is null
        /// or
        /// a target member is not <see cref="MethodBase"/>
        /// or
        /// a target value is not of type <typeparamref name="TReturn"/>.
        /// </returns>
        /// <example>
        /// var success = Reflector.Invoke(
        ///     DateTime.Now,
        ///     null,
        ///     "Date.AddYears",
        ///     new[] { typeof(int) },
        ///     out DateTime result, arguments: 10);
        ///
        /// // success == true;
        /// // result.Year == DateTime.Now.Date.Year + 10.
        /// </example>
        public static bool Invoke<TReturn>(
            object instance,
            Type type,
            string memberPath,
            Type[] parameterTypes,
            out TReturn returnValue,
            BindingFlags bindingFlags = DefaultBindingFlags,
            params object[] arguments)
        {
            if ((instance == null && type == null) || string.IsNullOrEmpty(memberPath))
            {
                returnValue = default;

                return false;
            }

            var container = instance;
            var containerType = container?.GetType() ?? type;
            MemberInfo[] memberInfos;

            while (true)
            {
                var dot = memberPath.IndexOf('.');
                var size = (dot == -1) ? memberPath.Length : dot;
                var prop = memberPath.Substring(0, size);

                if (dot == -1 && prop.Equals("ctor", StringComparison.OrdinalIgnoreCase))
                {
                    prop = ".ctor";
                }

                memberInfos = containerType
                    .GetTypeInfo()
                    .GetMember(
                        prop,
                        MemberTypes.Field | MemberTypes.Property | MemberTypes.Method | MemberTypes.Constructor,
                        bindingFlags);

                if (dot == -1)
                {
                    break;
                }

                var found = GetNextContainer(memberInfos, ref container, ref containerType);

                if (!found)
                {
                    returnValue = default;

                    return false;
                }

                memberPath = memberPath.Substring(prop.Length + 1);
            }

            MethodBase methodBaseInfo = null;

            for (int i = memberInfos.Length - 1; i >= 0; i--)
            {
                if (memberInfos[i] is MethodBase mbi && ParameterTypesMatch(mbi.GetParameters(), parameterTypes))
                {
                    methodBaseInfo = mbi;

                    break;
                }
            }

            if (methodBaseInfo != null)
            {
                object rawValue = null;

                if (container != null || methodBaseInfo.IsStatic)
                {
                    rawValue = methodBaseInfo.Invoke(container, arguments);
                }
                else if (methodBaseInfo is ConstructorInfo constructorInfo)
                {
                    rawValue = constructorInfo.Invoke(arguments);
                }

                if (rawValue == null)
                {
                    returnValue = default;

                    return true;
                }
                else if (rawValue is TReturn)
                {
                    returnValue = (TReturn)rawValue;

                    return true;
                }
            }

            returnValue = default;

            return false;
        }
    }
}
