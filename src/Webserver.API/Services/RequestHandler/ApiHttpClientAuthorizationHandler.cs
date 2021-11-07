// Copyright (c) 2021, Siemens AG
// 
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Requests;
using Siemens.Simatic.S7.Webserver.API.Responses;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandler
{
    /// <summary>
    /// Static class to enable a user to get an Authorized HTTPClient (or also an authorized HttpClient with a WebApplicationCookie)
    /// </summary>
    public static class ApiHttpClientAuthorizationHandler
    { 
        /// <summary>
        /// use this static function to get an HTTPClient that is authenticated to the API of the baseAddress given (DNS/IP) 
        /// Will send an ApiLogin Request to the given address' plcs Api (/api/jsonrpc) and set the token if the login was successful to the headers 
        /// of the HttpClient => Will be used to Authenticate for all further API Mehtod calls
        /// HINT: Will set the ConnectionClose to false
        /// </summary>
        /// <param name="ClientConfiguration">
        /// ClientConfiguration containing mandatory (and maybe further) Properties:
        /// Property BaseAddressip address of the string or DNS Name
        /// Property Username username for login
        /// Property Password password for login
        /// Property ApiRequestFactory">Request factory to get request from
        /// </param>
        /// <returns></returns>
        public static async Task<HttpClient> GetAuthorizedHTPPClientAsync(ClientConfiguration ClientConfiguration)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = ClientConfiguration.AllowAutoRedirect
            };
            HttpClient httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.ConnectionClose = ClientConfiguration.ConnectionClose;
            httpClient.BaseAddress = new Uri("https://" + ClientConfiguration.BaseAddress);
            httpClient.Timeout = ClientConfiguration.TimeOut;
            var reqFact = ClientConfiguration.ApiRequestFactory;
            var apiLoginRequest = reqFact.GetApiLoginRequest(ClientConfiguration.Username, ClientConfiguration.Password);
            if (apiLoginRequest.Params != null)
            {
                apiLoginRequest.Params = apiLoginRequest.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string apiLoginRequestString = JsonConvert.SerializeObject(apiLoginRequest, 
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.UTF8.GetBytes(apiLoginRequestString);
            ByteArrayContent request_body = new ByteArrayContent(byteArr);
            request_body.Headers.ContentType = ClientConfiguration.MediaTypeHeaderValue 
                ?? new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            // send the request
            var response = await httpClient.PostAsync("api/jsonrpc", request_body);
            ResponseChecker.CheckHttpResponseForErrors(response, apiLoginRequestString);
            var respString = await response.Content.ReadAsStringAsync();
            ResponseChecker.CheckResponseStringForErros(respString, apiLoginRequestString);
            var apiLoginResponse = JsonConvert.DeserializeObject<ApiLoginResponse>(respString);
            if (apiLoginResponse.Id != apiLoginRequest.Id)
            {
                throw new Exception("ids of request and response are not equal!");
            }
            if (apiLoginResponse.JsonRpc != apiLoginRequest.JsonRpc)
            {
                throw new Exception("jsonrpc of request and response are not equal!");
            }

            //add the authorization token to the httpclients request headers so all methods afterwards can be performed with the auth token
            httpClient.DefaultRequestHeaders.Add("X-Auth-Token", apiLoginResponse.Result.Token);

            if(ClientConfiguration.DiscardPasswordAfterConnect)
            {
                ClientConfiguration.Password = "";
            }
            // return the authorized httpclient
            return httpClient;
        }

        /// <summary>
        /// use this static function to get an HTTPClient that is authenticated to the API of the baseAddress given (DNS/IP) WITH the webapp cookie 
        /// in case you need it 
        /// Will send an ApiLogin Request to the given address' plcs Api (/api/jsonrpc) and set the token if the login was successful to the headers of the 
        /// HttpClient => Will be used to Authenticate for all further API Mehtod calls
        /// HINT: Will set the ConnectionClose to false
        /// </summary>
        /// <param name="ClientConfiguration">
        /// ClientConfiguration containing mandatory (and maybe further) Properties:
        /// Property BaseAddressip address of the string or DNS Name
        /// Property Username username for login
        /// Property Password password for login
        /// Property ApiRequestFactory">Request factory to get request from
        /// </param>
        /// <param name="include_web_application_cookie">include web application cookie - if you need it you can get the web application cookie value for access of userdef. webapp pages</param>
        /// <returns></returns>
        public static async Task<HttpClientAndWebAppCookie> GetAuthorizedHTPPClientAsync(ClientConfiguration ClientConfiguration, bool include_web_application_cookie)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = ClientConfiguration.AllowAutoRedirect
            };
            HttpClient httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.ConnectionClose = ClientConfiguration.ConnectionClose;
            httpClient.BaseAddress = new Uri("https://" + ClientConfiguration.BaseAddress);
            httpClient.Timeout = ClientConfiguration.TimeOut;
            var reqFact = ClientConfiguration.ApiRequestFactory;
            var apiLoginRequest = reqFact.GetApiLoginRequest(ClientConfiguration.Username, ClientConfiguration.Password, include_web_application_cookie);
            if (apiLoginRequest.Params != null)
            {
                apiLoginRequest.Params = apiLoginRequest.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string apiLoginRequestString = JsonConvert.SerializeObject(apiLoginRequest, 
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.UTF8.GetBytes(apiLoginRequestString);
            ByteArrayContent request_body = new ByteArrayContent(byteArr);
            request_body.Headers.ContentType = ClientConfiguration.MediaTypeHeaderValue 
                ?? new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            // send the request and check for errors
            var response = await httpClient.PostAsync("api/jsonrpc", request_body);
            ResponseChecker.CheckHttpResponseForErrors(response, apiLoginRequestString);
            var respString = await response.Content.ReadAsStringAsync();
            ResponseChecker.CheckResponseStringForErros(respString, apiLoginRequestString);
            var apiLoginResponse = JsonConvert.DeserializeObject<ApiLoginResponse>(respString);
            if (apiLoginResponse.Id != apiLoginRequest.Id)
            {
                throw new Exception("ids of request and response are not equal!");
            }
            if (apiLoginResponse.JsonRpc != apiLoginRequest.JsonRpc)
            {
                throw new Exception("jsonrpc of request and response are not equal!");
            }

            //add the authorization token to the httpclients request headers so all methods afterwards can be performed with the auth token
            httpClient.DefaultRequestHeaders.Add("X-Auth-Token", apiLoginResponse.Result.Token);
            
            if (ClientConfiguration.DiscardPasswordAfterConnect)
            {
                ClientConfiguration.Password = "";
            }
            // return the authorized httpclient with the webapplicationcookie
            return new HttpClientAndWebAppCookie(httpClient, apiLoginResponse.Result.Web_application_cookie);
        }

    }
}
