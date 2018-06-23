// <copyright file="ReflectorTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2017-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests
{
    using System;
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
        /// Tests the <see cref="Reflector.GetInfo(object, Type, string, Type[], bool, BindingFlags)"/>.
        /// </summary>
        [MemberTest(Name = nameof(Reflector.GetInfo))]
        public static void GetInfo()
        {
            Root instance = null;
            Type type = null;
            string memberPath = null;
            Type[] paramTypes = null;
            var requireIntermediateValues = false;
            MembersInfo result = null;

            When("the method is executed", () =>
            {
                Act(() =>
                {
                    result = Reflector.GetInfo(instance, type, memberPath, paramTypes, requireIntermediateValues);
                });

                And("instance is null", () =>
                {
                    instance = null;

                    And("type is null", () =>
                    {
                        type = null;

                        Should("return null", () =>
                        {
                            Assert.That(result, Is.Null);
                        });
                    });

                    And("type is not null", () =>
                    {
                        type = typeof(Root);

                        And("memberPath is null", () =>
                        {
                            memberPath = null;

                            Should("return null", () =>
                            {
                                Assert.That(result, Is.Null);
                            });
                        });

                        And("memberPath is empty", () =>
                        {
                            memberPath = string.Empty;

                            Should("return null", () =>
                            {
                                Assert.That(result, Is.Null);
                            });
                        });

                        And("memberPath exists", () =>
                        {
                            memberPath = "StaticSubItemProperty.SubSubItemProperty.NumberProperty";

                            Should("return info for the member by the specified member path", () =>
                            {
                                Assert.That(result.Members[0].Name, Is.EqualTo("NumberProperty"));
                            });
                        });
                    });
                });

                And("instance is not null", () =>
                {
                    instance = new Root();

                    And("intermediate values are not null", () =>
                    {
                        And("memberPath with properties exists", () =>
                        {
                            memberPath = "SubItemProperty.SubSubItemProperty.NumberProperty";

                            Should("return info for the member by the specified member path", () =>
                            {
                                Assert.That(result.Members[0].Name, Is.EqualTo("NumberProperty"));
                            });
                        });

                        And("memberPath with fields exists", () =>
                        {
                            memberPath = "SubItemField.SubSubItemField.NumberField";

                            Should("return info for the member by the specified member path", () =>
                            {
                                Assert.That(result.Members[0].Name, Is.EqualTo("NumberField"));
                            });
                        });

                        And("memberPath does not exist", () =>
                        {
                            memberPath = "SubItemProperty.NO_MEMBER.NumberProperty";

                            Should("return null", () =>
                            {
                                Assert.That(result, Is.Null);
                            });
                        });
                    });

                    And("intermediate values are null", () =>
                    {
                        instance.SubItemProperty.SubSubItemProperty = null;
                        memberPath = "SubItemProperty.SubSubItemProperty.NumberProperty";

                        And("intermediate values are required", () =>
                        {
                            requireIntermediateValues = true;

                            Should("return null", () =>
                            {
                                Assert.That(result, Is.Null);
                            });
                        });
                    });
                });
            });
        }
    }
}
