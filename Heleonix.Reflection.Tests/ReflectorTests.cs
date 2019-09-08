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
        /// Tests the <see cref="Reflector.Get{TReturn}(object, Type, string, out TReturn, BindingFlags)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.Get))]
        public static void Get()
        {
            Root instance = null;
            Type type = null;
            string memberPath = null;
            string result = null;
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
                        memberPath = "StaticSubItemProperty.SubSubItemProperty.TextProperty";

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
                            Root.StaticSubItemProperty.SubSubItemProperty.TextProperty = "12345";

                            Should("provide target value and return true", () =>
                            {
                                Assert.That(result, Is.EqualTo("12345"));
                                Assert.That(returnValue, Is.True);
                            });
                        });

                        And("target is null", () =>
                        {
                            Root.StaticSubItemProperty.SubSubItemProperty.TextProperty = null;

                            Should("provide default value and return true", () =>
                            {
                                Assert.That(result, Is.Null);
                                Assert.That(returnValue, Is.True);
                            });
                        });

                        And("target is of wrong type", () =>
                        {
                            memberPath = "StaticSubItemProperty.SubSubItemProperty.ObjectProperty";

                            Root.StaticSubItemProperty.SubSubItemProperty.ObjectProperty = 12345;

                            Should("provide default value and return false", () =>
                            {
                                Assert.That(result, Is.Null);
                                Assert.That(returnValue, Is.False);
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
                        memberPath = "SubItemProperty.SubSubItemProperty.TextProperty";

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
                            instance.SubItemProperty.SubSubItemProperty.TextProperty = "11111";

                            Should("provide target value", () =>
                            {
                                Assert.That(result, Is.EqualTo("11111"));
                                Assert.That(returnValue, Is.True);
                            });
                        });
                    });

                    And("memberPath has an indexer", () =>
                    {
                        And("the indexer is on the last item in the memberPath", () =>
                        {
                            memberPath = "ItemsProperty[2]";

                            And("value by the index is not null", () =>
                            {
                                Should("provide target value", () =>
                                {
                                    Assert.That(result, Is.EqualTo("333"));
                                    Assert.That(returnValue, Is.True);
                                });
                            });

                            And("value by the index is null", () =>
                            {
                                instance.ItemsProperty[2] = null;

                                Should("provide null", () =>
                                {
                                    Assert.That(result, Is.Null);
                                    Assert.That(returnValue, Is.True);
                                });
                            });
                        });

                        And("the indexer is in a middle of the memberPath", () =>
                        {
                            And("the collection is a list", () =>
                            {
                                instance.SubItemProperty.SubSubItemsListProperty.AddRange(
                                new[]
                                {
                                    new SubSubItem { TextProperty = "11" },
                                    new SubSubItem { TextProperty = "22" },
                                });
                                memberPath = "SubItemProperty.SubSubItemsListProperty[1].TextProperty";

                                Should("provide target value", () =>
                                {
                                    Assert.That(result, Is.EqualTo("22"));
                                    Assert.That(returnValue, Is.True);
                                });

                                And("the index is out of range", () =>
                                {
                                    memberPath = "SubItemProperty.SubSubItemsListProperty[1000].TextProperty";

                                    Should("provide default value and return false", () =>
                                    {
                                        Assert.That(result, Is.Null);
                                        Assert.That(returnValue, Is.False);
                                    });
                                });
                            });

                            And("the collection is not a list", () =>
                            {
                                Arrange(() =>
                                {
                                    memberPath = "SubItemProperty.SubSubItemsEnumerableProperty[1].TextProperty";
                                });

                                And("the collection is not null", () =>
                                {
                                    var queue = instance.SubItemProperty.SubSubItemsEnumerableProperty as Queue<SubSubItem>;

                                    queue.Enqueue(new SubSubItem { TextProperty = "111" });
                                    queue.Enqueue(new SubSubItem { TextProperty = "222" });

                                    Should("provide target value", () =>
                                    {
                                        Assert.That(result, Is.EqualTo("222"));
                                        Assert.That(returnValue, Is.True);
                                    });

                                    And("index is out of range", () =>
                                    {
                                        Arrange(() =>
                                        {
                                            memberPath = "SubItemProperty.SubSubItemsEnumerableProperty[9].TextProperty";
                                        });

                                        Should("provide default value and return false", () =>
                                        {
                                            Assert.That(result, Is.Null);
                                            Assert.That(returnValue, Is.False);
                                        });
                                    });
                                });

                                And("the collection is null", () =>
                                {
                                    instance.SubItemProperty.SubSubItemsEnumerableProperty = null;

                                    Should("provide default value and return false", () =>
                                    {
                                        Assert.That(result, Is.Null);
                                        Assert.That(returnValue, Is.False);
                                    });
                                });
                            });
                        });
                    });
                });

                And("memberPath with fields exists", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.TextField";

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
                        instance.SubItemField.SubSubItemField.TextField = "22222";

                        Should("provide target value and return true", () =>
                        {
                            Assert.That(result, Is.EqualTo(instance.SubItemField.SubSubItemField.TextField));
                            Assert.That(returnValue, Is.True);
                        });
                    });
                });

                And("intermediate memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER.TextProperty";

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
                        memberPath = "StaticSubItemProperty.SubSubItemProperty.TextProperty";

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
                            value = "12345";

                            Should("set the value and return true", () =>
                            {
                                Assert.That(
                                    Root.StaticSubItemProperty.SubSubItemProperty.TextProperty,
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
                        memberPath = "SubItemProperty.SubSubItemProperty.TextProperty";

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
                            value = "11111";

                            Should("set the value and return true", () =>
                            {
                                Assert.That(
                                    instance.SubItemProperty.SubSubItemProperty.TextProperty,
                                    Is.EqualTo(value));
                                Assert.That(returnValue, Is.True);
                            });

                            And("property does not have setter", () =>
                            {
                                memberPath = "SubItemProperty.SubSubItemProperty.StaticTextGetProperty";

                                Should("return false", () =>
                                {
                                    Assert.That(returnValue, Is.False);
                                });
                            });

                            And("container is null for static property", () =>
                            {
                                instance.SubItemProperty.SubSubItemProperty = null;
                                memberPath = "SubItemProperty.SubSubItemProperty.StaticTextProperty";
                                value = "111";

                                Should("set the value and return true", () =>
                                {
                                    Assert.That(SubSubItem.StaticTextProperty, Is.EqualTo(value));
                                    Assert.That(returnValue, Is.True);
                                });
                            });

                            And("container is null for instance property", () =>
                            {
                                instance.SubItemProperty.SubSubItemProperty = null;
                                memberPath = "SubItemProperty.SubSubItemProperty.TextProperty";

                                Should("return false", () =>
                                {
                                    Assert.That(returnValue, Is.False);
                                });
                            });
                        });
                    });

                    And("memberPath has an indexer", () =>
                    {
                        And("the indexer is on the last item in the memberPath", () =>
                        {
                            And("the target is not of type IList", () =>
                            {
                                memberPath = "ItemsProperty[2]";
                                value = "999";

                                Should("set the value and return true", () =>
                                {
                                    Assert.That(instance.ItemsProperty[2], Is.EqualTo(value));
                                    Assert.That(returnValue, Is.True);
                                });
                            });

                            And("the target is of type IList", () =>
                            {
                                memberPath = "SubItemProperty.SubSubItemProperty[0]";
                                instance.SubItemProperty.SubSubItemProperty = new SubSubItem();
                                value = "999";

                                Should("set the value and return true", () =>
                                {
                                    Assert.That(instance.SubItemProperty.SubSubItemProperty[0], Is.EqualTo(value));
                                    Assert.That(returnValue, Is.True);
                                });
                            });
                        });

                        And("the indexer is in a middle of the memberPath", () =>
                        {
                            And("the collection is a list", () =>
                            {
                                instance.SubItemProperty.SubSubItemsListProperty.AddRange(
                                    new[]
                                    {
                                        new SubSubItem { TextProperty = "11" },
                                        new SubSubItem { TextProperty = "22" },
                                    });
                                memberPath = "SubItemProperty.SubSubItemsListProperty[1].TextProperty";
                                value = "2222";

                                Should("set the value and return true", () =>
                                {
                                    Assert.That(
                                        instance.SubItemProperty.SubSubItemsListProperty[1].TextProperty,
                                        Is.EqualTo(value));
                                    Assert.That(returnValue, Is.True);
                                });

                                And("the index is out of range", () =>
                                {
                                    memberPath = "SubItemProperty.SubSubItemsListProperty[1000].TextProperty";

                                    Should("return false", () =>
                                    {
                                        Assert.That(returnValue, Is.False);
                                    });
                                });
                            });

                            And("the collection is not a list", () =>
                            {
                                var queue = instance.SubItemProperty.SubSubItemsEnumerableProperty as Queue<SubSubItem>;

                                queue.Enqueue(new SubSubItem { TextProperty = "111" });

                                memberPath = "SubItemProperty.SubSubItemsEnumerableProperty[0].TextProperty";

                                value = "222";

                                Should("set the value and return true", () =>
                                {
                                    Assert.That(queue.Peek().TextProperty, Is.EqualTo(value));
                                    Assert.That(returnValue, Is.True);
                                });
                            });
                        });
                    });
                });

                And("memberPath with fields exists", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.TextField";

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
                        value = "22222";

                        Should("set the value and return true", () =>
                        {
                            Assert.That(instance.SubItemField.SubSubItemField.TextField, Is.EqualTo(value));
                            Assert.That(returnValue, Is.True);
                        });

                        And("container is null for static field", () =>
                        {
                            instance.SubItemProperty.SubSubItemProperty = null;
                            memberPath = "SubItemProperty.SubSubItemProperty.StaticTextField";
                            value = "111";

                            Should("set the value and return true", () =>
                            {
                                Assert.That(SubSubItem.StaticTextField, Is.EqualTo(value));
                                Assert.That(returnValue, Is.True);
                            });
                        });
                    });
                });

                And("intermediate memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER.TextField";

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
            var parameterTypes = new Type[] { typeof(string), typeof(string) };
            string result = null;
            var arguments = new object[2];
            var returnValue = false;

            Act(() =>
            {
                returnValue = Reflector.Invoke<string>(
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
                        memberPath = "StaticSubItemProperty.SubSubItemProperty.Concat";

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

                            And("target member is method", () =>
                            {
                                arguments[0] = "11111";
                                arguments[1] = "22222";

                                Should("provide a result value and return true", () =>
                                {
                                    Assert.That(result, Is.EqualTo("1111122222"));
                                    Assert.That(returnValue, Is.True);
                                });
                            });

                            And("target member is not method", () =>
                            {
                                memberPath = "StaticSubItemProperty.SubSubItemProperty.TextProperty";

                                Should("provide default value and return false", () =>
                                {
                                    Assert.That(result, Is.Null);
                                    Assert.That(returnValue, Is.False);
                                });
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
                    memberPath = "SubItemProperty.SubSubItemProperty.Concat";

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
                        arguments[0] = "111";
                        arguments[1] = "222";

                        Should("provide a result value and return true", () =>
                        {
                            Assert.That(result, Is.EqualTo("111222"));
                            Assert.That(returnValue, Is.True);
                        });
                    });

                    And("target is null and is not static", () =>
                    {
                        instance.SubItemProperty.SubSubItemProperty = null;

                        Should("provide default value and return true", () =>
                        {
                            Assert.That(result, Is.Null);
                            Assert.That(returnValue, Is.True);
                        });
                    });
                });

                And("memberPath with fields exists", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.Concat";

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
                        arguments[0] = "123";
                        arguments[1] = "321";

                        Should("provide a result value and return true", () =>
                        {
                            Assert.That(result, Is.EqualTo("123321"));
                            Assert.That(returnValue, Is.True);
                        });
                    });
                });

                And("intermediate memberPath does not exist", () =>
                {
                    memberPath = "SubItemProperty.NO_MEMBER.Concat";

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
                    memberPath = "SubItemField.SubSubItemField.Concat";
                    parameterTypes[0] = typeof(int);
                    parameterTypes[1] = typeof(int);

                    Should("provide default value and return false", () =>
                    {
                        Assert.That(result, Is.Null);
                        Assert.That(returnValue, Is.False);
                    });
                });

                And("target memberPath parameter count does not match", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.Concat";
                    parameterTypes = new Type[] { typeof(string) };

                    Should("provide default value and return false", () =>
                    {
                        Assert.That(result, Is.Null);
                        Assert.That(returnValue, Is.False);
                    });
                });

                And("target memberPath parameter types are null", () =>
                {
                    memberPath = "SubItemField.SubSubItemField.Concat";
                    parameterTypes = null;
                    arguments[0] = "444";
                    arguments[1] = "333";

                    Should("provide result value and return true", () =>
                    {
                        Assert.That(result, Is.EqualTo("444333"));
                        Assert.That(returnValue, Is.True);
                    });
                });

                And("constructor is called", () =>
                {
                    And("result value is not of TReturn", () =>
                    {
                        memberPath = "SubItemProperty.SubSubItemProperty.ctor";
                        instance.SubItemProperty.SubSubItemProperty = null;
                        arguments = null;
                        parameterTypes = null;

                        Should("provide default value and return false", () =>
                        {
                            Assert.That(result, Is.Null);
                            Assert.That(returnValue, Is.False);
                        });
                    });

                    And("result value is of TReturn", () =>
                    {
                        memberPath = "SubItemProperty.SubSubItemProperty.TextProperty.ctor";
                        instance.SubItemProperty.SubSubItemProperty = new SubSubItem { TextProperty = null };
                        arguments = new object[] { 'a', 5 };
                        parameterTypes = new Type[] { typeof(char), typeof(int) };

                        Should("provide created object and return true", () =>
                        {
                            Assert.That(result, Is.InstanceOf<string>());
                            Assert.That(returnValue, Is.True);
                        });
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
            Expression<Func<Root, string>> memberPath = null;
            Func<Root, string> getter = null;

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
                memberPath = (Root root) => root.SubItemProperty.SubSubItemProperty.TextProperty;

                Should("return a getter", () =>
                {
                    Assert.That(getter, Is.Not.Null);
                });

                And("the getter is executed", () =>
                {
                    var instance = new Root();
                    instance.SubItemProperty.SubSubItemProperty.TextProperty = "11111";

                    var returnValue = getter(instance);

                    Should("return the target value", () =>
                    {
                        Assert.That(returnValue, Is.EqualTo(instance.SubItemProperty.SubSubItemProperty.TextProperty));
                    });
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.CreateGetter{TObject, TReturn}(string, Type)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.CreateGetter) + "(string, Type)")]
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
                memberPath = "SubItemProperty.SubSubItemProperty.TextProperty";

                And("container type is not null", () =>
                {
                    Should("return a getter", () =>
                    {
                        Assert.That(getter, Is.Not.Null);
                    });

                    And("the getter is executed", () =>
                    {
                        var instance = new Root();
                        instance.SubItemProperty.SubSubItemProperty.TextProperty = "11111";

                        var returnValue = getter(instance);

                        Should("return the target value", () =>
                        {
                            Assert.That(returnValue, Is.EqualTo(instance.SubItemProperty.SubSubItemProperty.TextProperty));
                        });
                    });
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.CreateGetter{TObject, TReturn}(string, Type)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.CreateGetter) + "(string, Type)")]
        public static void CreateGetter3()
        {
            string memberPath = null;
            Func<Root, object> getter = null;

            Act(() =>
            {
                getter = Reflector.CreateGetter<Root, object>(memberPath, null);
            });

            When("memberPath is not null and is not empty", () =>
            {
                memberPath = "SubItemProperty.SubSubItemProperty.TextProperty";

                And("container type is null", () =>
                {
                    Should("return a getter", () =>
                    {
                        Assert.That(getter, Is.Not.Null);
                    });

                    And("the getter is executed", () =>
                    {
                        var instance = new Root();
                        instance.SubItemProperty.SubSubItemProperty.TextProperty = "11111";

                        var returnValue = getter(instance);

                        Should("return the target value", () =>
                        {
                            Assert.That(returnValue, Is.EqualTo(instance.SubItemProperty.SubSubItemProperty.TextProperty));
                        });
                    });
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.CreateGetter{TObject, TReturn}(string, Type)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.CreateGetter) + "(string, Type)")]
        public static void CreateGetter4()
        {
            string memberPath = null;
            Func<Root, string> getter = null;

            Act(() =>
            {
                getter = Reflector.CreateGetter<Root, string>(memberPath, typeof(Root));
            });

            When("container type is TObject and the target type is TReturn, so no type conversions are needed", () =>
            {
                memberPath = "SubItemProperty.SubSubItemProperty.TextProperty";

                Should("return a getter", () =>
                {
                    Assert.That(getter, Is.Not.Null);
                });

                And("the getter is executed", () =>
                {
                    var instance = new Root();
                    instance.SubItemProperty.SubSubItemProperty.TextProperty = "11111";

                    var returnValue = getter(instance);

                    Should("return the target value", () =>
                    {
                        Assert.That(returnValue, Is.EqualTo(instance.SubItemProperty.SubSubItemProperty.TextProperty));
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
            Expression<Func<Root, string>> memberPath = null;
            Action<Root, string> setter = null;

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
                memberPath = (Root root) => root.SubItemProperty.SubSubItemProperty.TextProperty;

                Should("return a setter", () =>
                {
                    Assert.That(setter, Is.Not.Null);
                });

                And("the setter is executed", () =>
                {
                    var instance = new Root();

                    setter(instance, "11111");

                    Should("set the provided value", () =>
                    {
                        Assert.That(instance.SubItemProperty.SubSubItemProperty.TextProperty, Is.EqualTo("11111"));
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

            When("memberPath is not null and is not empty", () =>
            {
                memberPath = "SubItemProperty.SubSubItemProperty.TextProperty";

                And("container type is not null", () =>
                {
                    Should("return a setter", () =>
                    {
                        Assert.That(setter, Is.Not.Null);
                    });

                    And("the setter is executed", () =>
                    {
                        var instance = new Root();

                        setter(instance, "11111");

                        Should("set the provided value", () =>
                        {
                            Assert.That(instance.SubItemProperty.SubSubItemProperty.TextProperty, Is.EqualTo("11111"));
                        });
                    });
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.CreateSetter{TObject, TReturn}(string, Type)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.CreateSetter) + "(string, Type)")]
        public static void CreateSetter3()
        {
            string memberPath = null;
            Action<Root, object> setter = null;

            Act(() =>
            {
                setter = Reflector.CreateSetter<Root, object>(memberPath, null);
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

            When("memberPath is not null and is not empty", () =>
            {
                memberPath = "SubItemProperty.SubSubItemProperty.TextProperty";

                And("container type is null", () =>
                {
                    Should("return a setter", () =>
                    {
                        Assert.That(setter, Is.Not.Null);
                    });

                    And("the setter is executed", () =>
                    {
                        var instance = new Root();

                        setter(instance, "11111");

                        Should("set the provided value", () =>
                        {
                            Assert.That(instance.SubItemProperty.SubSubItemProperty.TextProperty, Is.EqualTo("11111"));
                        });
                    });
                });
            });
        }

        /// <summary>
        /// Tests the <see cref="Reflector.CreateSetter{TObject, TReturn}(string, Type)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.CreateSetter) + "(string, Type)")]
        public static void CreateSetter4()
        {
            string memberPath = null;
            Action<Root, string> setter = null;

            Act(() =>
            {
                setter = Reflector.CreateSetter<Root, string>(memberPath, typeof(Root));
            });

            When("container type is TObject and the target type is TReturn, so no type conversions are needed", () =>
            {
                memberPath = "SubItemProperty.SubSubItemProperty.TextProperty";

                Should("return a setter", () =>
                {
                    Assert.That(setter, Is.Not.Null);
                });

                And("the setter is executed", () =>
                {
                    var instance = new Root();

                    setter(instance, "11111");

                    Should("set the provided value", () =>
                    {
                        Assert.That(instance.SubItemProperty.SubSubItemProperty.TextProperty, Is.EqualTo("11111"));
                    });
                });
            });
        }

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
    }
}
