// <copyright file="Reflector.Set.Tests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Heleonix.Reflection.Tests.Common.Dummies;
    using Heleonix.Testing.NUnit.Aaa;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the <see cref="Reflector"/>.
    /// </summary>
    public static partial class ReflectorTests
    {
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
                            And("the collection is a dictionary", () =>
                            {
                                instance.SubItemProperty.SubSubItemsDictionaryProperty.Add(
                                    "First Key",
                                    new SubSubItem { TextProperty = "First Value" });
                                memberPath = "SubItemProperty.SubSubItemsDictionaryProperty[First Key].TextProperty";
                                value = "New First Value";

                                Should("set the value and return true", () =>
                                {
                                    Assert.That(
                                        instance.SubItemProperty.SubSubItemsDictionaryProperty["First Key"].TextProperty,
                                        Is.EqualTo(value));
                                    Assert.That(returnValue, Is.True);
                                });

                                And("the key does not exist", () =>
                                {
                                    memberPath = "SubItemProperty.SubSubItemsDictionaryProperty[NO KEY].TextProperty";

                                    Should("return false", () =>
                                    {
                                        Assert.That(returnValue, Is.False);
                                    });
                                });
                            });

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
        /// Tests the <see cref="Reflector.SetCoerced(object, Type, string, object, BindingFlags)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.SetCoerced))]
        public static void SetCoerced()
        {
            Root instance = null;
            Type type = null;
            string memberPath = null;
            object value = null;
            var returnValue = false;
            Exception exception = null;

            Arrange(() =>
            {
                exception = null;
                returnValue = false;
            });

            Act(() =>
            {
                try
                {
                    returnValue = Reflector.SetCoerced(instance, type, memberPath, value);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            When("instance is not null", () =>
            {
                instance = new Root();

                And("property is an enum and value is enum", () =>
                {
                    memberPath = "SubItemProperty.SubSubItemProperty.EnumProperty";
                    value = EnumItem.Value1;

                    Should("set the enum value and return true", () =>
                    {
                        Assert.That(
                            instance.SubItemProperty.SubSubItemProperty.EnumProperty,
                            Is.EqualTo(EnumItem.Value1));
                        Assert.That(returnValue, Is.True);
                    });
                });

                And("property is an enum and value is string", () =>
                {
                    memberPath = "SubItemProperty.SubSubItemProperty.EnumProperty";
                    value = "Value2";

                    Should("set the enum value and return true", () =>
                    {
                        Assert.That(
                            instance.SubItemProperty.SubSubItemProperty.EnumProperty,
                            Is.EqualTo(EnumItem.Value2));
                        Assert.That(returnValue, Is.True);
                    });
                });

                And("field is an enum and value is string", () =>
                {
                    memberPath = "SubItemProperty.SubSubItemProperty.EnumField";
                    value = "Value3";

                    Should("set the enum value and return true", () =>
                    {
                        Assert.That(
                            instance.SubItemProperty.SubSubItemProperty.EnumField,
                            Is.EqualTo(EnumItem.Value3));
                        Assert.That(returnValue, Is.True);
                    });
                });

                And("property is an enum and value is unconvertible datetime", () =>
                {
                    memberPath = "SubItemProperty.SubSubItemProperty.EnumProperty";
                    value = DateTime.Now;

                    Should("throw an exception and return false", () =>
                    {
                        Assert.That(exception, Is.InstanceOf<ArgumentException>());
                        Assert.That(returnValue, Is.False);
                    });
                });

                And("property is an enum and value is null", () =>
                {
                    memberPath = "SubItemProperty.SubSubItemProperty.EnumProperty";
                    value = null;

                    Should("set null and return true", () =>
                    {
                        Assert.That(instance.SubItemProperty.SubSubItemProperty.EnumProperty, Is.Null);
                        Assert.That(returnValue, Is.True);
                    });
                });
            });
        }
    }
}
