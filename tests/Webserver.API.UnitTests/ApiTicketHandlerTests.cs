// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.Services.Ticketing;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class ApiTicketHandlerTests : Base
    {
        [Test]
        public async Task T001_HandleDownloadAsync_SanitizesServerFilenameTraversal()
        {
            const string ticketId = "1234567890123456789012345678";
            var tempRoot = Path.Combine(Path.GetTempPath(), "s7webapi-ticket-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);
            var escapedPath = Path.Combine(Directory.GetParent(tempRoot).FullName, "escaped.txt");

            try
            {
                var handler = CreateTicketHandlerWithResponse(ticketId, "payload", "\"..\\escaped.txt\"");
                var result = await handler.HandleDownloadAsync(ticketId, pathToDownloadDirectory: tempRoot, fileName: null, fileExtension: null, overwriteExistingFile: false);

                Assert.That(File.Exists(result.File_Downloaded.FullName), Is.True);
                Assert.That(result.File_Downloaded.Name, Is.EqualTo("escaped.txt"));
                Assert.That(result.File_Downloaded.FullName.StartsWith(Path.GetFullPath(tempRoot), StringComparison.OrdinalIgnoreCase), Is.True);
                Assert.That(File.Exists(escapedPath), Is.False, "Traversal must not write outside the selected directory.");
            }
            finally
            {
                if (File.Exists(escapedPath))
                {
                    File.Delete(escapedPath);
                }

                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
        }

        [Test]
        public async Task T002_HandleDownloadAsync_UsesContainedPathForServerFilename()
        {
            const string ticketId = "1234567890123456789012345678";
            const string expectedPayload = "controlled content";
            var tempRoot = Path.Combine(Path.GetTempPath(), "s7webapi-ticket-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);

            try
            {
                var handler = CreateTicketHandlerWithResponse(ticketId, expectedPayload, "\"folder/valid.txt\"");

                var result = await handler.HandleDownloadAsync(ticketId, pathToDownloadDirectory: tempRoot, fileName: null, fileExtension: null, overwriteExistingFile: false);

                Assert.That(result.File_Downloaded.FullName.StartsWith(Path.GetFullPath(tempRoot), StringComparison.OrdinalIgnoreCase), Is.True);
                Assert.That(File.Exists(result.File_Downloaded.FullName), Is.True);
                Assert.That(await File.ReadAllTextAsync(result.File_Downloaded.FullName), Is.EqualTo(expectedPayload));
            }
            finally
            {
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
        }

        [Test]
        public void T003_HandleDownloadAsync_RejectsCallerProvidedTraversalFilename()
        {
            const string ticketId = "1234567890123456789012345678";
            var tempRoot = Path.Combine(Path.GetTempPath(), "s7webapi-ticket-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);
            var escapedPath = Path.Combine(Directory.GetParent(tempRoot).FullName, "caller-escape.txt");

            try
            {
                var handler = CreateTicketHandlerWithResponse(ticketId, "payload", "\"safe.txt\"");

                Assert.ThrowsAsync<IOException>(async () => await handler.HandleDownloadAsync(ticketId, pathToDownloadDirectory: tempRoot, fileName: "..\\caller-escape.txt", fileExtension: null, overwriteExistingFile: false));
                Assert.That(File.Exists(escapedPath), Is.False);
            }
            finally
            {
                if (File.Exists(escapedPath))
                {
                    File.Delete(escapedPath);
                }

                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
        }

        private ApiTicketHandler CreateTicketHandlerWithResponse(string ticketId, string payload, string serverSuggestedFileName)
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/ticket?id={ticketId}")
                .Respond(_ =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(Encoding.UTF8.GetBytes(payload))
                    };

                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = serverSuggestedFileName
                    };

                    return response;
                });

            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess);

            var client = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri($"https://{Ip}")
            };

            var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            return new ApiTicketHandler(requestHandler)
            {
                CheckAfterDownload = false
            };
        }
    }
}
