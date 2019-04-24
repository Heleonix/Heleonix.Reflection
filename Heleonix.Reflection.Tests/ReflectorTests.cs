// <copyright file="ReflectorTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2017-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests
{
    using System;
    using System.Collections.Generic;
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
    [ComponentTest(Type = typeof(Reflector))]
    public static class ReflectorTests
    {
        /// <summary>
        /// Tests the <see cref="Reflector.GetInfo(object, Type, string, Type[], BindingFlags)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.GetInfo))]
        public static void GetInfo()
        {
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
                        memberPath = "StaticSubItemProperty.SubSubItemProperty.NumberProperty";

                        Should("return info for the member by the specified member path", () =>
                        {
                            Assert.That(result[0].Name, Is.EqualTo("NumberProperty"));
                        });
                    });
                });
            });

            When("instance is not null", () =>
            {
                instance = new Root();

                And("memberPath with properties exists", () =>
                {
                    memberPath = "SubItemProperty.SubSubItemProperty.NumberProperty";

                    Should("return info for the member by the specified member path", () =>
                    {
                        Assert.That(result[0].Name, Is.EqualTo("NumberProperty"));
                    });
                });

                And("memberPath with fields exists", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.NumberField";

                    Should("return info for the member by the specified member path", () =>
                    {
                        Assert.That(result[0].Name, Is.EqualTo("NumberField"));
                    });
                });

                And("memberPath with method exists", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.Add";
                    parameterTypes = new[] { typeof(int), typeof(int) };

                    Should("return info for the member by the specified member path", () =>
                    {
                        Assert.That(result[0].Name, Is.EqualTo("Add"));
                    });
                });

                And("intermediate memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER.NumberProperty";

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

        /// <summary>
        /// Tests the <see cref="Reflector.GetMemberPath(LambdaExpression)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.GetMemberPath))]
        public static void GetMemberPath()
        {
            LambdaExpression expression = null;
            string result = null;

            Act(() =>
            {
                result = Reflector.GetMemberPath(expression);
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
        /// Tests the <see cref="Reflector.Get{TReturn}(object, Type, string, out TReturn, BindingFlags)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.Get))]
        public static void Get()
        {
            Root instance = null;
            Type type = null;
            string memberPath = null;
            object result = null;
            var returnValue = false;

            Act(() =>
            {
                returnValue = Reflector.Get(instance, type, memberPath, out result);
            });

            When("instance is null", () =>
            {
                instance = null;

                And("type is null", () =>
                {
                    type = null;

                    Should("provide default value and return false", () =>
                    {
                        Assert.That(result, Is.Null);
                        Assert.That(returnValue, Is.False);
                    });
                });

                And("type is not null", () =>
                {
                    type = typeof(Root);

                    And("memberPath is null", () =>
                    {
                        memberPath = null;

                        Should("provide default value and return false", () =>
                        {
                            Assert.That(result, Is.Null);
                            Assert.That(returnValue, Is.False);
                        });
                    });

                    And("memberPath is empty", () =>
                    {
                        memberPath = string.Empty;

                        Should("provide default value and return false", () =>
                        {
                            Assert.That(result, Is.Null);
                            Assert.That(returnValue, Is.False);
                        });
                    });

                    And("memberPath with properties exists", () =>
                    {
                        memberPath = "StaticSubItemProperty.SubSubItemProperty.NumberProperty";

                        And("intermediate path is null", () =>
                        {
                            Root.StaticSubItemProperty = null;

                            Should("provide default value and return false", () =>
                            {
                                Assert.That(result, Is.Null);
                                Assert.That(returnValue, Is.False);
                            });
                        });

                        And("intermediate path is not null", () =>
                        {
                            Root.StaticSubItemProperty = new SubItem();
                            Root.StaticSubItemProperty.SubSubItemProperty.NumberProperty = 12345;

                            Should("provide target value", () =>
                            {
                                Assert.That(result, Is.EqualTo(12345));
                                Assert.That(returnValue, Is.True);
                            });
                        });
                    });
                });
            });

            When("instance is not null", () =>
            {
                instance = new Root();

                And("memberPath with properties exists", () =>
                {
                    And("memberPath does not have an indexer", () =>
                    {
                        memberPath = "SubItemProperty.SubSubItemProperty.NumberProperty";

                        And("intermediate path is null", () =>
                        {
                            instance.SubItemProperty = null;

                            Should("provide default value and return false", () =>
                            {
                                Assert.That(result, Is.Null);
                                Assert.That(returnValue, Is.False);
                            });
                        });

                        And("intermediate path is not null", () =>
                        {
                            instance.SubItemProperty = new SubItem();
                            instance.SubItemProperty.SubSubItemProperty.NumberProperty = 11111;

                            Should("provide target value", () =>
                            {
                                Assert.That(result, Is.EqualTo(11111));
                                Assert.That(returnValue, Is.True);
                            });
                        });
                    });

                    And("memberPath has an indexer", () =>
                    {
                        And("the indexer is on the last item in the memberPath", () =>
                        {
                            memberPath = "ItemsProperty[2]";

                            Should("provide target value", () =>
                            {
                                Assert.That(result, Is.EqualTo(333));
                                Assert.That(returnValue, Is.True);
                            });
                        });

                        And("the indexer is in a middle of the memberPath", () =>
                        {
                            And("the collection is a list", () =>
                            {
                                instance.SubItemProperty.SubSubItemsListProperty.AddRange(
                                new[]
                                {
                                    new SubSubItem { NumberField = 11 },
                                    new SubSubItem { NumberField = 22 }
                                });
                                memberPath = "SubItemProperty.SubSubItemsListProperty[1].NumberField";

                                Should("provide target value", () =>
                                {
                                    Assert.That(result, Is.EqualTo(22));
                                    Assert.That(returnValue, Is.True);
                                });

                                And("the index is out of range", () =>
                                {
                                    memberPath = "SubItemProperty.SubSubItemsListProperty[1000].NumberField";

                                    Should("provide default value and return false", () =>
                                    {
                                        Assert.That(result, Is.Null);
                                        Assert.That(returnValue, Is.False);
                                    });
                                });
                            });

                            And("the collection is not a list", () =>
                            {
                                var queue = instance.SubItemProperty.SubSubItemsEnumerableProperty as Queue<SubSubItem>;

                                queue.Enqueue(new SubSubItem { NumberField = 111 });

                                memberPath = "SubItemProperty.SubSubItemsEnumerableProperty[0].NumberField";

                                Should("provide target value", () =>
                                {
                                    Assert.That(result, Is.EqualTo(111));
                                    Assert.That(returnValue, Is.True);
                                });
                            });
                        });
                    });
                });

                And("memberPath with fields exists", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.NumberField";

                    And("intermediate path is null", () =>
                    {
                        instance.SubItemField = null;

                        Should("provide default value and return false", () =>
                        {
                            Assert.That(result, Is.Null);
                            Assert.That(returnValue, Is.False);
                        });
                    });

                    And("intermediate path is not null", () =>
                    {
                        instance.SubItemField = new SubItem();
                        instance.SubItemField.SubSubItemField.NumberField = 22222;

                        Should("provide target value and return true", () =>
                        {
                            Assert.That(result, Is.EqualTo(22222));
                            Assert.That(returnValue, Is.True);
                        });
                    });
                });

                And("intermediate memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER.NumberProperty";

                    Should("provide default value and return false", () =>
                    {
                        Assert.That(result, Is.Null);
                        Assert.That(returnValue, Is.False);
                    });
                });

                And("target memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER";

                    Should("provide default value and return false", () =>
                    {
                        Assert.That(result, Is.Null);
                        Assert.That(returnValue, Is.False);
                    });
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.Set(object, Type, string, object, BindingFlags)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.Set))]
        public static void Set()
        {
            Root instance = null;
            Type type = null;
            string memberPath = null;
            object value = null;
            var returnValue = false;

            Act(() =>
            {
                returnValue = Reflector.Set(instance, type, memberPath, value);
            });

            When("instance is null", () =>
            {
                instance = null;

                And("type is null", () =>
                {
                    type = null;

                    Should("return false", () =>
                    {
                        Assert.That(returnValue, Is.False);
                    });
                });

                And("type is not null", () =>
                {
                    type = typeof(Root);

                    And("memberPath is null", () =>
                    {
                        memberPath = null;

                        Should("return false", () =>
                        {
                            Assert.That(returnValue, Is.False);
                        });
                    });

                    And("memberPath is empty", () =>
                    {
                        memberPath = string.Empty;

                        Should("return false", () =>
                        {
                            Assert.That(returnValue, Is.False);
                        });
                    });

                    And("memberPath with properties exists", () =>
                    {
                        memberPath = "StaticSubItemProperty.SubSubItemProperty.NumberProperty";

                        And("intermediate path is null", () =>
                        {
                            Root.StaticSubItemProperty = null;

                            Should("return false", () =>
                            {
                                Assert.That(returnValue, Is.False);
                            });
                        });

                        And("intermediate path is not null", () =>
                        {
                            Root.StaticSubItemProperty = new SubItem();
                            value = 12345;

                            Should("set the value and return true", () =>
                            {
                                Assert.That(
                                    Root.StaticSubItemProperty.SubSubItemProperty.NumberProperty,
                                    Is.EqualTo(value));
                                Assert.That(returnValue, Is.True);
                            });
                        });
                    });
                });
            });

            When("instance is not null", () =>
            {
                instance = new Root();

                And("memberPath with properties exists", () =>
                {
                    And("memberPath does not have an indexer", () =>
                    {
                        memberPath = "SubItemProperty.SubSubItemProperty.NumberProperty";

                        And("intermediate path is null", () =>
                        {
                            instance.SubItemProperty = null;

                            Should("return false", () =>
                            {
                                Assert.That(returnValue, Is.False);
                            });
                        });

                        And("intermediate path is not null", () =>
                        {
                            instance.SubItemProperty = new SubItem();
                            value = 11111;

                            Should("set the value and return true", () =>
                            {
                                Assert.That(
                                    instance.SubItemProperty.SubSubItemProperty.NumberProperty,
                                    Is.EqualTo(value));
                                Assert.That(returnValue, Is.True);
                            });
                        });
                    });

                    And("memberPath has an indexer", () =>
                    {
                        And("the indexer is on the last item in the memberPath", () =>
                        {
                            memberPath = "ItemsProperty[2]";
                            value = 999;

                            Should("set the value and return true", () =>
                            {
                                Assert.That(instance.ItemsProperty[2], Is.EqualTo(value));
                                Assert.That(returnValue, Is.True);
                            });
                        });

                        And("the indexer is in a middle of the memberPath", () =>
                        {
                            And("the collection is a list", () =>
                            {
                                instance.SubItemProperty.SubSubItemsListProperty.AddRange(
                                    new[]
                                    {
                                        new SubSubItem { NumberField = 11 },
                                        new SubSubItem { NumberField = 22 }
                                    });
                                memberPath = "SubItemProperty.SubSubItemsListProperty[1].NumberField";
                                value = 2222;

                                Should("set the value and return true", () =>
                                {
                                    Assert.That(
                                        instance.SubItemProperty.SubSubItemsListProperty[1].NumberField,
                                        Is.EqualTo(value));
                                    Assert.That(returnValue, Is.True);
                                });

                                And("the index is out of range", () =>
                                {
                                    memberPath = "SubItemProperty.SubSubItemsListProperty[1000].NumberField";

                                    Should("return false", () =>
                                    {
                                        Assert.That(returnValue, Is.False);
                                    });
                                });
                            });

                            And("the collection is not a list", () =>
                            {
                                var queue = instance.SubItemProperty.SubSubItemsEnumerableProperty as Queue<SubSubItem>;

                                queue.Enqueue(new SubSubItem { NumberField = 111 });

                                memberPath = "SubItemProperty.SubSubItemsEnumerableProperty[0].NumberField";

                                value = 222;

                                Should("set the value and return true", () =>
                                {
                                    Assert.That(queue.Peek().NumberField, Is.EqualTo(value));
                                    Assert.That(returnValue, Is.True);
                                });
                            });
                        });
                    });
                });

                And("memberPath with fields exists", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.NumberField";

                    And("intermediate path is null", () =>
                    {
                        instance.SubItemField = null;

                        Should("return false", () =>
                        {
                            Assert.That(returnValue, Is.False);
                        });
                    });

                    And("intermediate path is not null", () =>
                    {
                        instance.SubItemField = new SubItem();
                        value = 22222;

                        Should("set the value and return true", () =>
                        {
                            Assert.That(instance.SubItemField.SubSubItemField.NumberField, Is.EqualTo(value));
                            Assert.That(returnValue, Is.True);
                        });
                    });
                });

                And("intermediate memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER.NumberProperty";

                    Should("return false", () =>
                    {
                        Assert.That(returnValue, Is.False);
                    });
                });

                And("target memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER";

                    Should("return false", () =>
                    {
                        Assert.That(returnValue, Is.False);
                    });
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.Invoke{TReturn}(object, Type, string, Type[], out TReturn, BindingFlags, object[])"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.Invoke))]
        public static void Invoke()
        {
            Root instance = null;
            Type type = null;
            string memberPath = null;
            var parameterTypes = new Type[] { typeof(int), typeof(int) };
            object result = null;
            var arguments = new object[2];
            var returnValue = false;

            Act(() =>
            {
                returnValue = Reflector.Invoke(
                    instance,
                    type,
                    memberPath,
                    parameterTypes,
                    out result,
                    arguments: arguments);
            });

            When("instance is null", () =>
            {
                instance = null;

                And("type is null", () =>
                {
                    type = null;

                    Should("provide default value and return false", () =>
                    {
                        Assert.That(result, Is.Null);
                        Assert.That(returnValue, Is.False);
                    });
                });

                And("type is not null", () =>
                {
                    type = typeof(Root);

                    And("memberPath is null", () =>
                    {
                        memberPath = null;

                        Should("provide default value and return false", () =>
                        {
                            Assert.That(result, Is.Null);
                            Assert.That(returnValue, Is.False);
                        });
                    });

                    And("memberPath is empty", () =>
                    {
                        memberPath = string.Empty;

                        Should("provide default value and return false", () =>
                        {
                            Assert.That(result, Is.Null);
                            Assert.That(returnValue, Is.False);
                        });
                    });

                    And("memberPath with properties exists", () =>
                    {
                        memberPath = "StaticSubItemProperty.SubSubItemProperty.Add";

                        And("intermediate path is null", () =>
                        {
                            Root.StaticSubItemProperty = null;

                            Should("provide default value and return false", () =>
                            {
                                Assert.That(result, Is.Null);
                                Assert.That(returnValue, Is.False);
                            });
                        });

                        And("intermediate path is not null", () =>
                        {
                            Root.StaticSubItemProperty = new SubItem();
                            arguments[0] = 11111;
                            arguments[1] = 22222;

                            Should("provide a result value and return true", () =>
                            {
                                Assert.That(result, Is.EqualTo(33333));
                                Assert.That(returnValue, Is.True);
                            });
                        });
                    });
                });
            });

            When("instance is not null", () =>
            {
                instance = new Root();

                And("memberPath with properties exists", () =>
                {
                    memberPath = "SubItemProperty.SubSubItemProperty.Add";

                    And("intermediate path is null", () =>
                    {
                        instance.SubItemProperty = null;

                        Should("provide default value and return false", () =>
                        {
                            Assert.That(result, Is.Null);
                            Assert.That(returnValue, Is.False);
                        });
                    });

                    And("intermediate path is not null", () =>
                    {
                        instance.SubItemProperty = new SubItem();
                        arguments[0] = 111;
                        arguments[1] = 222;

                        Should("provide a result value and return true", () =>
                        {
                            Assert.That(result, Is.EqualTo(333));
                            Assert.That(returnValue, Is.True);
                        });
                    });
                });

                And("memberPath with fields exists", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.Add";

                    And("intermediate path is null", () =>
                    {
                        instance.SubItemField = null;

                        Should("provide default value and return false", () =>
                        {
                            Assert.That(result, Is.Null);
                            Assert.That(returnValue, Is.False);
                        });
                    });

                    And("intermediate path is not null", () =>
                    {
                        instance.SubItemField = new SubItem();
                        arguments[0] = 123;
                        arguments[1] = 321;

                        Should("provide a result value and return true", () =>
                        {
                            Assert.That(result, Is.EqualTo(444));
                            Assert.That(returnValue, Is.True);
                        });
                    });
                });

                And("intermediate memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER.Add";

                    Should("provide default value and return false", () =>
                    {
                        Assert.That(result, Is.Null);
                        Assert.That(returnValue, Is.False);
                    });
                });

                And("target memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER";

                    Should("provide default value and return false", () =>
                    {
                        Assert.That(result, Is.Null);
                        Assert.That(returnValue, Is.False);
                    });
                });

                And("target memberPath parameter types do not match", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.Add";
                    parameterTypes[0] = typeof(string);
                    parameterTypes[1] = typeof(string);

                    Should("provide default value and return false", () =>
                    {
                        Assert.That(result, Is.Null);
                        Assert.That(returnValue, Is.False);
                    });
                });

                And("target memberPath parameter types are null", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.Add";
                    parameterTypes = null;
                    arguments[0] = 444;
                    arguments[1] = 333;

                    Should("provide result value and return true", () =>
                    {
                        Assert.That(result, Is.EqualTo(777));
                        Assert.That(returnValue, Is.True);
                    });
                });

                And("constructor is called", () =>
                {
                    memberPath = "SubItemProperty.SubSubItemProperty.ctor";
                    instance.SubItemProperty.SubSubItemProperty = null;
                    arguments = null;
                    parameterTypes = null;

                    Should("provide created object and return true", () =>
                    {
                        Assert.That(result, Is.InstanceOf<SubSubItem>());
                        Assert.That(returnValue, Is.True);
                    });
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.CreateGetter{TObject, TReturn}(Expression{Func{TObject, TReturn}})"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.CreateGetter) + "(Expression<Func<TObject, TReturn>>)")]
        public static void CreateGetter1()
        {
            Expression<Func<Root, int>> memberPath = null;
            Func<Root, int> getter = null;

            Act(() =>
            {
                getter = Reflector.CreateGetter(memberPath);
            });

            When("memberPath is null", () =>
            {
                memberPath = null;

                Should("return null", () =>
                {
                    Assert.That(getter, Is.Null);
                });
            });

            When("memberPath is not null", () =>
            {
                memberPath = (Root root) => root.SubItemProperty.SubSubItemProperty.NumberField;

                Should("return a getter", () =>
                {
                    Assert.That(getter, Is.Not.Null);
                });

                And("the getter is executed", () =>
                {
                    var instance = new Root();
                    instance.SubItemProperty.SubSubItemProperty.NumberField = 11111;

                    var returnValue = getter(instance);

                    Should("return the target value", () =>
                    {
                        Assert.That(returnValue, Is.EqualTo(11111));
                    });
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.CreateGetter{TObject, TReturn}(string, Type)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.CreateGetter) + "(string,Type)")]
        public static void CreateGetter2()
        {
            string memberPath = null;
            Func<object, object> getter = null;

            Act(() =>
            {
                getter = Reflector.CreateGetter<object, object>(memberPath, typeof(Root));
            });

            When("memberPath is null", () =>
            {
                memberPath = null;

                Should("return null", () =>
                {
                    Assert.That(getter, Is.Null);
                });
            });

            When("memberPath is empty", () =>
            {
                memberPath = string.Empty;

                Should("return null", () =>
                {
                    Assert.That(getter, Is.Null);
                });
            });

            When("memberPath is not null and is not empty", () =>
            {
                memberPath = "SubItemProperty.SubSubItemProperty.NumberField";

                Should("return a getter", () =>
                {
                    Assert.That(getter, Is.Not.Null);
                });

                And("the getter is executed", () =>
                {
                    var instance = new Root();
                    instance.SubItemProperty.SubSubItemProperty.NumberField = 11111;

                    var returnValue = getter(instance);

                    Should("return the target value", () =>
                    {
                        Assert.That(returnValue, Is.EqualTo(11111));
                    });
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.CreateSetter{TObject, TReturn}(Expression{Func{TObject, TReturn}})"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.CreateSetter) + "(Expression<Func<TObject, TReturn>> memberPath)")]
        public static void CreateSetter1()
        {
            Expression<Func<Root, int>> memberPath = null;
            Action<Root, int> setter = null;

            Act(() =>
            {
                setter = Reflector.CreateSetter(memberPath);
            });

            When("memberPath is null", () =>
            {
                memberPath = null;

                Should("return null", () =>
                {
                    Assert.That(setter, Is.Null);
                });
            });

            When("memberPath is not null", () =>
            {
                memberPath = (Root root) => root.SubItemProperty.SubSubItemProperty.NumberField;

                Should("return a setter", () =>
                {
                    Assert.That(setter, Is.Not.Null);
                });

                And("the setter is executed", () =>
                {
                    var instance = new Root();

                    setter(instance, 11111);

                    Should("set the provided value", () =>
                    {
                        Assert.That(instance.SubItemProperty.SubSubItemProperty.NumberField, Is.EqualTo(11111));
                    });
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.CreateSetter{TObject, TReturn}(string, Type)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.CreateSetter) + "(string, Type)")]
        public static void CreateSetter2()
        {
            string memberPath = null;
            Action<object, object> setter = null;

            Act(() =>
            {
                setter = Reflector.CreateSetter<object, object>(memberPath, typeof(Root));
            });

            When("memberPath is null", () =>
            {
                memberPath = null;

                Should("return null", () =>
                {
                    Assert.That(setter, Is.Null);
                });
            });

            When("memberPath is empty", () =>
            {
                memberPath = string.Empty;

                Should("return null", () =>
                {
                    Assert.That(setter, Is.Null);
                });
            });

            When("memberPath is not null", () =>
            {
                memberPath = "SubItemProperty.SubSubItemProperty.NumberField";

                Should("return a setter", () =>
                {
                    Assert.That(setter, Is.Not.Null);
                });

                And("the setter is executed", () =>
                {
                    var instance = new Root();

                    setter(instance, 11111);

                    Should("set the provided value", () =>
                    {
                        Assert.That(instance.SubItemProperty.SubSubItemProperty.NumberField, Is.EqualTo(11111));
                    });
                });
            });
        }
    }
}
