// <copyright file="MembersInfo.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2017-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Provides information about found members.
    /// </summary>
    public class MembersInfo
    {
        /// <summary>
        /// Gets or sets the type of the container in which members (declared in it or in base classes) were found.
        /// </summary>
        /// <value>The type of the container.</value>
        public Type ContainerType { get; set; }

        /// <summary>
        /// Gets or sets the container object - instance of the type in which members (declared in it or in base classes) were found.
        /// </summary>
        /// <value>The container object.</value>
        public object ContainerObject { get; set; }

        /// <summary>
        /// Gets the found members.
        /// </summary>
        /// <value>The found members.</value>
        public List<MemberInfo> Members { get; } = new List<MemberInfo>();
    }
}