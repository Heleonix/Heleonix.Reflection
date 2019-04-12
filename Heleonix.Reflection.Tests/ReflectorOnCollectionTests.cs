// <copyright file="ReflectorOnCollectionTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2017-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Reflection.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Heleonix.Reflection.Tests.Common.Dummies;
    using NUnit.Framework;

    /// <summary>
    /// Test class for using reflection on collections
    /// </summary>
    [TestFixture]
    public class ReflectorOnCollectionTests
    {
        /// <summary>
        /// When input is IEnumerable should sets a new value for a property of item with given index
        /// </summary>
        [Test]
        public void SetOnCollection()
        {
            // Arrange
            var email = "maria@sklodowskacurie.pl";
            var name = "Sklodowska-Curie";
            var lines = new Dictionary<string, string>();
            lines.Add("PersonalInfo.LastName", name);
            lines.Add("PersonalInfo.EmailAddresses[0].Address", email);

            var o = new Member { PersonalInfo = new PersonalInfo { LastName = "Rosling", EmailAddresses = new List<EmailAddress>() } };
            o.PersonalInfo.EmailAddresses.Add(new EmailAddress { Address = "hans@rosling.se" });

            // Act
            foreach (var data in lines)
            {
                Reflector.Set(o, null, data.Key, data.Value);
            }

            // Assert
            Assert.AreEqual(name, o.PersonalInfo.LastName);
            Assert.AreEqual(email, o.PersonalInfo.EmailAddresses.First().Address);
        }

        /// <summary>
        /// When input is IEnumerable should return value of item with given index
        /// </summary>
        [Test]
        public void GetOnCollection()
        {
            // Arrange
            var email = "maria@sklodowskacurie.pl";
            var name = "Sklodowska-Curie";
            var lines = new Dictionary<string, string>();
            lines.Add("PersonalInfo.LastName", name);
            lines.Add("PersonalInfo.EmailAddresses[0].Address", email);

            var o = new Member { PersonalInfo = new PersonalInfo { LastName = "Rosling", EmailAddresses = new List<EmailAddress>() } };
            o.PersonalInfo.EmailAddresses.Add(new EmailAddress { Address = "hans@rosling.se" });

            // Act
            var emailSuccess = Reflector.Get(o, null, lines.Skip(1).First().Key, out object emailValue);
            var nameSuccess = Reflector.Get(o, null, lines.First().Key, out object nameValue);

            // Assert
            Assert.IsTrue(emailSuccess);
            Assert.IsTrue(nameSuccess);
            Assert.AreEqual(nameValue, o.PersonalInfo.LastName);
            Assert.AreEqual(emailValue, o.PersonalInfo.EmailAddresses.First().Address);
        }
    }
}
