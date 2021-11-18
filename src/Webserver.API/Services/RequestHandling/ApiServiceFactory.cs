using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using Siemens.Simatic.S7.Webserver.API.Services.PlcProgram;
using Siemens.Simatic.S7.Webserver.API.Services.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Factory to create services with standard implementation
    /// </summary>
    public class ApiStandardServiceFactory
    {
        private readonly IIdGenerator _idGenerator;
        private readonly IApiRequestParameterChecker _apiRequestParameterChecker;
        private readonly IApiResponseChecker _apiResponseChecker;
        private readonly IApiRequestFactory _apiRequestFactory;
        private readonly IApiWebAppResourceBuilder _apiWebAppResourceBuilder;
        
        /// <summary>
        /// Standard service factory: will use standard implementations for interfaces
        /// </summary>
        public ApiStandardServiceFactory()
        {
            _idGenerator = new GUIDGenerator();
            _apiRequestParameterChecker = new ApiRequestParameterChecker();
            _apiResponseChecker = new ApiResponseChecker();
            _apiRequestFactory = new ApiRequestFactory(_idGenerator, _apiRequestParameterChecker);
            _apiWebAppResourceBuilder = new ApiWebAppResourceBuilder();
        }

        

        public async Task<HttpClient> GetHttpClientAsync(string baseAddress, string username, string password)
        {
            return await GetHttpClientAsync(GetConnectionConfiguration(baseAddress, username, password));
        }

        public HttpClient GetHttpClient(string baseAddress, string username, string password)
            => GetHttpClientAsync(baseAddress, username, password).GetAwaiter().GetResult();

        public HttpClientConnectionConfiguration GetConnectionConfiguration(string baseAddress, string username, string password)
        {
            return new HttpClientConnectionConfiguration(baseAddress, username, password,
                TimeSpan.FromMinutes(10), false, false, true);
        }

        public async Task<HttpClientAndWebAppCookie> GetHttpClientAsync(string baseAddress, string username, string password, bool include_web_application_cookie)
        {
            return await GetHttpClientAsync(GetConnectionConfiguration(baseAddress, username, password), include_web_application_cookie);
        }

        public HttpClientAndWebAppCookie GetHttpClient(string baseAddress, string username, string password, bool include_web_application_cookie)
            => GetHttpClientAsync(baseAddress, username, password, include_web_application_cookie).GetAwaiter().GetResult();

        public async Task<HttpClientAndWebAppCookie> GetHttpClientAsync(HttpClientConnectionConfiguration connectionConfiguration, bool include_web_application_cookie)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = connectionConfiguration.AllowAutoRedirect
            };
            HttpClient httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.ConnectionClose = connectionConfiguration.ConnectionClose;
            httpClient.BaseAddress = new Uri("https://" + connectionConfiguration.BaseAddress);
            httpClient.Timeout = connectionConfiguration.TimeOut;
            var apiLoginRequest = _apiRequestFactory.GetApiLoginRequest(connectionConfiguration.Username, connectionConfiguration.Password, include_web_application_cookie);
            if (apiLoginRequest.Params != null)
            {
                apiLoginRequest.Params = apiLoginRequest.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string apiLoginRequestString = JsonConvert.SerializeObject(apiLoginRequest,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            byte[] byteArr = Encoding.UTF8.GetBytes(apiLoginRequestString);
            ByteArrayContent request_body = new ByteArrayContent(byteArr);
            request_body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            // send the request and check for errors
            var response = await httpClient.PostAsync("api/jsonrpc", request_body);
            _apiResponseChecker.CheckHttpResponseForErrors(response, apiLoginRequestString);
            var respString = await response.Content.ReadAsStringAsync();
            _apiResponseChecker.CheckResponseStringForErros(respString, apiLoginRequestString);
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

            if (connectionConfiguration.DiscardPasswordAfterConnect)
            {
                connectionConfiguration.DiscardPassword();
            }
            // return the authorized httpclient with the webapplicationcookie
            return new HttpClientAndWebAppCookie(httpClient, apiLoginResponse.Result.Web_application_cookie);
        }

        public HttpClientAndWebAppCookie GetHttpClient(HttpClientConnectionConfiguration connectionConfiguration, bool include_web_application_cookie)
            => GetHttpClientAsync(connectionConfiguration, include_web_application_cookie).GetAwaiter().GetResult();

        public async Task<HttpClient> GetHttpClientAsync(HttpClientConnectionConfiguration connectionConfiguration)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = connectionConfiguration.AllowAutoRedirect
            };
            HttpClient httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.ConnectionClose = connectionConfiguration.ConnectionClose;
            httpClient.BaseAddress = new Uri("https://" + connectionConfiguration.BaseAddress);
            httpClient.Timeout = connectionConfiguration.TimeOut;
            var apiLoginRequest = _apiRequestFactory.GetApiLoginRequest(connectionConfiguration.Username, connectionConfiguration.Password);
            if (apiLoginRequest.Params != null)
            {
                apiLoginRequest.Params = apiLoginRequest.Params
                    .Where(el => el.Value != null)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            string apiLoginRequestString = JsonConvert.SerializeObject(apiLoginRequest,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            byte[] byteArr = Encoding.UTF8.GetBytes(apiLoginRequestString);
            ByteArrayContent request_body = new ByteArrayContent(byteArr);
            request_body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            // send the request
            var response = await httpClient.PostAsync("api/jsonrpc", request_body);
            _apiResponseChecker.CheckHttpResponseForErrors(response, apiLoginRequestString);
            var respString = await response.Content.ReadAsStringAsync();
            _apiResponseChecker.CheckResponseStringForErros(respString, apiLoginRequestString);
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
            if (connectionConfiguration.DiscardPasswordAfterConnect)
            {
                connectionConfiguration.DiscardPassword();
            }
            // return the authorized httpclient
            return httpClient;
        }

        public HttpClient GetHttpClient(HttpClientConnectionConfiguration connectionConfiguration)
            => GetHttpClientAsync(connectionConfiguration).GetAwaiter().GetResult();

        public async Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(string baseAddress, string username, string password)
        {
            var httpClient = await GetHttpClientAsync(baseAddress, username, password);
            return new ApiHttpClientRequestHandler(httpClient, _apiRequestFactory, _apiResponseChecker);
        }

        public IApiRequestHandler GetApiHttpClientRequestHandler(string baseAddress, string username, string password)
            => GetApiHttpClientRequestHandlerAsync(baseAddress, username, password).GetAwaiter().GetResult();

        public ApiPlcProgramHandler GetPlcProgramHandler(IApiRequestHandler requestHandler)
        {
            return new ApiPlcProgramHandler(requestHandler, _apiRequestFactory);
        }

        public ApiResourceHandler GetApiResourceHandler(IApiRequestHandler requestHandler)
        {
            return new ApiResourceHandler(requestHandler, _apiWebAppResourceBuilder);
        }

        public ApiWebAppDeployer GetApiWebAppDeployer(IApiRequestHandler requestHandler)
        {
            return new ApiWebAppDeployer(requestHandler, GetApiResourceHandler(requestHandler));
        }

    }
}
