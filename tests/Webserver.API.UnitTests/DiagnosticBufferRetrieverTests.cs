// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Siemens.Simatic.S7.Webserver.API.Services.DiagnosticBuffer;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class DiagnosticBufferRetrieverTests : Base
    {
        private const string LastModified = "2026-09-27T00:00:00Z";

        [Test]
        public async Task RetrieveAllAsync_AppendsFiftyEntryChunksAndFinalPartialChunk()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Respond("application/json", BuildDiagnosticBufferResponse(50, 123));
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":100")
                .Respond("application/json", BuildDiagnosticBufferResponse(100, 123));
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":123")
                .Respond("application/json", BuildDiagnosticBufferResponse(123, 123));

            using var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") };
            var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var retriever = new DiagnosticBufferRetriever(requestHandler);

            var result = await retriever.RetrieveAllAsync(new CultureInfo("en-US"));

            Assert.Multiple(() =>
            {
                Assert.That(result.Entries.Count, Is.EqualTo(123));
                Assert.That(result.Entries[0].Short_Text, Is.EqualTo("entry-0"));
                Assert.That(result.Entries[49].Short_Text, Is.EqualTo("entry-49"));
                Assert.That(result.Entries[50].Short_Text, Is.EqualTo("entry-50"));
                Assert.That(result.Entries[122].Short_Text, Is.EqualTo("entry-122"));
                Assert.That(result.Count_Current, Is.EqualTo(123));
            });
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task RetrieveAllAsync_StopsAfterEmptyBufferResponse()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Respond("application/json", BuildDiagnosticBufferResponse(0, 0));

            using var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") };
            var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var retriever = new DiagnosticBufferRetriever(requestHandler);

            var result = await retriever.RetrieveAllAsync(new CultureInfo("en-US"));

            Assert.That(result.Entries, Is.Empty);
            Assert.That(result.Count_Current, Is.Zero);
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void RetrieveAllAsync_PropagatesRequestFailure()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Throw(new HttpRequestException("diagnostic buffer request failed"));

            using var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") };
            var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var retriever = new DiagnosticBufferRetriever(requestHandler);

            Assert.ThrowsAsync<HttpRequestException>(async () =>
                await retriever.RetrieveAllAsync(new CultureInfo("en-US")));
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void RetrieveAllAsync_ThrowsWhenLargerCountMakesNoProgress()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Respond("application/json", BuildDiagnosticBufferResponse(50, 75));
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":75")
                .Respond("application/json", BuildDiagnosticBufferResponse(50, 75));

            using var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") };
            var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var retriever = new DiagnosticBufferRetriever(requestHandler);

            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await retriever.RetrieveAllAsync(new CultureInfo("en-US")));

            Assert.That(exception.Message, Does.Contain("made no progress"));
            mockHttp.VerifyNoOutstandingExpectation();
        }

        private static string BuildDiagnosticBufferResponse(int returnedCount, int totalCount)
        {
            var entries = new JArray(Enumerable.Range(0, returnedCount).Select(index =>
                new JObject
                {
                    ["timestamp"] = $"2026-09-27T00:{index / 60:00}:{index % 60:00}Z",
                    ["status"] = "incoming",
                    ["long_text"] = $"entry-{index}",
                    ["short_text"] = $"entry-{index}",
                    ["help_text"] = string.Empty,
                    ["event"] = new JObject
                    {
                        ["textlist_id"] = 1,
                        ["text_id"] = index
                    }
                }));

            var result = new JObject
            {
                ["entries"] = entries,
                ["last_modified"] = LastModified,
                ["count_current"] = totalCount,
                ["count_max"] = 3200,
                ["language"] = "en-US"
            };

            return new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = "test",
                ["result"] = result
            }.ToString(Formatting.None);
        }
    }
}
