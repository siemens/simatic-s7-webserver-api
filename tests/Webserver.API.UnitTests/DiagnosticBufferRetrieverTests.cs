// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer;
using Siemens.Simatic.S7.Webserver.API.Services;
using Siemens.Simatic.S7.Webserver.API.Services.DiagnosticBuffer;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class DiagnosticBufferRetrieverTests : Base
    {
        private const string LastModified = "2026-09-27T00:00:00Z";
        private MockHttpMessageHandler _mockHttp;
        private HttpClient _client;
        private DiagnosticBufferRetriever _retriever;

        [SetUp]
        public void SetUp()
        {
            _mockHttp = new MockHttpMessageHandler();
            _client = new HttpClient(_mockHttp) { BaseAddress = new Uri($"https://{Ip}") };
            var handler = new ApiHttpClientRequestHandler(_client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            _retriever = new DiagnosticBufferRetriever(handler);
        }

        [TearDown]
        public void TearDown() => _client.Dispose();

        [TestCase(0)]
        [TestCase(50)]
        [TestCase(75)]
        public void RetrieveAll_ReturnsEveryEntryInResponseOrder(int totalCount)
        {
            ExpectBrowse(50).Respond("application/json", BuildResponse(Math.Min(50, totalCount), totalCount));
            if (totalCount > 50)
            {
                ExpectBrowse(totalCount).Respond("application/json", BuildResponse(totalCount, totalCount));
            }

            var result = _retriever.RetrieveAll(new CultureInfo("en-US"));

            Assert.That(result.Entries.Select(entry => entry.Short_Text),
                Is.EqualTo(Enumerable.Range(0, totalCount).Select(index => $"entry-{index}")));
            Assert.That(result.Count_Current, Is.EqualTo(totalCount));
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void RetrieveAll_PropagatesRequestFailureWithoutWrappingIt()
        {
            var failure = new HttpRequestException("diagnostic buffer request failed");
            ExpectBrowse(50).Throw(failure);

            var exception = Assert.Throws<HttpRequestException>(() =>
                _retriever.RetrieveAll(new CultureInfo("en-US")));

            Assert.That(exception, Is.SameAs(failure));
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void ServiceFactory_CreatesRetrieverUsingProvidedRequestHandler()
        {
            IApiServiceFactory factory = new ApiStandardServiceFactory();
            var handler = new ApiHttpClientRequestHandler(_client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var retriever = factory.GetDiagnosticBufferRetriever(handler);
            ExpectBrowse(50).Respond("application/json", BuildResponse(1, 1));

            var result = retriever.RetrieveAll(new CultureInfo("en-US"));

            Assert.That(result.Entries.Single().Short_Text, Is.EqualTo("entry-0"));
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(49)]
        [TestCase(50)]
        [TestCase(51)]
        [TestCase(100)]
        [TestCase(123)]
        public async Task RetrieveAllAsync_ReturnsEveryEntryInResponseOrder(int totalCount)
        {
            ExpectBrowse(50).Respond("application/json", BuildResponse(Math.Min(50, totalCount), totalCount));
            if (totalCount > 50)
            {
                ExpectBrowse(totalCount).Respond("application/json", BuildResponse(totalCount, totalCount));
            }

            var result = await _retriever.RetrieveAllAsync(new CultureInfo("en-US"));

            Assert.That(result.Entries.Select(entry => entry.Short_Text),
                Is.EqualTo(Enumerable.Range(0, totalCount).Select(index => $"entry-{index}")));
            Assert.That(result.Count_Current, Is.EqualTo(totalCount));
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task RetrieveAllAsync_DoesNotSpliceResponsesWhenTimestampPrecisionIsLost()
        {
            ExpectBrowse(50).Respond("application/json", BuildResponse(50, 75, "2026-09-27T00:00:00.514678521Z"));
            ExpectBrowse(75).Respond("application/json", BuildResponse(75, 75, "2026-09-27T00:00:00.514678531Z", 1));

            var result = await _retriever.RetrieveAllAsync(new CultureInfo("en-US"));

            Assert.That(result.Entries.Select(entry => entry.Short_Text),
                Is.EqualTo(Enumerable.Range(1, 75).Select(index => $"entry-{index}")));
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [TestCase(74, LastModified)]
        [TestCase(76, LastModified)]
        [TestCase(75, "2026-09-27T00:01:00Z")]
        public void RetrieveAllAsync_ThrowsWhenBufferChangeIsDetected(int changedCount, string lastModified)
        {
            ExpectBrowse(50).Respond("application/json", BuildResponse(50, 75));
            ExpectBrowse(75).Respond("application/json", BuildResponse(Math.Min(75, changedCount), changedCount, lastModified));

            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _retriever.RetrieveAllAsync(new CultureInfo("en-US")));

            Assert.That(exception.Message, Does.Contain("changed"));
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [TestCase(0)]
        [TestCase(50)]
        [TestCase(74)]
        [TestCase(76)]
        public void RetrieveAllAsync_RejectsIncompleteOrOversizedFinalResponse(int returnedCount)
        {
            ExpectBrowse(50).Respond("application/json", BuildResponse(50, 75));
            ExpectBrowse(75).Respond("application/json", BuildResponse(returnedCount, 75));

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _retriever.RetrieveAllAsync(new CultureInfo("en-US")));
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [TestCase(0, -1)]
        [TestCase(50, 3201)]
        [TestCase(49, 75)]
        [TestCase(1, 0)]
        public void RetrieveAllAsync_RejectsInvalidInitialResponse(int returnedCount, int totalCount)
        {
            ExpectBrowse(50).Respond("application/json", BuildResponse(returnedCount, totalCount));

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _retriever.RetrieveAllAsync(new CultureInfo("en-US")));
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void RetrieveAllAsync_PropagatesRequestFailure()
        {
            ExpectBrowse(50).Throw(new HttpRequestException("diagnostic buffer request failed"));

            Assert.ThrowsAsync<HttpRequestException>(async () =>
                await _retriever.RetrieveAllAsync(new CultureInfo("en-US")));
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void RetrieveAllAsync_PropagatesCancellationBetweenRequests()
        {
            using (var cancellation = new CancellationTokenSource())
            {
                ExpectBrowse(50).Respond(() =>
                {
                    cancellation.Cancel();
                    return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    {
                        Content = new StringContent(BuildResponse(50, 75))
                    });
                });

                Assert.CatchAsync<OperationCanceledException>(async () =>
                    await _retriever.RetrieveAllAsync(new CultureInfo("en-US"), cancellationToken: cancellation.Token));
                _mockHttp.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public async Task RetrieveAllAsync_ForwardsLanguageAndAttributeFiltersOnBothRequests()
        {
            var filters = new ApiDiagnosticBuffer_RequestFilters
            {
                Mode = ApiBrowseFilterMode.Include,
                Attributes = new List<ApiDiagnosticBufferBrowseFilterAttributes> { ApiDiagnosticBufferBrowseFilterAttributes.ShortText }
            };
            foreach (var count in new[] { 50, 75 })
            {
                ExpectBrowse(count)
                    .WithPartialContent("\"language\":\"de-DE\"")
                    .WithPartialContent("\"filters\":{\"mode\":\"include\",\"attributes\":[\"short_text\"]}")
                    .Respond("application/json", BuildResponse(count, 75));
            }

            await _retriever.RetrieveAllAsync(new CultureInfo("de-DE"), filters);
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        private MockedRequest ExpectBrowse(int count) => _mockHttp.Expect(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
            .WithPartialContent($"\"count\":{count}");

        private static string BuildResponse(int returnedCount, int totalCount, string lastModified = LastModified, int firstEntry = 0)
        {
            var entries = new JArray(Enumerable.Range(firstEntry, returnedCount).Select(index =>
                new JObject
                {
                    ["timestamp"] = $"2026-09-27T00:{index / 60:00}:{index % 60:00}Z",
                    ["status"] = "incoming",
                    ["long_text"] = $"entry-{index}",
                    ["short_text"] = $"entry-{index}",
                    ["help_text"] = string.Empty,
                    ["event"] = new JObject { ["textlist_id"] = 1, ["text_id"] = index }
                }));
            return new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = "test",
                ["result"] = new JObject
                {
                    ["entries"] = entries,
                    ["last_modified"] = lastModified,
                    ["count_current"] = totalCount,
                    ["count_max"] = 3200,
                    ["language"] = "en-US"
                }
            }.ToString(Formatting.None);
        }
    }
}
