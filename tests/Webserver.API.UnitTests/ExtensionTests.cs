// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Webserver.API.UnitTests
{
    public class ExtensionTests : Base
    {

        [Test]
        public void MakeSureNoIdIsContainedTwice_EnoughTime_Works()
        {
            var requests = new List<IApiRequest>();
            for (int i = 0; i < 500; i++)
            {
                requests.Add(new ApiRequest("", "", "1"));
            }
            if (ReqIdGenerator is CharSetIdGenerator)
            {
                var charSetGen = ReqIdGenerator as CharSetIdGenerator;
                Console.WriteLine($"Determined ThreadSleepTime:{charSetGen.ThreadSleepTime}");
            }
            if (ReqIdGenerator is GUIDGenerator)
            {
                var guidGen = ReqIdGenerator as GUIDGenerator;
                Console.WriteLine($"Determined DefaultLength:{guidGen.DefaultLength}");
            }
            requests = ApiRequestFactory.GetApiBulkRequestWithUniqueIds(requests, TimeSpan.FromMinutes(2)).ToList();
            Assert.That(requests.GroupBy(req => req.Id).Count() == requests.Count);
        }

        [Test]
        public void MakeSureNoIdIsContainedTwice_NotEnoughTime_DoesntRunWayTooLong()
        {
            var requests = new List<IApiRequest>();
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
            requests = ApiRequestFactory.GetApiBulkRequestWithUniqueIds(requests, timeOut).ToList();
            var end = DateTime.Now;
            var timeTakenMinusTwoSeconds = (end - start - timeOut - TimeSpan.FromSeconds(2)); // timespan fromseconds => can take longer but shouldnt take way(!) longer => accept 2 sec
            Assert.That(timeTakenMinusTwoSeconds < TimeSpan.FromSeconds(0));
            // also accept if "we were fast enough"
            Assert.That(requests.GroupBy(req => req.Id).Count() <= requests.Count);
        }
    }
}
