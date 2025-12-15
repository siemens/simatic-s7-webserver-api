// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RichardSzalay.MockHttp;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.FailsafeParameters;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults;
using Siemens.Simatic.S7.Webserver.API.Models.Technology;
using Siemens.Simatic.S7.Webserver.API.Models.TimeSettings;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using Siemens.Simatic.S7.Webserver.API.Services.PlcProgram;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBrowseResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InvalidParams); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketNotFound); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SystemIsBusy); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.CertificateUrl); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.GetPermissionsAdmin); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.GetPermissionsNone); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InvalidParams); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusActive); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketNotFound); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusCreated); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusActive); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusFailed); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusCompleted); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketStatusCompleted); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketNotFound); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.BrowseTicketsNoTickets); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.BrowseTicketsTwoTickets); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.LoginFailed); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var req = ApiRequestFactory.GetApiLoginRequest("Everybody", "wrong", null, ApiAuthenticationMode.Local);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.AlreadyAuthenticated); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var req = ApiRequestFactory.GetApiLoginRequest("Everybody", "", null, ApiAuthenticationMode.Local);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.NoResources); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var req = ApiRequestFactory.GetApiLoginRequest("Everybody", "", null, ApiAuthenticationMode.Local);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.LoginWorked); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var req = ApiRequestFactory.GetApiLoginRequest("Everybody", "", null, ApiAuthenticationMode.Local);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.LoginWorkedWithWebAppCookie); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var req = ApiRequestFactory.GetApiLoginRequest("Everybody", "", true, ApiAuthenticationMode.Local);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.LoginWorkedWithPasswordExpirationInformation); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var req = ApiRequestFactory.GetApiLoginRequest("Anonymous", "", true, ApiAuthenticationMode.Local);
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
        /// Unit test for Api.Login that returns every possible value
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T007_07_ApiLogin_ValidToken_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.LoginWorked); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiLoginAsync("Admin", "Siemens_1", ApiAuthenticationMode.Local)).Result;
            Assert.That(result.Token, Is.EqualTo("G8ejtdxTZ6fz8AIuwDG.tWf+6Cou"));
        }

        /// <summary>
        /// Unit test for Api.Login that returns every possible value
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T007_08_ApiLogin_ValidToken_InfrastructureErrorExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.LoginFailedInfrastructureError); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiInfrastructureErrorException>(async () => await TestHandler.ApiLoginAsync("Admin", "Siemens_1", ApiAuthenticationMode.Umc));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            await TestHandler.ApiPingAsync();
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiVersionResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeRun); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcReadOperatingModeAsync();
            if (res.Result != ApiPlcOperatingMode.Run)
                Assert.Fail($"unexpected response:{res.Result}");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeStop); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcReadOperatingModeAsync();
            if (res.Result != ApiPlcOperatingMode.Stop)
                Assert.Fail($"unexpected response:{res.Result}");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeStartup); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcReadOperatingModeAsync();
            if (res.Result != ApiPlcOperatingMode.Startup)
                Assert.Fail($"unexpected response:{res.Result}");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeHold); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcReadOperatingModeAsync();
            if (res.Result != ApiPlcOperatingMode.Hold)
                Assert.Fail($"unexpected response:{res.Result}");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeStopFwUpdate); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcReadOperatingModeAsync();
            if (res.Result != ApiPlcOperatingMode.Stop_fwupdate)
                Assert.Fail($"unexpected response:{res.Result}");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeInvUnknown); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeInvNone); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<Newtonsoft.Json.JsonSerializationException>(async () => await TestHandler.PlcReadOperatingModeAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T011_08_ApiPlcReadOperatingMode_runRedundant_RH_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeRunRedundant); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcReadOperatingModeAsync(ApiPlcRedundancyId.RedundancyId_1);
            if (res.Result != ApiPlcOperatingMode.Run_redundant)
                Assert.Fail($"unexpected response:{res.Result}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T011_09_ApiPlcReadOperatingMode_syncup_RH_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeSyncup); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcReadOperatingModeAsync(ApiPlcRedundancyId.RedundancyId_2);
            if (res.Result != ApiPlcOperatingMode.Syncup)
                Assert.Fail($"unexpected response:{res.Result}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T011_10_ApiPlcReadOperatingMode_runsyncup_RH_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeRunSyncup); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcReadOperatingModeAsync(ApiPlcRedundancyId.RedundancyId_2);
            if (res.Result != ApiPlcOperatingMode.Run_syncup)
                Assert.Fail($"unexpected response:{res.Result}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T011_11_ApiPlcReadOperatingMode_remoteunknown_RH_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadOpModeRemoteUnknown); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcReadOperatingModeAsync(ApiPlcRedundancyId.RedundancyId_2);
            if (res.Result != ApiPlcOperatingMode.Remote_unknown)
                Assert.Fail($"unexpected response:{res.Result}");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            await TestHandler.PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode.Stop);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T012_08_ApiRequestChangeOperatingMode_run_RH_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            await TestHandler.PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode.Stop, ApiPlcRedundancyId.RedundancyId_1);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseAll); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var resp = await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.Children);
            if (resp.Result.Count == 0)
            {
                Assert.Fail("plcprogramdata result given but not to user!");
            }
            mockHttp.Flush();
            mockHttp.Clear();
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseVarIsNotAStructure); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramAddressDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramInvalidAddress); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramnInvalidArrayIndex); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiInvalidArrayIndexException>(async () => await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.Children, "\"DataTypes\".\"Bool\"a"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T014_01_PlcProgramDownloadProfilingData_Success()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramDownloadProfilingDataSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            ApiSingleStringResponse response = await TestHandler.PlcProgramDownloadProfilingDataAsync();

            Assert.That(response.Result.Equals("jgxikeMgLryvP0YoHc.eqt8BY787"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T014_02_PlcProgramDownloadProfilingData_NoResources()
        {
            // This case could happen if the user downloads the profiling
            // data twice without clearing the ticket or downloading the actual data.

            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramDownloadProfilingDataNoResources); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            Assert.ThrowsAsync<ApiNoResourcesException>(async () => await TestHandler.PlcProgramDownloadProfilingDataAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T014_03_PlcProgramDownloadProfilingData_PermissionDenied()
        {

            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramDownloadProfilingDataPermissionDenied); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            Assert.ThrowsAsync<System.UnauthorizedAccessException>(async () => await TestHandler.PlcProgramDownloadProfilingDataAsync());
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PermissionDenied); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.Children, "\"DataTypes\".\"Bool\""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T015_02_ApiPlcProgramBrowseCodeBlocks_EmptyResult()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseCodeBlocksEmptyResult); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            ApiPlcProgramBrowseCodeBlocksResponse response = await TestHandler.PlcProgramBrowseCodeBlocksAsync();
            Assert.That(response.Result.Count == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T015_03_ApiPlcProgramBrowseCodeBlocks_InvalidParams()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseCodeBlocksInvalidParams); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.PlcProgramBrowseCodeBlocksAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T015_04_ApiPlcProgramBrowseCodeBlocks_Success()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseCodeBlocksSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            ApiPlcProgramBrowseCodeBlocksResponse response = await TestHandler.PlcProgramBrowseCodeBlocksAsync();

            Assert.That(response.Result.Count == 5);

            Assert.That(response.Result[0].Name == "Main");
            Assert.That(response.Result[0].BlockType == ApiPlcProgramBlockType.Ob);
            Assert.That(response.Result[0].BlockNumber == 1);

            Assert.That(response.Result[1].Name == "USEND");
            Assert.That(response.Result[1].BlockType == ApiPlcProgramBlockType.Sfb);
            Assert.That(response.Result[1].BlockNumber == 8);

            Assert.That(response.Result[2].Name == "COPY_HW");
            Assert.That(response.Result[2].BlockType == ApiPlcProgramBlockType.Sfc);
            Assert.That(response.Result[2].BlockNumber == 65509);

            Assert.That(response.Result[3].Name == "PRODTEST");
            Assert.That(response.Result[3].BlockType == ApiPlcProgramBlockType.Fb);
            Assert.That(response.Result[3].BlockNumber == 65522);

            Assert.That(response.Result[4].Name == "FC_14325");
            Assert.That(response.Result[4].BlockType == ApiPlcProgramBlockType.Fc);
            Assert.That(response.Result[4].BlockNumber == 14325);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T015_05_ApiPlcProgramBrowseCodeBlocks_EmptyBlockName()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseCodeBlocksEmptyBlockName); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            ApiPlcProgramBrowseCodeBlocksResponse response = await TestHandler.PlcProgramBrowseCodeBlocksAsync();

            Assert.That(response.Result.Count == 1);
            Assert.That(response.Result[0].Name == String.Empty);
            Assert.That(response.Result[0].BlockType == ApiPlcProgramBlockType.Ob);
            Assert.That(response.Result[0].BlockNumber == 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T015_06_ApiPlcProgramBrowseCodeBlocks_StringAsBlockNumber()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseCodeBlocksStringAsNumber); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            Assert.ThrowsAsync<Newtonsoft.Json.JsonSerializationException>(async () => await TestHandler.PlcProgramBrowseCodeBlocksAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T015_07_ApiPlcProgramBrowseCodeBlocks_PermissionDenied()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramBrowseCodeBlocksPermissionDenied); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            Assert.ThrowsAsync<System.UnauthorizedAccessException>(async () => await TestHandler.PlcProgramBrowseCodeBlocksAsync());
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PermissionDenied); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramUnsupportedAddress); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InternalError); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramReadFalseBool); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramReadRawFalseBool); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var resp = await TestHandler.PlcProgramReadAsync<object>("\"DataTypes\".\"Bool\"");
            if (resp.Result is JArray)
            {
                var jarr = (JArray)resp.Result;
                var respRes = jarr.ToObject<List<bool>>();
                if (respRes[0] != false)
                    Assert.Fail("not casted to \"false\" bool!");
            }
            else
                Assert.Fail($"raw mode returned sth else than jarray: {resp.Result.GetType()}");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InvalidParams); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.PlcProgramReadAsync<object>("\"DataTypes\".\"Struct1L\"", ApiPlcDataRepresentation.None));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InvalidParams); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.PlcProgramWriteAsync("\"DataTypes\".\"bool\"", true, ApiPlcDataRepresentation.None));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcProgramWriteAsync("\"DataTypes\".Bool", true, ApiPlcDataRepresentation.Simple);
            Assert.That(res.Result);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcProgramWriteAsync("\"DataTypes\".Bool", new int[1] { 1 }, ApiPlcDataRepresentation.Raw);
            Assert.That(res.Result);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppBrowse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.WebAppBrowseAsync();
            if (res.Result.Applications.Count != 2)
                Assert.Fail("2 appls returned but not given to user");
        }

        /// <summary>
        /// TestCase for WebApp.Browse method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T018_02_ApiWebAppBrowse_ApplDoesNotExist_throwsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiApplicationDoesNotExistException>(async () => await TestHandler.WebAppBrowseAsync("anotherWebAp"));
        }

        /// <summary>
        /// TestCase for WebApp.Browse method
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T018_03_ApiWebAppBrowse_CheckAllValues()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppBrowse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.WebAppBrowseAsync()).Result;
            Assert.Multiple(() =>
            {
                Assert.That(result.Max_Applications, Is.EqualTo(4), "Max_Applications");
                var app1 = result.Applications.First();
                Assert.That(app1.Name, Is.EqualTo("customerExampleManualAdjusted"), "app1.Name");
                Assert.That(app1.State, Is.EqualTo(ApiWebAppState.Enabled), "app1.State");
                Assert.That(app1.Type, Is.EqualTo(ApiWebAppType.User), "app1.Type");
                Assert.That(app1.Version, Is.EqualTo("V1.2"), "app1.Version");
                Assert.That(app1.Redirect_mode, Is.EqualTo(ApiWebAppRedirectMode.Redirect), "app1.Redirect_mode");
                Assert.That(app1.Default_page, Is.EqualTo("index.html"), "app1.Default_page");
                Assert.That(app1.Not_found_page, Is.EqualTo("index2.html"), "app1.Name");
                Assert.That(app1.Not_authorized_page, Is.EqualTo("login.html"), "app1.Name");
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T018_04_ApiWebAppBrowse_NoRedirectMode_Valid_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppBrowse_NoRedirectMode); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.WebAppBrowseAsync();
            if (res.Result.Applications.Count != 2)
                Assert.Fail("2 appls returned but not given to user");
            Assert.That(res.Result.Applications.First().Redirect_mode, Is.EqualTo(ApiWebAppRedirectMode.None));
        }

        /// <summary>
        /// TestCase for WebApp.Browse method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T019_01_ApiWebAppBrowseResources_ApplDoesNotExist_throwsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppBrowseResources); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var resp = await TestHandler.WebAppBrowseResourcesAsync("anotherWebAp");
            if (resp.Result.Max_Resources != 200)
                Assert.Fail($"max_resources are not 200 but:{resp.Result.Max_Resources}");
            if (resp.Result.Resources.Count != 7)
                Assert.Fail($"resources given are 7 but having only:{resp.Result.Resources.Count}");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var resp = await TestHandler.WebAppCreateAsync("thirdwebapp");
            Assert.That(resp.Result);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SystemIsReadOnly); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppAlreadyExists); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppLimitReached); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var resp = await TestHandler.WebAppCreateResourceAsync("customerExampleManualAdjusted", "someresName", "text/html", "2020-08-24T07:08:06.000Z");
            Assert.That(resp.Result, Is.EqualTo("YUryW9vc8FRVLU054XYWyHee9GWu"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceAlreadyExists); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppLimitReached); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SingleStringResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppInvalidMediaType); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppDeleteAsync(webAppName);
            Assert.That(resp.Result);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppDeleteResourceAsync(webAppName, "resName");
            Assert.That(resp.Result);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SystemIsReadOnly); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppRenameAsync(webAppName, "newWebAppName");
            Assert.That(resp.TrueOnSuccesResponse.Result);
            Assert.That(resp.NewWebApp.Name, Is.EqualTo("newWebAppName"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppRenameResourceAsync(webAppName, "resName", "newResName");
            Assert.That(resp.TrueOnSuccesResponse.Result);
            Assert.That(resp.NewResource.Name, Is.EqualTo("newResName"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetDefaultPageAsync(webAppName, "resName");
            Assert.That(resp.TrueOnSuccesResponse.Result);
            Assert.That(resp.NewWebApp.Default_page, Is.EqualTo("resName"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceVisibilityMustBePublic); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetNotFoundPageAsync(webAppName, "resName");
            Assert.That(resp.TrueOnSuccesResponse.Result);
            Assert.That(resp.NewWebApp.Not_found_page, Is.EqualTo("resName"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceVisibilityMustBePublic); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetNotAuthorizedPageAsync(webAppName, "resName");
            Assert.That(resp.TrueOnSuccesResponse.Result);
            Assert.That(resp.NewWebApp.Not_authorized_page, Is.EqualTo("resName"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceVisibilityMustBePublic); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetResourceETagAsync(webAppName, "resName", "etagVal");
            Assert.That(resp.TrueOnSuccesResponse.Result);
            Assert.That(resp.NewResource.Etag, Is.EqualTo("etagVal"));
            Assert.That(resp.NewResource.Name, Is.EqualTo("resName"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetResourceMediaTypeAsync(webAppName, "resName", "meadiaTypeVal");
            Assert.That(resp.TrueOnSuccesResponse.Result);
            Assert.That(resp.NewResource.Media_type, Is.EqualTo("meadiaTypeVal"));
            Assert.That(resp.NewResource.Name, Is.EqualTo("resName"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppInvalidMediaType); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetResourceModificationTimeAsync(webAppName, "resName", "2020-08-24T07:08:06Z");
            Assert.That(resp.TrueOnSuccesResponse.Result);
            var expectedTime = DateTime.Parse("2020-08-24T07:08:06Z").ToUniversalTime();
            Assert.That(resp.NewResource.Last_modified, Is.EqualTo(expectedTime)); // utc conversion from datetime parse
            Assert.That(resp.NewResource.Name, Is.EqualTo("resName"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetResourceVisibilityAsync(webAppName, "resName", ApiWebAppResourceVisibility.Public);
            Assert.That(resp.TrueOnSuccesResponse.Result);
            Assert.That(resp.NewResource.Visibility, Is.EqualTo(ApiWebAppResourceVisibility.Public));
            Assert.That(resp.NewResource.Name, Is.EqualTo("resName"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppSetStateAsync(webAppName, ApiWebAppState.Disabled);
            Assert.That(resp.TrueOnSuccesResponse.Result);
            Assert.That(resp.NewWebApp.Name, Is.EqualTo(webAppName));
            Assert.That(resp.NewWebApp.State, Is.EqualTo(ApiWebAppState.Disabled));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceContentHasBeenCorrupted); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppResourceContentIsNotReady); // Respond with JSON

            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

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
            Assert.Multiple(() =>
            {
                Assert.That(casted.Result, Is.EqualTo(ApiPlcOperatingMode.Run));
                Assert.That(firstcasted2.Result, Is.EqualTo(ApiPlcOperatingMode.Run));
                Assert.That(casted2.Result.First().Name, Is.EqualTo("DataTypes"));
                Assert.That(casted2.Result.First().Has_children, Is.EqualTo(true));
                Assert.That(casted3.Result, Is.EqualTo("/MiniWebCA_Cer.crt"));
                Assert.That(casted4.Result, Contains.Item(new ApiClass() { Name = "restore_plc" }));
            });
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkNoResources); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var bulkRequest = new List<IApiRequest>();
            // Does not matter!!
            bulkRequest.Add(ApiRequestFactory.GetApiPingRequest());
            Assert.ThrowsAsync<ApiBulkRequestException>(async () =>
            {
                await TestHandler.ApiBulkAsync(bulkRequest);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadSystemTime); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var systemTime = await TestHandler.PlcReadSystemTimeAsync();
            var expectedDate = new DateTime(2022, 03, 01, 17, 2, 4);
            expectedDate += TimeSpan.FromMilliseconds(238);
            expectedDate += TimeSpan.FromTicks(2960);
            Assert.That(expectedDate, Is.EqualTo(systemTime.Result.Timestamp));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadTimeSettings); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var timeSettings = await TestHandler.PlcReadTimeSettingsAsync();
            var result = timeSettings.Result;
            var dst = new DaylightSavingsTimeConfiguration(new PlcDate(3, 5, ApiDayOfWeek.Sun, 1, 0), new TimeSpan(0, 60, 0));
            var sdt = new StandardTimeConfiguration(new PlcDate(10, 5, ApiDayOfWeek.Sun, 2, 0));
            var expectedRule = new DaylightSavingsRule(sdt, dst);
            Assert.That(result.Current_offset, Is.EqualTo(TimeSpan.Zero));
            Assert.That(result.Rule, Is.EqualTo(expectedRule));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadTimeSettingsNoRule); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var timeSettings = await TestHandler.PlcReadTimeSettingsAsync();
            var result = timeSettings.Result;
            Assert.That(result.Current_offset, Is.EqualTo(TimeSpan.Zero));
            Assert.That(result.Utc_offset, Is.EqualTo(TimeSpan.Zero));
            var rule = result.Rule;
            Assert.That(rule, Is.EqualTo(null));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var expected = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            var resp = await TestHandler.PlcCreateBackupAsync();
            Assert.That(resp.Result == expected, "Failed");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var expected = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            var resp = await TestHandler.PlcRestoreBackupAsync();
            Assert.That(resp.Result == expected, "Failed");
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var expected = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            var resp = await TestHandler.PlcRestoreBackupAsync();
            Assert.That(resp.Result == expected, "Failed");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.FilesBrowseTestFolder); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var browsedResult = await TestHandler.FilesBrowseAsync();
            var result = browsedResult.Result;
            var expectedRes1 = new ApiFileResource() { Name = "pre3.png", Size = 20511, Type = ApiFileResourceType.File, Last_Modified = new DateTime(637818317050000000) };
            var expectedRes2 = new ApiFileResource() { Name = "uploadfromdev.png", Size = 110308, Type = ApiFileResourceType.File, Last_Modified = new DateTime(637818316970000000) };
            Assert.That(expectedRes1, Is.EqualTo(result.Resources[0]));
            Assert.That(expectedRes2, Is.EqualTo(result.Resources[1]));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.FilesDownloadAsync("/");
            var expectedResult = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            Assert.That(result.Result, Is.EqualTo(expectedResult));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.FilesCreateAsync("/");
            var expectedResult = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            Assert.That(result.Result, Is.EqualTo(expectedResult));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.FilesCreateDirectoryAsync("/Dir/newDir");
            Assert.That(result.Result, "The result is not true.");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.FilesRenameAsync("/Dir/newDir", "/Dir/renamedDir");
            Assert.That(result.Result, "The result is not true.");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.FilesDeleteAsync("/Dir/newDir");
            Assert.That(result.Result, "The result is not true.");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.FilesDeleteDirectoryAsync("/Dir/newDir");
            Assert.That(result.Result, "The result is not true.");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.FilesBrowseTestFolder); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var browsedResult = await TestHandler.FilesBrowseAsync();
            var result = browsedResult.Result;
            var expectedRes1 = new ApiFileResource() { Name = "pre3.png", Size = 20511, Type = ApiFileResourceType.File, Last_Modified = new DateTime(637818317050000000) };
            var expectedRes2 = new ApiFileResource() { Name = "uploadfromdev.png", Size = 110308, Type = ApiFileResourceType.File, Last_Modified = new DateTime(637818316970000000) };
            Assert.That(expectedRes1, Is.EqualTo(result.Resources[0]));
            Assert.That(expectedRes2, Is.EqualTo(result.Resources[1]));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.FilesDownloadAsync("/");
            var expectedResult = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            Assert.That(result.Result, Is.EqualTo(expectedResult));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.FilesDeleteAsync("/Dir/newDir");
            Assert.That(result.Result, "The result is not true.");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.DatalogsDownloadAndClearAsync("/");
            var expectedResult = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            Assert.That(result.Result, Is.EqualTo(expectedResult));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.SyslogResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiSyslogBrowseAsync()).Result;
            Assert.Multiple(() =>
            {
                Assert.That(result.Count_Total, Is.EqualTo(5));
                Assert.That(result.Count_Lost, Is.EqualTo(1));
                Assert.That(result.Entries.Count, Is.EqualTo(2));
                Assert.That(result.Entries[0].Raw, Is.EqualTo("I am a syslog, no need to question it!"));
                Assert.That(result.Entries[1].Raw, Is.EqualTo("I am a syslog, too. But the previous syslog is an impostor!"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.FailsafeReadParametersCPUResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.FailsafeReadParametersAsync(50);
            var fs = new FailsafeCPU();
            fs.Last_f_program_modification = new DateTime(2012, 4, 23, 18, 25, 43, 510);
            fs.Collective_signature = "BC7C0410";
            fs.Remaining_time = new TimeSpan(2, 33, 0);
            Assert.That(result.Result.Safety_mode);
            Assert.That(result.Result.Type, Is.EqualTo(ApiFailsafeHardwareType.F_cpu));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.FailsafeReadParametersModuleResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.FailsafeReadParametersAsync(50);
            var fs = new FailsafeModule();
            fs.F_monitoring_time = TimeSpan.FromMilliseconds(542);
            fs.F_destination_address = 123;
            fs.F_source_address = 321;
            fs.F_par_crc = "DBB32A1A";
            Assert.That(result.Result.Safety_mode);
            Assert.That(result.Result.Type, Is.EqualTo(ApiFailsafeHardwareType.F_module));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.FailsafeReadRuntimeGroupsResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            Assert.That(result.Result.Groups[0].Equals(noRemaining), "The groups don't match!");
            Assert.That(result.Result.Groups[1].Equals(withRemaining), "The groups don't match!");
        }

        /// <summary>
        /// TestCase for Api.ChangePassword
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T055_1_ApiChangePassword_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.ApiChangePasswordAsync("Admin", "adminpw", "newadminpw");
            Assert.That(result.Result, "Changing passwords not possible!");
        }

        /// <summary>
        /// TestCase for Api.ChangePassword
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T055_2_ApiChangePassword_RequestParam()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.ApiChangePasswordAsync("Admin", "adminpw", "newadminpw", ApiAuthenticationMode.Umc);
            Assert.That(result.Result, "Changing passwords not possible!");
        }

        /// <summary>
        /// TestCase for Api.GetPasswordPolicy
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T056_1_ApiGetPasswordPolicy_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiGetPasswordPolicy); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.ApiGetPasswordPolicyAsync();
            var expectedResult = new ApiPasswordPolicy();
            expectedResult.Requires_lowercase_characters = true;
            expectedResult.Requires_uppercase_characters = true;
            expectedResult.Min_password_length = 8;
            expectedResult.Max_password_length = 120;
            expectedResult.Min_digits = 1;
            expectedResult.Min_special_characters = 0;
            Assert.That(result.Result.Password_policy, Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// TestCase for Api.GetPasswordPolicy
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T056_2_ApiGetPasswordPolicy_RequestParam()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiGetPasswordPolicy); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.ApiGetPasswordPolicyAsync(ApiAuthenticationMode.Umc);
            var expectedResult = new ApiPasswordPolicy();
            expectedResult.Requires_lowercase_characters = true;
            expectedResult.Requires_uppercase_characters = true;
            expectedResult.Min_password_length = 8;
            expectedResult.Max_password_length = 120;
            expectedResult.Min_digits = 1;
            expectedResult.Min_special_characters = 0;
            Assert.That(result.Result.Password_policy, Is.EqualTo(expectedResult));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiGetAuthenticationModeMany); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.ApiGetAuthenticationModeAsync();
            var expectedResult = new List<ApiAuthenticationMode>();
            expectedResult.Add(ApiAuthenticationMode.Local);
            expectedResult.Add(ApiAuthenticationMode.Static);
            expectedResult.Add(ApiAuthenticationMode.Disabled);
            expectedResult.Add(ApiAuthenticationMode.Umc);
            Assert.That(result.Result.Authentication_modes.Count, Is.EqualTo(expectedResult.Count));
            Assert.That(expectedResult.SequenceEqual(result.Result.Authentication_modes), "Order of authetication modes is different!");
        }

        /// <summary>
        /// TestCase for Project.ReadLanguages with 4 languages
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T058_ProjectReadLanguages_Works_V31()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ProjectReadLanguagesMany_V31); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.ProjectReadLanguagesAsync();
            var expectedResult = new List<CultureInfo>();
            expectedResult.Add(new CultureInfo("en-US"));
            expectedResult.Add(new CultureInfo("en-GB"));
            expectedResult.Add(new CultureInfo("es-BR"));
            expectedResult.Add(new CultureInfo("ne-IN"));
            Assert.That(result.Result.Languages.Select(x => x.Language).Count(), Is.EqualTo(expectedResult.Count));
            Assert.That(expectedResult.SequenceEqual(result.Result.Languages.Select(x => x.Language)), "The order of languages are different, or they don't contain the same languages.");
        }

        /// <summary>
        /// TestCase for Project.ReadLanguages with 4 languages
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T058_ProjectReadLanguages_Works_V40()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ProjectReadLanguagesMany_V40); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.ProjectReadLanguagesAsync();

            var expectedResult1 = new ApiLanguage();
            expectedResult1.Active = true;
            expectedResult1.Language = new CultureInfo("en-US");
            expectedResult1.User_interface_languages = new List<CultureInfo>();
            expectedResult1.User_interface_languages.Add(new CultureInfo("de-DE"));
            expectedResult1.User_interface_languages.Add(new CultureInfo("en-US"));
            expectedResult1.User_interface_languages.Add(new CultureInfo("fr-FR"));

            var expectedResult2 = new ApiLanguage();
            expectedResult2.Active = false;
            expectedResult2.Language = new CultureInfo("ja-JP");
            expectedResult2.User_interface_languages = new List<CultureInfo>();

            var exp = new List<ApiLanguage>();
            exp.Add(expectedResult1);
            exp.Add(expectedResult2);

            Assert.That(exp.Count, Is.EqualTo(result.Result.Languages.Count));
            for (int i = 0; i < 2; i++)
            {
                Assert.That(result.Result.Languages[i].Equals(exp[i]));
            }
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadModeSelectorStateRun); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcReadModeSelectorStateAsync(ApiPlcRedundancyId.StandardPLC);
            Assert.That(res.Result.Mode_Selector, Is.EqualTo(ApiPlcModeSelectorState.Run));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadModeSelectorStateNoSwitch); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var res = await TestHandler.PlcReadModeSelectorStateAsync(ApiPlcRedundancyId.RedundancyId_1);
            Assert.That(res.Result.Mode_Selector, Is.EqualTo(ApiPlcModeSelectorState.NoSwitch));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TicketResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.ModulesDownloadServiceDataAsync(ApiPlcHwId.StandardPLC);
            var expectedResult = "dlBvEAfpgSVBfwlU7Py5TsVbmRTq";
            Assert.That(result.Result, Is.EqualTo(expectedResult));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.AlarmsAcknowledge); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.AlarmsAcknowledgeAsync("/");
            Assert.That(result.Result, Is.EqualTo(true));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcSetSystemTime); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.PlcSetSystemTimeAsync(new DateTime(2010, 10, 10));
            Assert.That(result.Result, "The time was not set successfully!");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcSetTimeSettingsAll); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            var offset = new TimeSpan(8, 0, 0);
            var dst = new DaylightSavingsTimeConfiguration(new PlcDate(12, 5, ApiDayOfWeek.Sun, 3, 0), new TimeSpan(0, 60, 0));
            var sdt = new StandardTimeConfiguration(new PlcDate(1, 1, ApiDayOfWeek.Mon, 23, 31));
            DaylightSavingsRule dsr = new DaylightSavingsRule(sdt, dst);

            var result = await TestHandler.PlcSetTimeSettingsAsync(offset, dsr);
            Assert.That(result.Result, "Couldn't set time settings!");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcSetTimeSettingsNoRule); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            var offset = new TimeSpan(8, 0, 0);

            var result = await TestHandler.PlcSetTimeSettingsAsync(offset);
            Assert.That(result.Result, "Couldn't set time settings!");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.QuantityStructuresResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.WebServerSetDefaultPageAsync("index.html");
            Assert.That(result.Result, "The result is not true.");
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.DefaultPageResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = await TestHandler.WebServerGetReadDefaultPageAsync();
            var expectedResult = "/~teszt2/index.html";
            Assert.That(result.Result.Default_page, Is.EqualTo(expectedResult));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiAlarmsBrowseResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiAlarmsBrowseAsync(new CultureInfo("en-US"))).Result;
            Assert.Multiple(() =>
            {
                Assert.That(result.Entries.Count, Is.EqualTo(2));
                Assert.That(result.Entries[0].Id, Is.EqualTo("9947888669857743000"));
                Assert.That(result.Entries[0].Alarm_Number, Is.EqualTo(512));
                Assert.That(result.Entries[0].Status, Is.EqualTo(ApiObjectDirectoryStatus.Incoming));
                Assert.That(result.Entries[0].Timestamp, Is.EqualTo(new DateTime(2012, 1, 1, 1, 1, 42, 47)));
                Assert.That(result.Entries[0].Acknowledgement.State, Is.EqualTo(ApiAlarmAcknowledgementState.Not_Acknowledged));
                Assert.That(result.Entries[0].Alarm_Text, Is.EqualTo("#1, 1"));
                Assert.That(result.Entries[0].Info_Text, Is.EqualTo("#1, 0"));
                Assert.That(result.Entries[0].Text_Inconsistent, Is.EqualTo(false));
                Assert.That(result.Last_Modified, Is.EqualTo(new DateTime(2012, 1, 1, 1, 1, 42, 99)));
                Assert.That(result.Count_Current, Is.EqualTo(51));
                Assert.That(result.Count_Max, Is.EqualTo(500));
                Assert.That(result.Language, Is.EqualTo("en-US"));
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
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.DiagnosticBufferBrowseResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiDiagnosticBufferBrowseAsync(new CultureInfo("en-US"))).Result;
            DateTime expected = new DateTime(2023, 06, 07, 18, 25, 43); //2023-06-07T18:25:43.514678531Z
            expected = expected.AddTicks(5146785);
            Assert.Multiple(() =>
            {
                Assert.That(result.Entries.Count, Is.EqualTo(2));
                Assert.That(result.Entries[0].Status, Is.EqualTo(ApiObjectDirectoryStatus.Outgoing));
                Assert.That(result.Entries[0].Short_Text, Is.EqualTo("Boot up  - CPU changes from OFF to STOP (initialization) mode"));
                Assert.That(result.Entries[1].Long_Text, Is.EqualTo("LONG TEXT"));
                Assert.That(result.Entries[1].Event.Textlist_Id, Is.EqualTo(3));
                Assert.That(result.Entries[1].Event.Text_Id, Is.EqualTo(26315));
                Assert.That(result.Last_Modified, Is.EqualTo(expected));
                Assert.That(result.Count_Current, Is.EqualTo(1234));
                Assert.That(result.Count_Max, Is.EqualTo(3200));
                Assert.That(result.Language, Is.EqualTo("en-US"));
            });
        }

        /// <summary>
        /// TestCase for ApiGetSessionInfo method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T069_ApiSessionInfo_FullResponseWorks()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiSessionInfoFullResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            var result = TestHandler.ApiGetSessionInfo().Result;
            Console.WriteLine($"{result}");
            Assert.Multiple(() =>
            {
                Assert.That(result.Username, Is.EqualTo("MyUser"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(ApiUserAuthenticationMode.Local));
                Assert.That(result.RuntimeTimeout, Is.EqualTo(new TimeSpan(0, 30, 0)));
                Assert.That(result.PasswordExpiration.Timestamp, Is.EqualTo(new DateTime(2012, 4, 23, 18, 25, 43)));
                Assert.That(result.PasswordExpiration.Warning, Is.EqualTo(true));
            });
        }

        /// <summary>
        /// TestCase for ApiGetSessionInfo method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T070_ApiSessionInfo_MinimumResponseWorks()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiSessionInfoMinimumResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);

            var result = TestHandler.ApiGetSessionInfo().Result;
            Assert.Multiple(() =>
            {
                Assert.That(result.Username, Is.EqualTo("Anonymous"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(ApiUserAuthenticationMode.None));
                Assert.That(result.PasswordExpiration, Is.Null);
                Assert.That(result.RuntimeTimeout, Is.Null);
            });
        }

        /// <summary>
        /// TestCase for WebApp.SetVersion method
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T071_1_ApiWebAppSetVersion_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiWebAppSetVersionAsync("testApp", "V1.2")).Result;
            Assert.That(result, Is.True);
        }

        /// <summary>
        /// TestCase for WebApp.SetVersion method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T071_2_ApiWebAppSetVersion_ApplDoesNotExist_throwsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiApplicationDoesNotExistException>(async () => await TestHandler.ApiWebAppSetVersionAsync("apple", "V1.2"));
        }

        /// <summary>
        /// TestCase for WebApp.SetVersion method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T071_3_ApiWebAppSetVersion_InvalidVersionString_throwsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InvalidVersionString); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiInvalidVersionStringException>(async () => await TestHandler.ApiWebAppSetVersionAsync("testApp", "xy"));
        }

        /// <summary>
        /// TestCase for WebApp.SetUrlRedirectMode method
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T072_1_ApiWebAppSetUrlRedirectMode_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiWebAppSetUrlRedirectModeAsync("testApp", ApiWebAppRedirectMode.Redirect)).Result;
            Assert.That(result, Is.True);
        }

        /// <summary>
        /// TestCase for WebApp.SetUrlRedirectMode method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T072_2_ApiWebAppSetUrlRedirectMode_ApplDoesNotExist_throwsExc()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebAppDoesNotExist); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiApplicationDoesNotExistException>(async () => await TestHandler.ApiWebAppSetUrlRedirectModeAsync("testApp", ApiWebAppRedirectMode.Redirect));
        }

        /// <summary>
        /// TestCase for ApiPlcReadCpuType method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T073_ApiPlcReadCpuType_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiPlcReadCpuTypeResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = TestHandler.ApiGetPlcCpuType().Result;
            Assert.Multiple(() =>
            {
                var expectedOrderNumber = "6ES7 513-1FM03-0AB0";
                var expectedProductName = "CPU 1513F-1 PN";
                Assert.That(result.Product_Name, Is.EqualTo(expectedProductName));
                Assert.That(result.Order_Number, Is.EqualTo(expectedOrderNumber));
                ApiPlcReadCpuTypeResult expected = new ApiPlcReadCpuTypeResult()
                {
                    Product_Name = expectedProductName,
                    Order_Number = expectedOrderNumber
                };
                Assert.That(result.Equals(expected), "The result object is not as expected!");
            });
        }

        /// <summary>
        /// TestCase for ApiPlcReadStationName method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T074_ApiPlcReadStationName_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiPlcReadStationNameResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = TestHandler.ApiGetPlcStationName().Result;
            var expectedStationName = "1513F";
            Assert.That(result.Station_Name, Is.EqualTo(expectedStationName));
            var expectedResult = new ApiPlcReadStationNameResult()
            {
                Station_Name = expectedStationName
            };
            Assert.That(result, Is.EqualTo(expectedResult), "The result object is not as expected!");
        }

        /// <summary>
        /// TestCase for ApiPlcReadModuleName method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T075_ApiPlcReadModuleName_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiPlcReadModuleNameResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = TestHandler.ApiGetPlcModuleName().Result;
            Assert.That(result.Module_name, Is.EqualTo("1513F"));
            var expectedResult = new ApiPlcReadModuleNameResult()
            {
                Module_name = "1513F"
            };
            Assert.That(result, Is.EqualTo(expectedResult), "The result object is not as expected!");
        }

        /// <summary>
        /// TestCase for Redundancy.ReadSystemState method
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T076_1_ApiRedundancyReadSystemState_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.RedundancyReadSystemStateResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiRedundancyReadSystemStateAsync()).Result;
            Assert.That(result.State, Is.EqualTo(ApiPlcRedundancySystemState.Run_redundant));
        }

        /// <summary>
        /// TestCase for Redundancy.ReadSystemState method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T076_2_ApiRedundancyReadSystemState_PermissionDenied()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PermissionDenied); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await TestHandler.ApiRedundancyReadSystemStateAsync());
        }

        /// <summary>
        /// TestCase for Redundancy.RequestChangeSystemState method
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T077_1_ApiRedundancyReadSystemState_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiRedundancyRequestChangeSystemStateAsync(ApiPlcRedundancySystemState.Stop)).Result;
            Assert.That(result, Is.True);
        }

        /// <summary>
        /// TestCase for Redundancy.RequestChangeSystemState method
        /// </summary>
        /// <returns></returns>
        [Test]
        public void T077_2_ApiRedundancyReadSystemState_PermissionDenied()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PermissionDenied); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await TestHandler.ApiRedundancyRequestChangeSystemStateAsync(ApiPlcRedundancySystemState.Stop));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T078_01_TechnologyRead_BoolIsFalse_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramReadFalseBool); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var resp = await TestHandler.TechnologyReadAsync<object>("\"DataTypes\".\"Bool\"");
            if ((bool)resp.Result != false)
                Assert.Fail("not casted to \"false\" bool!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T078_02_TechnologyRead_Array_works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcProgramReadRawFalseBool); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var resp = await TestHandler.TechnologyReadAsync<object>("\"DataTypes\".\"Bool\"");
            if (resp.Result is JArray)
            {
                var jarr = (JArray)resp.Result;
                var respRes = jarr.ToObject<List<bool>>();
                if (respRes[0] != false)
                    Assert.Fail("not casted to \"false\" bool!");
            }
            else
                Assert.Fail($"raw mode returned sth else than jarray: {resp.Result.GetType()}");
        }

        /// <summary>
        /// TestCase for Redundancy.ReadSystemInformation method
        /// </summary>
        [Test]
        public async Task T079_1_ApiRedundancyReadSystemInformation_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.RedundancyReadSystemInformationResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiRedundancyReadSystemInformationAsync()).Result;
            Assert.Multiple(() =>
            {
                Assert.That(result.Pairing_state, Is.EqualTo(ApiPlcRedundancyPairingState.Paired_single));
                Assert.That(result.Syncup_lock, Is.EqualTo(false));
                Assert.That((int)result.Connected_redundancy_id, Is.EqualTo(1));
                Assert.That(result.Standalone_operation, Is.EqualTo(false));

                var plc_1 = result.Plcs.Plc_1;
                Assert.That((int)plc_1.Redundancy_id, Is.EqualTo(1));
                Assert.That(plc_1.Role, Is.EqualTo(ApiPlcRedundancyRole.Backup));
                Assert.That(plc_1.Hwid, Is.EqualTo(65149));

                var plc_2 = result.Plcs.Plc_2;
                Assert.That((int)plc_2.Redundancy_id, Is.EqualTo(2));
                Assert.That(plc_2.Role, Is.EqualTo(ApiPlcRedundancyRole.Primary));
                Assert.That(plc_2.Hwid, Is.EqualTo(65349));
            });
        }

        /// <summary>
        /// TestCase for Redundancy.ReadSystemInformation method
        /// </summary>
        [Test]
        public void T079_2_ApiRedundancyReadSystemInformation_PermissionDenied()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PermissionDenied); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await TestHandler.ApiRedundancyReadSystemInformationAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T080_01_TechnologyBrowseObjects()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TechnologyBrowseObjectsResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var resp = await TestHandler.TechnologyBrowseObjectsAsync();
            var t1 = new ApiTechnologyObject() { Name = "Kinematics_1", Number = 2, Type = ApiTechnologyObjectType.To_Kinematics, Version = 6 };
            var t2 = new ApiTechnologyObject() { Name = "Int_1", Number = 3, Type = ApiTechnologyObjectType.To_Interpreter, Version = 5 };
            var exp = new List<ApiTechnologyObject>();
            exp.Add(t1);
            exp.Add(t2);

            Assert.That(resp.Result.Objects.SequenceEqual(exp));

            for (int i = 0; i < 2; i++)
            {
                Assert.That(resp.Result.Objects[i].Equals(exp[i]));
            }
            var result = new ApiTechnologyBrowseObjectsResult() { Objects = exp, Type = ApiTechnologyPlcType.S71500 };
            Assert.That(resp.Result, Is.EqualTo(result));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T080_02_TechnologyBrowseObjects_Empty()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TechnologyBrowseObjectsResponseEmpty); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var resp = await TestHandler.TechnologyBrowseObjectsAsync();
            Assert.That(!resp.Result.Objects.Any());
        }

        [Test]
        public async Task T080_03_TechnologyBrowseObjects_NearlyEmpty()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TechnologyBrowseObjectsResponse1500Empty); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var resp = await TestHandler.TechnologyBrowseObjectsAsync();
            Assert.That(!resp.Result.Objects.Any());
            Assert.That(resp.Result.Type, Is.EqualTo(ApiTechnologyPlcType.S71500));
        }

        /// <summary>
        /// TestCase for Redundancy.ReadSyncupProgress method
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T081_1_ApiRedundancyReadSyncupProgress_copying_work_memory()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.RedundancyReadSyncupProgress_CopyingWorkMemoryResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiRedundancyReadSyncupProgressAsync()).Result;
            Assert.Multiple(() =>
            {
                Assert.That(result.Syncup_phase, Is.EqualTo(ApiRedundancySyncupPhase.Copying_work_memory));
                Assert.That(result.Copying_work_memory.Current, Is.EqualTo(1000));
                Assert.That(result.Copying_work_memory.Total, Is.EqualTo(100000));
                Assert.That(result.Copying_memory_card, Is.Null, "result.Copying_memory_card");
                Assert.That(result.Minimizing_delay, Is.Null, "result.Minimizing_delay");
            });
        }

        /// <summary>
        /// TestCase for Redundancy.ReadSyncupProgress method
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T081_2_ApiRedundancyReadSyncupProgress_copying_memory_card()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.RedundancyReadSyncupProgress_CopyingMemoryCardResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiRedundancyReadSyncupProgressAsync()).Result;
            Assert.Multiple(() =>
            {
                Assert.That(result.Syncup_phase, Is.EqualTo(ApiRedundancySyncupPhase.Copying_memory_card));
                Assert.That(result.Copying_memory_card.Current_filename, Is.EqualTo("/DataLogs/MyDataLog_1.csv"), "result.Copying_memory_card.Current");
                Assert.That(result.Copying_memory_card.Current, Is.EqualTo(17024));
                Assert.That(result.Copying_memory_card.Total, Is.EqualTo(2045000));
                Assert.That(result.Copying_work_memory, Is.Null, "result.Copying_work_memory");
                Assert.That(result.Minimizing_delay, Is.Null, "result.Minimizing_delay");
            });
        }

        /// <summary>
        /// TestCase for Redundancy.ReadSyncupProgress method
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T081_3_ApiRedundancyReadSyncupProgress_minimizing_delay()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.RedundancyReadSyncupProgress_MinimizingDelayResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiRedundancyReadSyncupProgressAsync()).Result;
            Assert.Multiple(() =>
            {
                Assert.That(result.Syncup_phase, Is.EqualTo(ApiRedundancySyncupPhase.Minimizing_delay));
                Assert.That(result.Minimizing_delay.Hypothetical_cycle_time, Is.EqualTo(new TimeSpan(0, 0, 1)), "result.Minimizing_delay.Hypothetical_cycle_time");
                Assert.That(result.Minimizing_delay.Tolerable_cycle_time, Is.EqualTo(new TimeSpan(0, 0, 0, 0, 800)), "result.Minimizing_delay.Tolerable_cycle_time");
                Assert.That(result.Copying_work_memory, Is.Null, "result.Copying_work_memory");
                Assert.That(result.Copying_memory_card, Is.Null, "result.Copying_memory_card");
            });
        }

        /// <summary>
        /// TestCase for WebServer.ReadResponseHeaders method
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task T082_ApiWebServerReadResponseHeaders_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.WebServerReadResponseHeadersResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiWebServerReadResponseHeadersAsync()).Result;
            Assert.Multiple(() =>
            {
                Assert.That(result.Configured_headers.Count, Is.EqualTo(1), "result.Configured_headers.Count");
                Assert.That(result.Allowed_headers.Count, Is.EqualTo(1), "result.Allowed_headers.Count");
            });
            Assert.Multiple(() =>
            {
                Assert.That(result.Configured_headers[0].Pattern, Is.EqualTo("~**/*"), "result.Configured_headers[0].Pattern");
                Assert.That(result.Configured_headers[0].Header, Is.EqualTo("Content-Security-Policy: frame-ancestors *.somesite.com;"), "result.Configured_headers[0].Header");
                Assert.That(result.Allowed_headers[0].Pattern, Is.EqualTo("~**/*"), "result.Allowed_headers[0].Pattern");
                Assert.That(result.Allowed_headers[0].Key, Is.EqualTo("Content-Security-Policy"), "result.Allowed_headers[0].Key");
            });
        }

        /// <summary>
        /// TestCase for WebServer.ChangeResponseHeaders method
        /// </summary>
        [Test]
        public async Task T083_1_ApiWebServerChangeResponseHeaders_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TrueOnSuccess); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var result = (await TestHandler.ApiWebServerChangeResponseHeadersAsync("this is the header")).Result;
            Assert.That(result, Is.True);
        }

        /// <summary>
        /// TestCase for WebServer.ChangeResponseHeaders method
        /// </summary>
        [Test]
        public void T083_2_ApiWebServerChangeResponseHeaders_InvalidpatternException()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.InvalidPattern); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiInvalidPatternException>(async () => await TestHandler.ApiWebServerChangeResponseHeadersAsync("this is the header"));
        }

        /// <summary>
        /// TestCase for WebServer.ChangeResponseHeaders method
        /// </summary>
        [Test]
        public void T083_3_ApiWebServerChangeResponseHeaders_HTTPHeaderNotAllowed()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.HTTPHeaderNotAllowed); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiHTTPHeaderNotAllowedException>(async () => await TestHandler.ApiWebServerChangeResponseHeadersAsync("this is the header"));
        }

        /// <summary>
        /// TestCase for WebServer.ChangeResponseHeaders method
        /// </summary>
        [Test]
        public void T083_4_ApiWebServerChangeResponseHeaders_HTTPHeaderNotAllowed()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.HTTPHeaderInvalid); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiHTTPHeaderInvalidException>(async () => await TestHandler.ApiWebServerChangeResponseHeadersAsync("this is the header"));
        }

        /// <summary>
        /// TestCase for WebServer.ChangeResponseHeaders method
        /// </summary>
        [Test]
        public void T083_5_ApiWebServerChangeResponseHeaders_TooManyHTTPHeaders()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.TooManyHTTPHeaders); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiTooManyHTTPHeadersException>(async () => await TestHandler.ApiWebServerChangeResponseHeadersAsync("this is the header"));
        }

        /// <summary>
        /// TestCase for WebServer.ChangeResponseHeaders method
        /// </summary>
        [Test]
        public void T083_6_ApiWebServerChangeResponseHeaders_RequestTooLarge()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.RequestTooLarge); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            Assert.ThrowsAsync<ApiRequestTooLargeException>(async () => await TestHandler.ApiWebServerChangeResponseHeadersAsync("this is the header"));
        }

        [Test]
        public void T090_ApiLogin_WithErrorMessage_DoesNotThrowIncludingApiRequestString()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.RequestTooLarge); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var username = "ThisIsTheUserNameToBeUsed";
            var password = "ThisIsThePasswordToBeUsed";
            var exc = Assert.ThrowsAsync<ApiRequestTooLargeException>(async () => await TestHandler.ApiLoginAsync(username, password));
            Assert.Multiple(() =>
            {
                Assert.That(!exc.Message.Contains(username), "Username has been provided in the exception thrown!");
                Assert.That(!exc.Message.Contains(password), "Password has been provided in the exception thrown!");
                var currentExc = exc.InnerException;
                while (currentExc != null)
                {
                    Assert.That(!currentExc.Message.Contains(username), "Username has been provided in the exception thrown!");
                    Assert.That(!currentExc.Message.Contains(password), "Password has been provided in the exception thrown!");
                    currentExc = currentExc.InnerException;
                }
            });
        }

        [Test]
        public void T091_ReLogin_WithErrorMessage_DoesNotThrowIncludingApiRequestString()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.RequestTooLarge); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var username = "ThisIsTheUserNameToBeUsed";
            var password = "ThisIsThePasswordToBeUsed";
            var exc = Assert.ThrowsAsync<ApiRequestTooLargeException>(async () => await TestHandler.ReLoginAsync(username, password));
            Assert.Multiple(() =>
            {
                Assert.That(!exc.Message.Contains(username), "Username has been provided in the exception thrown!");
                Assert.That(!exc.Message.Contains(password), "Password has been provided in the exception thrown!");
                var currentExc = exc.InnerException;
                while (currentExc != null)
                {
                    Assert.That(!currentExc.Message.Contains(username), "Username has been provided in the exception thrown!");
                    Assert.That(!currentExc.Message.Contains(password), "Password has been provided in the exception thrown!");
                    currentExc = currentExc.InnerException;
                }
            });
        }

        [Test]
        public void T092_ApiChangePassword_WithErrorMessage_DoesNotThrowIncludingApiRequestString()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.RequestTooLarge); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var username = "ThisIsTheUserNameToBeUsed";
            var password = "ThisIsThePasswordToBeUsed";
            var nextPassword = "ThisIsTheNewPasswordToBeApplied";
            var exc = Assert.ThrowsAsync<ApiRequestTooLargeException>(async () => await TestHandler.ApiChangePasswordAsync(username, password, nextPassword));
            Assert.Multiple(() =>
            {
                Assert.That(!exc.Message.Contains(username), "Username has been provided in the exception thrown!");
                Assert.That(!exc.Message.Contains(password), "Password has been provided in the exception thrown!");
                Assert.That(!exc.Message.Contains(nextPassword), "New Password has been provided in the exception thrown!");
                var currentExc = exc.InnerException;
                while (currentExc != null)
                {
                    Assert.That(!currentExc.Message.Contains(username), "Username has been provided in the exception thrown!");
                    Assert.That(!currentExc.Message.Contains(password), "Password has been provided in the exception thrown!");
                    Assert.That(!currentExc.Message.Contains(nextPassword), "New Password has been provided in the exception thrown!");
                    currentExc = currentExc.InnerException;
                }
            });
        }

        [Test]
        public void T093_ApiBulkWithLogin_WithErrorMessage_DoesNotThrowIncludingApiRequestString()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBulkNoResources); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var username = "ThisIsTheUserNameToBeUsed";
            var password = "ThisIsThePasswordToBeUsed";
            var nextPassword = "ThisIsTheNewPasswordToBeApplied";
            var request1 = ApiRequestFactory.GetApiLoginRequest(username, password);
            var request2 = ApiRequestFactory.GetApiChangePasswordRequest(username, password, nextPassword);
            var exc = Assert.CatchAsync(async () => await TestHandler.ApiBulkAsync(new List<IApiRequest>() { request1, request2 }));
            Assert.Multiple(() =>
            {
                Assert.That(!exc.Message.Contains(username), "Username has been provided in the exception thrown!");
                Assert.That(!exc.Message.Contains(password), "Password has been provided in the exception thrown!");
                Assert.That(!exc.Message.Contains(nextPassword), "New Password has been provided in the exception thrown!");
                var currentExc = exc.InnerException;
                while (currentExc != null)
                {
                    Assert.That(!currentExc.Message.Contains(username), "Username has been provided in the exception thrown!");
                    Assert.That(!currentExc.Message.Contains(password), "Password has been provided in the exception thrown!");
                    Assert.That(!currentExc.Message.Contains(nextPassword), "New Password has been provided in the exception thrown!");
                    currentExc = currentExc.InnerException;
                }
            });
        }

        [Test]
        public void T093_RestoreBackup_WithErrorMessage_DoesNotThrowIncludingApiRequestString()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.RequestTooLarge); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var password = "ThisIsThePasswordToBeUsed";
            var exc = Assert.ThrowsAsync<ApiRequestTooLargeException>(async () => await TestHandler.PlcRestoreBackupAsync(password));
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

        [Test]
        public async Task T094_Modules_Browse()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiModulesBrowseResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = await TestHandler.ModulesBrowseAsync(mode: "children");
            Console.WriteLine($"first has children: {response.Result.Nodes.First().HasChildren}, child has children: {response.Result.Nodes.First().Children.First().HasChildren}");
            Assert.Multiple(() =>
            {
                Assert.That(response.Result.Nodes.Count, Is.EqualTo(1));
                Assert.That(response.Result.Nodes.First(), Is.EqualTo(new Module()
                {
                    Hwid = 32,
                    Type = ApiModulesNodeType.Device,
                    Name = "CPU1518",
                    SubType = ApiModulesNodeSubType.CentralDevice,
                    HasChildren = true,
                    Children = new List<Module>()
                { new Module() { Hwid = 49, Name ="CPU1518", Type = ApiModulesNodeType.Module, SubType = ApiModulesNodeSubType.Cpu, Attributes = new List<ApiModulesNodeAttribute>() { ApiModulesNodeAttribute.FirmwareUpdate, ApiModulesNodeAttribute.ServiceData }, HasChildren = true }  }
                }));
            });
        }

        [Test]
        public async Task T095_01_Modules_Read_Leds_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiModulesReadLedsResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = await TestHandler.ModulesReadLedsAsync(0);
            Assert.Multiple(() =>
            {
                Assert.That(response.Result.Leds.Count, Is.EqualTo(3));
                Assert.That(response.Result.Leds[0], Is.EqualTo(new ModulesLed() { Colors = new List<ApiLedColor>() { ApiLedColor.Green }, Status = ApiLedStatus.On, Type = ApiLedType.RunStop }));
                Assert.That(response.Result.Leds[1], Is.EqualTo(new ModulesLed() { Status = ApiLedStatus.Off, Type = ApiLedType.Error }));
                Assert.That(response.Result.Leds[2], Is.EqualTo(new ModulesLed() { Status = ApiLedStatus.Off, Type = ApiLedType.Maintenance }));
            });
        }

        [Test]
        public async Task T095_02_Modules_Read_Leds_Works()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiModulesReadLedsFlashingResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = await TestHandler.ModulesReadLedsAsync(0);
            Assert.Multiple(() =>
            {
                Assert.That(response.Result.Leds.Count, Is.EqualTo(3));
                Assert.That(response.Result.Leds[0], Is.EqualTo(new ModulesLed() { Colors = new List<ApiLedColor>() { ApiLedColor.Green, ApiLedColor.Yellow }, Status = ApiLedStatus.Flashing, Type = ApiLedType.RunStop, Period = ParseISO8601Duration("PT0.5S") }));
                Assert.That(response.Result.Leds[1], Is.EqualTo(new ModulesLed() { Colors = new List<ApiLedColor>() { ApiLedColor.Red }, Status = ApiLedStatus.Flashing, Type = ApiLedType.Error, Period = ParseISO8601Duration("PT0.5S") }));
                Assert.That(response.Result.Leds[2], Is.EqualTo(new ModulesLed() { Colors = new List<ApiLedColor>() { ApiLedColor.Yellow }, Status = ApiLedStatus.Flashing, Type = ApiLedType.Maintenance, Period = ParseISO8601Duration("PT0.5S") }));
            });
        }

        [Test]
        public async Task T096_PlcReadMemoryInformationResponse()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadMemoryInformationResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            PlcMemoryInformationResponse response = await TestHandler.PlcReadMemoryInformationAsync();
            Assert.Multiple(() =>
            {
                var codeWorkMemory = new PlcMemoryInformationAbsolute() { FreeBytes = 11816885, TotalBytes = 12582912 };
                var dataWorkMemory = new PlcMemoryInformationAbsolute() { FreeBytes = 148802946, TotalBytes = 157286400 };
                var retentiveMemory = new PlcMemoryInformationAbsolute() { FreeBytes = 4704634, TotalBytes = 4719264 };
                var dataTypeMemory = new PlcMemoryInformationPercentage() { FreePercentage = (float)98.6 };
                var plcMemoryInf = new PlcMemoryInformation() { CodeWorkMemory = codeWorkMemory, DataWorkMemory = dataWorkMemory, RetentiveMemory = retentiveMemory, DataTypeMemory = dataTypeMemory };
                Assert.That(response.Result.CodeWorkMemory, Is.EqualTo(codeWorkMemory));
                Assert.That(response.Result.DataWorkMemory, Is.EqualTo(dataWorkMemory));
                Assert.That(response.Result.RetentiveMemory, Is.EqualTo(retentiveMemory));
                Assert.That(response.Result.DataTypeMemory, Is.EqualTo(dataTypeMemory));
                Assert.That(response.Result, Is.EqualTo(plcMemoryInf));
            });
        }

        [Test]
        public async Task T097_ProjectReadInformation()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ProjectReadInformationResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            ProjectInformationResponse response = await TestHandler.ProjectReadInformationAsync();
            Assert.Multiple(() =>
            {
                var expectedVersions = new List<ProjectInformationVersion>()
                {
                     new ProjectInformationVersion() { Source = ApiVersionSource.TiaPortal, Version = "V21.0.0.0"}
                };
                ProjectInformation expected = new ProjectInformation() { ProjectName = "ThisIsAnAwesomeTestProjectName", Versions = expectedVersions };
                Assert.That(response.Result.Versions.SequenceEqual(expectedVersions));
                Assert.That(response.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T098_PlcRuntimeInformation()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcRuntimeInformationResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            PlcRuntimeInformationResponse response = await TestHandler.PlcReadRuntimeInformationAsync();
            Assert.Multiple(() =>
            {

                var expectedLoad = new PlcRuntimeInformationLoad()
                {
                    Actual = new PlcRuntimeInformationLoadActual() { ProgramLoadCyclicProgramObsPercentage = 0, ProgramLoadHighPriorityObsPercentage = 0, CurrentCommunicationLoadPercentage = 1 },
                    Configured = new PlcRuntimeInformationLoadConfigured() { MaxCommunicationLoadPercentage = 50 }
                };
                var expectedCycleTime = new PlcRuntimeInformationCycleTime()
                {
                    Actual = new PlcRuntimeInformationCycleTimeActual()
                    {
                        Shortest = ParseISO8601Duration("PT0.000206S"),
                        Current = ParseISO8601Duration("PT0.000973S"),
                        Longest = ParseISO8601Duration("PT2.009973S")
                    },
                    Configured = new PlcRuntimeInformationCycleTimeConfigured() { Min = TimeSpan.Zero, Max = ParseISO8601Duration("PT0.15S") }
                };
                var expected = new PlcRuntimeInformation() { Load = expectedLoad, CycleTime = expectedCycleTime };
                Assert.That(response.Result.Load, Is.EqualTo(expectedLoad));
                Assert.That(response.Result.CycleTime, Is.EqualTo(expectedCycleTime));
                Assert.That(response.Result, Is.EqualTo(expected));
            });
        }

        private TimeSpan ParseISO8601Duration(string iso8601Duration)
        {
            // Use your existing TimeSpanISO8601Converter
            var converter = new TimeSpanISO8601Converter();
            using (var reader = new JsonTextReader(new StringReader($"\"{iso8601Duration}\"")))
            {
                reader.Read();
                return (TimeSpan)converter.ReadJson(reader, typeof(TimeSpan), null, JsonSerializer.Create());
            }
        }


        [Test]
        public async Task T099_CommunicationReadProtocolResources()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.CommunicationReadProtocolResourcesResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            CommunicationProtocolResourcesResponse response = await TestHandler.CommunicationReadProtocolResourcesAsync();
            Assert.Multiple(() =>
            {
                var expectedProtocols = new CommunicationProtocolResourcesProtocols() { Hmi = new CommunicationProtocolResourcesProtocolsHmi() { Subscriptions = new CommunicationProtocolResourcesProtocolsHmiSubscriptions() { Free = 750, Max = 750 } } };
                var expected = new CommunicationProtocolResources() { Protocols = expectedProtocols };
                Assert.That(response.Result, Is.EqualTo(expected));
            });
        }


        [Test]
        public async Task T100_ModulesReadParameters()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadParametersResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            ModulesParametersResponse response = await TestHandler.ModulesReadParametersAsync(49);
            Assert.Multiple(() =>
            {
                var expectedGeneral = new ModulesNodeGeneral()
                {
                    Name = "CPU1518",
                    Class = "CPU 1518-3 PN",
                    Type = ApiModulesNodeType.Module,
                    Sub_Type = ApiModulesNodeSubType.None,
                    Attributes = new List<ApiModulesNodeAttribute>() { ApiModulesNodeAttribute.FirmwareUpdate, ApiModulesNodeAttribute.ServiceData },
                };
                var expectedGeoAddress = new ModulesNodeGeoAddress()
                {
                    Actual = new ModulesNodeGeoAddressActual() { IoSystem = 0, Device = 0, Slot = 1, Subslot = 0, Rack = 0, RemovedByConfigurationControl = false },
                    Configured = new ModulesNodeGeoAddressConfigured() { IoSystem = 0, Device = 0, Slot = 1, Subslot = 0, Rack = 0 },
                };
                var expectedParameters = new ModulesNodeParameters()
                {
                    Versions = new ModulesNodeVersions()
                    {
                        Bootloader = new ModuleVersion() { Type = 'V', Major = 4, Patch = 1, Minor = 0 },
                        Motion = new ModulesNodeVersions_Motion() { Packages = new List<ModulesNodeVersions_Motion_Package>() { new ModulesNodeVersions_Motion_Package() { Name = "MC Base", External = "V10.0.2" } } }
                    }
                };
                var expected = new ModulesParameters()
                {
                    General = expectedGeneral,
                    GeoAddress = expectedGeoAddress,
                    Parameters = expectedParameters
                };
                Assert.That(response.Result.General, Is.EqualTo(expectedGeneral));
                Assert.That(response.Result.GeoAddress, Is.EqualTo(expectedGeoAddress));
                Assert.That(response.Result.Parameters, Is.EqualTo(expectedParameters));
                Assert.That(response.Result.Parameters.Versions, Is.EqualTo(expectedParameters.Versions));
                Assert.That(response.Result.Parameters.Versions.Motion, Is.EqualTo(expectedParameters.Versions.Motion));
                Assert.That(response.Result.Parameters.Versions.Bootloader, Is.EqualTo(expectedParameters.Versions.Bootloader));
                Assert.That(response.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T101_ModulesReadIdentificationMaintenanceAsync_IM0_Actual()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadIdentificationMaintenanceIm0ResponseActualResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = (await TestHandler.ModulesReadIdentificationMaintenanceAsync(49, (uint)0, "actual"));
            var casted = response as ModulesIMxResponse<ModulesIdentificationMaintenance_IM0_Data>;
            Assert.That(casted, Is.Not.Null);
            Assert.That(casted.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expectedData = new ModulesIdentificationMaintenance_IM0_Data() { ManufacturerId = 42, OrderNumber = "6ES7 518-3AT10-0AB0 ", SerialNumber = "10S C-521E1fFj24", SoftwareRevision = new ModuleVersion() { Type = 'V', Major = 4, Minor = 1, Patch = 0 }, ImSupported = 14, ImVersion = new ModulesIdentificationMaintenance_IM0_ImVersion() { Minor = 1, Major = 1 } };
                var expected = new ModulesIMxResult<ModulesIdentificationMaintenance_IM0_Data>() { Data = expectedData };
                Assert.That(casted.Result.Data, Is.EqualTo(expected.Data));
                Assert.That(casted.Result.Data.SoftwareRevision, Is.EqualTo(expected.Data.SoftwareRevision));
                Assert.That(casted.Result.Data.ImVersion, Is.EqualTo(expected.Data.ImVersion));
                Assert.That(casted.Result, Is.EqualTo(expected));
            });
        }


        [Test]
        public async Task T102_ModulesReadIdentificationMaintenanceAsync_IM1_Actual()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadIdentificationMaintenanceIm1ResponseActualResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = (await TestHandler.ModulesReadIdentificationMaintenanceAsync(49, (uint)1, "actual"));
            var casted = response as ModulesIMxResponse<ModulesIdentificationMaintenance_IM1_Data>;
            Assert.That(casted, Is.Not.Null);
            Assert.That(casted.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expectedData = new ModulesIdentificationMaintenance_IM1_Data() { PlantDesignation = "CPU1518                         ", LocationIdentifier = "                      " };
                var expected = new ModulesIMxResult<ModulesIdentificationMaintenance_IM1_Data>() { Data = expectedData };
                Assert.That(casted.Result.Data, Is.EqualTo(expected.Data));
                Assert.That(casted.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T103_ModulesReadIdentificationMaintenanceAsync_IM2_Actual()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadIdentificationMaintenanceIm2ResponseActualResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = (await TestHandler.ModulesReadIdentificationMaintenanceAsync(49, (uint)2, "actual"));
            var casted = response as ModulesIMxResponse<ModulesIdentificationMaintenance_IM2_Data>;
            Assert.That(casted, Is.Not.Null);
            Assert.That(casted.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expectedData = new ModulesIdentificationMaintenance_IM2_Data() { InstallationDate = DateTime.Parse("2025-10-10 07:31") };
                var expected = new ModulesIMxResult<ModulesIdentificationMaintenance_IM2_Data>() { Data = expectedData };
                Assert.That(casted.Result.Data, Is.EqualTo(expected.Data));
                Assert.That(casted.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T104_ModulesReadIdentificationMaintenanceAsync_IM3_Actual()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadIdentificationMaintenanceIm3ResponseActualResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = (await TestHandler.ModulesReadIdentificationMaintenanceAsync(49, (uint)3, "actual"));
            var casted = response as ModulesIMxResponse<ModulesIdentificationMaintenance_IM3_Data>;
            Assert.That(casted, Is.Not.Null);
            Assert.That(casted.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expectedData = new ModulesIdentificationMaintenance_IM3_Data() { AdditionalInformation = "ThisIsAnAwesomeTestProjectName                           " };
                var expected = new ModulesIMxResult<ModulesIdentificationMaintenance_IM3_Data>() { Data = expectedData };
                Assert.That(casted.Result.Data, Is.EqualTo(expected.Data));
                Assert.That(casted.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T105_ModulesReadIdentificationMaintenanceAsync_IM0_Configured()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadIdentificationMaintenanceIm0ResponseConfiguredResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = (await TestHandler.ModulesReadIdentificationMaintenanceAsync(49, (uint)0, "configured"));
            var casted = response as ModulesIMxResponse<ModulesIdentificationMaintenance_IM0_Data>;
            Assert.That(casted, Is.Not.Null);
            Assert.That(casted.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expectedData = new ModulesIdentificationMaintenance_IM0_Data() { ManufacturerId = 42, OrderNumber = "6ES7 518-3AT10-0AB0 ", SerialNumber = "                ", SoftwareRevision = new ModuleVersion() { Type = 'V', Major = 4, Minor = 1, Patch = 0 }, ImSupported = 0, ImVersion = new ModulesIdentificationMaintenance_IM0_ImVersion() { Minor = 0, Major = 0 } };
                var expected = new ModulesIMxResult<ModulesIdentificationMaintenance_IM0_Data>() { Data = expectedData };
                Assert.That(casted.Result.Data, Is.EqualTo(expected.Data));
                Assert.That(casted.Result.Data.SoftwareRevision, Is.EqualTo(expected.Data.SoftwareRevision));
                Assert.That(casted.Result.Data.ImVersion, Is.EqualTo(expected.Data.ImVersion));
                Assert.That(casted.Result, Is.EqualTo(expected));
            });
        }


        [Test]
        public async Task T106_ModulesReadIdentificationMaintenanceAsync_IM1_Configured()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadIdentificationMaintenanceIm1ResponseConfiguredResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = (await TestHandler.ModulesReadIdentificationMaintenanceAsync(49, (uint)1, "configured"));
            var casted = response as ModulesIMxResponse<ModulesIdentificationMaintenance_IM1_Data>;
            Assert.That(casted, Is.Not.Null);
            Assert.That(casted.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expectedData = new ModulesIdentificationMaintenance_IM1_Data() { PlantDesignation = "CPU1518                         ", LocationIdentifier = "                      " };
                var expected = new ModulesIMxResult<ModulesIdentificationMaintenance_IM1_Data>() { Data = expectedData };
                Assert.That(casted.Result.Data, Is.EqualTo(expected.Data));
                Assert.That(casted.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T107_ModulesReadIdentificationMaintenanceAsync_IM2_Configured()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadIdentificationMaintenanceIm2ResponseConfiguredResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = (await TestHandler.ModulesReadIdentificationMaintenanceAsync(49, (uint)2, "configured"));
            var casted = response as ModulesIMxResponse<ModulesIdentificationMaintenance_IM2_Data>;
            Assert.That(casted, Is.Not.Null);
            Assert.That(casted.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expectedData = new ModulesIdentificationMaintenance_IM2_Data() { InstallationDate = DateTime.Parse("2025-10-10 07:31") };
                var expected = new ModulesIMxResult<ModulesIdentificationMaintenance_IM2_Data>() { Data = expectedData };
                Assert.That(casted.Result.Data, Is.EqualTo(expected.Data));
                Assert.That(casted.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T108_01_ModulesReadIdentificationMaintenanceAsync_IM3_Configured()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadIdentificationMaintenanceIm3ResponseConfiguredResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = (await TestHandler.ModulesReadIdentificationMaintenanceAsync(49, (uint)3, "configured"));
            var casted = response as ModulesIMxResponse<ModulesIdentificationMaintenance_IM3_Data>;
            Assert.That(casted, Is.Not.Null);
            Assert.That(casted.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expectedData = new ModulesIdentificationMaintenance_IM3_Data() { AdditionalInformation = "ThisIsAnAwesomeTestProjectName                           " };
                var expected = new ModulesIMxResult<ModulesIdentificationMaintenance_IM3_Data>() { Data = expectedData };
                Assert.That(casted.Result.Data, Is.EqualTo(expected.Data));
                Assert.That(casted.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T108_02_ModulesReadIdentificationMaintenanceAsync_IM3_Configured()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadIdentificationMaintenanceIm3ResponseConfiguredResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            var response = (await TestHandler.ModulesReadIdentificationMaintenanceAsync(49, ModulesReadIdentificationMaintenanceNumber.Im3, "configured"));
            var casted = response as ModulesIMxResponse<ModulesIdentificationMaintenance_IM3_Data>;
            Assert.That(casted, Is.Not.Null);
            Assert.That(casted.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expectedData = new ModulesIdentificationMaintenance_IM3_Data() { AdditionalInformation = "ThisIsAnAwesomeTestProjectName                           " };
                var expected = new ModulesIMxResult<ModulesIdentificationMaintenance_IM3_Data>() { Data = expectedData };
                Assert.That(casted.Result.Data, Is.EqualTo(expected.Data));
                Assert.That(casted.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T109_ModulesReadStatus_en()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadStatusResponse_EN); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            ModulesStatusResponse response = (await TestHandler.ModulesReadStatusAsync(49, new CultureInfo("en-us")));
            Assert.That(response.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expected = new ModulesStatus() { Status = new ModulesStatusDetails() { Own = ApiModulesNodeState.Good, Subordinate = ApiModulesNodeState.Good }, Language = new CultureInfo("en-US") };
                Assert.That(response.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T110_ModulesReadStatus_de()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadStatusResponse_DE); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            ModulesStatusResponse response = (await TestHandler.ModulesReadStatusAsync(49, new CultureInfo("de-de")));
            Assert.That(response.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expected = new ModulesStatus() { Status = new ModulesStatusDetails() { Own = ApiModulesNodeState.Good, Subordinate = ApiModulesNodeState.Good }, Language = new CultureInfo("de-de") };
                Assert.That(response.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T110_ModulesReadStatus_invalidLanguage()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ModulesReadStatusResponse_InvalidCulture); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            ModulesStatusResponse response = (await TestHandler.ModulesReadStatusAsync(49));
            Assert.That(response.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expected = new ModulesStatus() { Status = new ModulesStatusDetails() { Own = ApiModulesNodeState.Good, Subordinate = ApiModulesNodeState.Good } };
                Assert.That(response.Result, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task T111_PlcReadLoadMemoryInformationResponse()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.PlcReadLoadMemoryInformationResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory, ApiResponseChecker, ApiRequestSplitter);
            PlcLoadMemoryInformationResponse response = (await TestHandler.PlcReadLoadMemoryInformationAsync());
            Assert.That(response.Result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                var expected = new PlcLoadMemoryInformation() { LoadMemory = new PlcLoadMemory() { Aging = new PlcLoadMemoryAging() { }, FreeBytes = 22499328, TotalBytes = 25176064 } };
                Assert.That(response.Result, Is.EqualTo(expected));
            });
        }

        //
    }
}
