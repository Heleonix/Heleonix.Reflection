// <copyright file="ReflectorTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests
{
    using System.Reflection;
    using Heleonix.Reflection.Tests.Common.Dummies;
    using Heleonix.Testing.NUnit.Aaa;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the <see cref="Reflector"/>.
    /// </summary>
    [ComponentTest(Type = typeof(Reflector))]
    public static partial class ReflectorTests
    {
        /// <summary>
        /// Tests the <see cref="Reflector.IsStatic(System.Reflection.PropertyInfo)"/>.
        /// </summary>
        [MemberTest(Name =nameof(Reflector.IsStatic))]
        public static void IsStatic()
        {
            var result = false;
            PropertyInfo propertyInfo = null;

            Act(() =>
            {
                result = Reflector.IsStatic(propertyInfo);
            });

            When("the pased parameter is null", () =>
            {
                propertyInfo = null;

                Should("return false", () =>
                {
                    Assert.That(result, Is.False);
                });
            });

            When("the pased parameter is non-static property", () =>
            {
                propertyInfo = typeof(SubSubItem).GetProperty("TextProperty");

                Should("return false", () =>
                {
                    Assert.That(result, Is.False);
                });
            });

            When("the pased parameter is static get property", () =>
            {
                propertyInfo = typeof(SubSubItem).GetProperty("StaticTextGetProperty");

                Should("return true", () =>
                {
                    Assert.That(result, Is.True);
                });
            });

            When("the pased parameter is static set property", () =>
            {
                propertyInfo = typeof(SubSubItem).GetProperty("StaticTextSetProperty");

                Should("return true", () =>
                {
                    Assert.That(result, Is.True);
                });
            });

            When("the pased parameter is non-static set property", () =>
            {
                propertyInfo = typeof(SubSubItem).GetProperty("TextSetProperty");

                Should("return false", () =>
                {
                    Assert.That(result, Is.False);
                });
            });

            When("the pased parameter is non-static get property", () =>
            {
                propertyInfo = typeof(SubSubItem).GetProperty("TextGetProperty");

                Should("return false", () =>
                {
                    Assert.That(result, Is.False);
                });
            });
        }
    }
}
