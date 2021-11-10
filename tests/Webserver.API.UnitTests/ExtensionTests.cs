// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Extensions;
using Siemens.Simatic.S7.Webserver.API.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class ExtensionTests : Base
    {

        [Test]
        public void MakeSureNoIdIsContainedTwice()
        {
            var requests = new List<ApiRequest>();
            for(int i = 0; i< 500;i++)
            {
                requests.Add(new ApiRequest("", "", "1"));
            }
            if (ReqIdGenerator is CharSetIdGenerator)
            {
                var charSetGen = ReqIdGenerator as CharSetIdGenerator;
                Console.WriteLine($"Determined ThreadSleepTime:{charSetGen.ThreadSleepTime}");
            }
            Assert.That(requests.MakeSureRequestIdsAreUnique(ReqIdGenerator, TimeSpan.FromMinutes(2)) == true);
        }

        [Test]
        public void MakeSureNoIdIsContainedTwice_TooSlow()
        {
            var requests = new List<ApiRequest>();
            for (int i = 0; i < 1000000; i++)
            {
                requests.Add(new ApiRequest("", "", "1"));
            }
            if (ReqIdGenerator is CharSetIdGenerator)
            {
                var charSetGen = ReqIdGenerator as CharSetIdGenerator;
                Console.WriteLine($"Determined ThreadSleepTime:{charSetGen.ThreadSleepTime}");
            }
            TimeSpan timeOut = TimeSpan.FromMilliseconds(1);
            var start = DateTime.Now;
            Console.WriteLine($"Could successfully \"uniquelify\" requests?:{requests.MakeSureRequestIdsAreUnique(ReqIdGenerator, timeOut)}");
            var end = DateTime.Now;
            Assert.That((end - start - timeOut - TimeSpan.FromMilliseconds(300)) < TimeSpan.FromSeconds(0));
        }
    }
}
