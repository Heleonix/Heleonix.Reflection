// <copyright file="PersonalInfo.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2017-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests.Common.Dummies
{
    using System.Collections.Generic;

    /// <summary>
    /// An internal class
    /// </summary>
    internal class PersonalInfo
    {
        /// <summary>
        /// Gets or sets LastName
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets e-mails
        /// </summary>
        public List<EmailAddress> EmailAddresses { get; set; }
    }
}
