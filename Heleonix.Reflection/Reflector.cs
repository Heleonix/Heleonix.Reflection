// <copyright file="Reflector.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2017-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides functionality for working with reflection.
    /// </summary>
    public static class Reflector
    {
        /// <summary>
        /// The default binding flags.
        /// </summary>
        private const BindingFlags DefaultBindingFlags
            = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        /// <summary>
        /// The dot char to split strings.
        /// </summary>
        private static readonly char[] DotSplitChar = new[] { '.' };

        /// <summary>
        /// Gets information about members.
        /// </summary>
        /// <param name="instance">A root object.</param>
        /// <param name="type">A type of a root object.
        /// If <paramref name="instance"/> is not <c>null</c>, then its typeis used instead.
        /// </param>
        /// <param name="memberPath">A path to a member.</param>
        /// <param name="paramTypes">Types of parameters to find methods or constructors.
        /// If <c>null</c> is passed, then types of parameters are ignored.</param>
        /// <param name="requireIntermediateValues">
        /// Determines whether intermediate members within the given path must not be <c>null</c>.
        /// </param>
        /// <param name="bindingFlags">Binding flags to find members.</param>
        /// <exception cref="AmbiguousMatchException">
        /// More than one member is found on the intermediate path of the <paramref name="memberPath"/>.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        /// An intermediate member on a path thrown an exception. See inner exception for details.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Failed to invoke an intermediate member on a path. See inner exception for details.
        /// </exception>
        /// <returns>Information about found members or <c>null</c> if no members are found
        /// or they are not reachable or they are not accessible.
        /// </returns>
        /// <example>
        /// var dt = DateTime.Now;
        ///
        /// var info = Reflector.GetInfo(instance: dt, type: null, memberPath: "TimeOfDay.Negate", requireIntermediateValues: true);
        ///
        /// // info.ContainerObject == dt;
        /// // info.ContainerType == typeof(DateTime);
        /// // info.Members[0]: MethodInfo about the Negate method
        /// </example>
        /// <example>
        /// var tuple = new Tuple{Tuple{int}}(null);
        ///
        /// var info1 = Reflector.GetInfo(tuple, null, "Item1.Item1", requireIntermediateValues: true);
        ///
        /// // info1 == null
        ///
        /// var info2 = Reflector.GetInfo(tuple, null, "Item1.Item1", requireIntermediateValues: false);
        ///
        /// // info2 == typeof(int)
        /// </example>
        public static MembersInfo GetInfo(
            object instance,
            Type type,
            string memberPath,
            Type[] paramTypes = null,
            bool requireIntermediateValues = false,
            BindingFlags bindingFlags = DefaultBindingFlags)
        {
            if (instance == null && type == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(memberPath))
            {
                return null;
            }

            var foundMembers = new MembersInfo { ContainerObject = instance, ContainerType = type };
            var intermediateFoundMember = new MembersInfo();

            if (foundMembers.ContainerObject != null)
            {
                foundMembers.ContainerType = foundMembers.ContainerObject.GetType();
            }

            var paths = memberPath.Split(DotSplitChar, StringSplitOptions.RemoveEmptyEntries);

            foreach (var path in paths.Take(paths.Length - 1))
            {
                var memberInfo = foundMembers
                    .ContainerType
                    .GetTypeInfo()
                    .GetMember(path, MemberTypes.Field | MemberTypes.Property, bindingFlags)
                    .FirstOrDefault();

                if (memberInfo == null)
                {
                    return null;
                }

                intermediateFoundMember.Members.Clear();
                intermediateFoundMember.Members.Add(memberInfo);
                intermediateFoundMember.ContainerObject = foundMembers.ContainerObject;

                Get(intermediateFoundMember, out object intermediateMemberInstance);

                if (requireIntermediateValues && intermediateMemberInstance == null)
                {
                    return null;
                }

                foundMembers.ContainerObject = intermediateMemberInstance;

                foundMembers.ContainerType = foundMembers.ContainerObject?.GetType()
                    ?? (memberInfo.MemberType == MemberTypes.Property
                        ? ((PropertyInfo)memberInfo).PropertyType
                        : ((FieldInfo)memberInfo).FieldType);
            }

            if (paths[paths.Length - 1].EndsWith("ctor", StringComparison.OrdinalIgnoreCase))
            {
                paths[paths.Length - 1] = "." + paths[paths.Length - 1];
            }

            var membersInfo = foundMembers.ContainerType.GetTypeInfo().GetMembers(bindingFlags)
                .Where(mi => mi.Name == paths[paths.Length - 1]);

            membersInfo = membersInfo.Where(mi =>
                mi.MemberType == MemberTypes.Property || mi.MemberType == MemberTypes.Field
                || (mi.MemberType == MemberTypes.Method
                    && ParameterTypesMatch(((MethodInfo)mi).GetParameters(), paramTypes))
                || (mi.MemberType == MemberTypes.Constructor
                    && ParameterTypesMatch(((ConstructorInfo)mi).GetParameters(), paramTypes)));

            foundMembers.Members.AddRange(membersInfo);

            return foundMembers.Members.Count == 0 ? null : foundMembers;
        }

        /// <summary>
        /// Determines whether the specified property is static by its getter (if it is defined) or by its setter (if it is defined).
        /// </summary>
        /// <param name="info">The property information.</param>
        /// <returns><c>true</c> if the specified property is static; otherwise, <c>false</c>.</returns>
        public static bool IsStatic(PropertyInfo info)
            => (info != null && info.CanRead && info.GetMethod.IsStatic)
            || (info != null && info.CanWrite && info.SetMethod.IsStatic);

#if !NETSTANDARD1_6
        /// <summary>
        /// Gets the types by a simple name (a name without namespace) in the calling assembly and in the assemblies loaded into the current domain.
        /// </summary>
        /// <param name="simpleName">A simple name of types to load.</param>
        /// <returns>A list of found types.</returns>
        public static IList<Type> GetTypes(string simpleName)
        {
            var types = Assembly.GetCallingAssembly().GetTypes().Where(t => t.Name == simpleName).ToList();

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            types.AddRange(loadedAssemblies.SelectMany(a => a.GetTypes().Where(t => t.Name == simpleName)));

            return types;
        }
#endif

        /// <summary>
        /// Gets a path to a member which returns some type.
        /// </summary>
        /// <typeparam name="TObject">A type of an object.</typeparam>
        /// <param name="expression">An expression to find a member.</param>
        /// <returns>A path to a member.</returns>
        /// <example>
        /// var path = Reflector.GetMemberPath{DateTime}(dt => dt.TimeOfDay.Negate());
        ///
        /// // path: "TimeOfDay.Negate"
        /// </example>
        public static string GetMemberPath<TObject>(Expression<Func<TObject, object>> expression)
            => GetMemberPath(expression as Expression);

        /// <summary>
        /// Gets a path to a member which returns <c>void</c>.
        /// </summary>
        /// <typeparam name="TObject">A type of an object.</typeparam>
        /// <param name="expression">An expression to find a member.</param>
        /// <returns>A path to a member.</returns>
        /// <example>
        /// var path = Reflector.GetMemberPath{List{int}}(list => list.Clear());
        ///
        /// // path: "Clear"
        /// </example>
        public static string GetMemberPath<TObject>(Expression<Action<TObject>> expression)
            => GetMemberPath(expression as Expression);

        /// <summary>
        /// Gets a path to a member using the specified (probably dynamically built) expression,
        /// which must be of type or inherited from the <see cref="LambdaExpression"/>.
        /// </summary>
        /// <param name="expression">An expression to find a member.</param>
        /// <returns>A name of a member.</returns>
        public static string GetMemberPath(Expression expression)
            => Regex.Replace(
                string.Join(
                ".",
                (expression as LambdaExpression)?.Body.ToString().Split('.').Skip(1) ?? Enumerable.Empty<string>()),
                @"\(.*\)",
                string.Empty);

        /// <summary>
        /// Sets a provided value to the provided <see cref="MembersInfo"/>.
        /// </summary>
        /// <param name="info">The member information.</param>
        /// <param name="value">A value to set.</param>
        /// <exception cref="TargetInvocationException">
        /// Target thrown an exception during execution. See inner exception for details.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Could not invoke a member for current object's state. See inner exception for details.
        /// </exception>
        /// <returns>
        /// <c>true</c> in case of success, otherwise <c>false</c> if the <paramref name="info"/> is <c>null</c>
        /// or <see cref="MembersInfo.ContainerObject"/> is <c>null</c> and a member is not static
        /// or <see cref="PropertyInfo.CanWrite"/> is <c>false</c>.
        /// </returns>
        public static bool Set(MembersInfo info, object value)
        {
            try
            {
                var memberInfo = info?.Members.FirstOrDefault();

                if (memberInfo is PropertyInfo propertyInfo
                    && (info.ContainerObject != null || IsStatic(propertyInfo)) && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(info.ContainerObject, value);

                    return true;
                }
                else if (memberInfo is FieldInfo fieldInfo && (info.ContainerObject != null || fieldInfo.IsStatic))
                {
                    fieldInfo.SetValue(info.ContainerObject, value);

                    return true;
                }

                return false;
            }
            catch (TargetInvocationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets a value of the provided <see cref="MembersInfo"/>.
        /// </summary>
        /// <typeparam name="TReturn">A type to cast return value to.</typeparam>
        /// <param name="info">The member information.</param>
        /// <param name="value">A returned value.</param>
        /// <exception cref="TargetInvocationException">
        /// Target thrown an exception during execution. See inner exception for details.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Could not invoke a member for current object's state. See inner exception for details.
        /// </exception>
        /// <returns>
        /// <c>true</c> in case of success, otherwise <c>false</c> if an <paramref name="info"/> is <c>null</c>
        /// or the <see cref="MembersInfo.ContainerObject"/> is <c>null</c> and a member is not static
        /// or the return value is not of type <typeparamref name="TReturn"/>
        /// or <see cref="PropertyInfo.CanRead"/> is <c>false</c>.
        /// </returns>
        public static bool Get<TReturn>(MembersInfo info, out TReturn value)
        {
            try
            {
                var memberInfo = info?.Members.FirstOrDefault();

                object rawValue = null;

                value = default;

                if (memberInfo is PropertyInfo propertyInfo
                    && (info.ContainerObject != null || IsStatic(propertyInfo)) && propertyInfo.CanRead)
                {
                    rawValue = propertyInfo.GetValue(info.ContainerObject);
                }
                else if (memberInfo is FieldInfo fieldInfo && (info.ContainerObject != null || fieldInfo.IsStatic))
                {
                    rawValue = fieldInfo.GetValue(info.ContainerObject);
                }
                else
                {
                    return false;
                }

                if (rawValue == null)
                {
                    return true;
                }

                if (rawValue is TReturn)
                {
                    value = (TReturn)rawValue;

                    return true;
                }

                return false;
            }
            catch (TargetInvocationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Invokes a method provided by <paramref name="info"/> with specified arguments.
        /// </summary>
        /// <typeparam name="TReturn">A type to cast return value to.</typeparam>
        /// <param name="info">The member information.</param>
        /// <param name="result">A returned value.</param>
        /// <param name="arguments">Arguments to pass to a method.</param>
        /// <exception cref="TargetInvocationException">
        /// Target thrown an exception during execution. See inner exception for details.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Could not invoke a member for current object's state. See inner exception for details.
        /// </exception>
        /// <returns>
        /// <c>true</c> in case of success, otherwise <c>false</c> if the <paramref name="info"/> is <c>null</c>
        /// or the <see cref="MembersInfo.ContainerObject"/> is <c>null</c> and a member is not static
        /// or the return value is not of type <typeparamref name="TReturn"/>.
        /// </returns>
        public static bool Invoke<TReturn>(MembersInfo info, out TReturn result, params object[] arguments)
        {
            try
            {
                if (info?.Members.FirstOrDefault() is MethodBase memberInfo
                    && memberInfo.MemberType.HasFlag(MemberTypes.Method | MemberTypes.Constructor)
                    && (info.ContainerObject != null || memberInfo.IsStatic))
                {
                    var rawResult = memberInfo.Invoke(info.ContainerObject, arguments);

                    if (rawResult == null)
                    {
                        result = default;

                        return true;
                    }

                    if (rawResult is TReturn)
                    {
                        result = (TReturn)rawResult;

                        return true;
                    }
                }

                result = default;

                return false;
            }
            catch (TargetInvocationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Checks if parameters have corresponding types.
        /// </summary>
        /// <param name="paramInfos">The parameter info.</param>
        /// <param name="paramTypes">The parameter types.</param>
        /// <returns><c>true</c> if parameters match, otherwise <c>false</c>.</returns>
        private static bool ParameterTypesMatch(ICollection<ParameterInfo> paramInfos, IList<Type> paramTypes)
            => paramTypes == null || (paramTypes.Count == paramInfos.Count
                && !paramInfos.Where((t, i) => t.ParameterType != paramTypes[i]).Any());
    }
}