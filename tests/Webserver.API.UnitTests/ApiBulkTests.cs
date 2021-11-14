// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Siemens.Simatic.S7.Webserver.API.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    class ApiBulkTests : Base
    {
        [Test]
        public void ApiBulk_RequestsSameId()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBrowseResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);

            var requests = new List<ApiRequest>();
            for(int i = 0; i < 100; i++)
            {
                requests.Add(new ApiRequest("", "2.0", i.ToString()));
            }
            requests.Add(new ApiRequest("Api.Ping", "2.0", "10"));
            Assert.ThrowsAsync<ArgumentException>(async() => await TestHandler.ApiBulkAsync(requests));
        }

        [Test]
        public async Task ApiBulk_RequestsUniqueIds()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkManyResponses); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            var requests = new List<ApiRequest>();
            for (int i = 0; i < 100; i++)
            {
                requests.Add(new ApiRequest("Api.Ping", "2.0", i.ToString()));
            }
            var response = await TestHandler.ApiBulkAsync(requests);
            Assert.That(response.SuccessfulResponses.Count() == 100);
        }
    }
}
