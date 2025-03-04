// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Webserver.API.UnitTests
{
    public class ApiRequestSplitterTests
    {
        public IApiRequestSplitter splitter;
        public IApiRequestSplitter splitter2;

        public ApiRequestSplitterTests()
        {
            splitter = new ApiRequestSplitter();
            splitter2 = new ApiRequestSplitterByBytes();
        }

        /// <summary>
        /// bigger loops take too long to run since the splitter takes so long
        /// </summary>
        /// <param name="loopAmount">amount of loops -> Api Ping requests to be split.</param>
        [Test]
        public void ApiRequestSplitter_AndByBytes_SameResult([Values(1,5,10/*5000, 100_000, 500_000*/)] int loopAmount)
        {
            var requests = new List<ApiRequest>();
            for (int i = 0; i < loopAmount; i++)
            {
                requests.Add(new ApiRequest("Api.Ping", "2.0", i.ToString()));
            }
            CheckSplitter_SameResults(requests, 150);
            CheckSplitter_SameResults(requests, 64 * 1024);
            CheckSplitter_SameResults(requests, 128 * 1024);
        }

        public void CheckSplitter_SameResults(IEnumerable<IApiRequest> requests, long maxRequestSize)
        {
            var sw = Stopwatch.StartNew();
            var res1 = splitter.GetMessageChunks(requests, maxRequestSize);
            var time1 = sw.Elapsed;
            Console.WriteLine($"time taken {splitter.GetType()}: {time1}");
            sw = Stopwatch.StartNew();
            var res2 = splitter2.GetMessageChunks(requests, maxRequestSize);
            var time2 = sw.Elapsed;
            Console.WriteLine($"time taken {splitter2.GetType()}: {time2}");
            // determine 'betterness in percent'
            var ticks1 = time1.Ticks;
            var ticks2 = time2.Ticks;
            var greater = ticks1 > ticks2 ? ticks1 : ticks2;
            var less = ticks1 < ticks2 ? ticks1 : ticks2;
            var factor = (double)greater / (double)less;
            factor -= 1;
            factor *= 100;
            Console.WriteLine($"{(ticks1 < ticks2 ? $"{splitter.GetType()}" : $"{splitter2.GetType()}")} is better by {factor}%!");
            CheckEquality(res1, res2);
        }

        private void CheckEquality(IEnumerable<byte[]> arr1,  IEnumerable<byte[]> arr2)
        {
            Assert.That(arr1.Count(), Is.EqualTo(arr2.Count()));
            var counter = 0;
            var arr2Array = arr2.ToArray();
            foreach ( var item in arr1)
            {
                if (!item.SequenceEqual(arr2Array[counter]))
                {
                    var itemString = Encoding.UTF8.GetString(item);
                    var itemString2 = Encoding.UTF8.GetString(arr2Array[counter]);
                    Assert.That(itemString, Is.EqualTo(itemString2));
                }
                counter++;
            }
        }
    }
}
