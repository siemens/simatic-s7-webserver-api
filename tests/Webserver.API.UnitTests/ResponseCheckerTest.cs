// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Net;
using System.Net.Http;

namespace Webserver.API.UnitTests
{
    public class ResponseCheckerTest : Base
    {
        [Test]
        public void ValidResponse_ErrorInName_NoExceptionThrown()
        {
            ApiResponseChecker.CheckResponseStringForErros(ResponseStrings.PlcProgramBrowseAll, "{\"method\":\"PlcProgram.Browse\",\"jsonrpc\":\"2.0\",\"id\":\"c02cvuwa\",\"params\":{\"var\":\"\"DataTypes\"\",\"mode\":\"children\"}}");
            ApiResponseChecker.CheckResponseStringForErros(ResponseStrings.PlcProgramBrowseErrorStruct, "{\"method\":\"PlcProgram.Browse\",\"jsonrpc\":\"2.0\",\"id\":\"ibf8wom\",\"params\":{\"var\":\"\"DataTypes\".ErrorStr\",\"mode\":\"children\"}}");
        }

        [Test]
        public void HttpError_ForPlcRestoreBackup_ExceptionMessageDoesNotContainRestorePassword()
        {
            const string secret = "PLC-restore-secret-7Y!";
            var serializedRequest = ApiRequestFactory.GetPlcRestoreBackupRequest(secret).ToString();

            using (var response = new HttpResponseMessage(HttpStatusCode.BadRequest){ReasonPhrase = "test failure"})
            {
                var ex = Assert.Throws<InvalidHttpRequestException>(() =>
                ApiResponseChecker.CheckHttpResponseForErrors(response, serializedRequest));

                Assert.That(ex, Is.Not.Null);
                Assert.That(ex.Message.IndexOf(secret, StringComparison.Ordinal) < 0, Is.True,
                    $"Exception message must not contain restore password: {secret}, message was: {ex.Message}");
            }
        }

        [Test]
        public void RestoreBackup_WithHttpBadRequest_DoesNotExposePasswordInExceptionChain()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond(HttpStatusCode.BadRequest, "application/json", "{\"error\":\"test failure\"}");
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var password = "ThisIsThePasswordToBeUsed";
            var exc = Assert.ThrowsAsync<InvalidHttpRequestException>(async () => await TestHandler.PlcRestoreBackupAsync(password));
            Assert.Multiple(() =>
            {
                Assert.That(!exc.Message.Contains(password), $"Password has been provided in the exception thrown!{Environment.NewLine}{exc.Message}");
                var currentExc = exc.InnerException;
                while (currentExc != null)
                {
                    Assert.That(!currentExc.Message.Contains(password), $"Password has been provided in the exception thrown!{Environment.NewLine}{currentExc.Message}");
                    currentExc = currentExc.InnerException;
                }
            });
        }
    }
}
