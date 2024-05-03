// <copyright file="SubItem.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests.Common.Dummies
{
    using System.Collections.Generic;

    /// <summary>
    /// The sub item type.
    /// </summary>
    public class SubItem
    {
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1401 // Fields must be private
        /// <summary>
        /// Gets or sets the sub item field.
        /// </summary>
        public SubSubItem SubSubItemField = new SubSubItem();
#pragma warning restore SA1401 // Fields must be private
#pragma warning restore CA1051 // Do not declare visible instance fields

        /// <summary>
        /// Gets or sets the sub sub item property.
        /// </summary>
        public SubSubItem SubSubItemProperty { get; set; } = new SubSubItem();

        /// <summary>
        /// Gets the sub sub items list property.
        /// </summary>
        public List<SubSubItem> SubSubItemsListProperty { get; } = new List<SubSubItem>();

        /// <summary>
        /// Gets or sets the sub sub items enumerable property.
        /// </summary>
        public IEnumerable<SubSubItem> SubSubItemsEnumerableProperty { get; set; } = new Queue<SubSubItem>();

        /// <summary>
        /// Gets or sets the sub sub items string dictionary property.
        /// </summary>
        public Dictionary<string, SubSubItem> SubSubItemsStringDictionaryProperty { get; set; } = new Dictionary<string, SubSubItem>();

        /// <summary>
        /// Gets or sets the sub sub items int dictionary property.
        /// </summary>
        public Dictionary<int, SubSubItem> SubSubItemsIntDictionaryProperty { get; set; } = new Dictionary<int, SubSubItem>();
    }
}