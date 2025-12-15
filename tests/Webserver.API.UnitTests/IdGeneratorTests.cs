// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using System;

namespace Webserver.API.UnitTests
{
    class IdGeneratorTests : Base
    {

        [Test]
        public void GUIDGenerator_InvalidLength_ThrowsArgumentOutOfRangeException()
        {
            var reqIdGen = new GUIDGenerator();
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new GUIDGenerator(reqIdGen.DefaultLength + 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new GUIDGenerator(0);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new GUIDGenerator(-3);
            });
        }

        [Test]
        public void CharSetIdGenerator_InvalidLengthOrCharSet_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new CharSetIdGenerator(null, 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new CharSetIdGenerator("", 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new CharSetIdGenerator("a", 0);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new CharSetIdGenerator("a", -10);
            });
        }
    }
}
