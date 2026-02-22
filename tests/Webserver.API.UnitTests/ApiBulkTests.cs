// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
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
        public void ApiBulk_RequestsSameId_ThrowsArgumentException()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBrowseResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            var requests = new List<ApiRequest>();
            for (int i = 0; i < 100; i++)
            {
                requests.Add(new ApiRequest("", "2.0", i.ToString()));
            }
            requests.Add(new ApiRequest("Api.Ping", "2.0", "10"));
            Assert.ThrowsAsync<ArgumentException>(async () => await TestHandler.ApiBulkAsync(requests));
        }

        [Test]
        public async Task ApiBulk_Send100RequestsWithUniqueIds_Returns100SucessfulResponses()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkManyResponses); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var requests = new List<ApiRequest>();
            for (int i = 0; i < 100; i++)
            {
                requests.Add(new ApiRequest("Api.Ping", "2.0", i.ToString()));
            }
            var response = await TestHandler.ApiBulkAsync(requests);
            Assert.That(response.SuccessfulResponses.Count(), Is.EqualTo(100));
        }

        [Test]
        public void ApiBulk_RequestBiggerThanMaxReqSize_Throws()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkManyResponses); // Respond with JSON -> 100 requests
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var requests = new List<IApiRequest>();

            var sampleReq = new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate());

            if (sampleReq.Params != null)
            {
                sampleReq.Params = sampleReq.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string serialized = JsonConvert.SerializeObject(sampleReq, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.UTF8.GetBytes(serialized);
            // size should be ~100
            var sizeofArr = byteArr.Length;
            Assert.That(sizeofArr, Is.GreaterThan(90).And.LessThan(110));
            TestHandler.MaxRequestSize = 5; // -> 20 chunks
            requests.Add(sampleReq);
            requests = ApiRequestFactory.GetApiBulkRequestWithUniqueIds(requests).ToList();
            Assert.ThrowsAsync<ApiRequestBiggerThanMaxMessageSizeException>(async () =>
            {
                await TestHandler.ApiBulkAsync(requests);
            });
        }

        [Test]
        public void ApiBulk_SecondAndTherebyLastRequestBiggerThanMaxReqSize_Throws()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkManyResponses); // Respond with JSON -> 100 requests
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var requests = new List<IApiRequest>();

            var sampleReqLittleBytes = new ApiRequest("Api.Ping", "2.0", ReqIdGenerator.Generate());
            if (sampleReqLittleBytes.Params != null)
            {
                sampleReqLittleBytes.Params = sampleReqLittleBytes.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string sampleReqLittleBytesSerialized = JsonConvert.SerializeObject(sampleReqLittleBytes, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArrLittleBytes = Encoding.UTF8.GetBytes(sampleReqLittleBytesSerialized);

            var sampleReq = new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate());
            if (sampleReq.Params != null)
            {
                sampleReq.Params = sampleReq.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string serialized = JsonConvert.SerializeObject(sampleReq, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.UTF8.GetBytes(serialized);

            TestHandler.MaxRequestSize = byteArrLittleBytes.Length + 5;
            Assert.That(byteArr.Length, Is.GreaterThan(TestHandler.MaxRequestSize));
            requests.Add(sampleReqLittleBytes);
            requests.Add(sampleReq);
            requests = ApiRequestFactory.GetApiBulkRequestWithUniqueIds(requests).ToList();
            var exc = Assert.ThrowsAsync<ApiRequestBiggerThanMaxMessageSizeException>(async () =>
            {
                await TestHandler.ApiBulkAsync(requests);
            });
            Assert.That(exc.Message, Contains.Substring("exceeds MaxRequestSize"));
        }

        [Test]
        public void ApiBulk_SecondRequestBiggerThanMaxReqSize_ThirdValid_Throws()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkManyResponses); // Respond with JSON -> 100 requests
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var requests = new List<IApiRequest>();

            var sampleReqLittleBytes = new ApiRequest("Api.Ping", "2.0", ReqIdGenerator.Generate());
            if (sampleReqLittleBytes.Params != null)
            {
                sampleReqLittleBytes.Params = sampleReqLittleBytes.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string sampleReqLittleBytesSerialized = JsonConvert.SerializeObject(sampleReqLittleBytes, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArrLittleBytes = Encoding.UTF8.GetBytes(sampleReqLittleBytesSerialized);

            var sampleReq = new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate());
            if (sampleReq.Params != null)
            {
                sampleReq.Params = sampleReq.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string serialized = JsonConvert.SerializeObject(sampleReq, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.UTF8.GetBytes(serialized);

            TestHandler.MaxRequestSize = byteArrLittleBytes.Length + 5;
            Assert.That(byteArr.Length, Is.GreaterThan(TestHandler.MaxRequestSize));
            requests.Add(sampleReqLittleBytes);
            requests.Add(sampleReq);
            var sampleReqLittleBytes2 = new ApiRequest("Api.Ping", "2.0", ReqIdGenerator.Generate());
            requests.Add(sampleReqLittleBytes2);
            requests = ApiRequestFactory.GetApiBulkRequestWithUniqueIds(requests).ToList();
            var exc = Assert.ThrowsAsync<ApiRequestBiggerThanMaxMessageSizeException>(async () =>
            {
                await TestHandler.ApiBulkAsync(requests);
            });
            Assert.That(exc.Message, Contains.Substring("exceeds MaxRequestSize"));
        }

        [Test]
        public async Task ApiBulk_BiggerThanMaxRequestSize_Chunked()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkManyResponses); // Respond with JSON -> 100 requests
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var requests = new List<IApiRequest>();

            var sampleReq = new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate());

            if (sampleReq.Params != null)
            {
                sampleReq.Params = sampleReq.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string serialized = JsonConvert.SerializeObject(sampleReq, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.UTF8.GetBytes(serialized);
            // size should be ~100
            var sizeofArr = byteArr.Length;
            Assert.That(sizeofArr, Is.GreaterThan(90).And.LessThan(110));
            TestHandler.MaxRequestSize = 150; // -> 1 request per chunk chunk
            requests.Add(sampleReq);
            requests.Add(new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate()));
            requests.Add(new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate()));
            requests.Add(new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate()));
            requests.Add(new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate()));
            var response = await TestHandler.ApiBulkAsync(requests);
            Assert.That(response.SuccessfulResponses.Count(), Is.EqualTo(5 * 100)); // 5 requests, the answer (105) has 100 success
        }

        [Test]
        public async Task ApiBulk_BiggerThanMaxRequestSize_ChunkedWithMultipleWithinChunk()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkManyResponses); // Respond with JSON -> 100 requests
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var requests = new List<IApiRequest>();

            var sampleReq = new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate());

            if (sampleReq.Params != null)
            {
                sampleReq.Params = sampleReq.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string serialized = JsonConvert.SerializeObject(sampleReq, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.UTF8.GetBytes(serialized);
            // size should be ~100
            var sizeofArr = byteArr.Length;
            Assert.That(sizeofArr, Is.GreaterThan(90).And.LessThan(110));
            TestHandler.MaxRequestSize = 350; // -> 3 request per chunk chunk
            requests.Add(sampleReq);
            requests.Add(new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate()));
            requests.Add(new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate()));
            requests.Add(new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate()));
            requests.Add(new ApiRequest("Api.Ping100SizeUsingCharsEx", "2.0", ReqIdGenerator.Generate()));
            var response = await TestHandler.ApiBulkAsync(requests);
            Assert.That(response.SuccessfulResponses.Count(), Is.EqualTo(2 * 100)); // 5 requests, the answer (105) has 100 success
        }
    }
}
