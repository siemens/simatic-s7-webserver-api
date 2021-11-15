// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.ApiPlcProgramDataTypes;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Request Handlerusing the Microsoft.Net.HttpClient to send the requests to the API
    /// </summary>
    public class ApiHttpClientRequestHandler : IApiRequestHandler
    {
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

        private readonly HttpClient HttpClient;

        private readonly IApiRequestFactory ApiRequestFactory;

        private readonly IApiResponseChecker ApiResponseChecker;

        /// <summary>
        /// The ApiHttpClientRequestHandler will Send Post Requests,
        /// before sending the Request it'll remove those Parameters that have the value null for their keys 
        /// (keep in mind when using - when not using the ApiRequestFactory)
        /// </summary>
        /// <param name="httpClient">authorized httpClient with set Header: 'X-Auth-Token'</param>
        /// <param name="apiRequestFactory"></param>
        public ApiHttpClientRequestHandler(HttpClient httpClient, IApiRequestFactory apiRequestFactory, IApiResponseChecker apiResponseChecker)
        {
            this.HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.ApiRequestFactory = apiRequestFactory ?? throw new ArgumentNullException(nameof(apiRequestFactory));
            this.ApiResponseChecker = apiResponseChecker ?? throw new ArgumentNullException(nameof(apiResponseChecker));
        }

        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <param name="apiRequest">Api Request to send to the plc (Json Serialized - null properties are deleted)</param>
        /// <returns>string: response from thePLC</returns>
        public async Task<string> SendPostRequestAsync(IApiRequest apiRequest)
        {
            if(apiRequest.Params != null)
            {
                apiRequest.Params = apiRequest.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string apiRequestString = JsonConvert.SerializeObject(apiRequest, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.GetBytes(apiRequestString);
            ByteArrayContent request_body = new ByteArrayContent(byteArr);
            request_body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
            var response = await HttpClient.PostAsync(JsonRpcApi, request_body);
            ApiResponseChecker.CheckHttpResponseForErrors(response, apiRequestString);
            var responseString = await response.Content.ReadAsStringAsync();
            ApiResponseChecker.CheckResponseStringForErros(responseString, apiRequestString);
            return responseString;
        }

        /// <summary>
        /// Send an Api.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>An Array of ApiClass (and Id,Jsonrpc)</returns>
        public async Task<ApiArrayOfApiClassResponse> ApiBrowseAsync()
        {
            var req = ApiRequestFactory.GetApiBrowseRequest();
            var responseString = await SendPostRequestAsync(req);
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
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        public async Task<ApiBrowseTicketsResponse> ApiBrowseTicketsAsync(string ticketId = null)
        {
            var req = ApiRequestFactory.GetApiBrowseTicketsRequest(ticketId);
            var responseString = await SendPostRequestAsync(req);
            var arrOfApiClassResponse = JsonConvert.DeserializeObject<ApiBrowseTicketsResponse>(responseString);
            return arrOfApiClassResponse;
        }
        /// <summary>
        /// Send an Api.BrowseTickets Request using the Request from the ApiRequestFactory
        /// </summary>

        public ApiBrowseTicketsResponse ApiBrowseTickets(string ticketId) => ApiBrowseTicketsAsync(ticketId).GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.BrowseTickets Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        public async Task<ApiBrowseTicketsResponse> ApiBrowseTicketsAsync(ApiTicket ticket)
        {
            return await ApiBrowseTicketsAsync(ticket.Id);
        }
        /// <summary>
        /// Send an Api.BrowseTickets Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        public ApiBrowseTicketsResponse ApiBrowseTickets(ApiTicket ticket) => ApiBrowseTicketsAsync(ticket).GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.CloseTicket Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="ticketId">ticket id (28 chars)</param>
        /// <returns>True to indicate Success</returns>
        public async Task<ApiTrueOnSuccessResponse> ApiCloseTicketAsync(string ticketId)
        {
            var req = ApiRequestFactory.GetApiCloseTicketRequest(ticketId);
            var responseString = await SendPostRequestAsync(req);
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
        /// <returns>True to indicate Success</returns>
        public async Task<ApiTrueOnSuccessResponse> ApiCloseTicketAsync(ApiTicket ticket)
        {
            return await ApiCloseTicketAsync(ticket.Id);
        }
        /// <summary>
        /// Send an Api.CloseTicket Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="ticket">ticket containing ticket id (28 chars)</param>
        /// <returns>True to indicate Success</returns>
        public ApiTrueOnSuccessResponse ApiCloseTicket(ApiTicket ticket) => ApiCloseTicketAsync(ticket).GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.GetCertificateUrl Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>ApiSingleStringResponse that contians the URL to the certificate</returns>
        public async Task<ApiSingleStringResponse> ApiGetCertificateUrlAsync()
        {
            var req = ApiRequestFactory.GetApiGetCertificateUrlRequest();
            string response = await SendPostRequestAsync(req);
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
        /// <returns>Array of ApiClass (in this case permissions)</returns>
        public async Task<ApiArrayOfApiClassResponse> ApiGetPermissionsAsync()
        {
            var req = ApiRequestFactory.GetApiGetPermissionsRequest();
            string response = await SendPostRequestAsync(req);
            return JsonConvert.DeserializeObject<ApiArrayOfApiClassResponse>(response);
        }
        /// <summary>
        /// Send an Api.GetPermissions Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>Array of ApiClass (in this case permissions)</returns>
        public ApiArrayOfApiClassResponse ApiGetPermissions() => ApiGetPermissionsAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api.Logout Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>True to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> ApiLogoutAsync()
        {
            var req = ApiRequestFactory.GetApiLogoutRequest();
            string response = await SendPostRequestAsync(req);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
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
        public async Task<ApiSingleStringResponse> ApiPingAsync()
        {
            var req = ApiRequestFactory.GetApiPingRequest();
            string response = await SendPostRequestAsync(req);
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
        public async Task<ApiDoubleResponse> ApiVersionAsync()
        {
            var req = ApiRequestFactory.GetApiVersionRequest(); 
            string response = await SendPostRequestAsync(req);
            var responseObj = JsonConvert.DeserializeObject<ApiDoubleResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send an Api.Version Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>a double that contains the value for the current ApiVersion</returns>
        public ApiDoubleResponse ApiVersion() => ApiVersionAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.ReadOperatingMode Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>The current Plc OperatingMode</returns>
        public async Task<ApiReadOperatingModeResponse> PlcReadOperatingModeAsync()
        {
            var req = ApiRequestFactory.GetApiPlcReadOperatingModeRequest();
            string response = await SendPostRequestAsync(req);
            var responseObj = JsonConvert.DeserializeObject<ApiReadOperatingModeResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a Plc.ReadOperatingMode Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <returns>The current Plc OperatingMode</returns>
        public ApiReadOperatingModeResponse PlcReadOperatingMode() => PlcReadOperatingModeAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Send a Plc.RequestChangeOperatingMode Request using the Request from the ApiRequestFactory
        /// Method to change the plc operating mode
        /// valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.
        /// </summary>
        /// <returns>valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.</returns>
        public async Task<ApiTrueOnSuccessResponse> PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode plcOperatingMode)
        {
            var req = ApiRequestFactory.GetApiPlcRequestChangeOperatingModeRequest(plcOperatingMode);
            string response = await SendPostRequestAsync(req);
            var responseObj = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a Plc.RequestChangeOperatingMode Request using the Request from the ApiRequestFactory
        /// Method to change the plc operating mode
        /// valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.
        /// </summary>
        /// <returns>valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.</returns>
        public ApiTrueOnSuccessResponse PlcRequestChangeOperatingMode(ApiPlcOperatingMode plcOperatingMode)
            => PlcRequestChangeOperatingModeAsync(plcOperatingMode).GetAwaiter().GetResult();

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
        public async Task<ApiPlcProgramBrowseResponse> PlcProgramBrowseAsync(ApiPlcProgramBrowseMode plcProgramBrowseMode, string var = null)
        {
            var req = ApiRequestFactory.GetApiPlcProgramBrowseRequest(plcProgramBrowseMode, var);
            string response = await SendPostRequestAsync(req);
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
        /// <param name="plcProgramBrowseMode"></param>
        /// <returns>PlcProgramBrowseResponse: An Array of ApiPlcProgramData</returns>
        public async Task<ApiPlcProgramBrowseResponse> PlcProgramBrowseAsync(ApiPlcProgramBrowseMode plcProgramBrowseMode, ApiPlcProgramData var)
        {
            string varName = var.GetVarNameForMethods();
            return await PlcProgramBrowseAsync(plcProgramBrowseMode, varName);
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
        public async Task<ApiResultResponse<T>> PlcProgramReadAsync<T>(string var, ApiPlcProgramReadOrWriteMode? plcProgramReadMode = null)
        {
            var req = ApiRequestFactory.GetApiPlcProgramReadRequest(var, plcProgramReadMode);
            string response = await SendPostRequestAsync(req);
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
        public ApiResultResponse<T> PlcProgramRead<T>(ApiPlcProgramData var, ApiPlcProgramReadOrWriteMode? plcProgramReadMode = null) => PlcProgramReadAsync<T>(var, plcProgramReadMode).GetAwaiter().GetResult();

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
        public async Task<ApiResultResponse<T>> PlcProgramReadAsync<T>(ApiPlcProgramData var, ApiPlcProgramReadOrWriteMode? plcProgramReadMode = null)
        {
            //RequestParameterChecker.CheckPlcProgramReadOrWriteDataType(var.Datatype, true);
            string varName = var.GetVarNameForMethods();
            return await PlcProgramReadAsync<T>(varName, plcProgramReadMode);
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
        public ApiResultResponse<T> PlcProgramRead<T>(string var, ApiPlcProgramReadOrWriteMode? plcProgramReadMode = null) => PlcProgramReadAsync<T>(var, plcProgramReadMode).GetAwaiter().GetResult();

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
        public async Task<ApiTrueOnSuccessResponse> PlcProgramWriteAsync(ApiPlcProgramData var, object valueToBeSet, ApiPlcProgramReadOrWriteMode? plcProgramWriteMode = null)
        {
            string varName = var.GetVarNameForMethods();
            // ApiRequestFactory.CheckPlcProgramReadOrWriteDataType(var.Datatype); will also be called by GetApiPlcProgramWriteValueToBeSet!
            var writeVal = ApiRequestFactory.GetApiPlcProgramWriteValueToBeSet(var.Datatype, valueToBeSet);
            return await PlcProgramWriteAsync(varName, writeVal, plcProgramWriteMode);
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
        public ApiTrueOnSuccessResponse PlcProgramWrite(ApiPlcProgramData var, object valueToBeSet, ApiPlcProgramReadOrWriteMode? plcProgramWriteMode = null)
            => PlcProgramWriteAsync(var, valueToBeSet, plcProgramWriteMode).GetAwaiter().GetResult();

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
        public async Task<ApiTrueOnSuccessResponse> PlcProgramWriteAsync(string var, object valueToBeSet, ApiPlcProgramReadOrWriteMode? plcProgramWriteMode = null)
        {
            var req = ApiRequestFactory.GetApiPlcProgramWriteRequest(var, valueToBeSet, plcProgramWriteMode); 
            string response = await SendPostRequestAsync(req);
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
        public ApiTrueOnSuccessResponse PlcProgramWrite(string var, object valueToBeSet, ApiPlcProgramReadOrWriteMode? plcProgramWriteMode = null)
            => PlcProgramWriteAsync(var, valueToBeSet, plcProgramWriteMode).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.Browse Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppData">webappdata that should be requested</param>
        /// <returns>ApiWebAppBrowseResponse: Containing WebAppBrowseResult: Max_Applications:uint, 
        /// Applications: Array of ApiWebAppdata containing one element: the webappdata that has been requested</returns>
        public async Task<ApiWebAppBrowseResponse> WebAppBrowseAsync(ApiWebAppData webAppData)
        {
            return await WebAppBrowseAsync(webAppData.Name);
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
        /// <returns>ApiWebAppBrowseResponse: Containing WebAppBrowseResult: Max_Applications:uint, 
        /// Applications: Array of ApiWebAppdata containing one element: the webappdata that has been requested</returns>
        public async Task<ApiWebAppBrowseResponse> WebAppBrowseAsync(string webAppName = null)
        {
            var req = ApiRequestFactory.GetApiWebAppBrowseRequest(webAppName);
            string response = await SendPostRequestAsync(req);
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
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public async Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(string webAppName, string resourceName = null)
        {
            var req = ApiRequestFactory.GetApiWebAppBrowseResourcesRequest(webAppName, resourceName);
            string response = await SendPostRequestAsync(req);
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
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public async Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(ApiWebAppData webApp, string resourceName = null)
        {
            return await WebAppBrowseResourcesAsync(webApp.Name, resourceName);
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
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public async Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(string webAppName, ApiWebAppResource resource)
        {
            return await WebAppBrowseResourcesAsync(webAppName, resource.Name);
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
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,
        /// Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        public async Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(ApiWebAppData webApp, ApiWebAppResource resource)
        {
            return await WebAppBrowseResourcesAsync(webApp.Name, resource.Name);
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
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppCreateAsync(string webAppName, ApiWebAppState? apiWebAppState = null)
        {
            var req = ApiRequestFactory.GetApiWebAppCreateRequest(webAppName, apiWebAppState); 
            string response = await SendPostRequestAsync(req);
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
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppCreateAsync(ApiWebAppData webApp)
        {
            // ApiRequestFactory.CheckState(webApp.State); will be called in WebAppCreate in Factory.GetApiWebAppCreateRequest
            return await WebAppCreateAsync(webApp.Name, webApp.State);
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
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public async Task<ApiTicketIdResponse> WebAppCreateResourceAsync(string webAppName, string resourceName, string media_type,
            string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null)
        {
            var req = ApiRequestFactory.GetApiWebAppCreateResourceRequest(webAppName, resourceName, media_type,
                last_modified, apiWebAppResourceVisibility, etag); 
            string response = await SendPostRequestAsync(req);
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
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public async Task<ApiTicketIdResponse> WebAppCreateResourceAsync(ApiWebAppData webApp, string resourceName, string media_type, 
            string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null)
        {
            return await WebAppCreateResourceAsync(webApp.Name, resourceName, media_type, last_modified, apiWebAppResourceVisibility, etag);
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
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public async Task<ApiTicketIdResponse> WebAppCreateResourceAsync(string webAppName, ApiWebAppResource resource)
        {
            return await WebAppCreateResourceAsync(webAppName, resource.Name, resource.Media_type, 
                resource.Last_modified.ToString(DateTimeFormatting.ApiDateTimeFormat), resource.Visibility, resource.Etag);
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
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        public async Task<ApiTicketIdResponse> WebAppCreateResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource)
        {
            return await WebAppCreateResourceAsync(webApp.Name, resource.Name, resource.Media_type, 
                resource.Last_modified.ToString(DateTimeFormatting.ApiDateTimeFormat), resource.Visibility, resource.Etag);
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
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteAsync(string webAppName)
        {
            var req = ApiRequestFactory.GetApiWebAppDeleteRequest(webAppName);
            string response = await SendPostRequestAsync(req);
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
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteAsync(ApiWebAppData webApp)
        {
            return await WebAppDeleteAsync(webApp.Name);
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
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(string webAppName, string resourceName)
        {
            var req = ApiRequestFactory.GetApiWebAppDeleteResourceRequest(webAppName, resourceName);
            string response = await SendPostRequestAsync(req);
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
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(ApiWebAppData webApp, string resourceName)
        {
            return await WebAppDeleteResourceAsync(webApp.Name, resourceName);
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
        /// <param name="webApp">webapp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        public ApiTrueOnSuccessResponse WebAppDeleteResource(string webAppName, ApiWebAppResource resource) => WebAppDeleteResourceAsync(webAppName, resource).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.DeleteRespource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource)
        {
            return await WebAppDeleteResourceAsync(webApp.Name, resource.Name);
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
        /// <returns>true to indicate success</returns>
        public async Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(string webAppName, ApiWebAppResource resource)
        {
            return await WebAppDeleteResourceAsync(webAppName, resource.Name);
        }

        /// <summary>
        /// Send a WebApp.DownloadResource Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public async Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(string webAppName, string resourceName)
        {
            var req = ApiRequestFactory.GetApiWebAppDownloadResourceRequest(webAppName, resourceName);
            string response = await SendPostRequestAsync(req);
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
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public async Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(ApiWebAppData webApp, string resourceName)
        {
            return await WebAppDownloadResourceAsync(webApp.Name, resourceName);
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
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public async Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource)
        {
            return await WebAppDownloadResourceAsync(webApp.Name, resource.Name);
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
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        public async Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(string webAppName, ApiWebAppResource resource)
        {
            return await WebAppDownloadResourceAsync(webAppName, resource.Name);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a WebApp that only has the information: 
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppRenameAsync(string webAppName, string newWebAppName)
        {
            var req = ApiRequestFactory.GetApiWebAppRenameRequest(webAppName, newWebAppName);
            string response = await SendPostRequestAsync(req);
            var responseObj = new ApiTrueWithWebAppResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if(responseObj.TrueOnSuccesResponse.Result)
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the given WebApp that has the change:
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppRenameAsync(ApiWebAppData webApp, string newWebAppName)
        {
            var resp = await WebAppRenameAsync(webApp.Name, newWebAppName);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a Resource that only has the information: 
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(string webAppName, string resourceName, 
            string newResourceName)
        {
            var req = ApiRequestFactory.GetApiWebAppRenameResourceRequest(webAppName, resourceName, newResourceName);
            string response = await SendPostRequestAsync(req);
            ApiTrueWithResourceResponse responseObj = new ApiTrueWithResourceResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if(responseObj.TrueOnSuccesResponse.Result)
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
        /// <returns>This function will return the TrueOnSuccessResponse and a Resource that only has the information: 
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(ApiWebAppData webApp, string resourceName, 
            string newResourceName)
        {
            return await WebAppRenameResourceAsync(webApp.Name, resourceName, newResourceName);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the Resource given that has the following change: 
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource, 
            string newResourceName)
        {
            var basicResp = await WebAppRenameResourceAsync(webApp.Name, resource.Name, newResourceName);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the Resource given that has the following change: 
        /// name which equals the newname</returns>
        public async Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(string webAppName, ApiWebAppResource resource, 
            string newResourceName)
        {
            var basicResp = await WebAppRenameResourceAsync(webAppName, resource.Name, newResourceName);
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
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Default_Page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(string webAppName, string resourceName)
        {
            var req = ApiRequestFactory.GetApiWebAppSetDefaultPageRequest(webAppName, resourceName ?? "");
            string response = await SendPostRequestAsync(req);
            var responseObj = new ApiTrueWithWebAppResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if(responseObj.TrueOnSuccesResponse.Result)
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Default_Page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(ApiWebAppData webApp, string resourceName)
        {
            var resp = await WebAppSetDefaultPageAsync(webApp.Name, resourceName);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Default_Page:   which equals the resource.Name
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(ApiWebAppData webApp, ApiWebAppResource resource)
        {
            var resp = await WebAppSetDefaultPageAsync(webApp.Name, resource.Name);
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
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Default_Page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(string webAppName, ApiWebAppResource resource)
        {
            return await WebAppSetDefaultPageAsync(webAppName, resource.Name);
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
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:                   which equals webAppName
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(string webAppName, string resourceName)
        {
            var req = ApiRequestFactory.GetApiWebAppSetNotAuthorizedPageRequest(webAppName, resourceName ?? "");
            string response = await SendPostRequestAsync(req);
            var responseObj = new ApiTrueWithWebAppResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if (responseObj.TrueOnSuccesResponse.Result)
            {
                responseObj.NewWebApp = new ApiWebAppData() { Name = webAppName, Not_authorized_page = (resourceName == ""?null:resourceName) };
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(ApiWebAppData webApp, string resourceName)
        {
            var resp = await WebAppSetNotAuthorizedPageAsync(webApp.Name, resourceName);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(ApiWebAppData webApp, ApiWebAppResource resource)
        {
            var resp = await WebAppSetNotAuthorizedPageAsync(webApp.Name, resource.Name);
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
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:                   which equals webAppName
        /// Not_authorized_page:    which equals the resource.Name
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(string webAppName, ApiWebAppResource resource)
        {
            return await WebAppSetNotAuthorizedPageAsync(webAppName, resource.Name);
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
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Not_found_page: which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(string webAppName, string resourceName)
        {
            var req = ApiRequestFactory.GetApiWebAppSetNotFoundPageRequest(webAppName, resourceName??"");
            string response = await SendPostRequestAsync(req);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_found_page:   which equals the resourceName
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(ApiWebAppData webApp, string resourceName)
        {
            var resp = await WebAppSetNotFoundPageAsync(webApp.Name, resourceName);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_found_page:   which equals the resource.Name
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(ApiWebAppData webApp, ApiWebAppResource resource)
        {
            var resp = await WebAppSetNotFoundPageAsync(webApp.Name, resource.Name);
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
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Not_found_page: which equals the resource.Name
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(string webAppName, ApiWebAppResource resource)
        {
            return await WebAppSetNotFoundPageAsync(webAppName, resource.Name);
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
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:  which equals the webAppName
        /// State: which equals the state
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetStateAsync(string webAppName, ApiWebAppState apiWebAppState)
        {
            var req = ApiRequestFactory.GetApiWebAppSetStateRequest(webAppName, apiWebAppState);
            string response = await SendPostRequestAsync(req);
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
        /// <param name="apiWebAppState">State the WebApp should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:  which equals the webAppName
        /// State: which equals the state
        /// </returns>
        public ApiTrueWithWebAppResponse WebAppSetState(string webAppName, ApiWebAppState newApiWebAppState)
            => WebAppSetStateAsync(webAppName, newApiWebAppState).GetAwaiter().GetResult();

        /// <summary>
        /// Send a WebApp.SetState Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that state should be set for</param>
        /// <param name="apiWebAppState">State the WebApp should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// State: which equals the state
        /// </returns>
        public async Task<ApiTrueWithWebAppResponse> WebAppSetStateAsync(ApiWebAppData webApp, ApiWebAppState apiWebAppState)
        {
            var resp = await WebAppSetStateAsync(webApp.Name, apiWebAppState);
            resp.NewWebApp = webApp.ShallowCopy();
            resp.NewWebApp.State = apiWebAppState;
            return resp;
        }
        /// <summary>
        /// Send a WebApp.SetState Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that state should be set for</param>
        /// <param name="apiWebAppState">State the WebApp should have</param>
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
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Etag: which equals the newEtagValue
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(string webAppName, string resourceName, 
            string newETagValue)
        {
            var req = ApiRequestFactory.GetApiSetResourceETagRequest(webAppName, resourceName, newETagValue ?? "");
            string response = await SendPostRequestAsync(req);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Etag: which equals the newEtagValue
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(ApiWebAppData webApp, string resourceName, 
            string newETagValue)
        {
            return await WebAppSetResourceETagAsync(webApp.Name, resourceName, newETagValue);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Etag: which equals the newEtagValue
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(ApiWebAppData webApp, ApiWebAppResource resource, 
            string newETagValue)
        {
            var basicResp = await WebAppSetResourceETagAsync(webApp.Name, resource.Name, newETagValue);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Etag: which equals the newEtagValue
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(string webAppName, ApiWebAppResource resource, 
            string newETagValue)
        {
            var basicResp = await WebAppSetResourceETagAsync(webAppName, resource.Name, newETagValue);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:       which equals the resourceName
        /// MediaType:  which equals the newMediaType
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(string webAppName, string resourceName, 
            string newMediaType)
        {
            var req = ApiRequestFactory.GetApiSetResourceMediaTypeRequest(webAppName, resourceName, newMediaType);
            string response = await SendPostRequestAsync(req);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:       which equals the resourceName
        /// MediaType:  which equals the newMediaType
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(ApiWebAppData webApp, string resourceName, 
            string newMediaType)
        {
            return await WebAppSetResourceMediaTypeAsync(webApp.Name, resourceName, newMediaType);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// MediaType: which equals the newMediaType
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(ApiWebAppData webApp, ApiWebAppResource resource, 
            string newMediaType)
        {
            var basicResp = await WebAppSetResourceMediaTypeAsync(webApp.Name, resource.Name, newMediaType);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// MediaType: which equals the newMediaType
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(string webAppName, ApiWebAppResource resource, 
            string newMediaType)
        {
            var basicResp = await WebAppSetResourceMediaTypeAsync(webAppName, resource.Name, newMediaType);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, string resourceName, 
            string newModificationTime)
        {
            var req = ApiRequestFactory.GetApiSetResourceModificationTimeRequest(webAppName, resourceName, newModificationTime);
            string response = await SendPostRequestAsync(req);
            var responseObj = new ApiTrueWithResourceResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            responseObj.NewResource = new ApiWebAppResource()
            { Name = resourceName, Last_modified = XmlConvert.ToDateTime(newModificationTime, XmlDateTimeSerializationMode.Utc)};
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
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, string resourceName, 
            string newModificationTime)
        {
            return await WebAppSetResourceModificationTimeAsync(webApp.Name, resourceName, newModificationTime);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, ApiWebAppResource resource, 
            string newModificationTime)
        {
            var basicResp = await WebAppSetResourceModificationTimeAsync(webApp.Name, resource.Name, newModificationTime);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, string resourceName, 
            DateTime newModificationTime)
        {
            return await WebAppSetResourceModificationTimeAsync(webAppName, resourceName, 
                newModificationTime.ToString(DateTimeFormatting.ApiDateTimeFormat));
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
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, string resourceName,
            DateTime newModificationTime)
        {
            return await WebAppSetResourceModificationTimeAsync(webApp.Name, resourceName, 
                newModificationTime.ToString(DateTimeFormatting.ApiDateTimeFormat));
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, ApiWebAppResource resource, 
            DateTime newModificationTime)
        {
            var basicResp = await WebAppSetResourceModificationTimeAsync(webApp.Name, resource.Name, 
                newModificationTime.ToString(DateTimeFormatting.ApiDateTimeFormat));
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, ApiWebAppResource resource, 
            DateTime newModificationTime)
        {
            var basicResp = await WebAppSetResourceModificationTimeAsync(webAppName, resource.Name, 
                newModificationTime.ToString(DateTimeFormatting.ApiDateTimeFormat));
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, ApiWebAppResource resource, 
            string newModificationTime)
        {
            var basicResp = await WebAppSetResourceModificationTimeAsync(webAppName, resource.Name, newModificationTime);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Visibility: which equals the newVisibility
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(string webAppName, string resourceName, 
            ApiWebAppResourceVisibility newResourceVisibility)
        {
            var req = ApiRequestFactory.GetApiSetResourceVisibilityRequest(webAppName, resourceName, newResourceVisibility);
            string response = await SendPostRequestAsync(req);
            var responseObj = new ApiTrueWithResourceResponse();
            responseObj.TrueOnSuccesResponse = JsonConvert.DeserializeObject<ApiTrueOnSuccessResponse>(response);
            if (responseObj.TrueOnSuccesResponse.Result)
            {
                responseObj.NewResource = new ApiWebAppResource() { Name = resourceName, Visibility = newResourceVisibility};
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
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(ApiWebAppData webApp, string resourceName, 
            ApiWebAppResourceVisibility newResourceVisibility)
        {
            return await WebAppSetResourceVisibilityAsync(webApp.Name, resourceName, newResourceVisibility);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(ApiWebAppData webApp, ApiWebAppResource resource, 
            ApiWebAppResourceVisibility newResourceVisibility)
        {
            var basicResp = await WebAppSetResourceVisibilityAsync(webApp.Name, resource.Name, newResourceVisibility);
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
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        public async Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(string webAppName, ApiWebAppResource resource, 
            ApiWebAppResourceVisibility newResourceVisibility)
        {
            var basicResp = await WebAppSetResourceVisibilityAsync(webAppName, resource.Name, newResourceVisibility);
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
        /// Function to get the ByteArray Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <returns>Bytearray given from the PLC</returns>
        public async Task<byte[]> DownloadTicketAsync(string ticketId)
        {
            var request_body = new ByteArrayContent(new byte[0]);
            request_body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            var response = await HttpClient.PostAsync($"/api/ticket?id={ticketId}", request_body);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
        /// <summary>
        /// Function to get the ByteArray Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <returns>Bytearray given from the PLC</returns>
        public byte[] DownloadTicket(string ticketId) => DownloadTicketAsync(ticketId).GetAwaiter().GetResult();

        /// <summary>
        /// Function to send the ByteArrayContent for a Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="data">ByteArray that should be sent to the plc Ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        public async Task UploadTicketAsync(string ticketId, ByteArrayContent data)
        {
            data.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            try
            {
                var response = await HttpClient.PostAsync($"/api/ticket?id={ticketId}", data);
                response.EnsureSuccessStatusCode();
            }
            catch(Exception e)
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
        /// Function to Read and send the ByteArrayContent for a file with the Ticketing Endpoint Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="pathToFile">File Bytes will be Read and saved into ByteArrayContent - then sent to the ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        public async Task UploadTicketAsync(string ticketId, string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException($"file at: {pathToFile} not found!");
            }
            var fileContent = new ByteArrayContent(File.ReadAllBytes(pathToFile));
            await UploadTicketAsync(ticketId, fileContent);
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
        /// Send a Api.Login Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <returns>ApiLoginResponse: contains ApiTokenResult: Token(auth token string) and if requested Web_application_cookie</returns>
        public async Task<ApiLoginResponse> ApiLoginAsync(string userName, string password, bool? includeWebApplicationCookie = null)
        {
            var req = ApiRequestFactory.GetApiLoginRequest(userName, password, includeWebApplicationCookie);
            string response = await SendPostRequestAsync(req);
            var responseObj = new ApiLoginResponse();
            responseObj = JsonConvert.DeserializeObject<ApiLoginResponse>(response);
            return responseObj;
        }
        /// <summary>
        /// Send a Api.Login Request using the Request from the ApiRequestFactory
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <returns>ApiLoginResponse: contains ApiTokenResult: Token(auth token string) and if requested Web_application_cookie</returns>
        public ApiLoginResponse ApiLogin(string userName, string password, bool? includeWebApplicationCookie = null) => ApiLoginAsync(userName, password, includeWebApplicationCookie).GetAwaiter().GetResult();

        /// <summary>
        /// Send an Api Bulk Request
        /// </summary>
        /// <param name="apiRequests">Api Requests to be sent as Bulk</param>
        /// <returns>List of ApiResultResponses with Result as object - not "directly" casted to the expected Result type</returns>
        public async Task<ApiBulkResponse> ApiBulkAsync(IEnumerable<IApiRequest> apiRequests)
        {
            if((apiRequests.GroupBy(el => el.Id).Count() != apiRequests.Count()))
            {
                throw new ArgumentException($"{nameof(apiRequests)} contains multiple requests with the same Id!");
            }
            foreach(var apiRequest in apiRequests)
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
            ByteArrayContent request_body = new ByteArrayContent(byteArr);
            request_body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
            var response = await HttpClient.PostAsync(JsonRpcApi, request_body);
            ApiResponseChecker.CheckHttpResponseForErrors(response, apiRequestString);
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
            return bulkResponse;
        }
        /// <summary>
        /// Send an Api Bulk Request
        /// </summary>
        /// <param name="apiRequests">Api Requests to be sent as Bulk</param>
        /// <returns>List of ApiResultResponses with Result as object - not "directly" casted to the expected Result type</returns>
        public ApiBulkResponse ApiBulk(IEnumerable<IApiRequest> apiRequests)
        {
            throw new NotImplementedException();
        }
    }
}
