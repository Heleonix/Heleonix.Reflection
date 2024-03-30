// <copyright file="SubSubItem.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests.Common.Dummies
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The sub sub item type.
    /// </summary>
    public class SubSubItem
    {
#pragma warning disable SA1401 // Fields must be private
#pragma warning disable CA2211 // Non-constant fields should not be visible

        /// <summary>
        /// The static text field.
        /// </summary>
        public static string StaticTextField;
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning restore SA1401 // Fields must be private

#pragma warning disable SA1401 // Fields must be private
#pragma warning disable CA1051 // Do not declare visible instance fields

        /// <summary>
        /// The text field.
        /// </summary>
        public string TextField;

        /// <summary>
        /// The enum field.
        /// </summary>
        public EnumItem EnumField;
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore SA1401 // Fields must be private

        /// <summary>
        /// Holds the value of the static text property.
        /// </summary>
#pragma warning disable S4487 // Unread "private" fields should be removed
        private static string staticTextSetProperty;
#pragma warning restore S4487 // Unread "private" fields should be removed

        /// <summary>
        /// Provides a list for the indexer.
        /// </summary>
        private readonly IList<string> list = new List<string> { "111" };

        /// <summary>
        /// Holds the value of the static text property.
        /// </summary>
#pragma warning disable S4487 // Unread "private" fields should be removed
        private string textSetProperty;
#pragma warning restore S4487 // Unread "private" fields should be removed

        /// <summary>
        /// Gets text.
        /// </summary>
        public static string StaticTextGetProperty { get; }

        /// <summary>
        /// Sets text.
        /// </summary>
#pragma warning disable S2376 // Write-only properties should not be used
        public static string StaticTextSetProperty
        {
            set
            {
                staticTextSetProperty = value;
            }
        }
#pragma warning restore S2376 // Write-only properties should not be used

        /// <summary>
        /// Gets or sets the static text.
        /// </summary>
        public static string StaticTextProperty { get; set; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string TextGetProperty { get; }

        /// <summary>
        /// Sets text.
        /// </summary>
#pragma warning disable S2376 // Write-only properties should not be used
        public string TextSetProperty
        {
            set
            {
                this.textSetProperty = value;
            }
        }
#pragma warning restore S2376 // Write-only properties should not be used

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string TextProperty { get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        public object ObjectProperty { get; set; }

        /// <summary>
        /// Gets or sets the enum.
        /// </summary>
        public EnumItem? EnumProperty { get; set; }

        /// <summary>
        /// Gets or sets an item by the specified index.
        /// </summary>
        /// <param name="index">An index to get or set an item.</param>
        /// <returns>An item by the specified index.</returns>
        public string this[int index]
        {
            get
            {
                return this.list[index];
            }

            set
            {
                this.list[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><paramref name="left"/> + <paramref name="right"/>.</returns>
        public virtual string Concat(string left, string right) => left + right;

        /// <summary>
        /// Adds the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><paramref name="left"/> + <paramref name="right"/>.</returns>
        public virtual double Add(double left, double right) => left + right;

        /// <summary>
        /// Adds the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><paramref name="left"/> + <paramref name="right"/>.</returns>
        public virtual float Add(float left, float right) => left + right;
    }
}