// <copyright file="Reflector.Create.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides functionality for working with reflection.
    /// </summary>
    public static partial class Reflector
    {
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
        /// // value == DateTime.Now.Date.Month.
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
        /// // value == DateTime.Now.Date.Month.
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
        /// // root.Child.Value == 12345.
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
        /// // root.Child.Value == 12345.
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
    }
}
