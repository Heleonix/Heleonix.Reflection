// <copyright file="Reflector.Get.Tests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests
{
    using System;
    using System.Collections.Generic;
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
                            And("the collection is a string-keyed dictionary", () =>
                            {
                                instance.SubItemProperty.SubSubItemsStringDictionaryProperty.Add(
                                    "First Key",
                                    new SubSubItem { TextProperty = "First Value" });
                                memberPath = "SubItemProperty.SubSubItemsStringDictionaryProperty[First Key].TextProperty";

                                Should("provide the target value", () =>
                                {
                                    Assert.That(result, Is.EqualTo("First Value"));
                                    Assert.That(returnValue, Is.True);
                                });

                                And("the key does not exist", () =>
                                {
                                    memberPath = "SubItemProperty.SubSubItemsStringDictionaryProperty[NO KEY].TextProperty";

                                    Should("provide the default value and return false", () =>
                                    {
                                        Assert.That(result, Is.Null);
                                        Assert.That(returnValue, Is.False);
                                    });
                                });
                            });

                            And("the collection is an int-keyed dictionary", () =>
                            {
                                instance.SubItemProperty.SubSubItemsIntDictionaryProperty.Add(
                                    12345,
                                    new SubSubItem { TextProperty = "First Value" });
                                memberPath = "SubItemProperty.SubSubItemsIntDictionaryProperty[12345].TextProperty";

                                Should("provide the target value", () =>
                                {
                                    Assert.That(result, Is.EqualTo("First Value"));
                                    Assert.That(returnValue, Is.True);
                                });

                                And("the key does not exist", () =>
                                {
                                    memberPath = "SubItemProperty.SubSubItemsStringDictionaryProperty[111].TextProperty";

                                    Should("provide the default value and return false", () =>
                                    {
                                        Assert.That(result, Is.Null);
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
    }
}
