// <copyright file="Reflector.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2017-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

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
        /// The property OR field flags to determine member types if they're property or field.
        /// </summary>
        private const MemberTypes PropertyOrFieldMemberTypes = MemberTypes.Property | MemberTypes.Field;

#pragma warning disable CA1825 // Avoid zero-length array allocations.
        /// <summary>
        /// The empty member information.
        /// </summary>
        private static readonly MemberInfo[] EmptyMemberInfo = new MemberInfo[0];
#pragma warning restore CA1825 // Avoid zero-length array allocations.

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
        /// // info[0].MemberType == MemberTypes.Property;
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
            MemberInfo[] memberInfos = null;

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

                var memberInfo = memberInfos != null && memberInfos.Length > 0 ? memberInfos[0] : null;

                if (memberInfo is PropertyInfo propertyInfo
                    && (container != null || IsStatic(propertyInfo)) && propertyInfo.CanRead)
                {
                    container = propertyInfo.GetValue(container);
                    containerType = container?.GetType() ?? propertyInfo.PropertyType;
                }
                else if (memberInfo is FieldInfo fieldInfo && (container != null || fieldInfo.IsStatic))
                {
                    container = fieldInfo.GetValue(container);
                    containerType = container?.GetType() ?? fieldInfo.FieldType;
                }
                else
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
                    || (mi is MethodBase mbi && ParameterTypesMatch(mbi.GetParameters(), parameterTypes)))
                {
                    matchedMemberInfo.Add(mi);
                }
            }

            return matchedMemberInfo.ToArray();
        }

        /// <summary>
        /// Determines whether the specified property is static by its getter (if it is defined) or by its setter (if it is defined).
        /// </summary>
        /// <param name="info">The property information.</param>
        /// <returns><c>true</c> if the specified property is static; otherwise, <c>false</c>.</returns>
        public static bool IsStatic(PropertyInfo info)
            => info != null && ((info.CanRead && info.GetMethod.IsStatic) || (info.CanWrite && info.SetMethod.IsStatic));

#if !NETSTANDARD1_6
        /// <summary>
        /// Gets the types by a simple name (a name without namespace) in the calling assembly and in the assemblies loaded into the current domain.
        /// </summary>
        /// <param name="simpleName">A simple name of types to load.</param>
        /// <returns>An array of found types.</returns>
        public static Type[] GetTypes(string simpleName)
        {
            var types = Assembly.GetCallingAssembly().GetTypes().Where(t => t.Name == simpleName).ToList();

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            types.AddRange(loadedAssemblies.SelectMany(a => a.GetTypes().Where(t => t.Name == simpleName)));

            return types.ToArray();
        }
#endif

        /// <summary>
        /// Gets a path to a member which returns some type.
        /// </summary>
        /// <typeparam name="TObject">A type of an object.</typeparam>
        /// <param name="memberPath">An expression to find a member.</param>
        /// <returns>A path to a member.</returns>
        /// <example>
        /// var path = Reflector.GetMemberPath{DateTime}(dt => dt.TimeOfDay.Negate());
        ///
        /// // path: "TimeOfDay.Negate"
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
        /// // path: "Clear"
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

            var path = memberPath.Body.ToString();

            // obj => obj.Item.SubItem.SubSubItem
            //           ^ 10
            var pathIndex = path.IndexOf('.');

            if (pathIndex <= 0)
            {
                return string.Empty;
            }

            // obj => obj.Item.SubItem.SubSubMethod(int)
            //                                  36 ^   ^ 40
            var parenthesesIndex = path.LastIndexOf('(');

            parenthesesIndex = parenthesesIndex == -1 ? path.Length : parenthesesIndex;

            return path.Substring(pathIndex + 1, parenthesesIndex - 1 - pathIndex);
        }

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
            MemberInfo memberInfo = null;

            while (memberPath.Length > 0)
            {
                var dot = memberPath.IndexOf('.');
                var size = (dot == -1) ? memberPath.Length : dot;
                var prop = memberPath.Substring(0, size);

                var index = 0;
                var property = string.Empty;
                MemberInfo[] members;

                if (IsIndexer(prop))
                {
                    property = prop.Substring(0, prop.IndexOf('['));
                    index = int.Parse(prop.Split('[', ']')[1], CultureInfo.CurrentCulture);

                    members = containerType
                            .GetTypeInfo()
                            .GetMember(property, MemberTypes.Field | MemberTypes.Property, bindingFlags);

                    memberInfo = members != null && members.Length > 0 ? members[0] : null;
                    if (memberInfo is PropertyInfo propertyInfo2 && (container != null || IsStatic(propertyInfo2)) && propertyInfo2.CanRead)
                    {
                        container = propertyInfo2.GetValue(container);
                        containerType = container?.GetType() ?? propertyInfo2.PropertyType;
                    }
                    else if (memberInfo is FieldInfo fieldInfo2 && (container != null || fieldInfo2.IsStatic))
                    {
                        container = fieldInfo2.GetValue(container);
                        containerType = container?.GetType() ?? fieldInfo2.FieldType;
                    }

                    var enumerable = container as IEnumerable;
                    if (enumerable != null)
                    {
                        var counter = 0;
                        var found = false;
                        foreach (var item in enumerable)
                        {
                            if (counter == index)
                            {
                                container = item;
                                containerType = container?.GetType();

                                found = true;
                                break;
                            }

                            counter++;
                        }

                        if (!found)
                        {
                            value = default;
                            return false;
                        }
                    }
                    else
                    {
                        value = default;
                        return false;
                    }
                }
                else
                {
                    members = containerType
                    .GetTypeInfo()
                    .GetMember(prop, MemberTypes.Field | MemberTypes.Property, bindingFlags);

                    memberInfo = members != null && members.Length > 0 ? members[0] : null;

                    if (memberInfo is PropertyInfo propertyInfo
                        && (container != null || IsStatic(propertyInfo)) && propertyInfo.CanRead)
                    {
                        container = propertyInfo.GetValue(container);
                        containerType = container?.GetType() ?? propertyInfo.PropertyType;
                    }
                    else if (memberInfo is FieldInfo fieldInfo && (container != null || fieldInfo.IsStatic))
                    {
                        container = fieldInfo.GetValue(container);
                        containerType = container?.GetType() ?? fieldInfo.FieldType;
                    }
                    else
                    {
                        value = default;
                        return false;
                    }
                }

                if (dot == -1)
                {
                    break;
                }

                memberPath = memberPath.Substring(prop.Length + 1);
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
        /// public class Root { public Child Child { get; set; } = new Child(); }
        /// public class Child { public int Value { get; set; } }
        ///
        /// var root = new Root();
        ///
        /// var success = Reflector.Set(root, null, "Child.Value", 12345);
        ///
        /// // success == true;
        /// // root.Child.Value == 12345;
        /// </example>
        public static bool Set(
            object instance,
            Type type,
            string memberPath,
            object value,
            BindingFlags bindingFlags = DefaultBindingFlags)
        {
            if ((instance == null && type == null) || string.IsNullOrEmpty(memberPath))
            {
                return false;
            }

            var container = instance;
            var containerType = container?.GetType() ?? type;
            MemberInfo memberInfo = null;

            while (true)
            {
                var dot = memberPath.IndexOf('.');
                var size = (dot == -1) ? memberPath.Length : dot;
                var prop = memberPath.Substring(0, size);

                var index = 0;
                var property = string.Empty;
                MemberInfo[] members;

                if (IsIndexer(prop))
                {
                    property = prop.Substring(0, prop.IndexOf('['));
                    index = int.Parse(prop.Split('[', ']')[1], CultureInfo.CurrentCulture);

                    members = containerType
                            .GetTypeInfo()
                            .GetMember(property, MemberTypes.Field | MemberTypes.Property, bindingFlags);

                    memberInfo = members != null && members.Length > 0 ? members[0] : null;
                    if (memberInfo is PropertyInfo propertyInfo2 && (container != null || IsStatic(propertyInfo2)) && propertyInfo2.CanRead)
                    {
                        container = propertyInfo2.GetValue(container);
                        containerType = container?.GetType() ?? propertyInfo2.PropertyType;
                    }
                    else if (memberInfo is FieldInfo fieldInfo2 && (container != null || fieldInfo2.IsStatic))
                    {
                        container = fieldInfo2.GetValue(container);
                        containerType = container?.GetType() ?? fieldInfo2.FieldType;
                    }

                    var enumerable = container as IEnumerable;
                    if (enumerable != null)
                    {
                        var counter = 0;
                        var found = false;
                        foreach (var item in enumerable)
                        {
                            if (counter == index)
                            {
                                container = item;
                                containerType = container?.GetType();

                                found = true;
                                break;
                            }

                            counter++;
                        }

                        if (!found)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    members = containerType
                    .GetTypeInfo()
                    .GetMember(prop, MemberTypes.Field | MemberTypes.Property, bindingFlags);

                    memberInfo = members != null && members.Length > 0 ? members[0] : null;

                    if (dot == -1)
                    {
                        break;
                    }

                    if (memberInfo is PropertyInfo propertyInfo
                        && (container != null || IsStatic(propertyInfo)) && propertyInfo.CanRead)
                    {
                        container = propertyInfo.GetValue(container);
                        containerType = container?.GetType() ?? propertyInfo.PropertyType;
                    }
                    else if (memberInfo is FieldInfo fieldInfo && (container != null || fieldInfo.IsStatic))
                    {
                        container = fieldInfo.GetValue(container);
                        containerType = container?.GetType() ?? fieldInfo.FieldType;
                    }
                    else
                    {
                        return false;
                    }
                }

                memberPath = memberPath.Substring(prop.Length + 1);
            }

            if (memberInfo is PropertyInfo pi && (container != null || IsStatic(pi)) && pi.CanWrite)
            {
                pi.SetValue(container, value);

                return true;
            }
            else if (memberInfo is FieldInfo fi && (container != null || fi.IsStatic))
            {
                fi.SetValue(container, value);

                return true;
            }

            return false;
        }

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
        /// // result.Year == DateTime.Now.Date.Year + 10;
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
            MemberInfo[] memberInfos = null;

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

                var memberInfo = memberInfos != null && memberInfos.Length > 0 ? memberInfos[0] : null;

                if (memberInfo is PropertyInfo propertyInfo
                    && (container != null || IsStatic(propertyInfo)) && propertyInfo.CanRead)
                {
                    container = propertyInfo.GetValue(container);
                    containerType = container?.GetType() ?? propertyInfo.PropertyType;
                }
                else if (memberInfo is FieldInfo fieldInfo && (container != null || fieldInfo.IsStatic))
                {
                    container = fieldInfo.GetValue(container);
                    containerType = container?.GetType() ?? fieldInfo.FieldType;
                }
                else
                {
                    returnValue = default;

                    return false;
                }

                memberPath = memberPath.Substring(prop.Length + 1);
            }

            MethodBase methodInfo = null;

            for (int i = memberInfos.Length - 1; i >= 0; i--)
            {
                if (memberInfos[i] is MethodBase mbi && ParameterTypesMatch(mbi.GetParameters(), parameterTypes))
                {
                    methodInfo = mbi;

                    break;
                }
            }

            if (methodInfo != null)
            {
                object rawValue = null;

                if (container != null || methodInfo.IsStatic)
                {
                    rawValue = methodInfo.Invoke(container, arguments);
                }
                else if (methodInfo is ConstructorInfo constructorInfo)
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

        /// <summary>
        /// Creates a getter. Works with exactly specified types without conversion. This is the fastest implementation.
        /// </summary>
        /// <typeparam name="TObject">The concrete type of the container's object.</typeparam>
        /// <typeparam name="TReturn">The concrete type of the member.</typeparam>
        /// <param name="memberPath">The path to a member.</param>
        /// <returns>
        /// A compiled delegate to get a value
        /// or
        /// <c>null</c> if the <paramref name="memberPath"/> is <c>null</c>.
        /// </returns>
        /// <example>
        /// var getter = Reflector.CreateGetter(dt => dt.Date.Month);
        ///
        /// var value = getter(DateTime.Now);
        ///
        /// // value == DateTime.Now.Date.Month;
        /// </example>
        public static Func<TObject, TReturn> CreateGetter<TObject, TReturn>(
            Expression<Func<TObject, TReturn>> memberPath)
                => memberPath?.Compile();

        /// <summary>
        /// Creates a getter. Can create getters with any convertable types for polimorphic usage.
        /// </summary>
        /// <typeparam name="TObject">The type of the desired object in a delegate to create.</typeparam>
        /// <typeparam name="TReturn">The type of the desired member in a delegate to create.</typeparam>
        /// <param name="memberPath">The path to a member.</param>
        /// <param name="containerType">
        /// A type of a container's object which contains the member.
        /// If null is specified, then <typeparamref name="TObject"/> is used without conversion.
        /// </param>
        /// <returns>
        /// A compiled delegate to get a value
        /// or
        /// <c>null</c> if the <paramref name="memberPath"/> is <c>null</c> or empty.
        /// </returns>
        /// <example>
        /// var getter = Reflector.CreateGetter{object, object}("Date.Month", typeof(DateTime));
        ///
        /// var value = getter(DateTime.Now);
        ///
        /// // value == DateTime.Now.Date.Month;
        /// </example>
        public static Func<TObject, TReturn> CreateGetter<TObject, TReturn>(
            string memberPath,
            Type containerType = null)
        {
            if (string.IsNullOrEmpty(memberPath))
            {
                return null;
            }

            var param = Expression.Parameter(typeof(TObject));

            Expression body = param;

            if (containerType != null && containerType != typeof(TObject))
            {
                body = Expression.Convert(param, containerType);
            }

            while (true)
            {
                var dot = memberPath.IndexOf('.');
                var size = (dot == -1) ? memberPath.Length : dot;
                var member = memberPath.Substring(0, size);

                body = Expression.PropertyOrField(body, member);

                if (dot == -1)
                {
                    break;
                }

                memberPath = memberPath.Substring(member.Length + 1);
            }

            if (body.Type != typeof(TReturn))
            {
                body = Expression.Convert(body, typeof(TReturn));
            }

            return Expression.Lambda<Func<TObject, TReturn>>(body, param).Compile();
        }

        /// <summary>
        /// Creates the setter.
        /// Works with exactly specified types without conversion.
        /// This is the fastest implementation.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TValue">The type of the final member.</typeparam>
        /// <param name="memberPath">The path to a member.</param>
        /// <returns>A compiled delegate to set a value
        /// or
        /// <c>null</c> if <paramref name="memberPath"/> is <c>null</c>.</returns>
        /// <example>
        /// public class Root { public Child Child { get; set; } = new Child(); }
        /// public class Child { public int Value { get; set; } }
        ///
        /// var setter = Reflector.CreateSetter{Root, int}(r => r.Child.Value);
        /// var root = new Root();
        ///
        /// setter(root, 12345);
        ///
        /// // root.Child.Value == 12345;
        /// </example>
        public static Action<TObject, TValue> CreateSetter<TObject, TValue>(
            Expression<Func<TObject, TValue>> memberPath)
        {
            if (memberPath == null)
            {
                return null;
            }

            var param = Expression.Parameter(memberPath.Body.Type);

            return Expression
                .Lambda<Action<TObject, TValue>>(
                    Expression.Assign(memberPath.Body, param),
                    memberPath.Parameters.First(),
                    param)
                .Compile();
        }

        /// <summary>
        /// Creates a setter. Can create setters with any convertable types for polimorphic usage.
        /// </summary>
        /// <typeparam name="TObject">The type of the desired object in a delegate to create.</typeparam>
        /// <typeparam name="TValue">The type of the desired member in a delegate to create.</typeparam>
        /// <param name="memberPath">The path to a member.</param>
        /// <param name="containerType">
        /// A type of a container's object which contains the member.
        /// If null is specified, then <typeparamref name="TObject"/> is used without conversion.
        /// </param>
        /// <returns>
        /// A compiled delegate to set a value
        /// or
        /// <c>null</c> if the <paramref name="memberPath"/> is <c>null</c> or empty.
        /// </returns>
        /// <example>
        /// public class Root { public Child Child { get; set; } = new Child(); }
        /// public class Child { public int Value { get; set; } }
        ///
        /// var setter = Reflector.CreateSetter{Root, int}("Child.Value", typeof(Root));
        /// var root = new Root();
        ///
        /// setter(root, 12345);
        ///
        /// // root.Child.Value == 12345;
        /// </example>
        public static Action<TObject, TValue> CreateSetter<TObject, TValue>(
            string memberPath,
            Type containerType = null)
        {
            if (string.IsNullOrEmpty(memberPath))
            {
                return null;
            }

            var containerParam = Expression.Parameter(typeof(TObject));

            Expression body = containerParam;

            if (containerType != null && containerType != typeof(TObject))
            {
                body = Expression.Convert(containerParam, containerType);
            }

            while (true)
            {
                var dot = memberPath.IndexOf('.');
                var size = (dot == -1) ? memberPath.Length : dot;
                var member = memberPath.Substring(0, size);

                body = Expression.PropertyOrField(body, member);

                if (dot == -1)
                {
                    break;
                }

                memberPath = memberPath.Substring(member.Length + 1);
            }

            var valueParam = Expression.Parameter(typeof(TValue));

            Expression value = valueParam;

            if (body.Type != typeof(TValue))
            {
                value = Expression.Convert(valueParam, body.Type);
            }

            return Expression
                .Lambda<Action<TObject, TValue>>(
                    Expression.Assign(body, value),
                    containerParam,
                    valueParam)
                .Compile();
        }

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

        private static bool IsIndexer(string prop)
        {
            return prop.Contains('[');
        }
    }
}