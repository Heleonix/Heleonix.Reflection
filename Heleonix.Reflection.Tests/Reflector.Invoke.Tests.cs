// <copyright file="Reflector.Invoke.Tests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests
{
    using System;
    using System.Reflection;
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
    }
}
