// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Requests;
using Siemens.Simatic.S7.Webserver.API.Responses;
using Siemens.Simatic.S7.Webserver.API.Services.PlcProgram;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class ApiRequestTests : Base
    {
        
        [Test]
        public async Task T001_ApiBrowseRequest()
        {
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(HttpMethod.Post, $"https://{Ip.ToString()}/api/jsonrpc")
                .Respond("application/json", ResponseStrings.ApiBrowseResponse); // Respond with JSON
            // Inject the handler or client into your application code
            var client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri($"https://{Ip.ToString()}");
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            var browseResExpectedList = new List<ApiClass>() { new ApiClass() { Name = "Api.Browse" }, new ApiClass() { Name = "Api.CloseTicket" }, new ApiClass() { Name = "Api.GetCertificateUrl" }, new ApiClass() { Name = "Api.GetPermissions" }, new ApiClass() { Name = "Api.BrowseTickets" }, new ApiClass() { Name = "Api.Login" }, new ApiClass() { Name = "Api.Logout" }, new ApiClass() { Name = "Api.Ping" }, new ApiClass() { Name = "Api.Version" }, new ApiClass() { Name = "Plc.ReadOperatingMode" }, new ApiClass() { Name = "Plc.RequestChangeOperatingMode" }, new ApiClass() { Name = "PlcProgram.Browse" }, new ApiClass() { Name = "PlcProgram.Read" }, new ApiClass() { Name = "PlcProgram.Write" }, new ApiClass() { Name = "WebApp.Browse" }, new ApiClass() { Name = "WebApp.BrowseResources" }, new ApiClass() { Name = "WebApp.Create" }, new ApiClass() { Name = "WebApp.CreateResource" }, new ApiClass() { Name = "WebApp.Delete" }, new ApiClass() { Name = "WebApp.DeleteResource" }, new ApiClass() { Name = "WebApp.DownloadResource" }, new ApiClass() { Name = "WebApp.Rename" }, new ApiClass() { Name = "WebApp.RenameResource" }, new ApiClass() { Name = "WebApp.SetDefaultPage" }, new ApiClass() { Name = "WebApp.SetNotAuthorizedPage" }, new ApiClass() { Name = "WebApp.SetNotFoundPage" }, new ApiClass() { Name = "WebApp.SetResourceETag" }, new ApiClass() { Name = "WebApp.SetResourceMediaType" }, new ApiClass() { Name = "WebApp.SetResourceModificationTime" }, new ApiClass() { Name = "WebApp.SetResourceVisibility" }, new ApiClass() { Name = "WebApp.SetState" } };
            var browseRes = (await TestHandler.ApiBrowseAsync()).Result;
            if (!browseResExpectedList.SequenceEqual(browseRes))
            {
                var unexpected = browseRes.Except(browseResExpectedList);
                var expectedButMissing = browseResExpectedList.Except(browseRes);
                string assertString = $"Api Browse result is not equal!unexpectedly found:{Environment.NewLine}";
                foreach(var res in unexpected)
                {
                    assertString += $"{res.Name}";
                }
                assertString += $"{Environment.NewLine}expected but did not find: {Environment.NewLine}";
                foreach(var res in expectedButMissing)
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            List<ApiClass> expRes = new List<ApiClass>() { new ApiClass() { Name = "read_diagnostics" }, new ApiClass() { Name = "read_value" }, new ApiClass() { Name = "write_value" }, new ApiClass() { Name = "acknowledge_alarms" }, new ApiClass() { Name = "firmware_update" }, new ApiClass() { Name = "open_user_pages" }, new ApiClass() { Name = "read_file" }, new ApiClass() { Name = "write_file" }, new ApiClass() { Name = "change_operating_mode" }, new ApiClass() { Name = "flash_leds" }, new ApiClass() { Name = "backup_plc" }, new ApiClass() { Name = "restore_plc" }, new ApiClass() { Name = "manage_user_pages" } };
            var resp = await TestHandler.ApiGetPermissionsAsync();
            if (resp.Result == expRes)
                Assert.Fail("correct server response but not correctly given to user with true!");
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            List<ApiClass> expRes = new List<ApiClass>();
            var resp = await TestHandler.ApiGetPermissionsAsync();
            if (resp.Result == expRes)
                Assert.Fail("correct server response but not correctly given to user with true!");
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            string chars28 = "abcdefghijklmnopqrstuvwxyzab";
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () => await TestHandler.ApiCloseTicketAsync(chars28+"a"));
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            var req = ApiRequestFactory.GetApiLoginRequest("Everybody", "", true);
            var res = JsonConvert.DeserializeObject<ApiLoginResponse>(await TestHandler.SendPostRequestAsync(req));
            if (string.IsNullOrEmpty(res.Result.Token))
                Assert.Fail("token is empty or null altough server returned with!");
            if (string.IsNullOrEmpty(res.Result.Web_application_cookie))
                Assert.Fail("token is empty or null altough server returned with!");
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            if(dataTypesDb.Parents == null)
            {
                throw new Exception("Parents default to null!");
            }
            var dataTypesChildren = await TestHandler.PlcProgramBrowseAsync(ApiPlcProgramBrowseMode.Children, dataTypesDb);
            if(dataTypesDb.Children.Count != 0)
            {
                throw new Exception("Children have been modified!");
            }
            if(dataTypesChildren.Result.Any(el => el.Parents.Count != 0))
            {
                throw new Exception("Parents have been modified!");
            }
            var plcProgramHandler = new ApiPlcProgramHandler(TestHandler, ApiRequestFactory);
            var dataTypesChildrenWithInfo = await plcProgramHandler.PlcProgramBrowseSetChildrenAndParentsAsync(ApiPlcProgramBrowseMode.Children, dataTypesDb);
            if(dataTypesDb.Children.Count == 0)
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
            if(dtlChildren.Any(el => el.Parents.Count != 2 || !el.Parents.Any(par => par.Name == "DataTypes") || !el.Parents.Any(par => par.Name =="DTL")))
            {
                throw new Exception("Parents have not been added as expected!");
            }
            if(dataTypesDb.Children.First(el => el.Name == "DTL").Children.Count != 8)
            {
                throw new Exception("Object links are not updated as expected!");
            }
            if(dataTypesChildDTL.Children.Count != 8)
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            var resp = await TestHandler.WebAppBrowseResourcesAsync("anotherWebAp");
            if (resp.Result.Max_Resources != 200)
                Assert.Fail("max_resources are not 200 but:" + resp.Result.Max_Resources.ToString());
            if(resp.Result.Resources.Count != 7)
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            // name with > 100 chars
            string invalidName = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789";
            await TestHandler.WebAppCreateAsync(invalidName);
            Assert.ThrowsAsync<ApiInvalidApplicationNameException>(async () =>
                await TestHandler.WebAppCreateAsync(invalidName+"0"));
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            // name with > 100 chars
            string invalidName = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789";
            await TestHandler.WebAppCreateResourceAsync("webapp", invalidName+invalidName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat));
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            string validName = "myResource";
            var validEtag = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" + "0123456789012345678901234567";
            await TestHandler.WebAppCreateResourceAsync("webapp", validName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat), ApiWebAppResourceVisibility.Public, validEtag);
            Assert.ThrowsAsync<ApiInvalidETagException>(async () =>
                await TestHandler.WebAppCreateResourceAsync("webapp", validName, "text/html", DateTime.Now.ToString(DateTimeFormatting.ApiDateTimeFormat), ApiWebAppResourceVisibility.Public, validEtag+"0"));
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            string webAppName = "webApp";
            var resp = await TestHandler.WebAppDeleteResourceAsync(webAppName,"resName");
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiApplicationDoesNotExistException>(async () =>
                await TestHandler.WebAppDeleteResourceAsync(webAppName,"resName"));
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            string webAppName = "webApp";
            var validEtag = "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" + "0123456789012345678901234567";
            await TestHandler.WebAppSetResourceETagAsync(webAppName, "resName", validEtag);
            Assert.ThrowsAsync<ApiInvalidETagException>(async () =>
                await TestHandler.WebAppSetResourceETagAsync(webAppName, "resName", validEtag+"0"));
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            string webAppName = "webApp";
            Assert.ThrowsAsync<ApiInvalidParametersException>(async () =>
                await TestHandler.WebAppSetStateAsync(webAppName, ApiWebAppState.None ));
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);

            var bulkRequest = new List<ApiRequest>();
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
            TestHandler = new ApiHttpClientRequestHandler(client, ApiRequestFactory);
            var bulkRequest = new List<ApiRequest>();
            // Does not matter!!
            bulkRequest.Add(ApiRequestFactory.GetApiPingRequest());
            Assert.ThrowsAsync<ApiBulkRequestException>(async () =>
            {
                var bulkResult = (await TestHandler.ApiBulkAsync(bulkRequest));
            });
        }



    }
}
