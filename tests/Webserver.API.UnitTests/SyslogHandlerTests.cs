// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Services;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.Services.Syslog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class SyslogHandlerTests : Base
    {
        [Test]
        public void NullRequestHandler_IsRejected()
            => Assert.Throws<ArgumentNullException>(() => new SyslogHandler(null));

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(20)]
        [TestCase(21)]
        [TestCase(39)]
        [TestCase(40)]
        [TestCase(123)]
        public async Task RetrieveAllAsync_TraversesUnwrappedBuffer(int size)
        {
            using (var s = new Session(this, Ring((uint)size, size, 0)))
            {
                var result = await s.Handler.RetrieveAllAsync();
                Assert.That(result.Entries.Select(e => e.Raw),
                    Is.EqualTo(Enumerable.Range(0, size).Select(i => "entry-" + (size - i))));
                Assert.That(result.Count_Total, Is.EqualTo(size));
                Assert.That(result.Count_Lost, Is.Zero);
                Assert.That(s.Requests[0]["first"], Is.Null);
                Assert.That(s.Requests.All(p => p["first"] == null || (uint)p["first"] > 0), Is.True);
            }
        }

        [TestCase(20)]
        [TestCase(1)]
        public void RetrieveAllAsync_AnchorOnlyBeforeOldestBoundary_FailsWithoutPartialSuccess(int pageLimit)
        {
            int calls = 0;
            var ring = Ring(100, 53, 0, pageLimit);
            using (var s = new Session(this, async (p, ct) =>
            {
                Assert.That(++calls, Is.LessThanOrEqualTo(2), "A no-progress response must not be retried.");
                var r = await ring(p, ct);
                if (calls == 2)
                    r["result"]["entries"] = new JArray(r["result"]["entries"][0].DeepClone());
                return r;
            }))
            {
                var ex = Assert.ThrowsAsync<InvalidOperationException>(() => s.Handler.RetrieveAllAsync());
                Assert.That(ex.Message, Does.Contain("no progress"));
                Assert.That(calls, Is.EqualTo(2));
                Assert.That((uint)s.Requests[1]["first"], Is.EqualTo(101u - (uint)pageLimit));
            }
        }

        [TestCase(0u)]
        [TestCase(10u)]
        public void RetrieveAllAsync_WrappedBuffer_UnprovenBoundaryFailsClosed(uint lost)
        {
            // Oldest 20 events were overwritten; some/all were already saved to a server.
            using (var s = new Session(this, Ring(123, 103, lost)))
            {
                var ex = Assert.ThrowsAsync<InvalidOperationException>(() => s.Handler.RetrieveAllAsync());
                Assert.That(ex.Message, Does.Contain("no progress"));
                Assert.That(s.Requests.Select(p => (uint?)p["first"]),
                    Is.EqualTo(new uint?[] { null, 104, 85, 66, 47, 28, 21 }));
            }
        }

        [Test]
        public async Task RetrieveAllAsync_FullyLostPrefix_StopsAtProvenOldestBoundary()
        {
            using (var s = new Session(this, Ring(100, 53, 47)))
            {
                var result = await s.Handler.RetrieveAllAsync();
                Assert.That(result.Entries.Select(e => e.Raw),
                    Is.EqualTo(Enumerable.Range(48, 53).Reverse().Select(id => "entry-" + id)));
                Assert.That(result.Count_Total, Is.EqualTo(100));
                Assert.That(result.Count_Lost, Is.EqualTo(47));
                Assert.That(s.Requests.Select(p => (uint?)p["first"]),
                    Is.EqualTo(new uint?[] { null, 81, 62 }));
            }
        }

        [TestCase(2)]
        [TestCase(7)]
        [TestCase(20)]
        public async Task RetrieveAllAsync_ShortPages_ContinueUntilProvenOldestBoundary(int pageLimit)
        {
            using (var s = new Session(this, Ring(53, 53, 0, pageLimit)))
            {
                var result = await s.Handler.RetrieveAllAsync();
                Assert.That(result.Entries.Select(e => e.Raw),
                    Is.EqualTo(Enumerable.Range(1, 53).Reverse().Select(id => "entry-" + id)));
                Assert.That(s.Requests.Skip(1).All(p => (uint)p["first"] > 1), Is.True);
            }
        }

        [TestCase(2147483648u, 0u)]
        [TestCase(uint.MaxValue, 0u)]
        [TestCase(uint.MaxValue, 2147483648u)]
        public void RetrieveAllAsync_LargeCumulativeCounters_DoNotOverflowAtUnprovenBoundary(uint total, uint lost)
        {
            using (var s = new Session(this, Ring(total, 53, lost)))
            {
                var ex = Assert.ThrowsAsync<InvalidOperationException>(() => s.Handler.RetrieveAllAsync());
                Assert.That(ex.Message, Does.Contain("no progress"));
                Assert.That(s.Requests.Select(p => (uint?)p["first"]),
                    Is.EqualTo(new uint?[] { null, total - 19, total - 38, total - 52 }));
            }
        }

        [TestCase(2147483648u)]
        [TestCase(uint.MaxValue)]
        public async Task RetrieveAllAsync_LargeCumulativeCounters_CompleteAtProvenBoundary(uint total)
        {
            using (var s = new Session(this, Ring(total, 53, total - 53)))
            {
                var result = await s.Handler.RetrieveAllAsync();
                Assert.That(result.Entries.Count, Is.EqualTo(53));
                Assert.That(result.Entries.First().Raw, Is.EqualTo("entry-" + total));
                Assert.That(result.Entries.Last().Raw, Is.EqualTo("entry-" + (total - 52)));
                Assert.That(result.Count_Total, Is.EqualTo(total));
                Assert.That(result.Count_Lost, Is.EqualTo(total - 53));
                Assert.That(s.Requests.Count, Is.EqualTo(3));
            }
        }

        [Test]
        public async Task RetrieveAllAsync_IdenticalRawValues_AreNotDeduplicated()
        {
            using (var s = new Session(this, Ring(100, 43, 57, raw: id => "same event")))
            {
                var result = await s.Handler.RetrieveAllAsync();
                Assert.That(result.Entries.Count, Is.EqualTo(43));
                Assert.That(result.Entries.All(e => e.Raw == "same event"), Is.True);
            }
        }

        [TestCase(0u)]
        [TestCase(10u)]
        [TestCase(uint.MaxValue)]
        public async Task RetrieveAllAsync_EmptyRetainedBuffer_PreservesCounters(uint total)
        {
            using (var s = new Session(this, Ring(total, 0, total)))
            {
                var result = await s.Handler.RetrieveAllAsync();
                Assert.That(result.Entries, Is.Empty);
                Assert.That(result.Count_Total, Is.EqualTo(total));
                Assert.That(result.Count_Lost, Is.EqualTo(total));
                Assert.That(s.Requests.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void RetrieveAllAsync_EmptyInitialPageWithoutEmptyBufferProof_FailsClosed()
        {
            using (var s = new Session(this, (p, ct) =>
                Task.FromResult(Response(100, 0, Enumerable.Empty<string>()))))
            {
                var ex = Assert.ThrowsAsync<InvalidOperationException>(() => s.Handler.RetrieveAllAsync());
                Assert.That(ex.Message, Does.Contain("no progress"));
                Assert.That(s.Requests.Count, Is.EqualTo(1));
            }
        }

        [TestCase(1)]
        [TestCase(20)]
        public async Task RetrieveAllAsync_IdOneBoundary_DoesNotRequestFirstZero(int pageLimit)
        {
            using (var s = new Session(this, Ring(1, 1, 0, pageLimit)))
            {
                var result = await s.Handler.RetrieveAllAsync();
                Assert.That(result.Entries.Select(e => e.Raw), Is.EqualTo(new[] { "entry-1" }));
                Assert.That(s.Requests.Count, Is.EqualTo(1));
                Assert.That(s.Requests[0]["first"], Is.Null);
            }
        }

        [TestCase(ApiPlcRedundancyId.StandardPLC)]
        [TestCase(ApiPlcRedundancyId.RedundancyId_1)]
        [TestCase(ApiPlcRedundancyId.RedundancyId_2)]
        public async Task RetrieveAllAsync_RedundancyId_IsPassedOnEveryPage(ApiPlcRedundancyId redundancyId)
        {
            using (var s = new Session(this, Ring(50, 43, 7)))
            {
                await s.Handler.RetrieveAllAsync(redundancyId);
                foreach (var p in s.Requests)
                {
                    if (redundancyId == ApiPlcRedundancyId.StandardPLC)
                        Assert.That(p["redundancy_id"], Is.Null);
                    else Assert.That((int)p["redundancy_id"], Is.EqualTo((int)redundancyId));
                }
            }
        }

        [TestCase(101u, 0u)] // append only
        [TestCase(105u, 5u)] // overwrite unforwarded
        [TestCase(105u, 0u)] // overwrite forwarded: lost unchanged
        [TestCase(100u, 5u)]
        [TestCase(1u, 0u)] // reset or counter wrap
        public void RetrieveAllAsync_ChangedCounters_FailWithoutPartialSuccess(uint total, uint lost)
        {
            int calls = 0;
            var ring = Ring(100, 100, 0);
            using (var s = new Session(this, async (p, ct) =>
            {
                var r = await ring(p, ct);
                if (++calls == 2) { r["result"]["count_total"] = total; r["result"]["count_lost"] = lost; }
                return r;
            }))
            {
                var ex = Assert.ThrowsAsync<InvalidOperationException>(() => s.Handler.RetrieveAllAsync());
                Assert.That(ex.Message, Does.Contain("changed"));
                Assert.That(calls, Is.EqualTo(2));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RetrieveAllAsync_MissingOrWrongAnchor_IsRejected(bool empty)
        {
            int calls = 0;
            var ring = Ring(100, 100, 0);
            using (var s = new Session(this, async (p, ct) =>
            {
                var r = await ring(p, ct);
                if (++calls == 2) r["result"]["entries"] = empty ? new JArray() :
                    new JArray(new JObject { ["raw"] = "wrong entry" });
                return r;
            }))
            {
                var ex = Assert.ThrowsAsync<InvalidOperationException>(() => s.Handler.RetrieveAllAsync());
                Assert.That(ex.Message, Does.Contain("continuation entry"));
                Assert.That(calls, Is.EqualTo(2));
            }
        }

        [TestCase("missing-total", typeof(JsonSerializationException))]
        [TestCase("missing-lost", typeof(JsonSerializationException))]
        [TestCase("negative-total", typeof(JsonSerializationException))]
        [TestCase("overflow-total", typeof(JsonSerializationException))]
        [TestCase("lost-exceeds-total", typeof(InvalidOperationException))]
        [TestCase("zero-total-with-entries", typeof(InvalidOperationException))]
        [TestCase("all-lost-with-entries", typeof(InvalidOperationException))]
        [TestCase("missing-entries", typeof(InvalidOperationException))]
        [TestCase("null-entries", typeof(InvalidOperationException))]
        [TestCase("null-entry", typeof(InvalidOperationException))]
        [TestCase("null-raw", typeof(InvalidOperationException))]
        [TestCase("too-many-entries", typeof(InvalidOperationException))]
        [TestCase("missing-result", typeof(InvalidOperationException))]
        public void RetrieveAllAsync_MalformedResponse_IsRejected(string defect, Type exception)
        {
            var r = Response(100, 0, new[] { "entry-100" });
            var result = (JObject)r["result"];
            switch (defect)
            {
                case "missing-total": result.Remove("count_total"); break;
                case "missing-lost": result.Remove("count_lost"); break;
                case "negative-total": result["count_total"] = -1; break;
                case "overflow-total": result["count_total"] = 4294967296L; break;
                case "lost-exceeds-total": result["count_lost"] = 101; break;
                case "zero-total-with-entries": result["count_total"] = 0; break;
                case "all-lost-with-entries": result["count_lost"] = 100; break;
                case "missing-entries": result.Remove("entries"); break;
                case "null-entries": result["entries"] = null; break;
                case "null-entry": result["entries"] = new JArray(JValue.CreateNull()); break;
                case "null-raw": result["entries"][0]["raw"] = null; break;
                case "too-many-entries":
                    result["entries"] = new JArray(Enumerable.Range(0, 21).Select(i => new JObject { ["raw"] = "entry-" + i }));
                    break;
                case "missing-result": r.Remove("result"); break;
            }
            using (var s = new Session(this, (p, ct) => Task.FromResult(r)))
                Assert.ThrowsAsync(exception, () => s.Handler.RetrieveAllAsync());
        }

        [Test]
        public void RetrieveAllAsync_PreCanceled_DoesNotSendRequest()
        {
            using (var cts = new CancellationTokenSource())
            using (var s = new Session(this, Ring(100, 100, 0)))
            {
                cts.Cancel();
                Assert.ThrowsAsync<OperationCanceledException>(() => s.Handler.RetrieveAllAsync(cancellationToken: cts.Token));
                Assert.That(s.Requests, Is.Empty);
            }
        }

        [Test]
        public async Task RetrieveAllAsync_InFlightCancellation_StopsRequest()
        {
            var entered = new TaskCompletionSource<bool>();
            using (var cts = new CancellationTokenSource())
            using (var s = new Session(this, async (p, ct) =>
            {
                entered.SetResult(true);
                await Task.Delay(Timeout.Infinite, ct);
                return null;
            }))
            {
                var task = s.Handler.RetrieveAllAsync(cancellationToken: cts.Token);
                await entered.Task;
                cts.Cancel();
                try
                {
                    await task;
                    Assert.Fail("The in-flight request was not canceled.");
                }
                catch (OperationCanceledException)
                {
                    Assert.That(task.IsCanceled, Is.True);
                }
                Assert.That(s.Requests.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void RetrieveAllAsync_CanceledBetweenPages_DoesNotRequestNextPage()
        {
            using (var cts = new CancellationTokenSource())
            {
                var ring = Ring(100, 100, 0);
                using (var s = new Session(this, async (p, ct) =>
                {
                    var r = await ring(p, ct);
                    cts.Cancel();
                    return r;
                }))
                {
                    Assert.CatchAsync<OperationCanceledException>(() => s.Handler.RetrieveAllAsync(cancellationToken: cts.Token));
                    Assert.That(s.Requests.Count, Is.EqualTo(1));
                }
            }
        }

        [Test]
        public void RetrieveAllAsync_RequestFailure_IsNotWrapped()
        {
            var expected = new HttpRequestException("test failure");
            using (var s = new Session(this, (p, ct) => Task.FromException<JObject>(expected)))
                Assert.That(Assert.ThrowsAsync<HttpRequestException>(() => s.Handler.RetrieveAllAsync()), Is.SameAs(expected));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void RetrieveAll_SingleThreadContext_DoesNotDeadlockOrWrapExceptions(bool fail)
        {
            var context = new QueuedContext();
            var expected = new HttpRequestException("test failure");
            var ring = Ring(43, 43, 0);
            using (var s = new Session(this, async (p, ct) =>
            {
                await Task.Yield(); // Force the HTTP/request-handler call chain to suspend.
                if (fail) throw expected;
                return await ring(p, ct);
            }))
            using (var finished = new ManualResetEventSlim())
            {
                Exception actual = null;
                int count = -1;
                var thread = new Thread(() =>
                {
                    SynchronizationContext.SetSynchronizationContext(context);
                    try { count = s.Handler.RetrieveAll().Entries.Count; }
                    catch (Exception ex) { actual = ex; }
                    finally { finished.Set(); }
                }) { IsBackground = true };
                thread.Start();
                bool completedWithoutPumping = finished.Wait(TimeSpan.FromSeconds(5));
                // Recover a regression without leaving a blocked foreground thread.
                if (!completedWithoutPumping)
                    for (int i = 0; i < 100 && !finished.IsSet; i++) { context.Drain(); finished.Wait(10); }
                thread.Join(1000);
                Assert.That(completedWithoutPumping, Is.True, "The call required its blocked caller context.");
                if (fail) Assert.That(actual, Is.SameAs(expected));
                else { Assert.That(actual, Is.Null); Assert.That(count, Is.EqualTo(43)); }
            }
        }

        [Test]
        public void ServiceFactory_ExposesHandler()
        {
            using (var s = new Session(this, Ring(0, 0, 0)))
            {
                IApiServiceFactory factory = new ApiStandardServiceFactory();
                Assert.That(factory.GetSyslogHandler(s.RequestHandler), Is.InstanceOf<SyslogHandler>());
            }
        }

        // Independent ring model: occupancy and lost are separate inputs. Unknown IDs
        // are rejected so retrieval cannot rely on unspecified out-of-range behavior.
        private static Func<JObject, CancellationToken, Task<JObject>> Ring(
            uint total, int retained, uint lost, int pageLimit = 20, Func<uint, string> raw = null)
        {
            var ids = Enumerable.Range(0, retained).Select(i => total - (uint)i).ToList();
            return (p, ct) =>
            {
                uint? first = (uint?)p["first"];
                uint count = (uint)p["count"];
                Assert.That(count, Is.InRange(1u, 20u));
                if (first.HasValue)
                {
                    Assert.That(first.Value, Is.Not.Zero);
                    Assert.That(ids, Does.Contain(first.Value), "Requested an ID outside the retained buffer.");
                }
                var selected = ids.Where(id => id <= (first ?? total)).Take(Math.Min((int)count, pageLimit))
                    .Select(id => raw == null ? "entry-" + id : raw(id));
                return Task.FromResult(Response(total, lost, selected));
            };
        }

        private static JObject Response(uint total, uint lost, IEnumerable<string> raw)
            => new JObject
            {
                ["jsonrpc"] = "2.0", ["id"] = "test",
                ["result"] = new JObject
                {
                    ["count_total"] = total, ["count_lost"] = lost,
                    ["entries"] = new JArray(raw.Select(value => new JObject { ["raw"] = value }))
                }
            };

        private sealed class Session : IDisposable
        {
            private readonly HttpClient client;
            public readonly List<JObject> Requests = new List<JObject>();
            public readonly ApiHttpClientRequestHandler RequestHandler;
            public readonly SyslogHandler Handler;
            public Session(Base owner, Func<JObject, CancellationToken, Task<JObject>> browse)
            {
                client = new HttpClient(new ScriptedHttpHandler(async (request, ct) =>
                {
                    var json = JObject.Parse(await request.Content.ReadAsStringAsync());
                    Assert.That((string)json["method"], Is.EqualTo("Syslog.Browse"));
                    var p = (JObject)json["params"];
                    Requests.Add(p);
                    var response = await browse(p, ct);
                    response["id"] = json["id"];
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    { Content = new StringContent(response.ToString(Formatting.None)) };
                })) { BaseAddress = new Uri("https://syslog.invalid") };
                RequestHandler = new ApiHttpClientRequestHandler(client, owner.ApiRequestFactory,
                    owner.ApiResponseChecker, owner.ApiRequestSplitter);
                Handler = new SyslogHandler(RequestHandler);
            }
            public void Dispose() => client.Dispose();
        }

        private sealed class ScriptedHttpHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send;
            public ScriptedHttpHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send)
                => this.send = send;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
                => send(request, ct);
        }

        private sealed class QueuedContext : SynchronizationContext
        {
            private readonly ConcurrentQueue<Action> callbacks = new ConcurrentQueue<Action>();
            public override void Post(SendOrPostCallback callback, object state) => callbacks.Enqueue(() => callback(state));
            public void Drain() { while (callbacks.TryDequeue(out var callback)) callback(); }
        }
    }
}
