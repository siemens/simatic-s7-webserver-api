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
    public class ApiStandardServiceFactory : IApiServiceFactory
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

        /// <summary>
        /// Create an ApiStandardServiceFactory with (optionally) customized Implementations of the interfaces
        /// </summary>
        /// <param name="idGenerator">Request Id generator</param>
        /// <param name="apiRequestParameterChecker">parameter checker for the requestfactory</param>
        /// <param name="apiResponseChecker">response checker for the requestfactory and requesthandler,...</param>
        /// <param name="apiRequestFactory">request factory for request generation</param>
        /// <param name="apiWebAppResourceBuilder">resource builder for resource handler, deployer,...</param>
        public ApiStandardServiceFactory(IIdGenerator idGenerator, IApiRequestParameterChecker apiRequestParameterChecker, 
            IApiResponseChecker apiResponseChecker, IApiRequestFactory apiRequestFactory, IApiWebAppResourceBuilder apiWebAppResourceBuilder)
        {
            _idGenerator = idGenerator;
            _apiRequestParameterChecker = apiRequestParameterChecker;
            _apiResponseChecker = apiResponseChecker;
            _apiRequestFactory = apiRequestFactory;
            _apiWebAppResourceBuilder = apiWebAppResourceBuilder;
        }



        /// <summary>
        /// Get an httpclient using standard values for <see cref="HttpClientConnectionConfiguration"/>
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
        public async Task<HttpClient> GetHttpClientAsync(string baseAddress, string username, string password)
        {
            return await GetHttpClientAsync(GetConnectionConfiguration(baseAddress, username, password));
        }

        /// <summary>
        /// Get an httpclient using standard values for <see cref="HttpClientConnectionConfiguration"/>
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
        public HttpClient GetHttpClient(string baseAddress, string username, string password)
            => GetHttpClientAsync(baseAddress, username, password).GetAwaiter().GetResult();

        /// <summary>
        /// Get a <see cref="HttpClientConnectionConfiguration"/> using standard values for timeout, connectionclose, 
        /// allowAutoRedirect and discardpasswortafterconnect
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>A HttpClientConnectionConfiguration with standard values</returns>
        public HttpClientConnectionConfiguration GetConnectionConfiguration(string baseAddress, string username, string password)
        {
            return new HttpClientConnectionConfiguration(baseAddress, username, password,
                TimeSpan.FromMinutes(10), false, false, true);
        }

        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) using standard values for <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        public async Task<HttpClientAndWebAppCookie> GetHttpClientAsync(string baseAddress, string username, string password, bool include_web_application_cookie)
        {
            return await GetHttpClientAsync(GetConnectionConfiguration(baseAddress, username, password), include_web_application_cookie);
        }

        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) using standard values for <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        public HttpClientAndWebAppCookie GetHttpClient(string baseAddress, string username, string password, bool include_web_application_cookie)
            => GetHttpClientAsync(baseAddress, username, password, include_web_application_cookie).GetAwaiter().GetResult();

        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
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

        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        public HttpClientAndWebAppCookie GetHttpClient(HttpClientConnectionConfiguration connectionConfiguration, bool include_web_application_cookie)
            => GetHttpClientAsync(connectionConfiguration, include_web_application_cookie).GetAwaiter().GetResult();

        /// <summary>
        /// Get an httpclient using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
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

        /// <summary>
        /// Get an httpclient using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
        public HttpClient GetHttpClient(HttpClientConnectionConfiguration connectionConfiguration)
            => GetHttpClientAsync(connectionConfiguration).GetAwaiter().GetResult();

        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using standard values for <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public async Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(string baseAddress, string username, string password)
        {
            var httpClient = await GetHttpClientAsync(baseAddress, username, password);
            return new ApiHttpClientRequestHandler(httpClient, _apiRequestFactory, _apiResponseChecker);
        }

        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using standard values for <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public IApiRequestHandler GetApiHttpClientRequestHandler(string baseAddress, string username, string password)
            => GetApiHttpClientRequestHandlerAsync(baseAddress, username, password).GetAwaiter().GetResult();

        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public async Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(HttpClientConnectionConfiguration connectionConfiguration)
        {
            var httpClient = await GetHttpClientAsync(connectionConfiguration);
            return new ApiHttpClientRequestHandler(httpClient, _apiRequestFactory, _apiResponseChecker);
        }

        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public IApiRequestHandler GetApiHttpClientRequestHandler(HttpClientConnectionConfiguration connectionConfiguration)
            => GetApiHttpClientRequestHandlerAsync(connectionConfiguration).GetAwaiter().GetResult();

        /// <summary>
        /// Get A apiPlcProgramHandler with the given requestHandler and the set apiRequestFactory
        /// </summary>
        /// <param name="requestHandler">Request Handler the plcProgramHandler shall use</param>
        /// <returns>an <see cref="ApiPlcProgramHandler"/></returns>
        public IApiPlcProgramHandler GetPlcProgramHandler(IApiRequestHandler requestHandler)
        {
            return new ApiPlcProgramHandler(requestHandler, _apiRequestFactory);
        }

        /// <summary>
        /// Get A resourceHandler with the given requestHandler and the set apiResourceBuilder
        /// </summary>
        /// <param name="requestHandler">Request Handler the Resource Handler shall use</param>
        /// <returns>an <see cref="ApiResourceHandler"/></returns>
        public IApiResourceHandler GetApiResourceHandler(IApiRequestHandler requestHandler)
        {
            return new ApiResourceHandler(requestHandler, _apiWebAppResourceBuilder);
        }

        /// <summary>
        /// Get A apiWebAppDeployer with the given requestHandler and an apiResourceHandler <see cref="GetApiResourceHandler(IApiRequestHandler)"/>
        /// </summary>
        /// <param name="requestHandler">Request Handler the deployer and resourceHandler shall use</param>
        /// <returns>an <see cref="ApiWebAppDeployer"/></returns>
        public IApiWebAppDeployer GetApiWebAppDeployer(IApiRequestHandler requestHandler)
        {
            return new ApiWebAppDeployer(requestHandler, GetApiResourceHandler(requestHandler));
        }

    }
}
