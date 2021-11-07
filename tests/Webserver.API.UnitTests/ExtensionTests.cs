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
        public void MakeSureNoIdIsContainedTwiceSleepTime16()
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
            requests.MakeSureRequestIdsAreUnique(ReqIdGenerator);
            
            Console.WriteLine($"Determined ThreadSleepTime:{ReqIdGenerator}");
            if (requests.GroupBy(el => el.Id).Count() != requests.Count)
            {
                Assert.Fail("Requests are not handled as they should be!");
            }
            
        }
    }
}
