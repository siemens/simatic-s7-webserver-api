// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer;
using Siemens.Simatic.S7.Webserver.API.Models.FailsafeParameters;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using Siemens.Simatic.S7.Webserver.API.Models.TimeSettings;
using Siemens.Simatic.S7.Webserver.API.Services.PlcProgram;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class ApiRequestTests : Base
    {
        [Test]
        public async Task T001_ApiBrowseRequest_ExpectedMethods()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBrowseResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var browseResExpectedList = new List<ApiClass>() { new ApiClass() { Name = "Api.Browse" }, new ApiClass() { Name = "Api.CloseTicket" }, new ApiClass() { Name = "Api.GetCertificateUrl" }, new ApiClass() { Name = "Api.GetPermissions" }, new ApiClass() { Name = "Api.BrowseTickets" }, new ApiClass() { Name = "Api.Login" }, new ApiClass() { Name = "Api.Logout" }, new ApiClass() { Name = "Api.Ping" }, new ApiClass() { Name = "Api.Version" }, new ApiClass() { Name = "Plc.ReadOperatingMode" }, new ApiClass() { Name = "Plc.RequestChangeOperatingMode" }, new ApiClass() { Name = "PlcProgram.Browse" }, new ApiClass() { Name = "PlcProgram.Read" }, new ApiClass() { Name = "PlcProgram.Write" }, new ApiClass() { Name = "WebApp.Browse" }, new ApiClass() { Name = "WebApp.BrowseResources" }, new ApiClass() { Name = "WebApp.Create" }, new ApiClass() { Name = "WebApp.CreateResource" }, new ApiClass() { Name = "WebApp.Delete" }, new ApiClass() { Name = "WebApp.DeleteResource" }, new ApiClass() { Name = "WebApp.DownloadResource" }, new ApiClass() { Name = "WebApp.Rename" }, new ApiClass() { Name = "WebApp.RenameResource" }, new ApiClass() { Name = "WebApp.SetDefaultPage" }, new ApiClass() { Name = "WebApp.SetNotAuthorizedPage" }, new ApiClass() { Name = "WebApp.SetNotFoundPage" }, new ApiClass() { Name = "WebApp.SetResourceETag" }, new ApiClass() { Name = "WebApp.SetResourceMediaType" }, new ApiClass() { Name = "WebApp.SetResourceModificationTime" }, new ApiClass() { Name = "WebApp.SetResourceVisibility" }, new ApiClass() { Name = "WebApp.SetState" } };
            var browseRes = (await TestHandler.ApiBrowseAsync()).Result;
            if (!browseResExpectedList.SequenceEqual(browseRes))
            {
                var unexpected = browseRes.Except(browseResExpectedList);
                var expectedButMissing = browseResExpectedList.Except(browseRes);
                string assertString = $"Api Browse result is not equal!unexpectedly found:{Environment.NewLine}";
                foreach (var res in unexpected)
                {
                    assertString += $"{res.Name}";
                }
                assertString += $"{Environment.NewLine}expected but did not find: {Environment.NewLine}";
                foreach (var res in expectedButMissing)
                {
                    assertString += $"{res.Name}";
                }
                Assert.Fail(assertString);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T002_01_ApiCloseTicketRequest_InvalidParams_AccExceptions()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InvalidParams); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiCloseTicketAsync(""));
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiCloseTicketAsync("abc"));
            string chars29 = "abcdefghijklmnopqrstuvwxyzabc";
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiCloseTicketAsync(chars29));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T002_02_ApiCloseTicketRequest_NoTicket_AccExceptions()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketNotFound); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string chars28 = "abcdefghijklmnopqrstuvwxyzab";
            Assert.ThrowsAsync<ApiTicketNotFoundException>(async () => await TestHandler.ApiCloseTicketAsync(chars28));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T002_03_ApiCloseTicketRequest_SysBusy_AccExceptions()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SystemIsBusy); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string chars28 = "abcdefghijklmnopqrstuvwxyzab";
            Assert.ThrowsAsync<ApiSystemIsBusyException>(async () => await TestHandler.ApiCloseTicketAsync(chars28));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T002_04_ApiCloseTicketRequest_TicketClosable_AccExceptions()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string chars28 = "abcdefghijklmnopqrstuvwxyzab";
            var resp = await TestHandler.ApiCloseTicketAsync(chars28);
            if (!resp.Result)
                Assert.Fail("correct server response but no successresponse with true!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T003_ApiGetCertificateUrl_NoSpecial_CertificateUrl()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.CertificateUrl); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string expString = "/MiniWebCA_Cer.crt";
            var resp = await TestHandler.ApiGetCertificateUrlAsync();
            if (resp.Result != expString)
                Assert.Fail("correct server response but not correctly given to user with true!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T004_01_ApiGetPermissions_Admin_AllPermissions()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.GetPermissionsAdmin); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            List<ApiClass> expRes = new List<ApiClass>() {
                new ApiClass() { Name = "read_diagnostics" },
                new ApiClass() { Name = "read_value" },
                new ApiClass() { Name = "write_value" },
                new ApiClass() { Name = "acknowledge_alarms" },
                new ApiClass() { Name = "open_user_pages" },
                new ApiClass() { Name = "read_file" },
                new ApiClass() { Name = "write_file" },
                new ApiClass() { Name = "change_operating_mode" },
                new ApiClass() { Name = "flash_leds" },
                new ApiClass() { Name = "backup_plc" },
                new ApiClass() { Name = "restore_plc" },
                new ApiClass() { Name = "failsafe_admin" }, //only when PLC is failsafe
                new ApiClass() { Name = "manage_user_pages" },
                new ApiClass() { Name = "update_firmware" },
                new ApiClass() { Name = "change_time_settings" },
                new ApiClass() { Name = "download_service_data" },
                new ApiClass() { Name = "change_webserver_default_page" },
                new ApiClass() { Name = "read_watch_table_value" },
                new ApiClass() { Name = "write_watch_table_value" },
                new ApiClass() { Name = "read_syslog" }
            };

            var resp = await TestHandler.ApiGetPermissionsAsync();
            Assert.That(resp.Result.SequenceEqual(expRes), $"Permission don't match, missing: {string.Join(",", expRes.Except(resp.Result))}{string.Join(",", resp.Result.Except(expRes))}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T004_02_ApiGetPermissions_None_NoPermissions()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.GetPermissionsNone); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            List<ApiClass> expRes = new List<ApiClass>();
            var resp = await TestHandler.ApiGetPermissionsAsync();
            Assert.That(resp.Result.SequenceEqual(expRes), $"Permission don't match, missing: {string.Join(",", expRes.Except(resp.Result))}{string.Join(",", resp.Result.Except(expRes))}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T005_01_ApiBrowseTickets_InvalidParams_AccException()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InvalidParams); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiBrowseTicketsAsync("abc"));
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiCloseTicketAsync("abc"));
            string chars29 = "abcdefghijklmnopqrstuvwxyzabc";
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiBrowseTicketsAsync(chars29));
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiCloseTicketAsync(chars29));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T005_01_02_ApiBrowseTickets_InvalidParams_ResponseWouldBeValidButExceptionIsThrownBecauseOfCheck()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusActive); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiBrowseTicketsAsync("abc"));
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiCloseTicketAsync("abc"));
            string chars29 = "abcdefghijklmnopqrstuvwxyzabc";
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiBrowseTicketsAsync(chars29));
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiCloseTicketAsync(chars29));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T005_02_ApiBrowseTickets_TicketNotFound_AccException()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketNotFound); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string chars28 = "abcdefghijklmnopqrstuvwxyzab";
            Assert.ThrowsAsync<ApiTicketNotFoundException>(async () => await TestHandler.ApiCloseTicketAsync(chars28));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T005_04_ApiBrowseTickets_TicketCreated_CreatedTicket()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusCreated); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string chars28 = "abcdefghijklmnopqrstuvwxyzab";
            var resp = (await TestHandler.ApiBrowseTicketsAsync(chars28)).Result.Tickets.First();
            if (resp.State != ApiTicketState.Created)
                Assert.Fail("correct response from server but not in given obj!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T005_05_ApiBrowseTickets_TicketActive_ActiveTicket()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusActive); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string chars28 = "abcdefghijklmnopqrstuvwxyzab";
            var resp = (await TestHandler.ApiBrowseTicketsAsync(chars28)).Result.Tickets.First();
            if (resp.State != ApiTicketState.Active)
                Assert.Fail("correct response from server but not in given obj!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T005_06_ApiBrowseTickets_TicketFailed_FailedTicket()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusFailed); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string chars28 = "abcdefghijklmnopqrstuvwxyzab";
            var resp = (await TestHandler.ApiBrowseTicketsAsync(chars28)).Result.Tickets.First();
            if (resp.State != ApiTicketState.Failed)
                Assert.Fail("correct response from server but not in given obj!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T005_07_ApiBrowseTickets_TicketCompleted_CompletedTicket()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusCompleted); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string chars28 = "abcdefghijklmnopqrstuvwxyzab";
            var resp = (await TestHandler.ApiBrowseTicketsAsync(chars28)).Result.Tickets.First();
            if (resp.State != ApiTicketState.Completed)
                Assert.Fail("correct response from server but not in given obj!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T005_08_ApiBrowseTickets_TicketCompleted_DateTimeParsingWorks()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusCompleted); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string chars28 = "abcdefghijklmnopqrstuvwxyzab";
            var resp = (await TestHandler.ApiBrowseTicketsAsync(chars28)).Result.Tickets.First();
            CultureInfo provider = CultureInfo.InvariantCulture;
            string dateCreated = "2020-12-09T14:48:40.670Z";
            var dateTime = DateTime.ParseExact(dateCreated, DateTimeFormatting.ApiDateTimeFormat, provider);
            if (resp.Date_created.ToString() != dateTime.ToString())
                Assert.Fail("correct response from server but not in given obj!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T005_09_ApiBrowseTickets_InvalidParamsResponseWouldBeFine_InvalidParamsExceptionFromCheck()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketNotFound); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string chars28 = "abcdefghijklmnopqrstuvwxyzab";
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiCloseTicketAsync(chars28 + "a"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T006_01_ApiBrowseTickets_NoTickets_EmptyArray()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.BrowseTicketsNoTickets); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var resp = (await TestHandler.ApiBrowseTicketsAsync()).Result.Tickets;
            if (resp.Count != 0)
                Assert.Fail("not an emptyList!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T006_02_ApiBrowseTickets_TwoTickets_ArrayOfTwo()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.BrowseTicketsTwoTickets); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var resp = (await TestHandler.ApiBrowseTicketsAsync()).Result.Tickets;
            if (resp.Count != 2)
                Assert.Fail("not an emptyList!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T007_01_ApiLogin_InvalidLogin_UnauthorizedExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.LoginFailed); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var req = ApiRequestFactory.GetApiLoginRequest("Everybody", "wrong");
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await TestHandler.SendPostRequestAsync(req));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T007_02_ApiLogin_AlreadyLoggedIn_UnauthorizedExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.AlreadyAuthenticated); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var req = ApiRequestFactory.GetApiLoginRequest("Everybody", "");
            Assert.ThrowsAsync<ApiAlreadyAuthenticatedException>(async () => await TestHandler.SendPostRequestAsync(req));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T007_03_ApiLogin_NoResources_NoResourcesExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.NoResources); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var req = ApiRequestFactory.GetApiLoginRequest("Everybody", "");
            Assert.ThrowsAsync<ApiNoResourcesException>(async () => await TestHandler.SendPostRequestAsync(req));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T007_04_ApiLogin_ValidToken_TokenReturnedNoCookie()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.LoginWorked); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var req = ApiRequestFactory.GetApiLoginRequest("Everybody", "");
            var res = JsonConvert.DeserializeObject<ApiLoginResponse>(await TestHandler.SendPostRequestAsync(req));
            if (string.IsNullOrEmpty(res.Result.Token))
                Assert.Fail("token is empty or null altough server returned with!");
            if (!string.IsNullOrEmpty(res.Result.Web_application_cookie))
                Assert.Fail("token is empty or null altough server returned with!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T007_05_ApiLogin_ValidToken_TokenReturnedAndCookie()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.LoginWorkedWithWebAppCookie); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var req = ApiRequestFactory.GetApiLoginRequest("Everybody", "", true);
            var res = JsonConvert.DeserializeObject<ApiLoginResponse>(await TestHandler.SendPostRequestAsync(req));
            if (string.IsNullOrEmpty(res.Result.Token))
                Assert.Fail("token is empty or null altough server returned with!");
            if (string.IsNullOrEmpty(res.Result.Web_application_cookie))
                Assert.Fail("token is empty or null altough server returned with!");
        }

        /// <summary>
        /// Unit test for Api.Login that returns every possible value
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T007_06_ApiLogin_ValidToken_WithEverything()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.LoginWorkedWithPasswordExpirationInformation); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var req = ApiRequestFactory.GetApiLoginRequest("Anonymous", "", true);
            var res = JsonConvert.DeserializeObject<ApiLoginResponse>(await TestHandler.SendPostRequestAsync(req));
            if (string.IsNullOrEmpty(res.Result.Token))
                Assert.Fail("token is empty or null altough server returned with!");
            if (string.IsNullOrEmpty(res.Result.Web_application_cookie))
                Assert.Fail("token is empty or null altough server returned with!");
            Assert.That(res.Result.Runtime_timeout == TimeSpan.FromMinutes(30),
                "Timeout should be 30 minutes for this response!");
            Assert.That(res.Result.Password_expiration != null,
                "Password expiration should be present!");
            Assert.That(res.Result.Password_expiration.Timestamp == new DateTime(2023, 7, 7, 14, 28, 3),
                "The timestamp of the expiration doesn't match!");
            Assert.That(!res.Result.Password_expiration.Warning);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T008_ApiLogout_LoggedIn_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.ApiLogoutAsync();
            if (!res.Result)
                Assert.Fail("True on success response not true!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T009_ApiPing_NoSpecial_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.ApiPingAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T010_ApiVersion_NoSpecial_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiVersionResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.ApiVersionAsync();
            if (res.Result > 3 || res.Result < 2)
                Assert.Fail("response string of ApiVersion bigger than 3 or smaller than 2!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T011_01_ApiPlcReadOperatingMode_run_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeRun); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.PlcReadOperatingModeAsync();
            if (res.Result != ApiPlcOperatingMode.Run)
                Assert.Fail("unexpected response:" + res.Result.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T011_02_ApiPlcReadOperatingMode_stop_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeStop); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.PlcReadOperatingModeAsync();
            if (res.Result != ApiPlcOperatingMode.Stop)
                Assert.Fail("unexpected response:" + res.Result.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T011_03_ApiPlcReadOperatingMode_startup_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeStartup); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.PlcReadOperatingModeAsync();
            if (res.Result != ApiPlcOperatingMode.Startup)
                Assert.Fail("unexpected response:" + res.Result.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T011_04_ApiPlcReadOperatingMode_hold_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeHold); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.PlcReadOperatingModeAsync();
            if (res.Result != ApiPlcOperatingMode.Hold)
                Assert.Fail("unexpected response:" + res.Result.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T011_05_ApiPlcReadOperatingMode_hold_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeStopFwUpdate); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.PlcReadOperatingModeAsync();
            if (res.Result != ApiPlcOperatingMode.Stop_fwupdate)
                Assert.Fail("unexpected response:" + res.Result.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T011_06_ApiPlcReadOperatingMode_unknown_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeInvUnknown); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<Newtonsoft.Json.JsonSerializationException>(async () => await TestHandler.PlcReadOperatingModeAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T011_07_ApiPlcReadOperatingMode_None_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeInvNone); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<Newtonsoft.Json.JsonSerializationException>(async () => await TestHandler.PlcReadOperatingModeAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T012_01_ApiRequestChangeOperatingMode_InvModeRespWouldBeSuccess_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode.Hold));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T012_02_ApiRequestChangeOperatingMode_InvModeRespWouldBeSuccess_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode.None));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T012_03_ApiRequestChangeOperatingMode_InvModeRespWouldBeSuccess_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode.Startup));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T012_04_ApiRequestChangeOperatingMode_InvModeRespWouldBeSuccess_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode.Stop_fwupdate));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T012_05_ApiRequestChangeOperatingMode_InvModeRespWouldBeSuccess_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode.Unknown));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T012_06_ApiRequestChangeOperatingMode_run_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            await TestHandler.PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode.Run);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T012_07_ApiRequestChangeOperatingMode_run_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            await TestHandler.PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode.Stop);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T013_01_ApiPlcProgramBrowse_InvModeRespWouldBeSuccess_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.None));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T013_02_ApiPlcProgramBrowse_AllBrowsed_ApplsResponded()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseAll); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var resp = await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.Children);
            if (resp.Result.Count == 0)
            {
                Assert.Fail("plcprogramdata result given but not to user!");
            }
            mockHttp.Flush();
            mockHttp.Clear();
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseDataTypes); // Respond with JSON    
            var dataTypesDb = resp.Result.First(el => el.Name == "DataTypes");
            if (dataTypesDb.Parents == null)
            {
                throw new Exception("Parents default to null!");
            }
            var dataTypesChildren = await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.Children, dataTypesDb);
            if (dataTypesDb.Children.Count != 0)
            {
                throw new Exception("Children have been modified!");
            }
            if (dataTypesChildren.Result.Any(el => el.Parents.Count != 0))
            {
                throw new Exception("Parents have been modified!");
            }
            var plcProgramHandler = new ApiPlcProgramHandler(TestHandler, ApiRequestFactory);
            var dataTypesChildrenWithInfo = await plcProgramHandler.PlcProgramBrowseSetChildrenAndParentsAsync(ApiPlcProgramBrowseMode.Children, dataTypesDb);
            if (dataTypesDb.Children.Count == 0)
            {
                throw new Exception("Children have not been added!");
            }
            if (dataTypesChildrenWithInfo.Result.Any(el => el.Parents.Count == 0))
            {
                throw new Exception("Parents have not been added!");
            }
            mockHttp.Flush();
            mockHttp.Clear();
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseDataTypesDTL); // Respond with JSON    
            var dataTypesChildDTL = dataTypesChildrenWithInfo.Result.First(el => el.Name == "DTL");
            var dtlChildren = (await plcProgramHandler.PlcProgramBrowseSetChildrenAndParentsAsync(ApiPlcProgramBrowseMode.Children, dataTypesChildDTL)).Result;
            if (dtlChildren.Any(el => el.Parents.Count != 2 || !el.Parents.Any(par => par.Name == "DataTypes") || !el.Parents.Any(par => par.Name == "DTL")))
            {
                throw new Exception("Parents have not been added as expected!");
            }
            if (dataTypesDb.Children.First(el => el.Name == "DTL").Children.Count != 8)
            {
                throw new Exception("Object links are not updated as expected!");
            }
            if (dataTypesChildDTL.Children.Count != 8)
            {
                throw new Exception("Object links are not updated as expected!");
            }
            dtlChildren = (await plcProgramHandler.PlcProgramBrowseSetChildrenAndParentsAsync(ApiPlcProgramBrowseMode.Children, dataTypesChildDTL)).Result;
            if (dtlChildren.Any(el => el.Parents.Count != 2 || !el.Parents.Any(par => par.Name == "DataTypes") || !el.Parents.Any(par => par.Name == "DTL")))
            {
                throw new Exception("Parents have not been added as expected!");
            }
            if (dataTypesDb.Children.First(el => el.Name == "DTL").Children.Count != 8)
            {
                throw new Exception("Object links are not updated as expected!");
            }
            if (dataTypesChildDTL.Children.Count != 8)
            {
                throw new Exception("Object links are not updated as expected!");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T013_03_ApiPlcProgramBrowse_VarIsNotAStructure_ExcThrown()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseVarIsNotAStructure); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiVariableIsNotAStructureException>(async () => await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.Children, "\"DataTypes\".\"Bool\""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T013_04_ApiPlcProgramBrowse_AddresDoesNotExist_ExcThrown()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramAddressDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiAddresDoesNotExistException>(async () => await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.Children, "\"DataTypes\".\"Boola\""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T013_05_ApiPlcProgramBrowse_InvalidAddress_ExcThrown()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramInvalidAddress); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidAddressException>(async () => await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.Children, "\"DataTypes\".\"Bool\"a"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T013_06_ApiPlcProgramBrowse_InvalidArrayIndex_ExcThrown()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramnInvalidArrayIndex); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidArrayIndexException>(async () => await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.Children, "\"DataTypes\".\"Bool\"a"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T015_ApiPlcProgramBrowse_PermissionDenied_ExcThrown()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PermissionDenied); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.Children, "\"DataTypes\".\"Bool\""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T016_01_ApiPlcProgramRead_PermissionDenied_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PermissionDenied); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await TestHandler.PlcProgramReadAsync<bool>("\"DataTypes\".\"Bool\""));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T016_02_ApiPlcProgramRead_UnsupportedAddress_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramUnsupportedAddress); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiUnsupportedAddressException>(async () => await TestHandler.PlcProgramReadAsync<object>("\"DataTypes\".\"Struct1L\""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T016_03_ApiPlcProgramRead_InternalError_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InternalError); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInternalErrorException>(async () => await TestHandler.PlcProgramReadAsync<object>("\"DataTypes\".\"Struct1L\""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T016_04_ApiPlcProgramRead_BoolIsFalse_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramReadFalseBool); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var resp = await TestHandler.PlcProgramReadAsync<object>("\"DataTypes\".\"Bool\"");
            if ((bool)resp.Result != false)
                Assert.Fail("not casted to \"false\" bool!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T016_05_ApiPlcProgramRead_BoolIsFalse_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramReadRawFalseBool); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var resp = await TestHandler.PlcProgramReadAsync<object>("\"DataTypes\".\"Bool\"");
            if (resp.Result is JArray)
            {
                var jarr = (JArray)resp.Result;
                var respRes = jarr.ToObject<List<bool>>();
                if (respRes[0] != false)
                    Assert.Fail("not casted to \"false\" bool!");
            }
            else
                Assert.Fail("raw mode returned sth else than jarray: " + resp.Result.GetType().ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T016_06_ApiPlcProgramRead_InvalidMode_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InvalidParams); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.PlcProgramReadAsync<object>("\"DataTypes\".\"Struct1L\"", ApiPlcProgramReadOrWriteMode.None));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T017_01_ApiPlcProgramWrite_InvalidMode_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InvalidParams); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.PlcProgramWriteAsync("\"DataTypes\".\"bool\"", true, ApiPlcProgramReadOrWriteMode.None));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T017_02_ApiPlcProgramWrite_Valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.PlcProgramWriteAsync("\"DataTypes\".Bool", true, ApiPlcProgramReadOrWriteMode.Simple);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T017_03_ApiPlcProgramWrite_ValidRaw_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.PlcProgramWriteAsync("\"DataTypes\".Bool", new int[1] { 1 }, ApiPlcProgramReadOrWriteMode.Raw);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T018_01_ApiWebAppBrowse_Valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppBrowse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.WebAppBrowseAsync();
            if (res.Result.Applications.Count != 2)
                Assert.Fail("2 appls returned but not given to user");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T018_02_ApiWebAppBrowse_ApplDoesNotExist_throwsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiApplicationDoesNotExistException>(async () => await TestHandler.WebAppBrowseAsync("anotherWebAp"));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T019_01_ApiWebAppBrowseResources_ApplDoesNotExist_throwsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiApplicationDoesNotExistException>(async () => await TestHandler.WebAppBrowseResourcesAsync("anotherWebAp"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T019_02_ApiWebAppBrowseResources_ResourceDoesNotExist_throwsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiApplicationDoesNotExistException>(async () => await TestHandler.WebAppBrowseResourcesAsync("anotherWebAp", "nonexistantresourcename"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T019_03_ApiWebAppBrowseResources_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppBrowseResources); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var resp = await TestHandler.WebAppBrowseResourcesAsync("anotherWebAp");
            if (resp.Result.Max_Resources != 200)
                Assert.Fail("max_resources are not 200 but:" + resp.Result.Max_Resources.ToString());
            if (resp.Result.Resources.Count != 7)
                Assert.Fail("resources given are 7 but having only:" + resp.Result.Resources.Count.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T020_01_ApiWebAppCreate_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var resp = await TestHandler.WebAppCreateAsync("thirdwebapp");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T020_02_ApiWebAppCreate_SystemReadOnly_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SystemIsReadOnly); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiSystemIsReadOnlyException>(async () => await TestHandler.WebAppCreateAsync("thirdWebApp"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T020_02_ApiWebAppCreate_WebAppAlreadyExists_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppAlreadyExists); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiApplicationAlreadyExistsException>(async () => await TestHandler.WebAppCreateAsync("thirdWebApp"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T020_03_ApiWebAppCreate_WebAppLimitReached_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppLimitReached); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiApplicationLimitReachedException>(async () => await TestHandler.WebAppCreateAsync("fifthWebApp"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T020_04_ApiWebAppCreate_InvalidWebAppNameEmpty_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string invalidName = "";
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T020_05_ApiWebAppCreate_InvalidWebAppNameMoreThan100Chars_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            // name with > 100 chars
            string invalidName = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789";
            await TestHandler.WebAppCreateAsync(invalidName);
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName + "0"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T020_06_ApiWebAppCreate_InvalidWebAppNameInvalidChars_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            // name with > 100 chars
            string invalidName = "ab$c";
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName));
            invalidName = "ab&c";
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName));
            invalidName = "ab\\c";
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName));
            invalidName = "ab*c";
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName));
            invalidName = "ab,c";
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName));
            invalidName = "ab,c";
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName));
            invalidName = "ab(c";
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName));
            invalidName = "ab)c";
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName));
            invalidName = "ab*c";
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName));
            invalidName = "ab|c";
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T021_01_ApiWebAppCreateResource_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var resp = await TestHandler.WebAppCreateResourceAsync("customerExampleManualAdjusted", "someresName", "text/html", "2020-08-24T07:08:06.000Z");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T021_02_ApiWebAppCreateResource_ResourceAlreadyExists_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceAlreadyExists); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiResourceAlreadyExistsException>(async () => await TestHandler.WebAppCreateResourceAsync("customerExampleManualAdjusted", "someresName.html", "text/html", "2020-08-24T07:08:06.000Z"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T021_03_ApiWebAppCreateResource_ResourceLimitReached_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppLimitReached); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            Assert.ThrowsAsync<ApiApplicationLimitReachedException>(async () => await TestHandler.WebAppCreateAsync("fifthWebApp"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T021_04_ApiWebAppCreateResource_InvalidResourceEmpty_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string invalidName = "";
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () =>
                await TestHandler.WebAppCreateResourceAsync("webapp", invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T021_05_ApiWebAppCreateResource_InvalidResourceNameMoreThan200Chars_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            // name with > 100 chars
            string invalidName = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789";
            await TestHandler.WebAppCreateResourceAsync("webapp", invalidName + invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat));
            Assert.ThrowsAsync<ApiInvalidResourceNameException>(async () =>
                await TestHandler.WebAppCreateResourceAsync("webapp", invalidName + invalidName + "0", "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T021_06_ApiWebAppCreateResource_InvalidResourceNameInvalidChars_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string invalidName = "ab*c";
            await TestHandler.WebAppCreateResourceAsync("webapp", invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat));
            invalidName = "ab,c";
            await TestHandler.WebAppCreateResourceAsync("webapp", invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat));
            invalidName = "ab(c";
            await TestHandler.WebAppCreateResourceAsync("webapp", invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat));
            invalidName = "ab)c";
            await TestHandler.WebAppCreateResourceAsync("webapp", invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat));
            invalidName = "ab*c";
            await TestHandler.WebAppCreateResourceAsync("webapp", invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat));
            invalidName = "ab|c";
            await TestHandler.WebAppCreateResourceAsync("webapp", invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat));
            invalidName = "ab$c";
            Assert.ThrowsAsync<ApiInvalidResourceNameException>(async () =>
                await TestHandler.WebAppCreateResourceAsync("webapp", invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat)));
            invalidName = "ab&c";
            Assert.ThrowsAsync<ApiInvalidResourceNameException>(async () =>
                await TestHandler.WebAppCreateResourceAsync("webapp", invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat)));
            invalidName = "ab\\c";
            Assert.ThrowsAsync<ApiInvalidResourceNameException>(async () =>
                await TestHandler.WebAppCreateResourceAsync("webapp", invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T021_07_ApiWebAppCreateResource_InvalidEtag_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string validName = "myResource";
            var validEtag = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" + "0123456789012345678901234567";
            await TestHandler.WebAppCreateResourceAsync("webapp", validName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat), ApiWebAppResourceVisibility.Public, validEtag);
            Assert.ThrowsAsync<ApiInvalidETagException>(async () =>
                await TestHandler.WebAppCreateResourceAsync("webapp", validName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat), ApiWebAppResourceVisibility.Public, validEtag + "0"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T021_08_ApiWebAppCreateResource_InvalidMediaTypeResponse_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppInvalidMediaType); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string validName = "myResource";
            var validEtag = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" + "0123456789012345678901234567";
            Assert.ThrowsAsync<ApiInvalidMediaTypeException>(async () =>
                await TestHandler.WebAppCreateResourceAsync("webapp", validName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat), ApiWebAppResourceVisibility.Public, validEtag + ""));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T022_01_ApiWebAppDelete_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppDeleteAsync(webAppName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T022_02_ApiWebAppDelete_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiApplicationDoesNotExistException>(async () =>
                await TestHandler.WebAppDeleteAsync(webAppName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T023_01_ApiWebAppDeleteResource_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppDeleteResourceAsync(webAppName, "resName");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T023_02_ApiWebAppDeleteResource_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiApplicationDoesNotExistException>(async () =>
                await TestHandler.WebAppDeleteResourceAsync(webAppName, "resName"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T023_03_ApiWebAppDeleteResource_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiResourceDoesNotExistException>(async () =>
                await TestHandler.WebAppDeleteResourceAsync(webAppName, "resName"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T023_04_ApiWebAppDeleteResource_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SystemIsReadOnly); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiSystemIsReadOnlyException>(async () =>
                await TestHandler.WebAppDeleteResourceAsync(webAppName, "resName"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T024_01_ApiWebAppRename_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppRenameAsync(webAppName, "newWebAppName");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T024_02_ApiWebAppRename_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            string invalidWebAppName = "ab,c";
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppRenameAsync(webAppName, invalidWebAppName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T025_01_ApiWebAppRenameResource_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppRenameResourceAsync(webAppName, "resName", "newResName");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T025_02_ApiWebAppRenameResource_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            string invalidResName = "ab$c";
            Assert.ThrowsAsync<ApiInvalidResourceNameException>(async () =>
                await TestHandler.WebAppRenameResourceAsync(webAppName, "resName", invalidResName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T026_01_ApiWebAppSetDefaultPage_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetDefaultPageAsync(webAppName, "resName");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T026_02_ApiWebAppSetDefaultPage_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceVisibilityMustBePublic); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiResourceVisibilityIsNotPublicException>(async () =>
                await TestHandler.WebAppSetDefaultPageAsync(webAppName, "resName"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T027_01_ApiWebAppSetNotFoundPage_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetNotFoundPageAsync(webAppName, "resName");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T027_02_ApiWebAppWebAppSetNotFoundPage_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceVisibilityMustBePublic); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiResourceVisibilityIsNotPublicException>(async () =>
                await TestHandler.WebAppSetNotFoundPageAsync(webAppName, "resName"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T028_01_ApiWebAppSetNotAuthorizedPage_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetNotAuthorizedPageAsync(webAppName, "resName");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T028_02_ApiWebAppWebAppSetNotAuthorizedPage_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceVisibilityMustBePublic); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiResourceVisibilityIsNotPublicException>(async () =>
                await TestHandler.WebAppSetNotAuthorizedPageAsync(webAppName, "resName"));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T029_01_ApiWebAppSetResourceETag_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetResourceETagAsync(webAppName, "resName", "etagVal");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T029_02_ApiWebAppWebAppSetResourceETag_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var validEtag = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" + "0123456789012345678901234567";
            await TestHandler.WebAppSetResourceETagAsync(webAppName, "resName", validEtag);
            Assert.ThrowsAsync<ApiInvalidETagException>(async () =>
                await TestHandler.WebAppSetResourceETagAsync(webAppName, "resName", validEtag + "0"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T030_01_ApiWebAppSetResourceMediaType_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetResourceMediaTypeAsync(webAppName, "resName", "etagVal");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T030_02_ApiWebAppWebAppSetResourceMediaType_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppInvalidMediaType); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiInvalidMediaTypeException>(async () =>
                await TestHandler.WebAppSetResourceMediaTypeAsync(webAppName, "resName", "asdigasf"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T031_01_ApiWebAppSetResourceModificationTime_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetResourceModificationTimeAsync(webAppName, "resName", "2020-08-24T07:08:06Z");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T031_02_ApiWebAppWebAppSetResourceModificationTime_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            /*string webAppName = "webApp";
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () =>
                await TestHandler.WebAppSetResourceModificationTimeAsync(webAppName, "resName", "asdigasf"));*/
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T032_01_ApiWebAppSetResourceVisibility_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetResourceVisibilityAsync(webAppName, "resName", ApiWebAppResourceVisibility.Public);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T032_02_ApiWebAppWebAppSetResourceVisibility_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () =>
                await TestHandler.WebAppSetResourceVisibilityAsync(webAppName, "resName", ApiWebAppResourceVisibility.None));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T033_01_ApiWebAppSetState_valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetStateAsync(webAppName, ApiWebAppState.Disabled);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T033_02_ApiWebAppWebAppSetState_Invalid_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () =>
                await TestHandler.WebAppSetStateAsync(webAppName, ApiWebAppState.None));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T034_ApiWebAppDownloadResource_Corrupted_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceContentHasBeenCorrupted); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            string resourcename = "index.html";
            Assert.ThrowsAsync<ApiResourceContentHasBeenCorruptedException>(async () =>
                await TestHandler.WebAppDownloadResourceAsync(webAppName, resourcename));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T035_ApiWebAppDownloadResource_NotReady_ThrowsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceContentIsNotReady); // Respond with JSON

            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            string webAppName = "webApp";
            string resourcename = "index.html";

            Assert.ThrowsAsync<ApiResourceContentIsNotReadyException>(async () =>
                await TestHandler.WebAppDownloadResourceAsync(webAppName, resourcename));
        }

        /// <summary>
        /// TestCase for Api Bulk Request
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T036_01_ApiBulkRequest_Valid_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);

            var bulkRequest = new List<IApiRequest>();
            bulkRequest.Add(ApiRequestFactory.GetApiPlcReadOperatingModeRequest());
            bulkRequest.Add(ApiRequestFactory.GetApiPlcProgramBrowseRequest(ApiPlcProgramBrowseMode.Children));
            bulkRequest.Add(ApiRequestFactory.GetApiGetCertificateUrlRequest());
            bulkRequest.Add(ApiRequestFactory.GetApiGetPermissionsRequest());
            var bulkResult = (await TestHandler.ApiBulkAsync(bulkRequest));
            var bulkSuccessful = bulkResult.SuccessfulResponses.ToList();
            var first = bulkSuccessful[0];
            var second = bulkSuccessful[1];
            var third = bulkSuccessful[2];
            var fourth = bulkSuccessful[3];
            ApiResultResponse<ApiPlcOperatingMode> casted =
                JsonConvert.DeserializeObject<ApiResultResponse<ApiPlcOperatingMode>>(JsonConvert.SerializeObject(first));
            ApiReadOperatingModeResponse firstcasted2 =
                JsonConvert.DeserializeObject<ApiReadOperatingModeResponse>(JsonConvert.SerializeObject(first));
            ApiPlcProgramBrowseResponse casted2 =
                JsonConvert.DeserializeObject<ApiPlcProgramBrowseResponse>(JsonConvert.SerializeObject(second));
            ApiSingleStringResponse casted3 =
                JsonConvert.DeserializeObject<ApiSingleStringResponse>(JsonConvert.SerializeObject(third));
            ApiArrayOfApiClassResponse casted4 =
                JsonConvert.DeserializeObject<ApiArrayOfApiClassResponse>(JsonConvert.SerializeObject(fourth));
        }

        /// <summary>
        /// TestCase for Api Bulk Request
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T036_02_ApiBulkRequest_NoResources_Exception()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkNoResources); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var bulkRequest = new List<IApiRequest>();
            // Does not matter!!
            bulkRequest.Add(ApiRequestFactory.GetApiPingRequest());
            Assert.ThrowsAsync<ApiBulkRequestException>(async () =>
            {
                var bulkResult = (await TestHandler.ApiBulkAsync(bulkRequest));
            });
        }

        /// <summary>
        /// TestCase for Api ReadSystemTime
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T037_PlcReadSystemTime_Always_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadSystemTime); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var systemTime = await TestHandler.PlcReadSystemTimeAsync();
            var expectedDate = new DateTime(2022, 03, 01, 17, 2, 4);
            expectedDate += TimeSpan.FromMilliseconds(238);
            expectedDate += TimeSpan.FromTicks(2960);
            Assert.AreEqual(systemTime.Result.Timestamp, expectedDate);
        }

        /// <summary>
        /// TestCase for Api ReadTimeSettings
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T038_01_PlcReadTimeSettings_Rule_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadTimeSettings); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var timeSettings = await TestHandler.PlcReadTimeSettingsAsync();
            var result = timeSettings.Result;
            var dst = new DaylightSavingsTimeConfiguration(new PlcDate(3, 5, ApiDayOfWeek.Sun, 1, 0), new TimeSpan(0, 60, 0));
            var sdt = new StandardTimeConfiguration(new PlcDate(10, 5, ApiDayOfWeek.Sun, 2, 0));
            var expectedRule = new DaylightSavingsRule(sdt, dst);
            Assert.AreEqual(TimeSpan.Zero, result.Current_offset, "Current offset is not equal to expected value!");
            Assert.AreEqual(expectedRule, result.Rule, "Time setting rule doesn't match!");
        }

        /// <summary>
        /// TestCase for Api ReadTimeSettings
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T038_02_PlcReadTimeSettings_NoRule_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadTimeSettingsNoRule); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var timeSettings = await TestHandler.PlcReadTimeSettingsAsync();
            var result = timeSettings.Result;
            Assert.AreEqual(TimeSpan.Zero, result.Current_offset);
            Assert.AreEqual(TimeSpan.Zero, result.Utc_offset);
            var rule = result.Rule;
            Assert.AreEqual(null, rule);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T039_01_PlcCreateBackup_Valid_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var expected = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            var resp = await TestHandler.PlcCreateBackupAsync();
            Assert.IsTrue(resp.Result.ToString() == expected, "Failed");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T040_01_PlcRestoreBackup_Valid_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var expected = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            var resp = await TestHandler.PlcRestoreBackupAsync();
            Assert.IsTrue(resp.Result.ToString() == expected, "Failed");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T040_01_PlcRestoreBackup_DHCP_Valid_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{FQDN}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{FQDN}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var expected = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            var resp = await TestHandler.PlcRestoreBackupAsync();
            Assert.IsTrue(resp.Result.ToString() == expected, "Failed");
        }

        /// <summary>
        /// TestCase for Files Browse
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T041_01_FilesBrowse_TestFolder_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.FilesBrowseTestFolder); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var browsedResult = await TestHandler.FilesBrowseAsync();
            var result = browsedResult.Result;
            var expectedRes1 = new ApiFileResource() { Name = "pre3.png", Size = 20511, Type = ApiFileResourceType.File, Last_Modified = new DateTime(637818317050000000) };
            var expectedRes2 = new ApiFileResource() { Name = "uploadfromdev.png", Size = 110308, Type = ApiFileResourceType.File, Last_Modified = new DateTime(637818316970000000) };
            Assert.AreEqual(result.Resources[0], expectedRes1);
            Assert.AreEqual(result.Resources[1], expectedRes2);
        }

        /// <summary>
        /// TestCase for Files.Download
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T042_01_ApiFilesDownload_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.FilesDownloadAsync("/");
            var expectedResult = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            Assert.AreEqual(expectedResult, result.Result, "The tickets are not identical.");
        }

        /// <summary>
        /// TestCase for Files.Create
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T043_01_ApiFilesCreate_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.FilesCreateAsync("/");
            var expectedResult = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            Assert.AreEqual(expectedResult, result.Result, "The tickets are not identical.");
        }

        /// <summary>
        /// TestCase for Files.CreateDirectory
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T044_01_ApiFilesCreateDirectory_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.FilesCreateDirectoryAsync("/Dir/newDir");
            Assert.IsTrue(result.Result, "The result is not true.");
        }

        /// <summary>
        /// TestCase for Files.Rename
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T045_01_ApiFilesRename_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.FilesRenameAsync("/Dir/newDir", "/Dir/renamedDir");
            Assert.IsTrue(result.Result, "The result is not true.");
        }

        /// <summary>
        /// TestCase for Files.Delete
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T046_01_ApiFilesDelete_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.FilesDeleteAsync("/Dir/newDir");
            Assert.IsTrue(result.Result, "The result is not true.");
        }

        /// <summary>
        /// TestCase for Files.DeleteDirectory
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T047_01_ApiFilesDeleteDirectory_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.FilesDeleteDirectoryAsync("/Dir/newDir");
            Assert.IsTrue(result.Result, "The result is not true.");
        }

        /// <summary>
        /// TestCase for DataLogs Browse
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T048_01_ApiDataLogsBrowse_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.FilesBrowseTestFolder); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var browsedResult = await TestHandler.FilesBrowseAsync();
            var result = browsedResult.Result;
            var expectedRes1 = new ApiFileResource() { Name = "pre3.png", Size = 20511, Type = ApiFileResourceType.File, Last_Modified = new DateTime(637818317050000000) };
            var expectedRes2 = new ApiFileResource() { Name = "uploadfromdev.png", Size = 110308, Type = ApiFileResourceType.File, Last_Modified = new DateTime(637818316970000000) };
            Assert.AreEqual(result.Resources[0], expectedRes1);
            Assert.AreEqual(result.Resources[1], expectedRes2);
        }

        /// <summary>
        /// TestCase for Files.Download extended for datalogs support 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T049_01_ApiDataLogsDownload_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.FilesDownloadAsync("/");
            var expectedResult = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            Assert.AreEqual(expectedResult, result.Result, "The tickets are not identical.");
        }

        /// <summary>
        /// TestCase for Files.Delete extended for datalogs support
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T050_01_ApiDataLogsDelete_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.FilesDeleteAsync("/Dir/newDir");
            Assert.IsTrue(result.Result, "The result is not true.");
        }

        /// <summary>
        /// TestCase for Files.Delete extended for datalogs support
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T051_01_ApiDataLogsDownloadAndClear_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.DatalogsDownloadAndClearAsync("/");
            var expectedResult = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            Assert.AreEqual(expectedResult, result.Result, "The tickets are not identical.");
        }

        /// <summary>
        /// Testcase for ApiSyslog.Browse request
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T052_ApiSyslogBrowse_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SyslogResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = (await TestHandler.ApiSyslogBrowseAsync()).Result;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(5, result.Count_Total, "result Count_Total is not the expected value!");
                Assert.AreEqual(1, result.Count_Lost, "result Count_Lost is not the expected value!");
                Assert.AreEqual(2, result.Entries.Count, "result Entries.Count is not the expected value!");
                Assert.AreEqual("I am a syslog, no need to question it!", result.Entries[0].Raw, "result Entries[0].Raw is not the expected value!");
                Assert.AreEqual("I am a syslog, too. But the previous syslog is an impostor!", result.Entries[1].Raw, "result Entries[1].Raw is not the expected value!");
            });
        }
        /// <summary>
        /// TestCase for Failsafe.ReadParameters with CPU params
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T053_01_ApiFailsafeReadParameters_CPU_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.FailsafeReadParametersCPUResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.FailsafeReadParametersAsync(50);
            var fs = new FailsafeCPU();
            fs.Last_f_program_modification = new DateTime(2012, 4, 23, 18, 25, 43, 510);
            fs.Collective_signature = "BC7C0410";
            fs.Remaining_time = new TimeSpan(2, 33, 0);
            Assert.That(result.Result.Safety_mode);
            Assert.AreEqual(ApiFailsafeHardwareType.F_cpu, result.Result.Type);
            Assert.That(result.Result.Parameters is FailsafeCPU);
            Assert.That(result.Result.Parameters.Equals(fs));
        }

        /// <summary>
        /// TestCase for Failsafe.ReadParameters with Module params
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T053_02_ApiFailsafeReadParameters_Module_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.FailsafeReadParametersModuleResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.FailsafeReadParametersAsync(50);
            var fs = new FailsafeModule();
            fs.F_monitoring_time = TimeSpan.FromMilliseconds(542);
            fs.F_destination_address = 123;
            fs.F_source_address = 321;
            fs.F_par_crc = "DBB32A1A";
            Assert.That(result.Result.Safety_mode);
            Assert.AreEqual(ApiFailsafeHardwareType.F_module, result.Result.Type);
            Assert.That(result.Result.Parameters is FailsafeModule);
            Assert.That(result.Result.Parameters.Equals(fs));
        }

        /// <summary>
        /// TestCase for Failsafe.ReadRuntimeGroups
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T054_ApiFailsafeReadRuntimeGroups_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.FailsafeReadRuntimeGroupsResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.FailsafeReadRuntimeGroupsAsync();
            var noRemaining = new ApiFailsafeRuntimeGroup();
            noRemaining.Name = "F-runtime group 1";
            noRemaining.Signature = "E70AFF00";
            noRemaining.Cycle_time_current = TimeSpan.FromMilliseconds(550);
            noRemaining.Cycle_time_Max = TimeSpan.FromMilliseconds(600);
            noRemaining.Runtime_current = TimeSpan.FromMilliseconds(40);
            noRemaining.Runtime_max = TimeSpan.FromMilliseconds(200);
            var withRemaining = new ApiFailsafeRuntimeGroup();
            withRemaining.Name = "RTG_2";
            withRemaining.Signature = "CBA57022";
            withRemaining.Cycle_time_current = TimeSpan.FromMilliseconds(110);
            withRemaining.Cycle_time_Max = TimeSpan.FromMilliseconds(200);
            withRemaining.Runtime_current = TimeSpan.FromMilliseconds(50);
            withRemaining.Runtime_max = TimeSpan.FromMilliseconds(80);
            Assert.IsTrue(result.Result.Groups[0].Equals(noRemaining), "The groups don't match!");
            Assert.IsTrue(result.Result.Groups[1].Equals(withRemaining), "The groups don't match!");
        }

        /// <summary>
        /// TestCase for Api.ChangePassword
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T055_ApiChangePassword_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.ApiChangePasswordAsync("Admin", "adminpw", "newadminpw");
            Assert.IsTrue(result.Result, "Changing passwords not possible!");
        }

        /// <summary>
        /// TestCase for Api.GetPasswordPolicy
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T056_ApiGetPasswordPolicy_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiGetPasswordPolicy); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.ApiGetPasswordPolicyAsync();
            var expectedResult = new ApiPasswordPolicy();
            expectedResult.Requires_lowercase_characters = true;
            expectedResult.Requires_uppercase_characters = true;
            expectedResult.Min_password_length = 8;
            expectedResult.Max_password_length = 120;
            expectedResult.Min_digits = 1;
            expectedResult.Min_special_characters = 0;
            Assert.AreEqual(expectedResult, result.Result.Password_policy, "Password policies dont match");
        }

        /// <summary>
        /// TestCase for Api.GetAuthenticationMode with 3 modes
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T057_ApiGetAuthenticationMode_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiGetAuthenticationModeMany); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.ApiGetAuthenticationModeAsync();
            var expectedResult = new List<ApiAuthenticationMode>();
            expectedResult.Add(ApiAuthenticationMode.Local);
            expectedResult.Add(ApiAuthenticationMode.Static);
            expectedResult.Add(ApiAuthenticationMode.Disabled);
            Assert.AreEqual(expectedResult.Count, result.Result.Authentication_modes.Count, "Authentication modes count don't match!");
            Assert.That(expectedResult.SequenceEqual(result.Result.Authentication_modes), "Order of authetication modes is different!");
        }

        /// <summary>
        /// TestCase for Project.ReadLanguages with 4 languages
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T058_ProjectReadLanguages_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ProjectReadLanguagesMany); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.ProjectReadLanguagesAsync();
            var expectedResult = new List<CultureInfo>();
            expectedResult.Add(new CultureInfo("en-US"));
            expectedResult.Add(new CultureInfo("en-GB"));
            expectedResult.Add(new CultureInfo("es-BR"));
            expectedResult.Add(new CultureInfo("ne-IN"));
            Assert.AreEqual(expectedResult.Count, result.Result.Languages.Select(x => x.Language).Count(), "Languages don't match");
            Assert.That(expectedResult.SequenceEqual(result.Result.Languages.Select(x => x.Language)), "The order of languages are different, or they don't contain the same languages.");
        }

        /// <summary>
        /// TestCase for Plc.ReadModeSelectorState with Standard PLC
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T059_01_ApiPlcReadModeSelectorState_Standard_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadModeSelectorStateRun); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.PlcReadModeSelectorStateAsync(ApiPlcRedundancyId.StandardPLC);
            Assert.AreEqual(ApiPlcModeSelectorState.Run, res.Result.Mode_Selector, "Plc Mode Selector state is not in run mode!");
        }

        /// <summary>
        /// TestCase for Plc.ReadModeSelectorState with RH PLC system
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T059_02_ApiPlcReadModeSelectorState_RH_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadModeSelectorStateNoSwitch); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var res = await TestHandler.PlcReadModeSelectorStateAsync(ApiPlcRedundancyId.RedundancyId_1);
            Assert.AreEqual(ApiPlcModeSelectorState.NoSwitch, res.Result.Mode_Selector, "Plc Mode Selector state is not in noswitch mode!");
        }

        /// <summary>
        /// TestCase for Modules.DownloadServiceData
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T060_ApiModulesDownloadServiceData_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.ModulesDownloadServiceDataAsync(ApiPlcHwId.StandardPLC);
            var expectedResult = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            Assert.AreEqual(expectedResult, result.Result, "The tickets are not identical.");
        }

        /// <summary>
        /// TestCase for Alarms.Acknowledge
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T061_ApiAlarmsAcknowledge_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.AlarmsAcknowledge); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.AlarmsAcknowledgeAsync("/");
            Assert.AreEqual(true, result.Result, "The response should always contain true, but it doesn't.");
        }
        /// <summary>
        /// TestCase for Plc.SetSystemTime
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T062_PlcSetSystemTime_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcSetSystemTime); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.PlcSetSystemTimeAsync(new DateTime(2010, 10, 10));
            Assert.IsTrue(result.Result, "The time was not set successfully!");
        }

        /// <summary>
        /// TestCase for Plc.SetTimeSettings, both offset and rule object
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T063_01_ApiPlcSetTimeSettings_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcSetTimeSettingsAll); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);

            var offset = new TimeSpan(8, 0, 0);
            var dst = new DaylightSavingsTimeConfiguration(new PlcDate(12, 5, ApiDayOfWeek.Sun, 3, 0), new TimeSpan(0, 60, 0));
            var sdt = new StandardTimeConfiguration(new PlcDate(1, 1, ApiDayOfWeek.Mon, 23, 31));
            DaylightSavingsRule dsr = new DaylightSavingsRule(sdt, dst);

            var result = await TestHandler.PlcSetTimeSettingsAsync(offset, dsr);
            Assert.IsTrue(result.Result, "Couldn't set time settings!");
        }

        /// <summary>
        /// TestCase for Plc.SetTimeSettings
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T063_02_ApiPlcSetTimeSettingsNoRule_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcSetTimeSettingsNoRule); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);

            var offset = new TimeSpan(8, 0, 0);

            var result = await TestHandler.PlcSetTimeSettingsAsync(offset);
            Assert.IsTrue(result.Result, "Couldn't set time settings!");
        }

        /// <summary>
        /// TestCase for Api.GetQuantityStructures
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T064_ApiGetQuantityStructures()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.QuantityStructuresResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.ApiGetQuantityStructuresAsync();
            var expectedResult = new ApiQuantityStructure();
            expectedResult.Webapi_Max_Http_Request_Body_Size = 131072;
            expectedResult.Webapi_Max_Parallel_User_Sessions = 100;
            expectedResult.Webapi_Max_Parallel_Requests = 4;
            Assert.That(result.Result.Equals(expectedResult), "Quantity Structure is not as expected!");
        }
        /// <summary>
        /// TestCase for WebServer.SetDefaultPage
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T065_WebServerSetDefaultPage_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.WebServerSetDefaultPageAsync("index.html");
            Assert.IsTrue(result.Result, "The result is not true.");
        }
        /// <summary>
        /// TestCase for WebServer.ReadDefaultPage
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T066_WebServerReadDefaultPage_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.DefaultPageResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = await TestHandler.WebServerGetReadDefaultPageAsync();
            var expectedResult = "/~teszt2/index.html";
            Assert.AreEqual(expectedResult, result.Result.Default_page, "The result is not the expected.");
        }

        /// <summary>
        /// TestCase for ApiAlarmsBrowse 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T067_ApiAlarmsBrowse_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiAlarmsBrowseResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = (await TestHandler.ApiAlarmsBrowseAsync(new CultureInfo("en-US"))).Result;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, result.Entries.Count, "result.Entries.Count");
                Assert.AreEqual("9947888669857743000", result.Entries[0].Id, "result.Entries[0].Id");
                Assert.AreEqual(512, result.Entries[0].Alarm_Number, "result.Entries[0].Alarm_Number");
                Assert.AreEqual(ApiObjectDirectoryStatus.Incoming, result.Entries[0].Status, "result.Entries[0].Status");
                Assert.AreEqual(new DateTime(2012, 1, 1, 1, 1, 42, 47), result.Entries[0].Timestamp, "result.Entries[0].Timestamp");
                Assert.AreEqual(ApiAlarmAcknowledgementState.Not_Acknowledged, result.Entries[0].Acknowledgement.State, "result.Entries[0].Acknowledgement.State");
                Assert.AreEqual("#1, 1", result.Entries[0].Alarm_Text, "result.Entries[0].Alarm_Text");
                Assert.AreEqual("#1, 0", result.Entries[0].Info_Text, "result.Entries[0].Info_Text");
                Assert.AreEqual(false, result.Entries[0].Text_Inconsistent, "result.Entries[0].Text_Inconsistent");
                Assert.AreEqual(new DateTime(2012, 1, 1, 1, 1, 42, 99), result.Last_Modified, "result.Last_Modified");
                Assert.AreEqual(51, result.Count_Current, "result.Count_Current");
                Assert.AreEqual(500, result.Count_Max, "result.Count_Max");
                Assert.AreEqual("en-US", result.Language, "result.Language");
            });
        }

        /// <summary>
        /// TestCase for DiagnosticBuffer.Browse method
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T068_ApiDiagnosticBufferBrowse_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.DiagnosticBufferBrowseResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker);
            var result = (await TestHandler.ApiDiagnosticBufferBrowseAsync(new CultureInfo("en-US"))).Result;
            DateTime expected = new DateTime(2023, 06, 07, 18, 25, 43); //2023-06-07T18:25:43.514678531Z
            expected = expected.AddTicks(5146785);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, result.Entries.Count, "result.Entries.Count");
                Assert.AreEqual(ApiObjectDirectoryStatus.Outgoing, result.Entries[0].Status, "result.Entries[0].Status");
                Assert.AreEqual("Boot up  - CPU changes from OFF to STOP (initialization) mode", result.Entries[0].Short_Text, "result.Entries[0].Short_Text");
                Assert.AreEqual("LONG TEXT", result.Entries[1].Long_Text, "result.Entries[1].Long_Text");
                Assert.AreEqual(3, result.Entries[1].Event.Textlist_Id, "result.Entries[1].Event.Textlist_Id");
                Assert.AreEqual(26315, result.Entries[1].Event.Text_Id, "result.Entries[1].Event.Text_Id");
                Assert.AreEqual(expected, result.Last_Modified, "result.Last_Modified");
                Assert.AreEqual(1234, result.Count_Current, "result.Count_Current");
                Assert.AreEqual(3200, result.Count_Max, "result.Count_Max");
                Assert.AreEqual("en-US", result.Language, "result.Language");
            });
        }
    }
}
