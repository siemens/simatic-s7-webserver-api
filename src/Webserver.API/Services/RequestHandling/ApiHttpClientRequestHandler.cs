// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.AlarmsBrowse;
using Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using Siemens.Simatic.S7.Webserver.API.Models.TimeSettings;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Request Handlerusing the Microsoft.Net.HttpClient to send the requests to the API
    /// </summary>
    public class ApiHttpClientRequestHandler : IApiRequestHandler
    {
        private readonly HttpClient _httpClient;
        private readonly IApiRequestFactory _apiRequestFactory;
        private readonly IApiResponseChecker _apiResponseChecker;
        private readonly ILogger _logger;

        /// <summary>
        /// Max. Size of one 'single' Request (Bulk Request is also considered '1 request')
        /// prior or equal to fw version 3.0: 64KB (KiB?)
        /// after fw version 3.1: 128 KB (KiB?)
        /// </summary>
        public int MaxRequestSize { get; set; } = 64 * 1000;

        /// <summary>
        /// Should prob not be changed!
        /// appilication/json for requests to the jsonrpc api endpoint
        /// </summary>
        public string ContentType => "application/json";

        /// <summary>
        /// Should prob not be changed!
        /// Encoding.UTF8
        /// </summary>
        public Encoding Encoding => Encoding.UTF8;

        /// <summary>
        /// Should prob not be changed!
        /// api/jsonrpc endpoint of plc
        /// </summary>
        public string JsonRpcApi => "api/jsonrpc";

        /// <summary>
        /// The ApiHttpClientRequestHandler will Send Post Requests,
        /// before sending the Request it'll remove those Parameters that have the value null for their keys 
        /// (keep in mind when using - when not using the ApiRequestFactory)
        /// </summary>
        /// <param name="httpClient">authorized httpClient with set Header: 'X-Auth-Token'</param>
        /// <param name="apiRequestFactory"></param>
        /// <param name="apiResponseChecker">response checker for the requestfactory and requesthandler...</param>
        /// <param name="logger">Logger to be used.</param>
        public ApiHttpClientRequestHandler(HttpClient httpClient, IApiRequestFactory apiRequestFactory, IApiResponseChecker apiResponseChecker, ILogger logger = null)
        {
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this._apiRequestFactory = apiRequestFactory ?? throw new ArgumentNullException(nameof(apiRequestFactory));
            this._apiResponseChecker = apiResponseChecker ?? throw new ArgumentNullException(nameof(apiResponseChecker));
            this._logger = logger;
        }

        /// <summary>
        /// Initialize Quantity Structures according to ApiVersion (e.g. MaxRequestSize)
        /// </summary>
        /// <returns>Initialization Task</returns>
        public async Task InitAsync()
        {
            var version = (await ApiVersionAsync()).Result;
            if (version >= 4)
            {
                MaxRequestSize = 128 * 1000;
            }
            else
            {
                MaxRequestSize = 64 * 1000;
            }
            _logger?.LogDebug($"Api Version '{version}' -> Max Request Size limit determined: '{MaxRequestSize}'.");
        }

        /// <summary>
        /// Initialize Quantity Structures according to ApiVersion (e.g. MaxRequestSize)
        /// </summary>
        public void Init() => InitAsync().GetAwaiter().GetResult();

        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <param name="apiRequest">Api Request to send to the plc (Json Serialized - null properties are deleted)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>string: response from thePLC</returns>
        public async Task<string> SendPostRequestAsync(IApiRequest apiRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (apiRequest.Params != null)
            {
                apiRequest.Params = apiRequest.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string apiRequestString = JsonConvert.SerializeObject(apiRequest, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.GetBytes(apiRequestString);
            var started = DateTime.Now;
            _logger?.LogDebug($"Start sending request {apiRequest.Id}");
            var response = await SendPostRequestAsync(apiRequestString, cancellationToken);
            _logger?.LogDebug($"Got response for {apiRequest.Id} -> {DateTime.Now - started}");
            return response;
        }

        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <param name="apiRequest">Api Request to send to the plc (Json Serialized - null properties are deleted)</param>
        /// <returns>string: response from thePLC</returns>
        public string SendPostRequest(IApiRequest apiRequest) => SendPostRequestAsync(apiRequest).GetAwaiter().GetResult();

        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <param name="apiRequestWithIntId">Api Request to send to the plc (Json Serialized - null properties are deleted)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>string: response from thePLC</returns>
        public async Task<string> SendPostRequestAsync(IApiRequestIntId apiRequestWithIntId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (apiRequestWithIntId.Params != null)
            {
                apiRequestWithIntId.Params = apiRequestWithIntId.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string apiRequestString = JsonConvert.SerializeObject(apiRequestWithIntId, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.GetBytes(apiRequestString);
            var started = DateTime.Now;
            _logger?.LogDebug($"Start sending request {apiRequestWithIntId.Id}");
            var response = await SendPostRequestAsync(apiRequestString, cancellationToken);
            _logger?.LogDebug($"Got response for {apiRequestWithIntId.Id} -> {DateTime.Now - started}");
            return response;
        }

        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <param name="apiRequestWithIntId">Api Request to send to the plc (Json Serialized - null properties are deleted)</param>
        /// <returns>string: response from thePLC</returns>
        public string SendPostRequest(IApiRequestIntId apiRequestWithIntId) => SendPostRequestAsync(apiRequestWithIntId).GetAwaiter().GetResult();

        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <param name="apiRequestString">further information about the Api requeest the user tried to send (or was trying to send)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>string: response from thePLC</returns>
        public async Task<string> SendPostRequestAsync(string apiRequestString, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] byteArr = Encoding.GetBytes(apiRequestString);
            ByteArrayContent request_body = new ByteArrayContent(byteArr);
            request_body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
            var response = await _httpClient.PostAsync(JsonRpcApi, request_body, cancellationToken);
            _apiResponseChecker.CheckHttpResponseForErrors(response, apiRequestString);
#if NET6_0_OR_GREATER
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
#else
            var responseString = await response.Content.ReadAsStringAsync();
#endif
            _apiResponseChecker.CheckResponseStringForErros(responseString, apiRequestString);
            return responseString;
        }

        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <param name="apiRequestString">further information about the Api requeest the user tried to send (or was trying to send)</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>string: response from thePLC</returns>
        public async Task<List<string>> SendPostRequestAsyncFileName(string apiRequestString, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<string> result = new List<string>();
            byte[] byteArr = Encoding.GetBytes(apiRequestString);
            ByteArrayContent request_body = new ByteArrayContent(byteArr);
            request_body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
            var response = await _httpClient.PostAsync(JsonRpcApi, request_body);
            _apiResponseChecker.CheckHttpResponseForErrors(response, apiRequestString);
            var responseString = await response.Content.ReadAsStringAsync();
            _apiResponseChecker.CheckResponseStringForErros(responseString, apiRequestString);
            result.Add(responseString);
            //result.Add(response.Content.Headers.ContentDisposition.FileName);
            return result;
        }

        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <param name="apiRequestString">further information about the Api requeest the user tried to send (or was trying to send)</param>
        /// <returns>string: response from thePLC</returns>
        public string SendPostRequest(string apiRequestString) => SendPostRequestAsync(apiRequestString).GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>An Array of ApiClass (and Id,Jsonrpc)</returns>
        public async Task<ApiArrayOfApiClassResponse> ApiBrowseAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiBrowseRequest();
            var responseString = await SendPostRequestAsync(req, cancellationToken);
            var arrOfApiClassResponse = JsonConvert.DeserializeObject<ApiArrayOfApiClassResponse>(responseString);
            if (arrOfApiClassResponse.Result.Count == 0)
            {
                throw new ApiInvalidResponseException("Api.Browse returned an empty array!");
            }
            return arrOfApiClassResponse;
        }
        /// <summary>
        /// Send an Api.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>An Array of ApiClass (and Id,Jsonrpc)</returns>
        public ApiArrayOfApiClassResponse ApiBrowse() => ApiBrowseAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.BrowseTickets Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        public async Task<ApiBrowseTicketsResponse> ApiBrowseTicketsAsync(CancellationToken cancellationToken) => await ApiBrowseTicketsAsync((string)null, cancellationToken);

        /// <summary>
        /// Send an Api.BrowseTickets Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="ticketId">ticket to be browsed (null to browse all)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        public async Task<ApiBrowseTicketsResponse> ApiBrowseTicketsAsync(string ticketId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiBrowseTicketsRequest(ticketId);
            var responseString = await SendPostRequestAsync(req, cancellationToken);
            var arrOfApiClassResponse = JsonConvert.DeserializeObject<ApiBrowseTicketsResponse>(responseString);
            return arrOfApiClassResponse;
        }

        /// <summary>
        /// Send an Api.BrowseTickets Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="ticketId">ticket to be browsed (null to browse all)</param>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>

        public ApiBrowseTicketsResponse ApiBrowseTickets(string ticketId = null) => ApiBrowseTicketsAsync(ticketId).GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.BrowseTickets Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="ticket">ticket to be browsed (null to browse all)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        public async Task<ApiBrowseTicketsResponse> ApiBrowseTicketsAsync(ApiTicket ticket, CancellationToken cancellationToken = default(CancellationToken)) => await ApiBrowseTicketsAsync(ticket.Id, cancellationToken);

        /// <summary>
        /// Send an Api.BrowseTickets Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="ticket">ticket to be browsed (null to browse all)</param>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        public ApiBrowseTicketsResponse ApiBrowseTickets(ApiTicket ticket) => ApiBrowseTicketsAsync(ticket).GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.CloseTicket Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="ticketId">ticket id (28 chars)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>True to indicate Success</returns>
        public async Task<ApiTrueOnSuccessResponse> ApiCloseTicketAsync(string ticketId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiCloseTicketRequest(ticketId);
            var responseString = await SendPostRequestAsync(req, cancellationToken);
            var apiTrueOnSuccessResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(responseString);
            return apiTrueOnSuccessResponse;
        }

        /// <summary>
        /// Send an Api.CloseTicket Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="ticketId">ticket id (28 chars)</param>
        public ApiTrueOnSuccessResponse ApiCloseTicket(string ticketId) => ApiCloseTicketAsync(ticketId).GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.CloseTicket Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="ticket">ticket containing ticket id (28 chars)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>True to indicate Success</returns>
        public async Task<ApiTrueOnSuccessResponse> ApiCloseTicketAsync(ApiTicket ticket, CancellationToken cancellationToken = default(CancellationToken)) => await ApiCloseTicketAsync(ticket.Id);

        /// <summary>
        /// Send an Api.CloseTicket Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="ticket">ticket containing ticket id (28 chars)</param>
        /// <returns>True to indicate Success</returns>
        public ApiTrueOnSuccessResponse ApiCloseTicket(ApiTicket ticket) => ApiCloseTicketAsync(ticket).GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.ChangePassword request
        /// </summary>
        /// <param name="username">The user account for which the password shall be changed</param>
        /// <param name="currentPassword">The current password for the user</param>
        /// <param name="newPassword">The new password for the user</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if changing password for the user was successful</returns>
        public async Task<ApiTrueOnSuccessResponse> ApiChangePasswordAsync(string username, string currentPassword, string newPassword, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ApiChangePasswordBaseAsync(username, currentPassword, newPassword, null, cancellationToken);
        }

        /// <summary>
        /// Send an Api.ChangePassword request
        /// </summary>
        /// <param name="username">The user account for which the password shall be changed</param>
        /// <param name="currentPassword">The current password for the user</param>
        /// <param name="newPassword">The new password for the user</param>
        /// <param name="mode">The mode defines where the password change shall be performed on. If null, the PLC will treat it as local.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if changing password for the user was successful</returns>
        public async Task<ApiTrueOnSuccessResponse> ApiChangePasswordAsync(string username, string currentPassword, string newPassword, ApiAuthenticationMode mode, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ApiChangePasswordBaseAsync(username, currentPassword, newPassword, mode, cancellationToken);
        }

        /// <summary>
        /// Send an Api.ChangePassword request
        /// </summary>
        /// <param name="username">The user account for which the password shall be changed</param>
        /// <param name="currentPassword">The current password for the user</param>
        /// <param name="newPassword">The new password for the user</param>
        /// <param name="mode">The mode defines where the password change shall be performed on. If null, the PLC will treat it as local.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if changing password for the user was successful</returns>
        private async Task<ApiTrueOnSuccessResponse> ApiChangePasswordBaseAsync(string username, string currentPassword, string newPassword, ApiAuthenticationMode? mode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiChangePasswordRequest(username, currentPassword, newPassword, mode);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send an Api.ChangePassword request
        /// </summary>
        /// <param name="username">The user account for which the password shall be changed</param>
        /// <param name="currentPassword">The current password for the user</param>
        /// <param name="newPassword">The new password for the user</param>
        /// <param name="mode">The mode defines where the password change shall be performed on. If null, the PLC will treat it as local.</param>
        /// <returns>True if changing password for the user was successful</returns>
        public ApiTrueOnSuccessResponse ApiChangePassword(string username, string currentPassword, string newPassword, ApiAuthenticationMode mode) =>
            ApiChangePasswordAsync(username, currentPassword, newPassword, mode).GetAwaiter().GetResult();
        /// <summary>
        /// Send an Api.ChangePassword request
        /// </summary>
        /// <param name="username">The user account for which the password shall be changed</param>
        /// <param name="currentPassword">The current password for the user</param>
        /// <param name="newPassword">The new password for the user</param>
        /// <returns>True if changing password for the user was successful</returns>
        public ApiTrueOnSuccessResponse ApiChangePassword(string username, string currentPassword, string newPassword) =>
            ApiChangePasswordAsync(username, currentPassword, newPassword).GetAwaiter().GetResult();
        /// <summary>
        /// Send an Api.GetCertificateUrl Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>ApiSingleStringResponse that contians the URL to the certificate</returns>
        public async Task<ApiSingleStringResponse> ApiGetCertificateUrlAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiGetCertificateUrlRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            if (!response.Contains("/MiniWebCA_Cer.crt"))
                Console.WriteLine("unexpected response: " + response + " for Api.GetCertificateUrl!");
            return JsonConvert.DeserializeObject<ApiSingleStringResponse>(response);
        }
        /// <summary>
        /// Send an Api.GetCertificateUrl Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>ApiSingleStringResponse that contians the URL to the certificate</returns>
        public ApiSingleStringResponse ApiGetCertificateUrl() => ApiGetCertificateUrlAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.GetPermissions Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>Array of ApiClass (in this case permissions)</returns>
        public async Task<ApiArrayOfApiClassResponse> ApiGetPermissionsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiGetPermissionsRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiArrayOfApiClassResponse>(response);
        }
        /// <summary>
        /// Send an Api.GetQuantityStructures Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>Api Quantity Structure object</returns>
        public async Task<ApiGetQuantityStructuresResponse> ApiGetQuantityStructuresAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiGetQuantityStructuresRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiGetQuantityStructuresResponse>(response);
        }

        /// <summary>
        /// Send an Api.GetQuantityStructures Request  
        /// </summary>
        /// <returns>A QuantityStructure object</returns>
        public ApiGetQuantityStructuresResponse ApiGetQuantityStructures() => ApiGetQuantityStructuresAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.GetPermissions Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>Array of ApiClass (in this case permissions)</returns>
        public ApiArrayOfApiClassResponse ApiGetPermissions() => ApiGetPermissionsAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.Logout Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>True to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> ApiLogoutAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiLogoutRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
            return responseObj;
        }
        /// <summary>
        /// Send an Api.Logout Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>True to indicate success</returns>
        public ApiTrueOnSuccessResponse ApiLogout() => ApiLogoutAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.Ping Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>ApiSingleStringResponse - an Id that'll stay the same for the users session</returns>
        public async Task<ApiSingleStringResponse> ApiPingAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPingRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiSingleStringResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send an Api.Ping Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>ApiSingleStringResponse - an Id that'll stay the same for the users session</returns>
        public ApiSingleStringResponse ApiPing() => ApiPingAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.Version Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>a double that contains the value for the current ApiVersion</returns>
        public async Task<ApiDoubleResponse> ApiVersionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiVersionRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiDoubleResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send an Api.Version Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>a double that contains the value for the current ApiVersion</returns>
        public ApiDoubleResponse ApiVersion() => ApiVersionAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Perform a service data download on the corresponding module with hwid
        /// </summary>
        /// <param name="hwid">The HWID of a node (module) for which a service data file can be downloaded</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Ticket to use for downloading the service data</returns>
        public async Task<ApiTicketIdResponse> ModulesDownloadServiceDataAsync(ApiPlcHwId hwid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetModulesDownloadServiceData(hwid);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTicketIdResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Perform a service data download on the corresponding module with hwid
        /// </summary>
        /// <param name="hwid">The HWID of a node (module) for which a service data file can be downloaded</param>
        /// <returns>Ticket to use for downloading the service data</returns>
        public ApiTicketIdResponse ModulesDownloadServiceData(ApiPlcHwId hwid) =>
            ModulesDownloadServiceDataAsync(hwid).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.ReadOperatingMode Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>The current Plc OperatingMode</returns>
        public async Task<ApiReadOperatingModeResponse> PlcReadOperatingModeAsync(ApiPlcRedundancyId rhid = ApiPlcRedundancyId.StandardPLC, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcReadOperatingModeRequest(rhid);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiReadOperatingModeResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a Plc.ReadOperatingMode Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <returns>The current Plc OperatingMode</returns>
        public ApiReadOperatingModeResponse PlcReadOperatingMode(ApiPlcRedundancyId rhid = ApiPlcRedundancyId.StandardPLC) => PlcReadOperatingModeAsync(rhid).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.RequestChangeOperatingMode Request using the Request from the ApiRequestFactory
        /// Method to change the plc operating mode
        /// valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.
        /// </summary>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <param name="plcOperatingMode">Plc Operating mode wanted</param>
        /// <returns>valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.</returns>
        public async Task<ApiTrueOnSuccessResponse> PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode plcOperatingMode, ApiPlcRedundancyId rhid = ApiPlcRedundancyId.StandardPLC, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcRequestChangeOperatingModeRequest(plcOperatingMode, rhid);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a Plc.RequestChangeOperatingMode Request using the Request from the ApiRequestFactory
        /// Method to change the plc operating mode
        /// valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.
        /// </summary>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <param name="plcOperatingMode">Plc Operating mode wanted</param>
        /// <returns>valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.</returns>
        public ApiTrueOnSuccessResponse PlcRequestChangeOperatingMode(ApiPlcOperatingMode plcOperatingMode, ApiPlcRedundancyId rhid = ApiPlcRedundancyId.StandardPLC)
            => PlcRequestChangeOperatingModeAsync(plcOperatingMode, rhid).GetAwaiter().GetResult();

        /// <summary>
        /// Send a PlcProgram.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="var">
        /// Name of the variable to browse (symbolic)
        /// • if "mode" ="var" this Attribut is required. The Browse-Method
        /// searches for the variable to find the metadata of it.
        /// • if "mode" = "children", this attribute is optional. The Browse-Method
        /// searches for the variable and returns a list of child elements and metadata.
        /// 
        /// Name der zu durchsuchenden Variable
        /// • Wenn "mode" ="var", ist dieses Attribut erforderlich.Die Browse-Methode
        /// sucht nach der Variable, um die Metadaten der Variable zu finden.
        /// • Wenn "mode" = "children", ist dieses Attribut optional. Die Browse-Methode
        /// sucht die Variable und liefert eine Liste an Kind-Variablen und Metadaten.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <param name="plcProgramBrowseMode"></param>
        /// <returns>PlcProgramBrowseResponse: An Array of ApiPlcProgramData</returns>
        public async Task<ApiPlcProgramBrowseResponse> PlcProgramBrowseAsync(ApiPlcProgramBrowseMode plcProgramBrowseMode, string var = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcProgramBrowseRequest(plcProgramBrowseMode, var);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiPlcProgramBrowseResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a PlcProgram.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="var">
        /// Name of the variable to browse (symbolic)
        /// • if "mode" ="var" this Attribut is required. The Browse-Method
        /// searches for the variable to find the metadata of it.
        /// • if "mode" = "children", this attribute is optional. The Browse-Method
        /// searches for the variable and returns a list of child elements and metadata.
        /// 
        /// Name der zu durchsuchenden Variable
        /// • Wenn "mode" ="var", ist dieses Attribut erforderlich.Die Browse-Methode
        /// sucht nach der Variable, um die Metadaten der Variable zu finden.
        /// • Wenn "mode" = "children", ist dieses Attribut optional. Die Browse-Methode
        /// sucht die Variable und liefert eine Liste an Kind-Variablen und Metadaten.</param>
        /// <param name="plcProgramBrowseMode"></param>
        /// <returns>PlcProgramBrowseResponse: An Array of ApiPlcProgramData</returns>
        public ApiPlcProgramBrowseResponse PlcProgramBrowse(ApiPlcProgramBrowseMode plcProgramBrowseMode, string var = null) => PlcProgramBrowseAsync(plcProgramBrowseMode, var).GetAwaiter().GetResult();

        /// <summary>
        /// Send a PlcProgram.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="var">
        /// Name of the variable to browse (symbolic)
        /// • if "mode" ="var" this Attribut is required. The Browse-Method
        /// searches for the variable to find the metadata of it.
        /// • if "mode" = "children", this attribute is optional. The Browse-Method
        /// searches for the variable and returns a list of child elements and metadata.
        /// 
        /// Name der zu durchsuchenden Variable
        /// • Wenn "mode" ="var", ist dieses Attribut erforderlich.Die Browse-Methode
        /// sucht nach der Variable, um die Metadaten der Variable zu finden.
        /// • Wenn "mode" = "children", ist dieses Attribut optional. Die Browse-Methode
        /// sucht die Variable und liefert eine Liste an Kind-Variablen und Metadaten.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <param name="plcProgramBrowseMode"></param>
        /// <returns>PlcProgramBrowseResponse: An Array of ApiPlcProgramData</returns>
        public async Task<ApiPlcProgramBrowseResponse> PlcProgramBrowseAsync(ApiPlcProgramBrowseMode plcProgramBrowseMode, ApiPlcProgramData var, CancellationToken cancellationToken = default(CancellationToken))
        {
            string varName = var.GetVarNameForMethods();
            return await PlcProgramBrowseAsync(plcProgramBrowseMode, varName, cancellationToken);
        }
        /// <summary>
        /// Send a PlcProgram.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="var">
        /// Name of the variable to browse (symbolic)
        /// • if "mode" ="var" this Attribut is required. The Browse-Method
        /// searches for the variable to find the metadata of it.
        /// • if "mode" = "children", this attribute is optional. The Browse-Method
        /// searches for the variable and returns a list of child elements and metadata.
        /// 
        /// Name der zu durchsuchenden Variable
        /// • Wenn "mode" ="var", ist dieses Attribut erforderlich.Die Browse-Methode
        /// sucht nach der Variable, um die Metadaten der Variable zu finden.
        /// • Wenn "mode" = "children", ist dieses Attribut optional. Die Browse-Methode
        /// sucht die Variable und liefert eine Liste an Kind-Variablen und Metadaten.</param>
        /// <param name="plcProgramBrowseMode"></param>
        /// <returns>PlcProgramBrowseResponse: An Array of ApiPlcProgramData</returns>
        public ApiPlcProgramBrowseResponse PlcProgramBrowse(ApiPlcProgramBrowseMode plcProgramBrowseMode, ApiPlcProgramData var) => PlcProgramBrowseAsync(plcProgramBrowseMode, var).GetAwaiter().GetResult();

        /// <summary>
        /// Send a PlcProgram.Browse request for the code blocks.
        /// </summary>
        /// <returns>ApiPlcProgramBrowseCodeBlocksResponse: A collection of ApiPlcProgramBrowseCodeBlocksData objects.</returns>
        public ApiPlcProgramBrowseCodeBlocksResponse PlcProgramBrowseCodeBlocks() => PlcProgramBrowseCodeBlocksAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a PlcProgram.Browse request for the code blocks.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>ApiPlcProgramBrowseCodeBlocksResponse: A collection of ApiPlcProgramBrowseCodeBlocksData objects.</returns>
        public async Task<ApiPlcProgramBrowseCodeBlocksResponse> PlcProgramBrowseCodeBlocksAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcProgramBrowseCodeBlocksRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiPlcProgramBrowseCodeBlocksResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a PlcProgram.DownloadProfilingData request.
        /// </summary>
        /// <returns>ApiSingleStringResponse: Object containing the ticket ID for the data download.</returns>
        public ApiSingleStringResponse PlcProgramDownloadProfilingData() => PlcProgramDownloadProfilingDataAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a PlcProgram.DownloadProfilingData request.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>ApiSingleStringResponse: Object containing the ticket ID for the data download.</returns>
        public async Task<ApiSingleStringResponse> PlcProgramDownloadProfilingDataAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcProgramDownloadProfilingDataRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiSingleStringResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a PlcProgram.Read Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="var">Name of the variable to be read
        /// 
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramReadMode">
        /// this parameter is optional and defaults to "simple":
        /// "simple" will get the variable values according to the presentation of the manual - "supported Datatypes"
        /// "raw" : will get the variable values according to the presentation of the manual "raw"
        ///
        /// Aufzählung, die das Antwortformat für diese Methode festlegt:
        /// • "simple": liefert Variablenwerte gemäß der Darstellung
        /// "simple" in Kapitel "Unterstützte Datentypen (Seite 162)"
        /// • "raw": liefert Variablenwerte gemäß der Darstellung "raw"
        /// in Kapitel "Unterstützte Datentypen"</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiPlcProgramReadResponse: object with the value for the variables value to be read</returns>
        public async Task<ApiResultResponse<T>> PlcProgramReadAsync<T>(string var, ApiPlcDataRepresentation? plcProgramReadMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcProgramReadRequest(var, plcProgramReadMode);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiResultResponse<T>>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a PlcProgram.Read Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="var">Name of the variable to be read
        /// 
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramReadMode">
        /// this parameter is optional and defaults to "simple":
        /// "simple" will get the variable values according to the presentation of the manual - "supported Datatypes"
        /// "raw" : will get the variable values according to the presentation of the manual "raw"
        ///
        /// Aufzählung, die das Antwortformat für diese Methode festlegt:
        /// • "simple": liefert Variablenwerte gemäß der Darstellung
        /// "simple" in Kapitel "Unterstützte Datentypen (Seite 162)"
        /// • "raw": liefert Variablenwerte gemäß der Darstellung "raw"
        /// in Kapitel "Unterstützte Datentypen"</param>
        /// <returns>ApiPlcProgramReadResponse: object with the value for the variables value to be read</returns>
        public ApiResultResponse<T> PlcProgramRead<T>(ApiPlcProgramData var, ApiPlcDataRepresentation? plcProgramReadMode = null) => PlcProgramReadAsync<T>(var, plcProgramReadMode).GetAwaiter().GetResult();

        /// <summary>
        /// Send a PlcProgram.Browse Request using the Request from the ApiRequestFactory
        /// This function will build up the name with quotes from the parents given with the ApiPlcProgramDataand call PlcProgramRead
        /// </summary>
        /// <param name="var">
        /// Name of the variable to be read
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramReadMode">
        /// this parameter is optional and defaults to "simple":
        /// "simple" will get the variable values according to the presentation of the manual - "supported Datatypes"
        /// "raw" : will get the variable values according to the presentation of the manual "raw"
        ///
        /// Aufzählung, die das Antwortformat für diese Methode festlegt:
        /// • "simple": liefert Variablenwerte gemäß der Darstellung
        /// "simple" in Kapitel "Unterstützte Datentypen (Seite 162)"
        /// • "raw": liefert Variablenwerte gemäß der Darstellung "raw"
        /// in Kapitel "Unterstützte Datentypen"</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiPlcProgramReadResponse: object with the value for the variables value to be read</returns>
        /// <exception cref="ApiInvalidArrayIndexException">will be thrown if a ApiPlcProgramDatathat is an array will be given without an index</exception>
        public async Task<ApiResultResponse<T>> PlcProgramReadAsync<T>(ApiPlcProgramData var, ApiPlcDataRepresentation? plcProgramReadMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            //RequestParameterChecker.CheckPlcProgramReadOrWriteDataType(var.Datatype, true);
            string varName = var.GetVarNameForMethods();
            return await PlcProgramReadAsync<T>(varName, plcProgramReadMode, cancellationToken);
        }
        /// <summary>
        /// Send a PlcProgram.Browse Request using the Request from the ApiRequestFactory
        /// This function will build up the name with quotes from the parents given with the ApiPlcProgramDataand call PlcProgramRead
        /// </summary>
        /// <param name="var">
        /// Name of the variable to be read
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramReadMode">
        /// this parameter is optional and defaults to "simple":
        /// "simple" will get the variable values according to the presentation of the manual - "supported Datatypes"
        /// "raw" : will get the variable values according to the presentation of the manual "raw"
        ///
        /// Aufzählung, die das Antwortformat für diese Methode festlegt:
        /// • "simple": liefert Variablenwerte gemäß der Darstellung
        /// "simple" in Kapitel "Unterstützte Datentypen (Seite 162)"
        /// • "raw": liefert Variablenwerte gemäß der Darstellung "raw"
        /// in Kapitel "Unterstützte Datentypen"</param>
        /// <returns>ApiPlcProgramReadResponse: object with the value for the variables value to be read</returns>
        /// <exception cref="ApiInvalidArrayIndexException">will be thrown if a ApiPlcProgramDatathat is an array will be given without an index</exception>
        public ApiResultResponse<T> PlcProgramRead<T>(string var, ApiPlcDataRepresentation? plcProgramReadMode = null) => PlcProgramReadAsync<T>(var, plcProgramReadMode).GetAwaiter().GetResult();

        /// <summary>
        /// Send a PlcProgram.Write Request using the Request from the ApiRequestFactory
        /// This function will build up the name with quotes from the parents given with the ApiPlcProgramDataand call PlcProgramWrite
        /// </summary>
        /// <param name="var">
        /// Name of the variable to be read
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramWriteMode"></param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <param name="valueToBeSet"></param>
        /// <returns>true to indicate success</returns>
        /// <exception cref="ApiInvalidArrayIndexException">will be thrown if a ApiPlcProgramDatathat is an array will be given without an index</exception>
        public async Task<ApiTrueOnSuccessResponse> PlcProgramWriteAsync(ApiPlcProgramData var, object valueToBeSet, ApiPlcDataRepresentation? plcProgramWriteMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string varName = var.GetVarNameForMethods();
            // ApiRequestFactory.CheckPlcProgramReadOrWriteDataType(var.Datatype); will also be called by GetApiPlcProgramWriteValueToBeSet!
            var writeVal = _apiRequestFactory.GetApiPlcProgramWriteValueToBeSet(var.Datatype, valueToBeSet);
            return await PlcProgramWriteAsync(varName, writeVal, plcProgramWriteMode, cancellationToken);
        }
        /// <summary>
        /// Send a PlcProgram.Write Request using the Request from the ApiRequestFactory
        /// This function will build up the name with quotes from the parents given with the ApiPlcProgramDataand call PlcProgramWrite
        /// </summary>
        /// <param name="var">
        /// Name of the variable to be read
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramWriteMode"></param>
        /// <param name="valueToBeSet"></param>
        /// <returns>true to indicate success</returns>
        /// <exception cref="ApiInvalidArrayIndexException">will be thrown if a ApiPlcProgramDatathat is an array will be given without an index</exception>
        public ApiTrueOnSuccessResponse PlcProgramWrite(ApiPlcProgramData var, object valueToBeSet, ApiPlcDataRepresentation? plcProgramWriteMode = null)
            => PlcProgramWriteAsync(var, valueToBeSet, plcProgramWriteMode).GetAwaiter().GetResult();

        /// <summary>
        /// Send a PlcProgram.Write Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="var">
        /// Name of the variable to be read
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramWriteMode"></param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <param name="valueToBeSet"></param>
        /// <returns>true to indicate success</returns>
        /// <exception cref="ApiInvalidArrayIndexException">will be thrown if a ApiPlcProgramDatathat is an array will be given without an index</exception>
        public async Task<ApiTrueOnSuccessResponse> PlcProgramWriteAsync(string var, object valueToBeSet, ApiPlcDataRepresentation? plcProgramWriteMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcProgramWriteRequest(var, valueToBeSet, plcProgramWriteMode);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a PlcProgram.Write Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="var">
        /// Name of the variable to be read
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramWriteMode"></param>
        /// <param name="valueToBeSet"></param>
        /// <returns>true to indicate success</returns>
        /// <exception cref="ApiInvalidArrayIndexException">will be thrown if a ApiPlcProgramDatathat is an array will be given without an index</exception>
        public ApiTrueOnSuccessResponse PlcProgramWrite(string var, object valueToBeSet, ApiPlcDataRepresentation? plcProgramWriteMode = null)
            => PlcProgramWriteAsync(var, valueToBeSet, plcProgramWriteMode).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppData">webappdata that should be requested</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiWebAppBrowseResponse: Containing WebAppBrowseResult: Max_Applications:uint, 
        /// Applications: Array of ApiWebAppdata containing one element: the webappdata that has been requested</returns>
        public async Task<ApiWebAppBrowseResponse> WebAppBrowseAsync(ApiWebAppData webAppData, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppBrowseAsync(webAppData.Name, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppData">webappdata that should be requested</param>
        /// <returns>ApiWebAppBrowseResponse: Containing WebAppBrowseResult: Max_Applications:uint, 
        /// Applications: Array of ApiWebAppdata containing one element: the webappdata that has been requested</returns>
        public ApiWebAppBrowseResponse WebAppBrowse(ApiWebAppData webAppData) => WebAppBrowseAsync(webAppData).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webapp name in case only one is requested</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiWebAppBrowseResponse: Containing WebAppBrowseResult: Max_Applications:uint, 
        /// Applications: Array of ApiWebAppdata containing one element: the webappdata that has been requested</returns>
        public async Task<ApiWebAppBrowseResponse> WebAppBrowseAsync(string webAppName = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppBrowseRequest(webAppName);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiWebAppBrowseResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webapp name in case only one is requested</param>
        /// <returns>ApiWebAppBrowseResponse: Containing WebAppBrowseResult: Max_Applications:uint, 
        /// Applications: Array of ApiWebAppdata containing one element: the webappdata that has been requested</returns>
        public ApiWebAppBrowseResponse WebAppBrowse(string webAppName = null) => WebAppBrowseAsync(webAppName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.BrowseResources Request using the Request from the ApiRequestFactory
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData 
        /// (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webAppName">WebApp name to browse resources of</param>
        /// <param name="resourceName">If given only that resource will be inside the array (in case it exists)</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public async Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(string webAppName, string resourceName = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppBrowseResourcesRequest(webAppName, resourceName);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiWebAppBrowseResourcesResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.BrowseResources Request using the Request from the ApiRequestFactory
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData 
        /// (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webAppName">WebApp name to browse resources of</param>
        /// <param name="resourceName">If given only that resource will be inside the array (in case it exists)</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public ApiWebAppBrowseResourcesResponse WebAppBrowseResources(string webAppName, string resourceName = null) => WebAppBrowseResourcesAsync(webAppName, resourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.BrowseResources Request using the Request from the ApiRequestFactory
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData 
        /// (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webApp">WebApp.Name to browse resources of</param>
        /// <param name="resourceName">If given only that resource will be inside the array (in case it exists)</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public async Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(ApiWebAppData webApp, string resourceName = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppBrowseResourcesAsync(webApp.Name, resourceName, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.BrowseResources Request using the Request from the ApiRequestFactory
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData 
        /// (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webApp">WebApp.Name to browse resources of</param>
        /// <param name="resourceName">If given only that resource will be inside the array (in case it exists)</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public ApiWebAppBrowseResourcesResponse WebAppBrowseResources(ApiWebAppData webApp, string resourceName = null) => WebAppBrowseResourcesAsync(webApp, resourceName).GetAwaiter().GetResult();
        /// <summary>
        /// Send a WebApp.BrowseResources Request using the Request from the ApiRequestFactory
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData 
        /// (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webAppName">WebApp Name to browse resources of</param>
        /// <param name="resource">resource.Name to browse for</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public async Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(string webAppName, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppBrowseResourcesAsync(webAppName, resource.Name, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.BrowseResources Request using the Request from the ApiRequestFactory
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData 
        /// (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webAppName">WebApp Name to browse resources of</param>
        /// <param name="resource">resource.Name to browse for</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public ApiWebAppBrowseResourcesResponse WebAppBrowseResources(string webAppName, ApiWebAppResource resource) => WebAppBrowseResourcesAsync(webAppName, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.BrowseResources Request using the Request from the ApiRequestFactory
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData 
        /// (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webApp">webApp.Name to browse resources of</param>
        /// <param name="resource">resource.Name to browse for</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public async Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(ApiWebAppData webApp, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppBrowseResourcesAsync(webApp.Name, resource.Name, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.BrowseResources Request using the Request from the ApiRequestFactory
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData 
        /// (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webApp">webApp.Name to browse resources of</param>
        /// <param name="resource">resource.Name to browse for</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public ApiWebAppBrowseResourcesResponse WebAppBrowseResources(ApiWebAppData webApp, ApiWebAppResource resource) => WebAppBrowseResourcesAsync(webApp, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.Create Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webapp name for the app to be created</param>
        /// <param name="apiWebAppState">optional parameter: state the webapp should be in</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppCreateAsync(string webAppName, ApiWebAppState? apiWebAppState = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppCreateRequest(webAppName, apiWebAppState);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.Create Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webapp name for the app to be created</param>
        /// <param name="apiWebAppState">optional parameter: state the webapp should be in</param>
        /// <returns>true to indicate success</returns>
        public ApiTrueOnSuccessResponse WebAppCreate(string webAppName, ApiWebAppState? apiWebAppState = null) => WebAppCreateAsync(webAppName, apiWebAppState).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.Create Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">containing information about name and state for the app to be created</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppCreateAsync(ApiWebAppData webApp, CancellationToken cancellationToken = default(CancellationToken))
        {
            // ApiRequestFactory.CheckState(webApp.State); will be called in WebAppCreate in Factory.GetApiWebAppCreateRequest
            return await WebAppCreateAsync(webApp.Name, webApp.State, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.Create Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">containing information about name and state for the app to be created</param>
        /// <returns>true to indicate success</returns>
        public ApiTrueOnSuccessResponse WebAppCreate(ApiWebAppData webApp) => WebAppCreateAsync(webApp).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.CreateResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">name of the webapp to create the resource on</param>
        /// <param name="resourceName">resource name to be created with (typically provided with extension)</param>
        /// <param name="media_type">resource media type - see MIMEType.mapping</param>
        /// <param name="last_modified">Be sure to provide the RFC3339 format!</param>
        /// <param name="apiWebAppResourceVisibility">resource visibility (protect your confidential data)</param>
        /// <param name="etag">you can provide an etag as identification,... for your resource</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public async Task<ApiTicketIdResponse> WebAppCreateResourceAsync(string webAppName, string resourceName, string media_type,
            string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppCreateResourceRequest(webAppName, resourceName, media_type,
                last_modified, apiWebAppResourceVisibility, etag);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTicketIdResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.CreateResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">name of the webapp to create the resource on</param>
        /// <param name="resourceName">resource name to be created with (typically provided with extension)</param>
        /// <param name="media_type">resource media type - see MIMEType.mapping</param>
        /// <param name="last_modified">Be sure to provide the RFC3339 format!</param>
        /// <param name="apiWebAppResourceVisibility">resource visibility (protect your confidential data)</param>
        /// <param name="etag">you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public ApiTicketIdResponse WebAppCreateResource(string webAppName, string resourceName, string media_type, string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null)
            => WebAppCreateResourceAsync(webAppName, resourceName, media_type, last_modified, apiWebAppResourceVisibility, etag).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.CreateResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to create the resource on</param>
        /// <param name="resourceName">resource name to be created with (typically provided with extension)</param>
        /// <param name="media_type">resource media type - see MIMEType.mapping</param>
        /// <param name="last_modified">Be sure to provide the RFC3339 format!</param>
        /// <param name="apiWebAppResourceVisibility">resource visibility (protect your confidential data)</param>
        /// <param name="etag">you can provide an etag as identification,... for your resource</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public async Task<ApiTicketIdResponse> WebAppCreateResourceAsync(ApiWebAppData webApp, string resourceName, string media_type,
            string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppCreateResourceAsync(webApp.Name, resourceName, media_type, last_modified, apiWebAppResourceVisibility, etag, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.CreateResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to create the resource on</param>
        /// <param name="resourceName">resource name to be created with (typically provided with extension)</param>
        /// <param name="media_type">resource media type - see MIMEType.mapping</param>
        /// <param name="last_modified">Be sure to provide the RFC3339 format!</param>
        /// <param name="apiWebAppResourceVisibility">resource visibility (protect your confidential data)</param>
        /// <param name="etag">you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public ApiTicketIdResponse WebAppCreateResource(ApiWebAppData webApp, string resourceName, string media_type, string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null)
            => WebAppCreateResourceAsync(webApp, resourceName, media_type, last_modified, apiWebAppResourceVisibility, etag).GetAwaiter().GetResult();
        /// <summary>
        /// Send a WebApp.CreateResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">name of the webapp to create the resource on</param>
        /// <param name="resource">resource containing all the information: 
        /// Name:           name to be created with (typically provided with extension)
        /// Media_type:     resource media type - see MIMEType.mapping
        /// Last_modified:  Be sure to provide the RFC3339 format!
        /// Visibility:     resource visibility (protect your confidential data)
        /// Etag:           you can provide an etag as identification,... for your resource</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public async Task<ApiTicketIdResponse> WebAppCreateResourceAsync(string webAppName, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppCreateResourceAsync(webAppName, resource.Name, resource.Media_type,
                resource.Last_modified.ToString(DateTimeFormatting.ApiDateTimeFormat), resource.Visibility, resource.Etag, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.CreateResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">name of the webapp to create the resource on</param>
        /// <param name="resource">resource containing all the information: 
        /// Name:           name to be created with (typically provided with extension)
        /// Media_type:     resource media type - see MIMEType.mapping
        /// Last_modified:  Be sure to provide the RFC3339 format!
        /// Visibility:     resource visibility (protect your confidential data)
        /// Etag:           you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public ApiTicketIdResponse WebAppCreateResource(string webAppName, ApiWebAppResource resource) => WebAppCreateResourceAsync(webAppName, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.CreateResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to create the resource on</param>
        /// <param name="resource">resource containing all the information: 
        /// Name:           name to be created with (typically provided with extension)
        /// Media_type:     resource media type - see MIMEType.mapping
        /// Last_modified:  Be sure to provide the RFC3339 format!
        /// Visibility:     resource visibility (protect your confidential data)
        /// Etag:           you can provide an etag as identification,... for your resource</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public async Task<ApiTicketIdResponse> WebAppCreateResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppCreateResourceAsync(webApp.Name, resource.Name, resource.Media_type,
                resource.Last_modified.ToString(DateTimeFormatting.ApiDateTimeFormat), resource.Visibility, resource.Etag, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.CreateResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to create the resource on</param>
        /// <param name="resource">resource containing all the information: 
        /// Name:           name to be created with (typically provided with extension)
        /// Media_type:     resource media type - see MIMEType.mapping
        /// Last_modified:  Be sure to provide the RFC3339 format!
        /// Visibility:     resource visibility (protect your confidential data)
        /// Etag:           you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public ApiTicketIdResponse WebAppCreateResource(ApiWebAppData webApp, ApiWebAppResource resource) => WebAppCreateResourceAsync(webApp, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.Delete Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp to delete</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteAsync(string webAppName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppDeleteRequest(webAppName);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.Delete Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp to delete</param>
        /// <returns>true to indicate success</returns>
        public ApiTrueOnSuccessResponse WebAppDelete(string webAppName) => WebAppDeleteAsync(webAppName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.Delete Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to delete</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteAsync(ApiWebAppData webApp, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppDeleteAsync(webApp.Name, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.Delete Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to delete</param>
        /// <returns>true to indicate success</returns>
        public ApiTrueOnSuccessResponse WebAppDelete(ApiWebAppData webApp) => WebAppDeleteAsync(webApp).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.DeleteRespource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to delete</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(string webAppName, string resourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppDeleteResourceRequest(webAppName, resourceName);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.DeleteRespource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        public ApiTrueOnSuccessResponse WebAppDeleteResource(string webAppName, string resourceName) => WebAppDeleteResourceAsync(webAppName, resourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.DeleteRespource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webapp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to delete</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(ApiWebAppData webApp, string resourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppDeleteResourceAsync(webApp.Name, resourceName, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.DeleteRespource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webapp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        public ApiTrueOnSuccessResponse WebAppDeleteResource(ApiWebAppData webApp, string resourceName) => WebAppDeleteResourceAsync(webApp, resourceName).GetAwaiter().GetResult();
        /// <summary>
        /// Send a WebApp.DeleteRespource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webapp.Name of the webapp that contains the resource</param>
        /// <param name="resource">Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        public ApiTrueOnSuccessResponse WebAppDeleteResource(string webAppName, ApiWebAppResource resource) => WebAppDeleteResourceAsync(webAppName, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.DeleteRespource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to delete</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppDeleteResourceAsync(webApp.Name, resource.Name, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.DeleteRespource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        public ApiTrueOnSuccessResponse WebAppDeleteResource(ApiWebAppData webApp, ApiWebAppResource resource) => WebAppDeleteResourceAsync(webApp, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.DeleteRespource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to delete</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(string webAppName, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppDeleteResourceAsync(webAppName, resource.Name, cancellationToken);
        }

        /// <summary>
        /// Send a WebApp.DownloadResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to download</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public async Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(string webAppName, string resourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppDownloadResourceRequest(webAppName, resourceName);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTicketIdResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.DownloadResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public ApiTicketIdResponse WebAppDownloadResource(string webAppName, string resourceName) => WebAppDownloadResourceAsync(webAppName, resourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.DownloadResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to download</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public async Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(ApiWebAppData webApp, string resourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppDownloadResourceAsync(webApp.Name, resourceName, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.DownloadResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public ApiTicketIdResponse WebAppDownloadResource(ApiWebAppData webApp, string resourceName) => WebAppDownloadResourceAsync(webApp, resourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.DownloadResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to download</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public async Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppDownloadResourceAsync(webApp.Name, resource.Name, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.DownloadResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public ApiTicketIdResponse WebAppDownloadResource(ApiWebAppData webApp, ApiWebAppResource resource) => WebAppDownloadResourceAsync(webApp, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.DownloadResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to download</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public async Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(string webAppName, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppDownloadResourceAsync(webAppName, resource.Name, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.DownloadResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public ApiTicketIdResponse WebAppDownloadResource(string webAppName, ApiWebAppResource resource) => WebAppDownloadResourceAsync(webAppName, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.Rename Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that should be renamed</param>
        /// <param name="newWebAppName">New name for the WebApp</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a WebApp that only has the information: 
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppRenameAsync(string webAppName, string newWebAppName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppRenameRequest(webAppName, newWebAppName);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = new ApiTrueWithWebAppResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if (responseObj.TrueOnSuccesResponse.Result)
            {
                responseObj.NewWebApp = new ApiWebAppData() { Name = newWebAppName };
            }
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.Rename Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that should be renamed</param>
        /// <param name="newWebAppName">New name for the WebApp</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a WebApp that only has the information: 
        /// name which equals the newname</returns>
        public ApiTrueWithWebAppResponse WebAppRename(string webAppName, string newWebAppName) => WebAppRenameAsync(webAppName, newWebAppName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.Rename Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that should be renamed</param>
        /// <param name="newWebAppName">New name for the WebApp</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the given WebApp that has the change:
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppRenameAsync(ApiWebAppData webApp, string newWebAppName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resp = await WebAppRenameAsync(webApp.Name, newWebAppName, cancellationToken);
            resp.NewWebApp = webApp.ShallowCopy();
            resp.NewWebApp.Name = newWebAppName;
            return resp;
        }
        /// <summary>
        /// Send a WebApp.Rename Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that should be renamed</param>
        /// <param name="newWebAppName">New name for the WebApp</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the given WebApp that has the change:
        /// name which equals the newname</returns>
        public ApiTrueWithWebAppResponse WebAppRename(ApiWebAppData webApp, string newWebAppName) => WebAppRenameAsync(webApp, newWebAppName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.RenameResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a Resource that only has the information: 
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(string webAppName, string resourceName,
            string newResourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppRenameResourceRequest(webAppName, resourceName, newResourceName);
            string response = await SendPostRequestAsync(req, cancellationToken);
            ApiTrueWithResourceResponse responseObj = new ApiTrueWithResourceResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if (responseObj.TrueOnSuccesResponse.Result)
            {
                responseObj.NewResource = new ApiWebAppResource() { Name = newResourceName };
            }
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.RenameResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a Resource that only has the information: 
        /// name which equals the newname</returns>
        public ApiTrueWithResourceResponse WebAppRenameResource(string webAppName, string resourceName, string newResourceName)
            => WebAppRenameResourceAsync(webAppName, resourceName, newResourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.RenameResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a Resource that only has the information: 
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(ApiWebAppData webApp, string resourceName,
            string newResourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppRenameResourceAsync(webApp.Name, resourceName, newResourceName, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.RenameResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a Resource that only has the information: 
        /// name which equals the newname</returns>
        public ApiTrueWithResourceResponse WebAppRenameResource(ApiWebAppData webApp, string resourceName, string newResourceName)
            => WebAppRenameResourceAsync(webApp, resourceName, newResourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.RenameResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the Resource given that has the following change: 
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource,
            string newResourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppRenameResourceAsync(webApp.Name, resource.Name, newResourceName, cancellationToken);
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Name = newResourceName;
            return basicResp;
        }
        /// <summary>
        /// Send a WebApp.RenameResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the Resource given that has the following change: 
        /// name which equals the newname</returns>
        public ApiTrueWithResourceResponse WebAppRenameResource(ApiWebAppData webApp, ApiWebAppResource resource, string newResourceName)
            => WebAppRenameResourceAsync(webApp, resource, newResourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.RenameResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the Resource given that has the following change: 
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(string webAppName, ApiWebAppResource resource,
            string newResourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppRenameResourceAsync(webAppName, resource.Name, newResourceName, cancellationToken);
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Name = newResourceName;
            return basicResp;
        }
        /// <summary>
        /// Send a WebApp.RenameResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the Resource given that has the following change: 
        /// name which equals the newname</returns>
        public ApiTrueWithResourceResponse WebAppRenameResource(string webAppName, ApiWebAppResource resource, string newResourceName)
            => WebAppRenameResourceAsync(webAppName, resource, newResourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the default page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps default page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Default_Page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(string webAppName, string resourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppSetDefaultPageRequest(webAppName, resourceName ?? "");
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = new ApiTrueWithWebAppResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if (responseObj.TrueOnSuccesResponse.Result)
            {
                responseObj.NewWebApp = new ApiWebAppData() { Name = webAppName, Default_page = (resourceName == "" ? null : resourceName) };
            }
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the default page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Default_Page:   which equals the resourceName
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetDefaultPage(string webAppName, string resourceName) => WebAppSetDefaultPageAsync(webAppName, resourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the default page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps default page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Default_Page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(ApiWebAppData webApp, string resourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resp = await WebAppSetDefaultPageAsync(webApp.Name, resourceName, cancellationToken);
            resp.NewWebApp = webApp.ShallowCopy();
            resp.NewWebApp.Default_page = (resourceName == "" ? null : resourceName);
            return resp;
        }
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the default page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Default_Page:   which equals the resourceName
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetDefaultPage(ApiWebAppData webApp, string resourceName) => WebAppSetDefaultPageAsync(webApp, resourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the default page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps default page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Default_Page:   which equals the resource.Name
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(ApiWebAppData webApp, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resp = await WebAppSetDefaultPageAsync(webApp.Name, resource.Name, cancellationToken);
            resp.NewWebApp = webApp.ShallowCopy();
            resp.NewWebApp.Default_page = (resource.Name == "" ? null : resource.Name);
            return resp;
        }
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the default page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Default_Page:   which equals the resource.Name
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetDefaultPage(ApiWebAppData webApp, ApiWebAppResource resource) => WebAppSetDefaultPageAsync(webApp, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the default page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps default page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Default_Page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(string webAppName, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppSetDefaultPageAsync(webAppName, resource.Name, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the default page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Default_Page:   which equals the resourceName
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetDefaultPage(string webAppName, ApiWebAppResource resource) => WebAppSetDefaultPageAsync(webAppName, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not authorized page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:                   which equals webAppName
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(string webAppName, string resourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppSetNotAuthorizedPageRequest(webAppName, resourceName ?? "");
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = new ApiTrueWithWebAppResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if (responseObj.TrueOnSuccesResponse.Result)
            {
                responseObj.NewWebApp = new ApiWebAppData() { Name = webAppName, Not_authorized_page = (resourceName == "" ? null : resourceName) };
            }
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:                   which equals webAppName
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetNotAuthorizedPage(string webAppName, string resourceName) => WebAppSetNotAuthorizedPageAsync(webAppName, resourceName).GetAwaiter().GetResult();
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not authorized page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(ApiWebAppData webApp, string resourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resp = await WebAppSetNotAuthorizedPageAsync(webApp.Name, resourceName, cancellationToken);
            resp.NewWebApp = webApp.ShallowCopy();
            resp.NewWebApp.Not_authorized_page = (resourceName == "" ? null : resourceName);
            return resp;
        }
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetNotAuthorizedPage(ApiWebAppData webApp, string resourceName) => WebAppSetNotAuthorizedPageAsync(webApp, resourceName).GetAwaiter().GetResult();
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not authorized page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(ApiWebAppData webApp, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resp = await WebAppSetNotAuthorizedPageAsync(webApp.Name, resource.Name, cancellationToken);
            resp.NewWebApp = webApp.ShallowCopy();
            resp.NewWebApp.Not_authorized_page = (resource.Name == "" ? null : resource.Name);
            return resp;
        }
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetNotAuthorizedPage(ApiWebAppData webApp, ApiWebAppResource resource) => WebAppSetNotAuthorizedPageAsync(webApp, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not authorized page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:                   which equals webAppName
        /// Not_authorized_page:    which equals the resource.Name
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(string webAppName, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppSetNotAuthorizedPageAsync(webAppName, resource.Name, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:                   which equals webAppName
        /// Not_authorized_page:    which equals the resource.Name
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetNotAuthorizedPage(string webAppName, ApiWebAppResource resource) => WebAppSetNotAuthorizedPageAsync(webAppName, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not found page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not found page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Not_found_page: which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(string webAppName, string resourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppSetNotFoundPageRequest(webAppName, resourceName ?? "");
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = new ApiTrueWithWebAppResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if (responseObj.TrueOnSuccesResponse.Result)
            {
                responseObj.NewWebApp = new ApiWebAppData() { Name = webAppName, Not_found_page = (resourceName == "" ? null : resourceName) };
            }
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not found page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Not_found_page: which equals the resourceName
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetNotFoundPage(string webAppName, string resourceName) => WebAppSetNotFoundPageAsync(webAppName, resourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not found page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not found page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_found_page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(ApiWebAppData webApp, string resourceName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resp = await WebAppSetNotFoundPageAsync(webApp.Name, resourceName, cancellationToken);
            resp.NewWebApp = webApp.ShallowCopy();
            resp.NewWebApp.Not_found_page = (resourceName == "" ? null : resourceName);
            return resp;
        }
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not found page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_found_page:   which equals the resourceName
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetNotFoundPage(ApiWebAppData webApp, string resourceName) => WebAppSetNotFoundPageAsync(webApp, resourceName).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not found page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not found page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_found_page:   which equals the resource.Name
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(ApiWebAppData webApp, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resp = await WebAppSetNotFoundPageAsync(webApp.Name, resource.Name, cancellationToken);
            resp.NewWebApp = webApp.ShallowCopy();
            resp.NewWebApp.Not_found_page = (resource.Name == "" ? null : resource.Name);
            return resp;
        }
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not found page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_found_page:   which equals the resource.Name
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetNotFoundPage(ApiWebAppData webApp, ApiWebAppResource resource) => WebAppSetNotFoundPageAsync(webApp, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not found page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not found page</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Not_found_page: which equals the resource.Name
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(string webAppName, ApiWebAppResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppSetNotFoundPageAsync(webAppName, resource.Name, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not found page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Not_found_page: which equals the resource.Name
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetNotFoundPage(string webAppName, ApiWebAppResource resource) => WebAppSetNotFoundPageAsync(webAppName, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetState Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the state should be set for</param>
        /// <param name="apiWebAppState">State the WebApp should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:  which equals the webAppName
        /// State: which equals the state
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetStateAsync(string webAppName, ApiWebAppState apiWebAppState, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppSetStateRequest(webAppName, apiWebAppState);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = new ApiTrueWithWebAppResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if (responseObj.TrueOnSuccesResponse.Result)
            {
                responseObj.NewWebApp = new ApiWebAppData() { Name = webAppName, State = apiWebAppState };
            }
            return responseObj;
        }

        /// <summary>
        /// Send a WebApp.SetState Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the state should be set for</param>
        /// <param name="newApiWebAppState">State the WebApp should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:  which equals the webAppName
        /// State: which equals the state
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetState(string webAppName, ApiWebAppState newApiWebAppState)
            => WebAppSetStateAsync(webAppName, newApiWebAppState).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebServer.SetDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="defaultPage"></param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse</returns>
        public async Task<ApiTrueOnSuccessResponse> WebServerSetDefaultPageAsync(string defaultPage, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebserverSetDefaultPageRequest(defaultPage);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a WebServer.SetDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="defaultPage"></param>
        /// <returns></returns>
        ApiTrueOnSuccessResponse IApiRequestHandler.WebServerSetDefaultPage(string defaultPage)
            => WebServerSetDefaultPageAsync(defaultPage).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebServer.ReadDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>Returns the default page in a response object</returns>
        public async Task<ApiWebServerGetReadDefaultPageResponse> WebServerGetReadDefaultPageAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebserverReadDefaultPageRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiWebServerGetReadDefaultPageResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a WebServer.ReadDefaultPage Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>Returns the default page in a response object</returns>
        public ApiWebServerGetReadDefaultPageResponse WebServerGetReadDefaultPage()
            => WebServerGetReadDefaultPageAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetState Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that state should be set for</param>
        /// <param name="apiWebAppState">State the WebApp should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// State: which equals the state
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetStateAsync(ApiWebAppData webApp, ApiWebAppState apiWebAppState, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resp = await WebAppSetStateAsync(webApp.Name, apiWebAppState, cancellationToken);
            resp.NewWebApp = webApp.ShallowCopy();
            resp.NewWebApp.State = apiWebAppState;
            return resp;
        }

        /// <summary>
        /// Send a WebApp.SetState Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that state should be set for</param>
        /// <param name="newApiWebAppState">State the WebApp should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// State: which equals the state
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetState(ApiWebAppData webApp, ApiWebAppState newApiWebAppState)
            => WebAppSetStateAsync(webApp, newApiWebAppState).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceETag Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Etag: which equals the newEtagValue
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(string webAppName, string resourceName,
            string newETagValue, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiSetResourceETagRequest(webAppName, resourceName, newETagValue ?? "");
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = new ApiTrueWithResourceResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if (responseObj.TrueOnSuccesResponse.Result)
            {
                responseObj.NewResource = new ApiWebAppResource() { Name = resourceName, Etag = (newETagValue == "" ? null : newETagValue) };
            }
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.SetResourceETag Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Etag: which equals the newEtagValue
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceETag(string webAppName, string resourceName, string newETagValue)
            => WebAppSetResourceETagAsync(webAppName, resourceName, newETagValue).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceETag Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Etag: which equals the newEtagValue
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(ApiWebAppData webApp, string resourceName,
            string newETagValue, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppSetResourceETagAsync(webApp.Name, resourceName, newETagValue, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.SetResourceETag Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Etag: which equals the newEtagValue
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceETag(ApiWebAppData webApp, string resourceName, string newETagValue)
            => WebAppSetResourceETagAsync(webApp, resourceName, newETagValue).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceETag Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Etag: which equals the newEtagValue
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(ApiWebAppData webApp, ApiWebAppResource resource,
            string newETagValue, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppSetResourceETagAsync(webApp.Name, resource.Name, newETagValue, cancellationToken);
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Etag = (newETagValue == "" ? null : newETagValue);
            return basicResp;

        }
        /// <summary>
        /// Send a WebApp.SetResourceETag Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Etag: which equals the newEtagValue
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceETag(ApiWebAppData webApp, ApiWebAppResource resource, string newETagValue)
            => WebAppSetResourceETagAsync(webApp, resource, newETagValue).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceETag Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Etag: which equals the newEtagValue
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(string webAppName, ApiWebAppResource resource,
            string newETagValue, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppSetResourceETagAsync(webAppName, resource.Name, newETagValue, cancellationToken);
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Etag = (newETagValue == "" ? null : newETagValue);
            return basicResp;
        }
        /// <summary>
        /// Send a WebApp.SetResourceETag Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Etag: which equals the newEtagValue
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceETag(string webAppName, ApiWebAppResource resource, string newETagValue)
            => WebAppSetResourceETagAsync(webAppName, resource, newETagValue).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:       which equals the resourceName
        /// MediaType:  which equals the newMediaType
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(string webAppName, string resourceName,
            string newMediaType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiSetResourceMediaTypeRequest(webAppName, resourceName, newMediaType);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = new ApiTrueWithResourceResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if (responseObj.TrueOnSuccesResponse.Result)
            {
                responseObj.NewResource = new ApiWebAppResource() { Name = resourceName, Media_type = newMediaType };
            }
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:       which equals the resourceName
        /// MediaType:  which equals the newMediaType
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceMediaType(string webAppName, string resourceName, string newMediaType)
            => WebAppSetResourceMediaTypeAsync(webAppName, resourceName, newMediaType).GetAwaiter().GetResult();


        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:       which equals the resourceName
        /// MediaType:  which equals the newMediaType
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(ApiWebAppData webApp, string resourceName,
            string newMediaType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppSetResourceMediaTypeAsync(webApp.Name, resourceName, newMediaType, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:       which equals the resourceName
        /// MediaType:  which equals the newMediaType
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceMediaType(ApiWebAppData webApp, string resourceName, string newMediaType)
            => WebAppSetResourceMediaTypeAsync(webApp, resourceName, newMediaType).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// MediaType: which equals the newMediaType
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(ApiWebAppData webApp, ApiWebAppResource resource,
            string newMediaType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppSetResourceMediaTypeAsync(webApp.Name, resource.Name, newMediaType, cancellationToken);
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Media_type = newMediaType;
            return basicResp;
        }
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// MediaType: which equals the newMediaType
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceMediaType(ApiWebAppData webApp, ApiWebAppResource resource, string newMediaType)
            => WebAppSetResourceMediaTypeAsync(webApp, resource, newMediaType).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// MediaType: which equals the newMediaType
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(string webAppName, ApiWebAppResource resource,
            string newMediaType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppSetResourceMediaTypeAsync(webAppName, resource.Name, newMediaType, cancellationToken);
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Media_type = newMediaType;
            return basicResp;
        }
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// MediaType: which equals the newMediaType
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceMediaType(string webAppName, ApiWebAppResource resource, string newMediaType)
            => WebAppSetResourceMediaTypeAsync(webAppName, resource, newMediaType).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, string resourceName,
            string newModificationTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiSetResourceModificationTimeRequest(webAppName, resourceName, newModificationTime);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = new ApiTrueWithResourceResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            responseObj.NewResource = new ApiWebAppResource()
            { Name = resourceName, Last_modified = XmlConvert.ToDateTime(newModificationTime, XmlDateTimeSerializationMode.Utc) };
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceModificationTime(string webAppName, string resourceName, string newModificationTime)
            => WebAppSetResourceModificationTimeAsync(webAppName, resourceName, newModificationTime).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, string resourceName,
            string newModificationTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppSetResourceModificationTimeAsync(webApp.Name, resourceName, newModificationTime, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceModificationTime(ApiWebAppData webApp, string resourceName, string newModificationTime)
            => WebAppSetResourceModificationTimeAsync(webApp, resourceName, newModificationTime).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, ApiWebAppResource resource,
            string newModificationTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppSetResourceModificationTimeAsync(webApp.Name, resource.Name, newModificationTime, cancellationToken);
            var last_mod = basicResp.NewResource.Last_modified;
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Last_modified = last_mod;
            return basicResp;
        }
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceModificationTime(ApiWebAppData webApp, ApiWebAppResource resource, string newModificationTime)
            => WebAppSetResourceModificationTimeAsync(webApp, resource, newModificationTime).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, string resourceName,
            DateTime newModificationTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppSetResourceModificationTimeAsync(webAppName, resourceName,
                newModificationTime.ToString(DateTimeFormatting.ApiDateTimeFormat), cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceModificationTime(string webAppName, string resourceName, DateTime newModificationTime)
            => WebAppSetResourceModificationTimeAsync(webAppName, resourceName, newModificationTime).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, string resourceName,
            DateTime newModificationTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppSetResourceModificationTimeAsync(webApp.Name, resourceName,
                newModificationTime.ToString(DateTimeFormatting.ApiDateTimeFormat), cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceModificationTime(ApiWebAppData webApp, string resourceName, DateTime newModificationTime)
            => WebAppSetResourceModificationTimeAsync(webApp, resourceName, newModificationTime).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, ApiWebAppResource resource,
            DateTime newModificationTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppSetResourceModificationTimeAsync(webApp.Name, resource.Name,
                newModificationTime.ToString(DateTimeFormatting.ApiDateTimeFormat), cancellationToken);
            var last_mod = basicResp.NewResource.Last_modified;
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Last_modified = last_mod;
            return basicResp;
        }
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceModificationTime(ApiWebAppData webApp, ApiWebAppResource resource, DateTime newModificationTime)
            => WebAppSetResourceModificationTimeAsync(webApp, resource, newModificationTime).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, ApiWebAppResource resource,
            DateTime newModificationTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppSetResourceModificationTimeAsync(webAppName, resource.Name,
                newModificationTime.ToString(DateTimeFormatting.ApiDateTimeFormat), cancellationToken);
            var last_mod = basicResp.NewResource.Last_modified;
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Last_modified = last_mod;
            return basicResp;
        }
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceModificationTime(string webAppName, ApiWebAppResource resource, DateTime newModificationTime)
            => WebAppSetResourceModificationTimeAsync(webAppName, resource, newModificationTime).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, ApiWebAppResource resource,
            string newModificationTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppSetResourceModificationTimeAsync(webAppName, resource.Name, newModificationTime, cancellationToken);
            var last_mod = basicResp.NewResource.Last_modified;
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Last_modified = last_mod;
            return basicResp;
        }
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceModificationTime(string webAppName, ApiWebAppResource resource, string newModificationTime)
            => WebAppSetResourceModificationTimeAsync(webAppName, resource, newModificationTime).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Visibility: which equals the newVisibility
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(string webAppName, string resourceName,
            ApiWebAppResourceVisibility newResourceVisibility, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiSetResourceVisibilityRequest(webAppName, resourceName, newResourceVisibility);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = new ApiTrueWithResourceResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if (responseObj.TrueOnSuccesResponse.Result)
            {
                responseObj.NewResource = new ApiWebAppResource() { Name = resourceName, Visibility = newResourceVisibility };
            }
            return responseObj;
        }
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Visibility: which equals the newVisibility
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceVisibility(string webAppName, string resourceName, ApiWebAppResourceVisibility newResourceVisibility)
            => WebAppSetResourceVisibilityAsync(webAppName, resourceName, newResourceVisibility).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(ApiWebAppData webApp, string resourceName,
            ApiWebAppResourceVisibility newResourceVisibility, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await WebAppSetResourceVisibilityAsync(webApp.Name, resourceName, newResourceVisibility, cancellationToken);
        }
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceVisibility(ApiWebAppData webApp, string resourceName, ApiWebAppResourceVisibility newResourceVisibility)
            => WebAppSetResourceVisibilityAsync(webApp, resourceName, newResourceVisibility).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(ApiWebAppData webApp, ApiWebAppResource resource,
            ApiWebAppResourceVisibility newResourceVisibility, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppSetResourceVisibilityAsync(webApp.Name, resource.Name, newResourceVisibility, cancellationToken);
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Visibility = newResourceVisibility;
            return basicResp;
        }
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceVisibility(ApiWebAppData webApp, ApiWebAppResource resource, ApiWebAppResourceVisibility newResourceVisibility)
           => WebAppSetResourceVisibilityAsync(webApp, resource, newResourceVisibility).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(string webAppName, ApiWebAppResource resource,
            ApiWebAppResourceVisibility newResourceVisibility, CancellationToken cancellationToken = default(CancellationToken))
        {
            var basicResp = await WebAppSetResourceVisibilityAsync(webAppName, resource.Name, newResourceVisibility, cancellationToken);
            basicResp.NewResource = resource.ShallowCopy();
            basicResp.NewResource.Visibility = newResourceVisibility;
            return basicResp;
        }
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        public ApiTrueWithResourceResponse WebAppSetResourceVisibility(string webAppName, ApiWebAppResource resource, ApiWebAppResourceVisibility newResourceVisibility)
            => WebAppSetResourceVisibilityAsync(webAppName, resource, newResourceVisibility).GetAwaiter().GetResult();

        /// <summary>
        ///  Function to get the ByteArray and the HTTP response Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>HTTP response</returns>
        public async Task<HttpResponseMessage> DownloadTicketAndGetResponseAsync(string ticketId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request_body = new ByteArrayContent(new byte[0]);
            request_body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            var response = await _httpClient.PostAsync($"/api/ticket?id={ticketId}", request_body, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        /// <summary>
        ///  Function to get the ByteArray and the HTTP response Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <returns>HTTP response</returns>
        public HttpResponseMessage DownloadTicketAndGetResponse(string ticketId)
            => DownloadTicketAndGetResponseAsync(ticketId).GetAwaiter().GetResult();

        /// <summary>
        ///  Function to get the ByteArray and the HTTP response Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticket">The Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>HTTP response</returns>
        public async Task<HttpResponseMessage> DownloadTicketAndGetResponseAsync(ApiTicket ticket, CancellationToken cancellationToken = default(CancellationToken))
            => await DownloadTicketAndGetResponseAsync(ticket.Id, cancellationToken);

        /// <summary>
        ///  Function to get the ByteArray and the HTTP response Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticket">The Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <returns>HTTP response</returns>
        public HttpResponseMessage DownloadTicketAndGetResponse(ApiTicket ticket)
            => DownloadTicketAndGetResponseAsync(ticket).GetAwaiter().GetResult();

        /// <summary>
        /// Function to get the ByteArray Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>Bytearray given from the PLC</returns>
        public async Task<byte[]> DownloadTicketAsync(string ticketId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await DownloadTicketAndGetResponseAsync(ticketId, cancellationToken);
            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Function to get the ByteArray Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <returns>Bytearray given from the PLC</returns>
        public byte[] DownloadTicket(string ticketId) => DownloadTicketAsync(ticketId).GetAwaiter().GetResult();

        /// <summary>
        /// Function to get the ByteArray Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticket">The Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>Bytearray given from the PLC</returns>
        public async Task<byte[]> DownloadTicketAsync(ApiTicket ticket, CancellationToken cancellationToken = default(CancellationToken))
         => await DownloadTicketAsync(ticket.Id, cancellationToken);

        /// <summary>
        /// Function to get the ByteArray Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticket">The Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <returns>Bytearray given from the PLC</returns>
        public byte[] DownloadTicket(ApiTicket ticket) => DownloadTicketAsync(ticket).GetAwaiter().GetResult();

        /// <summary>
        /// Function to send the ByteArrayContent for a Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="data">ByteArray that should be sent to the plc Ticketing Endpoint</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Task/void</returns>
        public async Task UploadTicketAsync(string ticketId, ByteArrayContent data, CancellationToken cancellationToken = default(CancellationToken))
        {
            data.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            try
            {
                var response = await _httpClient.PostAsync($"/api/ticket?id={ticketId}", data, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                throw new ApiTicketingEndpointUploadException(ticketId, e);
            }
        }
        /// <summary>
        /// Function to send the ByteArrayContent for a Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="data">ByteArray that should be sent to the plc Ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        public void UploadTicket(string ticketId, ByteArrayContent data) => UploadTicketAsync(ticketId, data).GetAwaiter().GetResult();

        /// <summary>
        /// Function to send the ByteArrayContent for a Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticket">The Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="data">ByteArray that should be sent to the plc Ticketing Endpoint</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Task/void</returns>
        public async Task UploadTicketAsync(ApiTicket ticket, ByteArrayContent data, CancellationToken cancellationToken = default(CancellationToken))
            => await UploadTicketAsync(ticket.Id, data, cancellationToken);

        /// <summary>
        /// Function to send the ByteArrayContent for a Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticket">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="data">ByteArray that should be sent to the plc Ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        public void UploadTicket(ApiTicket ticket, ByteArrayContent data)
            => UploadTicketAsync(ticket, data).GetAwaiter().GetResult();

        /// <summary>
        /// Function to Read and send the ByteArrayContent for a file with the Ticketing Endpoint Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="pathToFile">File Bytes will be Read and saved into ByteArrayContent - then sent to the ticketing Endpoint</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Task/void</returns>
        public async Task UploadTicketAsync(string ticketId, string pathToFile, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException($"file at: {pathToFile} not found!");
            }
            var fileContent = new ByteArrayContent(File.ReadAllBytes(pathToFile));
            await UploadTicketAsync(ticketId, fileContent, cancellationToken);
        }
        /// <summary>
        /// Function to Read and send the ByteArrayContent for a file with the Ticketing Endpoint Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="pathToFile">File Bytes will be Read and saved into ByteArrayContent - then sent to the ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        public void UploadTicket(string ticketId, string pathToFile) => UploadTicketAsync(ticketId, pathToFile).GetAwaiter().GetResult();

        /// <summary>
        /// Function to Read and send the ByteArrayContent for a file with the Ticketing Endpoint Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="pathToFile">File Bytes will be Read and saved into ByteArrayContent - then sent to the ticketing Endpoint</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Task/void</returns>
        public async Task UploadTicketAsync(string ticketId, FileInfo pathToFile, CancellationToken cancellationToken = default(CancellationToken))
            => await UploadTicketAsync(ticketId, pathToFile.FullName, cancellationToken);
        /// <summary>
        /// Function to Read and send the ByteArrayContent for a file with the Ticketing Endpoint Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="pathToFile">File Bytes will be Read and saved into ByteArrayContent - then sent to the ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        public void UploadTicket(string ticketId, FileInfo pathToFile)
            => UploadTicketAsync(ticketId, pathToFile).GetAwaiter().GetResult();

        /// <summary>
        /// Function to Read and send the ByteArrayContent for a file with the Ticketing Endpoint Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticket">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="pathToFile">File Bytes will be Read and saved into ByteArrayContent - then sent to the ticketing Endpoint</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Task/void</returns>
        public async Task UploadTicketAsync(ApiTicket ticket, string pathToFile, CancellationToken cancellationToken = default(CancellationToken))
         => await UploadTicketAsync(ticket.Id, pathToFile, cancellationToken);

        /// <summary>
        /// Function to Read and send the ByteArrayContent for a file with the Ticketing Endpoint Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticket">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="pathToFile">File Bytes will be Read and saved into ByteArrayContent - then sent to the ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        public void UploadTicket(ApiTicket ticket, string pathToFile) => UploadTicketAsync(ticket, pathToFile).GetAwaiter().GetResult();

        /// <summary>
        /// Function to Read and send the ByteArrayContent for a file with the Ticketing Endpoint Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticket">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="pathToFile">File Bytes will be Read and saved into ByteArrayContent - then sent to the ticketing Endpoint</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Task/void</returns>
        public async Task UploadTicketAsync(ApiTicket ticket, FileInfo pathToFile, CancellationToken cancellationToken = default(CancellationToken))
            => await UploadTicketAsync(ticket, pathToFile.FullName, cancellationToken);
        /// <summary>
        /// Function to Read and send the ByteArrayContent for a file with the Ticketing Endpoint Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticket">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="pathToFile">File Bytes will be Read and saved into ByteArrayContent - then sent to the ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        public void UploadTicket(ApiTicket ticket, FileInfo pathToFile)
            => UploadTicketAsync(ticket, pathToFile).GetAwaiter().GetResult();

        /// <summary>
        /// Relogin
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns></returns>
        public async Task<ApiLoginResponse> ReLoginAsync(string userName, string password, bool? includeWebApplicationCookie = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ApiLogoutAsync(cancellationToken);
            return await ApiLoginAsync(userName, password, includeWebApplicationCookie, cancellationToken);
        }

        /// <summary>
        /// Relogin
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns></returns>
        public async Task<ApiLoginResponse> ReLoginAsync(string userName, string password, ApiAuthenticationMode loginMode, bool? includeWebApplicationCookie = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ApiLogoutAsync(cancellationToken);
            return await ApiLoginAsync(userName, password, loginMode, includeWebApplicationCookie, cancellationToken);
        }

        /// <summary>
        /// Relogin
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <returns></returns>
        public ApiLoginResponse ReLogin(string userName, string password, bool? includeWebApplicationCookie = null) => ReLoginAsync(userName, password, includeWebApplicationCookie).GetAwaiter().GetResult();

        /// <summary>
        /// Relogin
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <returns></returns>
        public ApiLoginResponse ReLogin(string userName, string password, ApiAuthenticationMode loginMode, bool? includeWebApplicationCookie = null) => ReLoginAsync(userName, password, loginMode, includeWebApplicationCookie).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Api.Login Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <returns>ApiLoginResponse: contains ApiTokenResult: Token(auth token string) and if requested Web_application_cookie</returns>
        public ApiLoginResponse ApiLogin(string userName, string password, bool? includeWebApplicationCookie = null) => ApiLoginAsync(userName, password, includeWebApplicationCookie).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Api.Login Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="mode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <returns>ApiLoginResponse: contains ApiTokenResult: Token(auth token string) and if requested Web_application_cookie</returns>
        public ApiLoginResponse ApiLogin(string userName, string password, ApiAuthenticationMode mode, bool? includeWebApplicationCookie = null) => ApiLoginAsync(userName, password, includeWebApplicationCookie).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Api.Login Request 
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiLoginResponse: contains ApiTokenResult: Token(auth token string) and if requested Web_application_cookie</returns>
        public async Task<ApiLoginResponse> ApiLoginAsync(string userName, string password, bool? includeWebApplicationCookie = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ApiLoginBaseAsync(userName, password, null, includeWebApplicationCookie, cancellationToken);
        }

        /// <summary>
        /// Send a Api.Login Request 
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <param name="mode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiLoginResponse: contains ApiTokenResult: Token(auth token string) and if requested Web_application_cookie</returns>
        public async Task<ApiLoginResponse> ApiLoginAsync(string userName, string password, ApiAuthenticationMode mode, bool? includeWebApplicationCookie = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ApiLoginBaseAsync(userName, password, mode, includeWebApplicationCookie, cancellationToken);
        }

        /// <summary>
        /// Send a Api.Login Request 
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <param name="mode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiLoginResponse: contains ApiTokenResult: Token(auth token string) and if requested Web_application_cookie</returns>
        private async Task<ApiLoginResponse> ApiLoginBaseAsync(string userName, string password, ApiAuthenticationMode? mode = null, bool? includeWebApplicationCookie = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiLoginRequest(userName, password, includeWebApplicationCookie, mode);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = new ApiLoginResponse();
            responseObj = JsonConvert.DeserializeObject<ApiLoginResponse>(response);
            if (!string.IsNullOrEmpty(responseObj.Result.Token))
            {
                if (_httpClient.DefaultRequestHeaders.Any(x => x.Key.Contains("X-Auth-Token")))
                {
                    _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
                }
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", responseObj.Result.Token);
            }
            return responseObj;
        }

        /// <summary>
        /// Send an Api Bulk Request
        /// </summary>
        /// <param name="apiRequests">Api Requests to be sent as Bulk</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>List of ApiResultResponses with Result as object - not "directly" casted to the expected Result type</returns>
        public async Task<ApiBulkResponse> ApiBulkAsync(IEnumerable<IApiRequest> apiRequests, CancellationToken cancellationToken = default(CancellationToken))
        {
            if ((apiRequests.GroupBy(el => el.Id).Count() != apiRequests.Count()))
            {
                throw new ArgumentException($"{nameof(apiRequests)} contains multiple requests with the same Id!");
            }
            foreach (var apiRequest in apiRequests)
            {
                if (apiRequest.Params != null)
                {
                    apiRequest.Params = apiRequest.Params
                        .Where(el => el.Value != null)
                        .ToDictionary(x => x.Key, x => x.Value);
                }
            }
            string apiRequestString = JsonConvert.SerializeObject(apiRequests, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.GetBytes(apiRequestString);
            var messageChunks = new List<byte[]>();
            if (byteArr.Length >= MaxRequestSize)
            {
                // comma needs to be added for multiple requests within Bulk
                var commaBytes = Encoding.GetBytes(",");
                _logger?.LogInformation($"Chunk the Requests into multiple Bulk Requests '{byteArr.Length}' is > '{MaxRequestSize}' -> split into sub requests.");
                var chunkLenSum = 0;
                var commaMissingSum = 0;
                var currentStream = new MemoryStream();
                foreach (var request in apiRequests)
                {
                    string requestString = JsonConvert.SerializeObject(request, new JsonSerializerSettings()
                    { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    byte[] requestByteArr = Encoding.GetBytes(requestString);
                    if (requestByteArr.Length > MaxRequestSize)
                    {
                        throw new InvalidOperationException($"Request Size '{requestByteArr.Length}' is bigger than the " +
                            $"MaxRequestSize '{MaxRequestSize}'! -> Not Possible to Chunk this Message!");
                    }
                    if (currentStream.Length + requestByteArr.Length + commaBytes.Length < MaxRequestSize)
                    {
                        currentStream.Append(commaBytes);
                        currentStream.Append(requestByteArr); // append it to the current stream
                    }
                    else // save the current stream, add the current request to a new byte array
                    {
                        var currentBuffer = currentStream.ToArray();
                        messageChunks.Add(currentBuffer);
                        chunkLenSum += currentBuffer.Length;
                        currentStream = new MemoryStream();
                        currentStream.Append(requestByteArr);
                        commaMissingSum += commaBytes.Length;
                    }
                }
                var currentBufferAfterwards = currentStream.ToArray();
                messageChunks.Add(currentBufferAfterwards);
                chunkLenSum += currentBufferAfterwards.Length;
                commaMissingSum += commaBytes.Length;
                // check for differences between buffers and the 'original' byte array
                if ((chunkLenSum + commaMissingSum) != byteArr.Length)
                {
                    throw new InvalidOperationException($"Programming error in Message Chunking -> chunks len together: '{chunkLenSum}' but they should be '{byteArr.Length}'!");
                }
            }
            else
            {
                messageChunks.Add(byteArr);
            }
            ApiBulkResponse result = new ApiBulkResponse();
            var successResponses = new List<ApiResultResponse<object>>();
            foreach (var messageChunk in messageChunks)
            {
                ByteArrayContent request_body = new ByteArrayContent(byteArr);
                request_body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
                var response = await _httpClient.PostAsync(JsonRpcApi, request_body, cancellationToken);
                _apiResponseChecker.CheckHttpResponseForErrors(response, apiRequestString);
                var responseString = await response.Content.ReadAsStringAsync();
                ApiBulkResponse bulkResponse = new ApiBulkResponse();
                var errorResponses = JsonConvert.DeserializeObject<IEnumerable<ApiErrorModel>>(responseString)
                    .Where(el => el.Error != null);
                bulkResponse.ErrorResponses = errorResponses;
                var successfulResponses = JsonConvert.DeserializeObject<IEnumerable<ApiResultResponse<object>>>(responseString)
                    .Where(el => el.Result != null);
                bulkResponse.SuccessfulResponses = successfulResponses;
                if (bulkResponse.ContainsErrors)
                {
                    throw new ApiBulkRequestException(bulkResponse);
                }
                successResponses.AddRange(bulkResponse.SuccessfulResponses);
            }
            result.SuccessfulResponses = successResponses;
            return result;
        }
        /// <summary>
        /// Send an Api Bulk Request
        /// </summary>
        /// <param name="apiRequests">Api Requests to be sent as Bulk</param>
        /// <returns>List of ApiResultResponses with Result as object - not "directly" casted to the expected Result type</returns>
        public ApiBulkResponse ApiBulk(IEnumerable<IApiRequest> apiRequests) => ApiBulkAsync(apiRequests).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.ReadSystemTime Request
        /// </summary>
        /// <returns>Current Plc Utc System Time</returns>
        public async Task<ApiPlcReadSystemTimeResponse> PlcReadSystemTimeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcReadSystemTimeRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiPlcReadSystemTimeResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Plc.ReadSystemTime Request
        /// </summary>
        /// <returns>Current Plc Utc System Time</returns>
        public ApiPlcReadSystemTimeResponse PlcReadSystemTime() => PlcReadSystemTimeAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send an Plc.SetSystemTime Request
        /// </summary>
        /// <param name="timestamp">The timestamp of the system time to be set</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if time was set successfully</returns>
        public async Task<ApiTrueOnSuccessResponse> PlcSetSystemTimeAsync(DateTime timestamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcSetSystemTimeRequest(timestamp);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send an Plc.SetSystemTime Request
        /// </summary>
        /// <param name="timestamp">The timestamp of the system time to be set</param>
        /// <returns>True if time was set successfully</returns>
        public ApiTrueOnSuccessResponse PlcSetSystemTime(DateTime timestamp) =>
            PlcSetSystemTimeAsync(timestamp).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.ReadTimeSettings Request
        /// </summary>
        /// <returns>Current Plc Time Settings</returns>
        public async Task<ApiPlcReadTimeSettingsResponse> PlcReadTimeSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcReadTimeSettingsRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiPlcReadTimeSettingsResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Plc.ReadTimeSettings Request
        /// </summary>
        /// <returns>Current Plc Time Settings</returns>
        public ApiPlcReadTimeSettingsResponse PlcReadTimeSettings() => PlcReadTimeSettingsAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.SetTimeSettings Request with parameters
        /// </summary>
        /// <param name="utcOffset">The time zone offset from the UTC time in hours</param>
        /// <param name="daylightSavings">(Optional) Represents the settings for daylight-savings. If there is no daylight-savings rule configured, the utcOffset is applied to calculate the local time</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if the settings are applied successfully</returns>
        public async Task<ApiTrueOnSuccessResponse> PlcSetTimeSettingsAsync(TimeSpan utcOffset, DaylightSavingsRule daylightSavings = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcSetTimeSettingsRequest(utcOffset, daylightSavings);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Plc.SetTimeSettings Request with parameters
        /// </summary>
        /// <param name="utcOffset">The time zone offset from the UTC time in hours</param>
        /// <param name="daylightSavings">(Optional) Represents the settings for daylight-savings. If there is no daylight-savings rule configured, the utcOffset is applied to calculate the local time</param>
        /// <returns>True if the settings are applied successfully</returns>
        public ApiTrueOnSuccessResponse PlcSetTimeSettings(TimeSpan utcOffset, DaylightSavingsRule daylightSavings = null) =>
            PlcSetTimeSettingsAsync(utcOffset, daylightSavings = null).GetAwaiter().GetResult();
        /// <summary>
        /// Send a Files.Browse Request
        /// </summary>
        /// <param name="resource">Path of the directory or file relative to the memory card root to fetch the entry list. 
        /// The resource name must start with a "/". The parameter may be omitted.In that case, it will default to "/".</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Browsed resources (files/dir/...)</returns>
        public async Task<ApiBrowseFilesResponse> FilesBrowseAsync(string resource = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiFilesBrowseRequest(string.IsNullOrEmpty(resource) ? "/" : resource);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiBrowseFilesResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Files.Browse Request
        /// </summary>
        /// <param name="resource">Path of the directory or file relative to the memory card root to fetch the entry list. 
        /// The resource name must start with a "/". The parameter may be omitted.In that case, it will default to "/".</param>
        /// <returns>Browsed resources (files/dir/...)</returns>
        public ApiBrowseFilesResponse FilesBrowse(string resource = null) => FilesBrowseAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Files.Browse Request
        /// </summary>
        /// <param name="resource">resource to browse: file/dir/...</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Browsed resources (files/dir/...)</returns>
        public async Task<ApiBrowseFilesResponse> FilesBrowseAsync(ApiFileResource resource, CancellationToken cancellationToken = default(CancellationToken))
            => await FilesBrowseAsync(resource.GetVarNameForMethods(), cancellationToken);

        /// <summary>
        /// Send a Files.Browse Request
        /// </summary>
        /// <param name="resource">resource to browse: file/dir/...</param>
        /// <returns>Browsed resources (files/dir/...)</returns>
        public ApiBrowseFilesResponse FilesBrowse(ApiFileResource resource) => FilesBrowseAsync(resource).GetAwaiter().GetResult();


        /// <summary>
        /// Send a Files.Download Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Ticket ID.</returns>
        public async Task<ApiTicketIdResponse> FilesDownloadAsync(string resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiFilesDownloadRequest(resource);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTicketIdResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Files.Download Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>Ticket ID.</returns>
        public ApiTicketIdResponse FilesDownload(string resource) => FilesDownloadAsync(resource).GetAwaiter().GetResult();


        /// <summary>
        /// Send a Files.Create request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Ticket ID.</returns>
        public async Task<ApiTicketIdResponse> FilesCreateAsync(string resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiFilesCreateRequest(resource);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var singleStringResp = JsonConvert.DeserializeObject<ApiSingleStringResponse>(response);
            var responseObj = new ApiTicketIdResponse(singleStringResp);
            return responseObj;
        }

        /// <summary>
        /// Send a Files.Create request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>Ticket ID.</returns>
        public ApiTicketIdResponse FilesCreate(string resource) => FilesCreateAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Files.Create request
        /// </summary>
        /// <param name="resource">FileInfo for informations about the file to the memory card root.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Ticket ID.</returns>
        public async Task<ApiTicketIdResponse> FilesCreateAsync(FileInfo resource, CancellationToken cancellationToken = default(CancellationToken))
        => await FilesCreateAsync(resource.FullName, cancellationToken);

        /// <summary>
        /// Send a Files.Create request
        /// </summary>
        /// <param name="resource">FileInfo for informations about the file to the memory card root.</param>
        /// <returns>Ticket ID.</returns>
        public ApiTicketIdResponse FilesCreate(FileInfo resource) => FilesCreateAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send Files.Rename request
        /// </summary>
        /// <param name="resource">Current path of file/folder</param>
        /// <param name="new_resource">New path of file/folder</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if the file or folder is renamed successfully</returns>
        public async Task<ApiTrueOnSuccessResponse> FilesRenameAsync(string resource, string new_resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiFilesRenameRequest(resource, new_resource);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send Files.Rename request
        /// </summary>
        /// <param name="resource">Current path of file/folder</param>
        /// <param name="new_resource">New path of file/folder</param>
        /// <returns>True if the file or folder is renamed successfully</returns>
        public ApiTrueOnSuccessResponse FilesRename(string resource, string new_resource) => FilesRenameAsync(resource, new_resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Files.Delete Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if the file is deleted successfully</returns>
        public async Task<ApiTrueOnSuccessResponse> FilesDeleteAsync(string resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiFilesDeleteRequest(resource);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Files.Delete Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>True if the file is deleted successfully</returns>
        public ApiTrueOnSuccessResponse FilesDelete(string resource) => FilesDeleteAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if the directory is created successfully</returns>
        public async Task<ApiTrueOnSuccessResponse> FilesCreateDirectoryAsync(string resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiFilesCreateDirectoryRequest(resource);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>True if the directory is created successfully</returns>
        public ApiTrueOnSuccessResponse FilesCreateDirectory(string resource) => FilesCreateDirectoryAsync(resource).GetAwaiter().GetResult();


        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">DirectoryInfo for informations about the file to the memory card root.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if the directory is created successfully</returns>
        public async Task<ApiTrueOnSuccessResponse> FilesCreateDirectoryAsync(DirectoryInfo resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiFilesCreateDirectoryRequest(resource.FullName);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">DirectoryInfo for informations about the file to the memory card root.</param>
        /// <returns>True if the directory is created successfully</returns>
        public ApiTrueOnSuccessResponse FilesCreateDirectory(DirectoryInfo resource) => FilesCreateDirectoryAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">The resource to create</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if the directory is created successfully</returns>
        public async Task<ApiTrueOnSuccessResponse> FilesCreateDirectoryAsync(ApiFileResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var varNameForMethods = resource.GetVarNameForMethods();
            return await FilesCreateDirectoryAsync(varNameForMethods, cancellationToken);
        }


        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">The resource to create.</param>
        /// <returns>True if the directory is created successfully</returns>
        public ApiTrueOnSuccessResponse FilesCreateDirectory(ApiFileResource resource)
         => FilesCreateDirectoryAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Files.DeleteDirectory Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if the directory is deleted successfully</returns>
        public async Task<ApiTrueOnSuccessResponse> FilesDeleteDirectoryAsync(string resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiFilesDeleteDirectoryRequest(resource);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Files.DeleteDirectory Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>True if the directory is deleted successfully</returns>
        public ApiTrueOnSuccessResponse FilesDeleteDirectory(string resource) => FilesDeleteDirectoryAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a DataLogs.DownloadAndClear Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Ticket ID.</returns>
        public async Task<ApiTicketIdResponse> DatalogsDownloadAndClearAsync(string resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiDatalogsDownloadAndClearRequest(resource);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTicketIdResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a DataLogs.DownloadAndClear Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>Ticket ID.</returns>
        public ApiTicketIdResponse DatalogsDownloadAndClear(string resource) => DatalogsDownloadAndClearAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send Plc.CreateBackup Request
        /// </summary>
        /// <returns>Ticket ID.</returns>
        public async Task<ApiTicketIdResponse> PlcCreateBackupAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetPlcCreateBackupRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTicketIdResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send Plc.CreateBackup Request
        /// </summary>
        /// <returns>Ticket ID.</returns>
        public ApiTicketIdResponse PlcCreateBackup() => PlcCreateBackupAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.RestoreBackup Request
        /// </summary>
        /// <param name="password"></param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns></returns>
        public async Task<ApiTicketIdResponse> PlcRestoreBackupAsync(string password = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetPlcRestoreBackupRequest(password);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiTicketIdResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Plc.RestoreBackup Request
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public ApiTicketIdResponse PlcRestoreBackup(string password = "") => PlcRestoreBackupAsync(password).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Files.Delete Request
        /// </summary>
        /// <param name="resource">the resource that shall be deleted.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if the file is deleted successfully</returns>
        public async Task<ApiTrueOnSuccessResponse> FilesDeleteAsync(ApiFileResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var varNameForMethods = resource.GetVarNameForMethods();
            return await FilesDeleteAsync(varNameForMethods, cancellationToken);
        }

        /// <summary>
        /// Send a Files.Delete Request
        /// </summary>
        /// <param name="resource">the resource that shall be deleted.</param>
        /// <returns>True if the file is deleted successfully</returns>
        public ApiTrueOnSuccessResponse FilesDelete(ApiFileResource resource)
        => FilesDeleteAsync(resource).GetAwaiter().GetResult();


        /// <summary>
        /// Send a Files.DeleteDirectory Request
        /// </summary>
        /// <param name="resource">the directory to delete.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>True if the directory is deleted successfully</returns>
        public async Task<ApiTrueOnSuccessResponse> FilesDeleteDirectoryAsync(ApiFileResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var varNameForMethods = resource.GetVarNameForMethods();
            return await FilesDeleteDirectoryAsync(varNameForMethods, cancellationToken);
        }

        /// <summary>
        /// Send a Files.DeleteDirectory Request
        /// </summary>
        /// <param name="resource">the directory to delete.</param>
        /// <returns>True if the directory is deleted successfully</returns>
        public ApiTrueOnSuccessResponse FilesDeleteDirectory(ApiFileResource resource)
        => FilesDeleteDirectoryAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Failsafe.ReadParameters request
        /// </summary>
        /// <param name="hwid">The hardware identifier from which the parameters shall be read</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Response with Failsafe parameters</returns>
        public async Task<ApiFailsafeReadParametersResponse> FailsafeReadParametersAsync(uint hwid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetFailsafeReadParametersRequest(hwid);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiFailsafeReadParametersResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Failsafe.ReadParameters request
        /// </summary>
        /// <param name="hwid">The hardware identifier from which the parameters shall be read</param>
        /// <returns>Response with Failsafe parameters</returns>
        public ApiFailsafeReadParametersResponse FailsafeReadParameters(uint hwid) =>
            FailsafeReadParametersAsync(hwid).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Failsafe.ReadRuntimeGroups request
        /// </summary>
        /// <returns>Response with Runtime Groups</returns>
        public async Task<ApiFailsafeReadRuntimeGroupsResponse> FailsafeReadRuntimeGroupsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetFailsafeReadRuntimeGroupsRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiFailsafeReadRuntimeGroupsResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Failsafe.ReadRuntimeGroups request
        /// </summary>
        /// <returns>Response with Runtime Groups</returns>
        public ApiFailsafeReadRuntimeGroupsResponse FailsafeReadRuntimeGroups() => FailsafeReadRuntimeGroupsAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.GetPasswordPolicy request
        /// </summary>
        /// <returns>ApiGetPasswordPolicy response</returns>
        public async Task<ApiGetPasswordPolicyResponse> ApiGetPasswordPolicyAsync(ApiAuthenticationMode mode = ApiAuthenticationMode.Local, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiGetPasswordPolicyRequest(mode);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiGetPasswordPolicyResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send an Api.GetPasswordPolicy request
        /// </summary>
        /// <returns>ApiGetPasswordPolicy response</returns>
        public ApiGetPasswordPolicyResponse ApiGetPasswordPolicy(ApiAuthenticationMode mode = ApiAuthenticationMode.Local) => ApiGetPasswordPolicyAsync(mode).GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.GetAuthenticationMode request
        /// </summary>
        /// <returns>A response containing the authentication modes</returns>
        public async Task<ApiGetAuthenticationModeResponse> ApiGetAuthenticationModeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiGetAuthenticationModeRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiGetAuthenticationModeResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send an Api.GetAuthenticationMode request
        /// </summary>
        /// <returns>A response containing the authentication modes</returns>
        public ApiGetAuthenticationModeResponse ApiGetAuthenticationMode() => ApiGetAuthenticationModeAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a Project.ReadLanguages Request
        /// </summary>
        /// <param name="mode">Determines whether all or only active languages should be returned. Default is 'active'.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Languages Response containing a list of languages</returns>
        public async Task<ApiReadLanguagesResponse> ProjectReadLanguagesAsync(ApiReadLanguagesMode? mode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiProjectReadLanguagesRequest(mode);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiReadLanguagesResponse>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Project.ReadLanguages Request
        /// </summary>
        /// <param name="mode">Determines whether all or only active languages should be returned. Default is 'active'.</param>
        /// <returns>Languages Response containing a list of languages</returns>
        public ApiReadLanguagesResponse ProjectReadLanguages(ApiReadLanguagesMode? mode = null) => ProjectReadLanguagesAsync(mode).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.ReadModeSelectorState request
        /// </summary>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Mode Selector state</returns>
        public async Task<ApiPlcReadModeSelectorStateResponse> PlcReadModeSelectorStateAsync(ApiPlcRedundancyId rhid = ApiPlcRedundancyId.StandardPLC, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcReadModeSelectorStateRequest(rhid);
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiPlcReadModeSelectorStateResponse>(response);
        }

        /// <summary>
        /// Send a Plc.ReadModeSelectorState request
        /// </summary>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <returns>Mode Selector state</returns>
        public ApiPlcReadModeSelectorStateResponse PlcReadModeSelectorState(ApiPlcRedundancyId rhid = ApiPlcRedundancyId.StandardPLC) =>
            PlcReadModeSelectorStateAsync(rhid).GetAwaiter().GetResult();

        /// <summary>
        /// This API method allows the user to read content of the PLC-internal syslog ring buffer.
        /// </summary>
        /// <param name="redundancy_id">(optional) The Redundancy ID parameter must be present when the request is executed on an R/H PLC. <br/> 
        ///                             In this case it must either have a value of 1 or 2, otherwise it is null.</param>
        /// <param name="count">(optional) The maximum number of syslog entries to be requested. Default value: 50 <br/>
        ///                     A count of 0 will omit any syslog entries from the response and only return the attributes last_modified, count_total and count_lost.</param>
        /// <param name="first">Optionally allows the user to provide the id of an entry as a starting point for the returned entries array. <br/>
        ///                     This allows the user to traverse through the syslog buffer using multiple API calls.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiSyslogBrowseResponse</returns>
        public async Task<ApiSyslogBrowseResponse> ApiSyslogBrowseAsync(ApiPlcRedundancyId? redundancy_id = null, uint? count = null, uint? first = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiSyslogBrowseRequest(redundancy_id, count, first);
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiSyslogBrowseResponse>(response);
        }
        /// <summary>
        /// This API method allows the user to read content of the PLC-internal syslog ring buffer.
        /// </summary>
        /// <param name="redundancy_id">(optional) The Redundancy ID parameter must be present when the request is executed on an R/H PLC. <br/> 
        ///                             In this case it must either have a value of 1 or 2, otherwise it is null.</param>
        /// <param name="count">(optional) The maximum number of syslog entries to be requested. Default value: 50 <br/>
        ///                     A count of 0 will omit any syslog entries from the response and only return the attributes last_modified, count_total and count_lost.</param>
        /// <param name="first">Optionally allows the user to provide the id of an entry as a starting point for the returned entries array. <br/>
        ///                     This allows the user to traverse through the syslog buffer using multiple API calls.</param>
        /// <returns>ApiSyslogBrowseResponse</returns>
        public ApiSyslogBrowseResponse ApiSyslogBrowse(ApiPlcRedundancyId? redundancy_id = null, uint? count = null, uint? first = null) => ApiSyslogBrowseAsync(redundancy_id, count, first).GetAwaiter().GetResult();

        /// <summary>
        /// This method allows the user to acknowledge a single alarm. <br/>
        /// This method will always return true, even when nothing is acknowledged.
        /// </summary>
        /// <param name="id">The Acknowledgement ID of the alarm which shall be acknowledged. <br/>
        /// The acknowledgement ID can be found in the alarm object that was returned by method Alarms.Browse.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiTrueOnSuccessResponse</returns>
        public async Task<ApiTrueOnSuccessResponse> AlarmsAcknowledgeAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiAlarmsAcknowledgeRequest(id);
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
        }
        /// <summary>
        /// This method allows the user to acknowledge a single alarm. <br/>
        /// This method will always return true, even when nothing is acknowledged.
        /// </summary>
        /// <param name="id">The Acknowledgement ID of the alarm which shall be acknowledged. <br/>
        /// The acknowledgement ID can be found in the alarm object that was returned by method Alarms.Browse.</param>
        /// <returns>ApiTrueOnSuccessResponse</returns>
        public ApiTrueOnSuccessResponse AlarmsAcknowledge(string id) => AlarmsAcknowledgeAsync(id).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Alarms.Browse request
        /// </summary>
        /// <returns>ApiAlarmsBrowseResponse</returns>
        /// <param name="language">The language in which the texts should be returned. 
        ///                        If the language is valid, then the response must contain the texts in the requested language. <br/>
        ///                        An empty string shall be treated the same as an invalid language string.
        ///                        </param>
        /// <param name="count">(optional) The maximum number of alarm entries to be requested. <br/>
        ///                     When not provided, the plc will return with the default amount: 50. <br/>
        ///                     The maximum possible count is 5000. <br/>
        ///                     A count of 0 must omit any alarm entries from the response and must only return the attributes last_modified, count_max and count_current. 
        ///                     </param>
        /// <param name="alarm_id">(optional) The CPU alarm ID for which the user wants to return the data. If this is provided, no count parameter can be provided as filter.</param>
        /// <param name="filters">(optional) Optional object that contains parameters to filter the response.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        public async Task<ApiAlarmsBrowseResponse> ApiAlarmsBrowseAsync(CultureInfo language, int? count = null, string alarm_id = null, ApiAlarms_RequestFilters filters = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiAlarmsBrowseRequest(language, count, alarm_id, filters);
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiAlarmsBrowseResponse>(response);
        }

        /// <summary>
        /// Send a Alarms.Browse request
        /// </summary>
        /// <returns>ApiAlarmsBrowseResponse</returns>
        /// <param name="language">The language in which the texts should be returned. 
        ///                        If the language is valid, then the response must contain the texts in the requested language. <br/>
        ///                        An empty string shall be treated the same as an invalid language string.
        ///                        </param>
        /// <param name="count">(optional) The maximum number of alarm entries to be requested. <br/>
        ///                     When not provided, the plc will return with the default amount: 50. <br/>
        ///                     The maximum possible count is 5000. <br/>
        ///                     A count of 0 must omit any alarm entries from the response and must only return the attributes last_modified, count_max and count_current. 
        ///                     </param>
        /// <param name="alarm_id">(optional) The CPU alarm ID for which the user wants to return the data. If this is provided, no count parameter can be provided as filter.</param>
        /// <param name="filters">(optional) Optional object that contains parameters to filter the response.</param>
        public ApiAlarmsBrowseResponse ApiAlarmsBrowse(CultureInfo language, int? count = null, string alarm_id = null, ApiAlarms_RequestFilters filters = null) => ApiAlarmsBrowseAsync(language, count, alarm_id, filters).GetAwaiter().GetResult();

        /// <summary>
        /// Send a DiagnosticBuffer.Browse request
        /// </summary>
        /// <param name="language">The language in which the texts should be returned. If the language is valid, then the response must contain the texts in the requested language.An empty string shall be treated the same as an invalid language string.</param>
        /// <param name="count">(optional) The maximum number of diagnostic buffer entries to be requested. Default value: 50. A count of 0 will omit any diagnostic buffer entries from the response</param>
        /// <param name="filters">(optional) ApiDiagnosticBufferBrowse_RequestFilters representing various filtering possibilities.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiDiagnosticBufferBrowseResponse</returns>
        public async Task<ApiDiagnosticBufferBrowseResponse> ApiDiagnosticBufferBrowseAsync(CultureInfo language, uint? count = null, ApiDiagnosticBuffer_RequestFilters filters = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiDiagnosticBufferBrowseRequest(language, count, filters);
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiDiagnosticBufferBrowseResponse>(response);
        }
        /// <summary>
        /// Send a DiagnosticBuffer.Browse request
        /// </summary>
        /// <param name="language">The language in which the texts should be returned. If the language is valid, then the response must contain the texts in the requested language.An empty string shall be treated the same as an invalid language string.</param>
        /// <param name="count">(optional) The maximum number of diagnostic buffer entries to be requested. Default value: 50. A count of 0 will omit any diagnostic buffer entries from the response</param>
        /// <param name="filters">(optional) ApiDiagnosticBufferBrowse_RequestFilters representing various filtering possibilities.</param>
        /// <returns>ApiDiagnosticBufferBrowseResponse</returns>
        public ApiDiagnosticBufferBrowseResponse ApiDiagnosticBufferBrowse(CultureInfo language, uint? count = null, ApiDiagnosticBuffer_RequestFilters filters = null) => ApiDiagnosticBufferBrowseAsync(language, count, filters).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Techology.Read Request 
        /// </summary>
        /// <param name="var">Name of the technology object to be read</param>
        /// <param name="mode">Determines the response format for this method. See the param for more details</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>A generic ApiResultResponse: object with the value for the variables value to be read</returns>
        public async Task<ApiResultResponse<T>> TechnologyReadAsync<T>(string var, ApiPlcDataRepresentation? mode = null, CancellationToken cancellationToken = default)
        {
            var req = _apiRequestFactory.GetTechnologyReadRequest(var, mode);
            string response = await SendPostRequestAsync(req, cancellationToken);
            var responseObj = JsonConvert.DeserializeObject<ApiResultResponse<T>>(response);
            return responseObj;
        }

        /// <summary>
        /// Send a Techology.Read Request 
        /// </summary>
        /// <param name="var">Name of the technology object to be read</param>
        /// <param name="mode">Determines the response format for this method. See the param for more details</param>
        /// <returns>A generic ApiResultResponse: object with the value for the variables value to be read</returns>
        public ApiResultResponse<T> TechnologyRead<T>(string var, ApiPlcDataRepresentation? mode = null) => TechnologyReadAsync<T>(var, mode).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebServer.ReadResponseHeaders request
        /// </summary>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiWebServerReadCustomHttpHeadersResponse</returns>
        public async Task<ApiWebServerReadResponseHeadersResponse> ApiWebServerReadResponseHeadersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebServerReadResponseHeadersRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiWebServerReadResponseHeadersResponse>(response);
        }

        /// <summary>
        /// Send a WebServer.ReadResponseHeaders request
        /// </summary>
        /// <returns>ApiWebServerReadCustomHttpHeadersResponse</returns>
        public ApiWebServerReadResponseHeadersResponse ApiWebServerReadResponseHeaders() => ApiWebServerReadResponseHeadersAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebServer.ChangeResponseHeaders request
        /// </summary>
        /// <param name="header"></param>
        /// <param name="pattern"></param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        public async Task<ApiTrueOnSuccessResponse> ApiWebServerChangeResponseHeadersAsync(string header = null, string pattern = "~**/*", CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebServerChangeResponseHeadersRequest(header, pattern);
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
        }
        /// <summary>
        /// Send a WebServer.ChangeResponseHeaders request
        /// </summary>
        public ApiTrueOnSuccessResponse ApiWebServerChangeResponseHeaders(string header = null, string pattern = "~**/*") => ApiWebServerChangeResponseHeadersAsync(header, pattern).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Redundancy.ReadSyncupProgress request
        /// </summary>
        /// <returns>ApiRedundancyReadSyncupProgressResponse</returns>
        public async Task<ApiRedundancyReadSyncupProgressResponse> ApiRedundancyReadSyncupProgressAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiRedundancyReadSyncupProgressRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            Console.WriteLine(response);
            return JsonConvert.DeserializeObject<ApiRedundancyReadSyncupProgressResponse>(response);
        }
        /// <summary>
        /// Send a Redundancy.ReadSyncupProgress request
        /// </summary>
        /// <returns>ApiRedundancyReadSyncupProgressResponse</returns>
        public ApiRedundancyReadSyncupProgressResponse ApiRedundancyReadSyncupProgress() => ApiRedundancyReadSyncupProgressAsync().GetAwaiter().GetResult();

        /// <summary>
        /// This method returns a complete list of all technology objects that are configured on the PLC.
        /// </summary>
        /// <returns>ApiTechnologyBrowseObjectsResponse</returns>
        public async Task<ApiTechnologyBrowseObjectsResponse> TechnologyBrowseObjectsAsync(CancellationToken cancellationToken = default)
        {
            var req = _apiRequestFactory.GetTechnologyBrowseObjectsRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiTechnologyBrowseObjectsResponse>(response);
        }

        /// <summary>
        /// This method returns a complete list of all technology objects that are configured on the PLC.
        /// </summary>
        /// <returns>ApiTechnologyBrowseObjectsResponse</returns>
        public ApiTechnologyBrowseObjectsResponse TechnologyBrowseObjects() => TechnologyBrowseObjectsAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a Redundancy.ReadSystemInformation request
        /// </summary>
        /// <returns>ApiRedundancyReadSystemInformationResponse</returns>
        public async Task<ApiRedundancyReadSystemInformationResponse> ApiRedundancyReadSystemInformationAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiRedundancyReadSystemInformationRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiRedundancyReadSystemInformationResponse>(response);
        }
        /// <summary>
        /// Send a Redundancy.ReadSystemInformation request
        /// </summary>
        /// <returns>ApiRedundancyReadSystemInformationResponse</returns>
        public ApiRedundancyReadSystemInformationResponse ApiRedundancyReadSystemInformation() => ApiRedundancyReadSystemInformationAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a Redundancy.ReadSystemState request
        /// </summary>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiDiagnosticBufferBrowseResponse</returns>
        public async Task<ApiRedundancyReadSystemStateResponse> ApiRedundancyReadSystemStateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiRedundancyReadSystemStateRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiRedundancyReadSystemStateResponse>(response);
        }
        /// <summary>
        /// Send a Redundancy.ReadSystemState request
        /// </summary>
        /// <returns>ApiDiagnosticBufferBrowseResponse</returns>
        public ApiRedundancyReadSystemStateResponse ApiRedundancyReadSystemState() => ApiRedundancyReadSystemStateAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a Redundancy.RequestChangeSystemState request
        /// </summary>
        /// <param name="state">The requested system state for the R/H system.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiTrueOnSuccessResponse</returns>
        public async Task<ApiTrueOnSuccessResponse> ApiRedundancyRequestChangeSystemStateAsync(ApiPlcRedundancySystemState state, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiRedundancyRequestChangeSystemStateRequest(state);
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
        }

        /// <summary>
        /// Send a Redundancy.RequestChangeSystemState request
        /// </summary>
        /// <param name="state">The requested system state for the R/H system.</param>
        /// <returns>ApiTrueOnSuccessResponse</returns>
        public ApiTrueOnSuccessResponse ApiRedundancyRequestChangeSystemState(ApiPlcRedundancySystemState state) => ApiRedundancyRequestChangeSystemStateAsync(state).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetVersion request
        /// </summary>
        /// <param name="webAppName">The application in which the resource is located.</param>
        /// <param name="version">The version of the application. The string may be empty to reset the version string.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns></returns>
        public async Task<ApiTrueOnSuccessResponse> ApiWebAppSetVersionAsync(string webAppName, string version, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppSetVersionRequest(webAppName, version);
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
        }

        /// <summary>
        /// Send a WebApp.SetVersion request
        /// </summary>
        /// <param name="webAppName">The application in which the resource is located.</param>
        /// <param name="version">The version of the application. The string may be empty to reset the version string.</param>
        /// <returns></returns>
        public ApiTrueOnSuccessResponse ApiWebAppSetVersion(string webAppName, string version) => ApiWebAppSetVersionAsync(webAppName, version).GetAwaiter().GetResult();


        /// <summary>
        /// Send a WebApp.SetUrlRedirectMode request
        /// </summary>
        /// <param name="webAppName">The application for which the redirect mode shall be changed.</param>
        /// <param name="redirect_mode">The redirect mode of the application. </param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns></returns>
        public async Task<ApiTrueOnSuccessResponse> ApiWebAppSetUrlRedirectModeAsync(string webAppName, ApiWebAppRedirectMode redirect_mode, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiWebAppSetUrlRedirectModeRequest(webAppName, redirect_mode);
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
        }
        /// <summary>
        /// Send a WebApp.SetUrlRedirectMode request
        /// </summary>
        /// <param name="webAppName">The application for which the redirect mode shall be changed.</param>
        /// <param name="redirect_mode">The redirect mode of the application. </param>
        /// <returns></returns>
        public ApiTrueOnSuccessResponse ApiWebAppSetUrlRedirectMode(string webAppName, ApiWebAppRedirectMode redirect_mode) => ApiWebAppSetUrlRedirectModeAsync(webAppName, redirect_mode).GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.ReadCpuType request
        /// </summary>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns></returns>
        public async Task<ApiPlcReadCpuTypeResponse> ApiGetPlcCpuTypeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcReadCpuTypeRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiPlcReadCpuTypeResponse>(response);
        }
        /// <summary>
        /// Send a Plc.ReadCpuType request
        /// </summary>
        /// <returns></returns>
        public ApiPlcReadCpuTypeResponse ApiGetPlcCpuType() => ApiGetPlcCpuTypeAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.ReadStationName request
        /// </summary>
        /// <returns></returns>
        public async Task<ApiPlcReadStationNameResponse> ApiGetPlcStationNameAsync(CancellationToken cancellationToken = default)
        {
            var req = _apiRequestFactory.GetApiPlcReadStationNameRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiPlcReadStationNameResponse>(response);
        }

        /// <summary>
        /// Send a Plc.ReadStationName request
        /// </summary>
        /// <returns></returns>
        public ApiPlcReadStationNameResponse ApiGetPlcStationName() => ApiGetPlcStationNameAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.ReadModuleName request
        /// </summary>
        /// <param name="redundancy_id">
        /// The Redundancy ID parameter must be present when the request is executed on an R/H PLC. It must either have a value of 1 or 2. <br/> 
        /// On non-R/H PLCs, the parameter must not be part of the request.</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns></returns>
        public async Task<ApiPlcReadModuleNameResponse> ApiGetPlcModuleNameAsync(uint? redundancy_id = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiPlcReadModuleNameRequest(redundancy_id);
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiPlcReadModuleNameResponse>(response);
        }

        /// <summary>
        /// Send a Plc.ReadModuleName request
        /// </summary>
        /// <param name="redundancy_id">
        /// The Redundancy ID parameter must be present when the request is executed on an R/H PLC. It must either have a value of 1 or 2. <br/> 
        /// On non-R/H PLCs, the parameter must not be part of the request.</param>
        /// <returns></returns>
        public ApiPlcReadModuleNameResponse ApiGetPlcModuleName(uint? redundancy_id = null) => ApiGetPlcModuleNameAsync(redundancy_id).GetAwaiter().GetResult();

        /// <summary>
        /// Send a GetSessionInfo request
        /// </summary>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns></returns>
        public async Task<ApiSessionInfoResponse> ApiGetSessionInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = _apiRequestFactory.GetApiGetSessionInfoRequest();
            string response = await SendPostRequestAsync(req, cancellationToken);
            return JsonConvert.DeserializeObject<ApiSessionInfoResponse>(response);
        }

        /// <summary>
        /// Send a GetSessionInfo request
        /// </summary>
        /// <returns></returns>
        public ApiSessionInfoResponse ApiGetSessionInfo() => ApiGetSessionInfoAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Cancel the outstanding requests of the HttpClient
        /// </summary>
        public void CancelPendingRequests()
        {
            _httpClient.CancelPendingRequests();
        }
    }
}
