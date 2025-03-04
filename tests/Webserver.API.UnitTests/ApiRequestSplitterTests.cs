using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

        [Test]
        public void ApiRequestSplitter_AndByBytes_SameResult([Values(1, 10, 20, 50, 1000, 100_000)] int loopAmount)
        {
            var requests = new List<ApiRequest>();
            for (int i = 0; i < loopAmount; i++)
            {
                requests.Add(new ApiRequest("Api.Ping", "2.0", i.ToString()));
            }
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
