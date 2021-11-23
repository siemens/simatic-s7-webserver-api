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
        public void IDGenerator_LengthTooLong_Throws()
        {
            var reqIdGen = new GUIDGenerator();
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var reqIdGen2 = new GUIDGenerator(reqIdGen.DefaultLength + 1);
                });
        }
    }
}
