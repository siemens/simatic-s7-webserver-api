// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.Services.Ticketing;
using Siemens.Simatic.S7.Webserver.API.Services.WebApp;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class ApiResourceHandlerTests : Base
    {
        [Test]
        public void T001_DeployResourceAsync_RejectsPathTraversalInResourceName()
        {
            var webAppRoot = Path.Combine(Path.GetTempPath(), "s7webapi-resource-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(webAppRoot);

            var parentEscapeFile = Path.Combine(Directory.GetParent(webAppRoot).FullName, "escape.txt");
            File.WriteAllText(parentEscapeFile, "should not be uploaded");

            var webApp = new ApiWebAppData
            {
                Name = "testapp",
                PathToWebAppDirectory = webAppRoot
            };

            var resource = new ApiWebAppResource
            {
                Name = "../escape.txt",
                Media_type = "text/plain",
                Last_modified = DateTime.UtcNow,
                Visibility = ApiWebAppResourceVisibility.Public
            };

            try
            {
                var mockHttp = new MockHttpMessageHandler();
                var client = new HttpClient(mockHttp)
                {
                    BaseAddress = new Uri($"https://{Ip}")
                };

                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var ticketHandler = new ApiTicketHandler(requestHandler)
                {
                    CheckAfterUpload = false
                };
                var resourceHandler = new ApiResourceHandler(requestHandler, new ApiWebAppResourceBuilder(), ticketHandler);

                Assert.ThrowsAsync<IOException>(async () => await resourceHandler.DeployResourceAsync(webApp, resource));
            }
            finally
            {
                if (File.Exists(parentEscapeFile))
                {
                    File.Delete(parentEscapeFile);
                }

                if (Directory.Exists(webAppRoot))
                {
                    Directory.Delete(webAppRoot, true);
                }
            }
        }

        [Test]
        public async Task T002_DeployResourceAsync_UploadsContainedResource()
        {
            var webAppRoot = Path.Combine(Path.GetTempPath(), "s7webapi-resource-tests", Guid.NewGuid().ToString("N"));
            var nestedDir = Path.Combine(webAppRoot, "assets");
            Directory.CreateDirectory(nestedDir);
            var resourceFilePath = Path.Combine(nestedDir, "index.html");
            File.WriteAllText(resourceFilePath, "<html>ok</html>");

            var webApp = new ApiWebAppData
            {
                Name = "testapp",
                PathToWebAppDirectory = webAppRoot
            };

            var resource = new ApiWebAppResource
            {
                Name = "assets/index.html",
                Media_type = "text/html",
                Last_modified = DateTime.UtcNow,
                Visibility = ApiWebAppResourceVisibility.Public
            };

            try
            {
                var ticketId = "1234567890123456789012345678";
                var mockHttp = new MockHttpMessageHandler();

                mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                    .Respond(request =>
                    {
                        var requestBody = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        if (requestBody.Contains("WebApp.CreateResource"))
                        {
                            return new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent($"{{\"jsonrpc\":\"2.0\",\"id\":\"x\",\"result\":\"{ticketId}\"}}")
                            };
                        }

                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(ResponseStrings.TrueOnSuccess)
                        };
                    });

                mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/ticket?id={ticketId}")
                    .Respond(HttpStatusCode.OK);

                var client = new HttpClient(mockHttp)
                {
                    BaseAddress = new Uri($"https://{Ip}")
                };

                var requestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
                var ticketHandler = new ApiTicketHandler(requestHandler)
                {
                    CheckAfterUpload = false
                };
                var resourceHandler = new ApiResourceHandler(requestHandler, new ApiWebAppResourceBuilder(), ticketHandler);

                await resourceHandler.DeployResourceAsync(webApp, resource);
            }
            finally
            {
                if (Directory.Exists(webAppRoot))
                {
                    Directory.Delete(webAppRoot, true);
                }
            }
        }
    }
}
