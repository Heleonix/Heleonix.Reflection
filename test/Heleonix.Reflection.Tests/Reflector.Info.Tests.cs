// <copyright file="Reflector.Info.Tests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Heleonix.Reflection;
    using Heleonix.Reflection.Tests.Common.Dummies;
    using Heleonix.Testing.NUnit.Aaa;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the <see cref="Reflector"/>.
    /// </summary>
    public static partial class ReflectorTests
    {
        /// <summary>
        /// Tests the <see cref="Reflector.GetTypes(string)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.GetTypes))]
        public static void GetTypes()
        {
            string simpleName = null;
            Type[] types = null;

            Act(() =>
            {
                types = Reflector.GetTypes(simpleName);
            });

            When("types with a provided simple name exist in the calling assembly", () =>
            {
                simpleName = nameof(SubSubItem);

                Should("return types", () =>
                {
                    Assert.That(types, Has.Length.EqualTo(1));
                    Assert.That(types[0].Name, Is.EqualTo(simpleName));
                });
            });

            When("types with a provided simple name exist in the referenced assembly", () =>
            {
                simpleName = nameof(Reflector);

                Should("return types", () =>
                {
                    Assert.That(types, Has.Length.EqualTo(1));
                    Assert.That(types[0].Name, Is.EqualTo(simpleName));
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.GetMemberPath{TObject}(Expression{Func{TObject, object}})"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.GetMemberPath) + "(Expression<Func<TObject, object>>)")]
        public static void GetMemberPath1()
        {
            Expression<Func<DateTime, object>> expression = null;
            string result = null;

            Act(() =>
            {
                result = Reflector.GetMemberPath(expression);
            });

            When("target memberPath is property", () =>
            {
                expression = (DateTime dt) => dt.Date.TimeOfDay.Hours;

                Should("return specified path", () =>
                {
                    Assert.That(result, Is.EqualTo("Date.TimeOfDay.Hours"));
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.GetMemberPath{TObject}(Expression{Action{TObject}})"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.GetMemberPath) + "(Expression<Action<TObject>>)")]
        public static void GetMemberPath2()
        {
            Expression<Action<DateTime>> expression = null;
            string result = null;

            Act(() =>
            {
                result = Reflector.GetMemberPath(expression);
            });

            When("target memberPath is method", () =>
            {
                expression = (DateTime dt) => dt.Date.TimeOfDay.ToString();

                Should("return specified path", () =>
                {
                    Assert.That(result, Is.EqualTo("Date.TimeOfDay.ToString"));
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.GetMemberPath(LambdaExpression)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.GetMemberPath) + "(LambdaExpression)")]
        public static void GetMemberPath3()
        {
            LambdaExpression expression = null;
            string result = null;

            Act(() =>
            {
                result = Reflector.GetMemberPath(expression);
            });

            When("memberPath expression is null", () =>
            {
                expression = null;

                Should("return an empty string", () =>
                {
                    Assert.That(result, Is.Empty);
                });
            });

            When("expression does not have member expression", () =>
            {
                expression = (Expression<Func<DateTime, DateTime>>)(dt => dt);

                Should("return an empty string", () =>
                {
                    Assert.That(result, Is.Empty);
                });
            });

            When("target memberPath is property", () =>
            {
                expression = (Expression<Func<DateTime, int>>)((DateTime dt) => dt.Date.TimeOfDay.Hours);

                Should("return specified path", () =>
                {
                    Assert.That(result, Is.EqualTo("Date.TimeOfDay.Hours"));
                });
            });

            When("target memberPath is method", () =>
            {
                expression = (Expression<Func<DateTime, string>>)((DateTime dt) => dt.Date.TimeOfDay.ToString());

                Should("return specified path", () =>
                {
                    Assert.That(result, Is.EqualTo("Date.TimeOfDay.ToString"));
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.GetInfo(object, Type, string, Type[], BindingFlags)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.GetInfo))]
        public static void GetInfo()
        {
            // TEST A CASE WHEN FOUND MEMBERS ARE NEITHER PROPERTY NOR FIELD NOR METHODS NOR CONSTRUCTORS
            // WHAT KIND IT COULD BE THEN (EVEN INDEXERS ARE KIND OF PROPERTY) ???
            Root instance = null;
            Type type = null;
            string memberPath = null;
            Type[] parameterTypes = null;
            MemberInfo[] result = null;

            Act(() =>
            {
                result = Reflector.GetInfo(instance, type, memberPath, parameterTypes);
            });

            When("instance is null", () =>
            {
                instance = null;

                And("type is null", () =>
                {
                    type = null;

                    Should("return an empty array", () =>
                    {
                        Assert.That(result, Has.Length.Zero);
                    });
                });

                And("type is not null", () =>
                {
                    type = typeof(Root);

                    And("memberPath is null", () =>
                    {
                        memberPath = null;

                        Should("return an empty array", () =>
                        {
                            Assert.That(result, Has.Length.Zero);
                        });
                    });

                    And("memberPath is empty", () =>
                    {
                        memberPath = string.Empty;

                        Should("return an empty array", () =>
                        {
                            Assert.That(result, Has.Length.Zero);
                        });
                    });

                    And("memberPath exists", () =>
                    {
                        memberPath = "StaticSubItemProperty.SubSubItemProperty.TextProperty";

                        Should("return info for the member by the specified member path", () =>
                        {
                            Assert.That(result[0].Name, Is.EqualTo("TextProperty"));
                        });
                    });
                });
            });

            When("instance is not null", () =>
            {
                instance = new Root();

                And("memberPath with properties exists", () =>
                {
                    memberPath = "SubItemProperty.SubSubItemProperty.TextProperty";

                    Should("return info for the member by the specified member path", () =>
                    {
                        Assert.That(result[0].Name, Is.EqualTo("TextProperty"));
                    });
                });

                And("memberPath with fields exists", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.TextField";

                    Should("return info for the member by the specified member path", () =>
                    {
                        Assert.That(result[0].Name, Is.EqualTo("TextField"));
                    });
                });

                And("memberPath with method exists", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.Concat";
                    parameterTypes = new[] { typeof(string), typeof(string) };

                    Should("return info for the member by the specified member path", () =>
                    {
                        Assert.That(result[0].Name, Is.EqualTo("Concat"));
                    });
                });

                And("memberPath with constructor exists", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.ctor";
                    parameterTypes = null;

                    Should("return info for the constructor", () =>
                    {
                        Assert.That(result[0].Name, Is.EqualTo(".ctor"));
                    });
                });

                And("intermediate memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER.TextProperty";

                    Should("return an empty array", () =>
                    {
                        Assert.That(result, Has.Length.Zero);
                    });
                });

                And("target memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER";

                    Should("return an empty array", () =>
                    {
                        Assert.That(result, Has.Length.Zero);
                    });
                });

                And("target memberPath is method and it does not match by parameter types", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.Add";
                    parameterTypes = new[] { typeof(string), typeof(string) };

                    Should("return an empty array", () =>
                    {
                        Assert.That(result, Has.Length.Zero);
                    });
                });

                And("target memberPath is method and parameter types are null and there is two matched methods", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.Add";
                    parameterTypes = null;

                    Should("return two MemberInfo instances", () =>
                    {
                        Assert.That(result, Has.Length.EqualTo(2));
                        Assert.That(result[0].Name, Is.EqualTo("Add"));
                        Assert.That(result[0].Name, Is.EqualTo("Add"));
                    });
                });
            });
        }
    }
}
