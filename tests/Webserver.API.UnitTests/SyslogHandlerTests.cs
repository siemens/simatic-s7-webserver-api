// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Services;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.Services.Syslog;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class SyslogHandlerTests : Base
    {
        [Test]
        public void SyslogHandler_NullRequestHandler_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SyslogHandler(null));
        }

        [Test]
        public async Task RetrieveAllAsync_EmptySyslog_ReturnsEmptyResult()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Respond("application/json", BuildSyslogResponse(0, 0, 0));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                var result = await handler.RetrieveAllAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(result.Entries, Is.Empty);
                    Assert.That(result.Count_Total, Is.Zero);
                    Assert.That(result.Count_Lost, Is.Zero);
                });
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public async Task RetrieveAllAsync_AllEntriesLost_ReturnsEmptyResult()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Respond("application/json", BuildSyslogResponse(0, 10, 10));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                var result = await handler.RetrieveAllAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(result.Entries, Is.Empty);
                    Assert.That(result.Count_Total, Is.EqualTo(10));
                    Assert.That(result.Count_Lost, Is.EqualTo(10));
                });
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public async Task RetrieveAllAsync_SinglePageComplete_ReturnsAllEntries()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Respond("application/json", BuildSyslogResponse(4, 5, 1, startId: 5));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                var result = await handler.RetrieveAllAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(result.Entries.Count, Is.EqualTo(4));
                    Assert.That(result.Entries[0].Raw, Is.EqualTo("entry-5"));
                    Assert.That(result.Entries[3].Raw, Is.EqualTo("entry-2"));
                    Assert.That(result.Count_Total, Is.EqualTo(5));
                    Assert.That(result.Count_Lost, Is.EqualTo(1));
                });
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public async Task RetrieveAllAsync_MultiplePagesWithFinalPartialPage_AggregatesCorrectly()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            // Page 1: count: 50, first: null -> returns 50 entries (123 down to 74)
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .With(m => !m.Content.ReadAsStringAsync().Result.Contains("\"first\""))
                .Respond("application/json", BuildSyslogResponse(50, 123, 0, startId: 123));

            // Page 2: count: 50, first: 73 -> returns 50 entries (73 down to 24)
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .WithPartialContent("\"first\":73")
                .Respond("application/json", BuildSyslogResponse(50, 123, 0, startId: 73));

            // Page 3: count: 23, first: 23 -> returns 23 entries (23 down to 1)
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":23")
                .WithPartialContent("\"first\":23")
                .Respond("application/json", BuildSyslogResponse(23, 123, 0, startId: 23));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                var result = await handler.RetrieveAllAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(result.Entries.Count, Is.EqualTo(123));
                    Assert.That(result.Entries[0].Raw, Is.EqualTo("entry-123"));
                    Assert.That(result.Entries[49].Raw, Is.EqualTo("entry-74"));
                    Assert.That(result.Entries[50].Raw, Is.EqualTo("entry-73"));
                    Assert.That(result.Entries[99].Raw, Is.EqualTo("entry-24"));
                    Assert.That(result.Entries[100].Raw, Is.EqualTo("entry-23"));
                    Assert.That(result.Entries[122].Raw, Is.EqualTo("entry-1"));
                    Assert.That(result.Count_Total, Is.EqualTo(123));
                    Assert.That(result.Count_Lost, Is.Zero);
                });
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public async Task RetrieveAllAsync_MultiplePagesWithCountLost_AggregatesCorrectly()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            // Total 123, Lost 20 => available = 103 (entries 123 down to 21)
            // Page 1: count: 50, first: null -> returns 50 entries (123 down to 74)
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .With(m => !m.Content.ReadAsStringAsync().Result.Contains("\"first\""))
                .Respond("application/json", BuildSyslogResponse(50, 123, 20, startId: 123));

            // Page 2: count: 50, first: 73 -> returns 50 entries (73 down to 24)
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .WithPartialContent("\"first\":73")
                .Respond("application/json", BuildSyslogResponse(50, 123, 20, startId: 73));

            // Page 3: count: 3, first: 23 -> returns 3 entries (23 down to 21)
            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":3")
                .WithPartialContent("\"first\":23")
                .Respond("application/json", BuildSyslogResponse(3, 123, 20, startId: 23));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                var result = await handler.RetrieveAllAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(result.Entries.Count, Is.EqualTo(103));
                    Assert.That(result.Entries[0].Raw, Is.EqualTo("entry-123"));
                    Assert.That(result.Entries[102].Raw, Is.EqualTo("entry-21"));
                    Assert.That(result.Count_Total, Is.EqualTo(123));
                    Assert.That(result.Count_Lost, Is.EqualTo(20));
                });
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public async Task RetrieveAllAsync_WithRedundancyId_PassesParameter()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"redundancy_id\":1")
                .Respond("application/json", BuildSyslogResponse(1, 1, 0, startId: 1));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                var result = await handler.RetrieveAllAsync(ApiPlcRedundancyId.RedundancyId_1);

                Assert.That(result.Entries.Count, Is.EqualTo(1));
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public void RetrieveAllAsync_SyslogTotalChangedDuringRetrieval_ThrowsInvalidOperationException()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Respond("application/json", BuildSyslogResponse(50, 100, 0, startId: 100));

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .WithPartialContent("\"first\":50")
                .Respond("application/json", BuildSyslogResponse(50, 105, 0, startId: 50));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.RetrieveAllAsync());
                Assert.That(ex.Message, Does.Contain("changed while it was being retrieved"));
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public void RetrieveAllAsync_SyslogLostChangedDuringRetrieval_ThrowsInvalidOperationException()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Respond("application/json", BuildSyslogResponse(50, 100, 0, startId: 100));

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .WithPartialContent("\"first\":50")
                .Respond("application/json", BuildSyslogResponse(50, 100, 5, startId: 50));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.RetrieveAllAsync());
                Assert.That(ex.Message, Does.Contain("changed while it was being retrieved"));
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public void RetrieveAllAsync_NoProgress_ThrowsInvalidOperationException()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Respond("application/json", BuildSyslogResponse(50, 75, 0, startId: 75));

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":25")
                .WithPartialContent("\"first\":25")
                .Respond("application/json", BuildSyslogResponse(0, 75, 0));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.RetrieveAllAsync());
                Assert.That(ex.Message, Does.Contain("made no progress"));
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public void RetrieveAllAsync_MalformedCountState_ThrowsInvalidOperationException()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Respond("application/json", BuildSyslogResponse(0, 5, 10));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.RetrieveAllAsync());
                Assert.That(ex.Message, Does.Contain("invalid syslog count state"));
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public void RetrieveAllAsync_RequestFailure_PropagatesException()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Throw(new HttpRequestException("syslog browse network failure"));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                Assert.ThrowsAsync<HttpRequestException>(async () => await handler.RetrieveAllAsync());
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public void RetrieveAllAsync_CancellationRequested_Cancels()
        {
            var mockHttp = new MockHttpMessageHandler();
            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                using (var cts = new CancellationTokenSource())
                {
                    cts.Cancel();

                    Assert.ThrowsAsync<OperationCanceledException>(async () =>
                        await handler.RetrieveAllAsync(cancellationToken: cts.Token));
                }
            }
        }

        [Test]
        public void RetrieveAll_Sync_ReturnsExpectedResult()
        {
            var mockHttp = new MockHttpMessageHandler();
            var url = $"https://{Ip}/api/jsonrpc";

            mockHttp.Expect(HttpMethod.Post, url)
                .WithPartialContent("\"count\":50")
                .Respond("application/json", BuildSyslogResponse(2, 2, 0, startId: 2));

            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var handler = new SyslogHandler(requestHandler);

                var result = handler.RetrieveAll();

                Assert.That(result.Entries.Count, Is.EqualTo(2));
                mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public void ApiStandardServiceFactory_GetSyslogHandler_ReturnsConfiguredHandler()
        {
            var mockHttp = new MockHttpMessageHandler();
            using (var client = new HttpClient(mockHttp) { BaseAddress = new Uri($"https://{Ip}") })
            {
                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var factory = new ApiStandardServiceFactory();

                var handler = factory.GetSyslogHandler(requestHandler);

                Assert.That(handler, Is.Not.Null);
                Assert.That(handler, Is.InstanceOf<SyslogHandler>());
            }
        }

        private static string BuildSyslogResponse(int returnedCount, uint totalCount, uint lostCount, int startId = 0)
        {
            var entries = new JArray(Enumerable.Range(0, returnedCount).Select(index =>
            {
                int currentId = startId > 0 ? (startId - index) : index;
                return new JObject
                {
                    ["raw"] = $"entry-{currentId}"
                };
            }));

            var result = new JObject
            {
                ["entries"] = entries,
                ["count_total"] = totalCount,
                ["count_lost"] = lostCount
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
