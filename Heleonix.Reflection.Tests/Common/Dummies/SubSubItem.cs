// <copyright file="SubSubItem.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2017-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests.Common.Dummies
{
    /// <summary>
    /// The sub sub item type.
    /// </summary>
    public class SubSubItem
    {
#pragma warning disable S1104 // Fields should not have public accessibility
#pragma warning disable SA1401 // Fields must be private
#pragma warning disable CA2211 // Non-constant fields should not be visible
#pragma warning disable S2223 // Non-constant static fields should not be visible
        /// <summary>
        /// The static integer field.
        /// </summary>
        public static int StaticNumberField = 1;
#pragma warning restore S2223 // Non-constant static fields should not be visible
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning restore SA1401 // Fields must be private
#pragma warning restore S1104 // Fields should not have public accessibility

#pragma warning disable S1104 // Fields should not have public accessibility
#pragma warning disable SA1401 // Fields must be private
#pragma warning disable CA1051 // Do not declare visible instance fields
        /// <summary>
        /// The object field.
        /// </summary>
        public object ObjectField = "some string";
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore SA1401 // Fields must be private
#pragma warning restore S1104 // Fields should not have public accessibility

#pragma warning disable S1104 // Fields should not have public accessibility
#pragma warning disable SA1401 // Fields must be private
#pragma warning disable CA1051 // Do not declare visible instance fields
        /// <summary>
        /// The number field.
        /// </summary>
        public int NumberField = 2;
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore SA1401 // Fields must be private
#pragma warning restore S1104 // Fields should not have public accessibility

        /// <summary>
        /// The static int set property.
        /// </summary>
        private static int staticIntSetProperty = 3;

        /// <summary>
        /// Gets 222.
        /// </summary>
        public static int StaticNumberGetProperty { get; } = 4;

        /// <summary>
        /// Gets or sets the static number.
        /// </summary>
        public static int StaticNumberProperty { get; set; } = 5;

#pragma warning disable S2376 // Write-only properties should not be used
#pragma warning disable CA1044 // Properties should not be write only
        /// <summary>
        /// Sets the static number.
        /// </summary>
        public static int StaticNumberSetProperty
#pragma warning restore CA1044 // Properties should not be write only
#pragma warning restore S2376 // Write-only properties should not be used
        {
            set { staticIntSetProperty = value; }
        }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public int NumberProperty { get; set; } = 6;

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        public object ObjectProperty { get; set; } = 7;

        /// <summary>
        /// Statics the function.
        /// </summary>
        /// <returns>111.</returns>
        public static int StaticFunction() => 8;

        /// <summary>
        /// Statics the action.
        /// </summary>
        public static void StaticAction()
        {
            // For testing pupropes it is empty.
        }

        /// <summary>
        /// Methods this instance.
        /// </summary>
        /// <returns>111.</returns>
        public virtual int Method() => 9;

        /// <summary>
        /// Adds the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><paramref name="left"/> + <paramref name="right"/>.</returns>
        public virtual int Add(int left, int right) => left + right;

        /// <summary>
        /// Objects the function.
        /// </summary>
        /// <returns>null</returns>
        public virtual object ObjectFunction() => null;

        /// <summary>
        /// Actions this instance.
        /// </summary>
        public virtual void Action()
        {
        }
    }
}