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
        public void ThrowOnLength()
        {
            var reqIdGen = new GUIDGenerator();
            var reqIdGen2 = new GUIDGenerator(reqIdGen.DefaultLength + 1);
            Assert.Throws<InvalidOperationException>(() =>  reqIdGen2.Generate());
        }
    }
}
