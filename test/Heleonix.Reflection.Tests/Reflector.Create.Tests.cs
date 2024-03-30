// <copyright file="Reflector.Create.Tests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests
{
    using System;
    using System.Linq.Expressions;
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
    }
}
