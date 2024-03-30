// <copyright file="Reflector.Info.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Provides functionality for working with reflection.
    /// </summary>
    public static partial class Reflector
    {
        /// <summary>
        /// Gets the types by a simple name (a name without namespace) in the calling assembly and in the assemblies loaded into the current domain.
        /// </summary>
        /// <param name="simpleName">A simple name of types to load.</param>
        /// <returns>An array of found types.</returns>
        public static Type[] GetTypes(string simpleName)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = loadedAssemblies.SelectMany(a => a.GetTypes().Where(t => t.Name == simpleName)).ToArray();

            return types;
        }

        /// <summary>
        /// Gets a path to a member which returns some type.
        /// </summary>
        /// <typeparam name="TObject">A type of an object.</typeparam>
        /// <param name="memberPath">An expression to find a member.</param>
        /// <returns>A path to a member.</returns>
        /// <example>
        /// var path = Reflector.GetMemberPath{DateTime}(dt => dt.TimeOfDay.Negate());
        ///
        /// // path: "TimeOfDay.Negate".
        /// </example>
        public static string GetMemberPath<TObject>(Expression<Func<TObject, object>> memberPath)
            => GetMemberPath(memberPath as LambdaExpression);

        /// <summary>
        /// Gets a path to a member which returns <c>void</c>.
        /// </summary>
        /// <typeparam name="TObject">A type of an object.</typeparam>
        /// <param name="memberPath">An expression to find a member.</param>
        /// <returns>A path to a member.</returns>
        /// <example>
        /// var path = Reflector.GetMemberPath{List{int}}(list => list.Clear());
        ///
        /// // path: "Clear".
        /// </example>
        public static string GetMemberPath<TObject>(Expression<Action<TObject>> memberPath)
            => GetMemberPath(memberPath as LambdaExpression);

        /// <summary>
        /// Gets a path to a member using the specified (probably dynamically built) expression.
        /// </summary>
        /// <param name="memberPath">An expression to find a member.</param>
        /// <returns>
        /// A name of a member
        /// or
        /// an empty string if <paramref name="memberPath"/> is <c>null</c>.
        /// .</returns>
        public static string GetMemberPath(LambdaExpression memberPath)
        {
            if (memberPath == null)
            {
                return string.Empty;
            }

            var path = memberPath.Body is UnaryExpression uex ? uex.Operand.ToString() : memberPath.Body.ToString();

            var pathIndex = path.IndexOf('.');

            if (pathIndex == -1)
            {
                return string.Empty;
            }

            var parenthesesIndex = path.LastIndexOf('(');

            parenthesesIndex = parenthesesIndex == -1 ? path.Length : parenthesesIndex;

            return path.Substring(pathIndex + 1, parenthesesIndex - 1 - pathIndex);
        }

        /// <summary>
        /// Gets information about members.
        /// </summary>
        /// <param name="instance">A root object.</param>
        /// <param name="type">A type of a root object.
        /// If <paramref name="instance"/> is not <c>null</c>, then its type is used instead.
        /// </param>
        /// <param name="memberPath">A path to a member.</param>
        /// <param name="parameterTypes">Types of parameters to find methods or constructors.
        /// If <c>null</c> is passed, then types of parameters are ignored.</param>
        /// <param name="bindingFlags">Binding flags to find members.</param>
        /// <exception cref="TargetException">
        /// An intermediate member on a path thrown an exception. See inner exception for details.
        /// </exception>
        /// <returns>
        /// Information about found members or an empty array if no members are found
        /// or
        /// they are not reachable
        /// or
        /// they are not accessible.
        /// </returns>
        /// <example>
        /// var dt = DateTime.Now;
        ///
        /// var info = Reflector.GetInfo(instance: dt, type: null, memberPath: "TimeOfDay.Negate");
        ///
        /// // info[0].Name == "Negate";
        /// // info[0].MemberType == MemberTypes.Property.
        /// </example>
        public static MemberInfo[] GetInfo(
            object instance,
            Type type,
            string memberPath,
            Type[] parameterTypes = null,
            BindingFlags bindingFlags = DefaultBindingFlags)
        {
            if ((instance == null && type == null) || string.IsNullOrEmpty(memberPath))
            {
                return EmptyMemberInfo;
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
                    return EmptyMemberInfo;
                }

                memberPath = memberPath.Substring(prop.Length + 1);
            }

            var matchedMemberInfo = new List<MemberInfo>();

            for (int i = memberInfos.Length - 1; i >= 0; i--)
            {
                var mi = memberInfos[i];

                if (PropertyOrFieldMemberTypes.HasFlag(mi.MemberType)
                    || ParameterTypesMatch(((MethodBase)mi).GetParameters(), parameterTypes))
                {
                    matchedMemberInfo.Add(mi);
                }
            }

            return matchedMemberInfo.ToArray();
        }
    }
}
