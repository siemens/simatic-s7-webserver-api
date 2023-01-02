// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    class IdGeneratorTests : Base
    {

        [Test]
        public void GUIDGenerator_InvalidLength_ThrowsArgumentOutOfRangeException()
        {
            var reqIdGen = new GUIDGenerator();
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var reqIdGen2 = new GUIDGenerator(reqIdGen.DefaultLength + 1);
                });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var reqIdGen2 = new GUIDGenerator(0);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var reqIdGen2 = new GUIDGenerator(-3);
            });
        }

        [Test]
        public void CharSetIdGenerator_InvalidLengthOrCharSet_ThrowsArgumentOutOfRangeException()
        {
            var reqIdGen = new CharSetIdGenerator();
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var reqIdGen2 = new CharSetIdGenerator(null, 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var reqIdGen2 = new CharSetIdGenerator("", 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var reqIdGen2 = new CharSetIdGenerator("a", 0);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var reqIdGen2 = new CharSetIdGenerator("a", -10);
            });
        }
    }
}
