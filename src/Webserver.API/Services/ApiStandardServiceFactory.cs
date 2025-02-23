// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using Siemens.Simatic.S7.Webserver.API.Services.Backup;
using Siemens.Simatic.S7.Webserver.API.Services.FileHandling;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using Siemens.Simatic.S7.Webserver.API.Services.PlcProgram;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.Services.Ticketing;
using Siemens.Simatic.S7.Webserver.API.Services.WebApp;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services
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
        private readonly ILogger _logger;
        private readonly IDataProtector _dataProtector;

        /// <summary>
        /// Standard service factory: will use standard implementations for interfaces
        /// </summary>
        /// <param name="logger">Logger to be invoked</param>
        /// <param name="dataProtector">Data protector (e.g. request response)</param>
        public ApiStandardServiceFactory(ILogger logger = null, IDataProtector dataProtector = null) 
        {
            _idGenerator = new GUIDGenerator();
            _apiRequestParameterChecker = new ApiRequestParameterChecker();
            _apiResponseChecker = new ApiResponseChecker();
            _apiRequestFactory = new ApiRequestFactory(_idGenerator, _apiRequestParameterChecker, logger);
            _apiWebAppResourceBuilder = GetApiWebAppResourceBuilder();
            _logger = logger;
            _dataProtector = dataProtector;
            if (_logger != null && _dataProtector == null)
            {
                _logger.LogWarning($"Data protector has not been provided (at some places the logger might want to use it dependant on the configuration.)");
            }
        }

        /// <summary>
        /// Create an ApiStandardServiceFactory with (optionally) customized Implementations of the interfaces
        /// </summary>
        /// <param name="idGenerator">Request Id generator</param>
        /// <param name="apiRequestParameterChecker">parameter checker for the requestfactory</param>
        /// <param name="apiResponseChecker">response checker for the requestfactory and requesthandler...</param>
        /// <param name="apiRequestFactory">request factory for request generation</param>
        /// <param name="apiWebAppResourceBuilder">resource builder for resource handler, deployer...</param>
        /// <param name="logger">Logger to be invoked</param>
        /// <param name="dataProtector">Data protector (e.g. request response)</param>
        public ApiStandardServiceFactory(IIdGenerator idGenerator, IApiRequestParameterChecker apiRequestParameterChecker,
            IApiResponseChecker apiResponseChecker, IApiRequestFactory apiRequestFactory, IApiWebAppResourceBuilder apiWebAppResourceBuilder,
            ILogger logger = null, IDataProtector dataProtector = null)
        {
            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
            _apiRequestParameterChecker = apiRequestParameterChecker ?? throw new ArgumentNullException(nameof(apiRequestParameterChecker));
            _apiResponseChecker = apiResponseChecker ?? throw new ArgumentNullException(nameof(apiResponseChecker));
            _apiRequestFactory = apiRequestFactory ?? throw new ArgumentNullException(nameof(apiRequestFactory));
            _apiWebAppResourceBuilder = apiWebAppResourceBuilder ?? throw new ArgumentNullException(nameof(apiWebAppResourceBuilder));
            _logger = logger;
            _dataProtector = dataProtector;
            if(_logger != null && _dataProtector == null)
            {
                _logger.LogWarning($"Data protector has not been provided (at some places the logger might want to use it dependant on the configuration.)");
            }
        }

        /// <summary>
        /// Get an httpclient using standard values for <see cref="HttpClientConnectionConfiguration"/>
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
        public async Task<HttpClient> GetHttpClientAsync(string baseAddress, string username, string password, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await GetHttpClientBaseAsync(GetConnectionConfiguration(baseAddress, username, password), cancellationToken: cancellationToken)).HttpClient;
        }

        /// <summary>
        /// Get an httpclient using standard values for <see cref="HttpClientConnectionConfiguration"/>
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
        public async Task<HttpClient> GetHttpClientAsync(string baseAddress, string username, string password, ApiAuthenticationMode loginMode, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await GetHttpClientBaseAsync(GetConnectionConfiguration(baseAddress, username, password), loginMode, cancellationToken: cancellationToken)).HttpClient;
        }

        /// <summary>
        /// Get an httpclient using standard values for <see cref="HttpClientConnectionConfiguration"/>
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
        public HttpClient GetHttpClient(string baseAddress, string username, string password, ApiAuthenticationMode loginMode)
            => GetHttpClientAsync(baseAddress, username, password, loginMode).GetAwaiter().GetResult();

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
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        public async Task<HttpClientAndWebAppCookie> GetHttpClientAsync(string baseAddress, string username, string password, ApiAuthenticationMode loginMode, bool include_web_application_cookie, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetHttpClientBaseAsync(GetConnectionConfiguration(baseAddress, username, password), loginMode, include_web_application_cookie, cancellationToken);
        }

        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) using standard values for <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        public async Task<HttpClientAndWebAppCookie> GetHttpClientAsync(string baseAddress, string username, string password, bool include_web_application_cookie, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetHttpClientBaseAsync(GetConnectionConfiguration(baseAddress, username, password), include_web_application_cookie: include_web_application_cookie, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) using standard values for <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        public HttpClientAndWebAppCookie GetHttpClient(string baseAddress, string username, string password, ApiAuthenticationMode loginMode, bool include_web_application_cookie)
            => GetHttpClientAsync(baseAddress, username, password, loginMode, include_web_application_cookie).GetAwaiter().GetResult();

        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection Configuration which should contains the base address, username, passwort etc.</param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        private async Task<HttpClientAndWebAppCookie> GetHttpClientBaseAsync(HttpClientConnectionConfiguration connectionConfiguration, ApiAuthenticationMode? loginMode = null, bool? include_web_application_cookie = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = connectionConfiguration.AllowAutoRedirect
            };
#if NET6_0_OR_GREATER
            // Ignoring SSL errors in net6.0 and greater
            if (ServerCertificateCallback.CertificateCallback != null)
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = ServerCertificateCallback.CertificateCallback;
            }
#endif
            HttpClient httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.ConnectionClose = connectionConfiguration.ConnectionClose;
            httpClient.BaseAddress = new Uri("https://" + connectionConfiguration.BaseAddress);
            httpClient.Timeout = connectionConfiguration.TimeOut;
            var apiLoginRequest = _apiRequestFactory.GetApiLoginRequest(connectionConfiguration.Username, connectionConfiguration.Password, include_web_application_cookie, loginMode);
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
            var response = await httpClient.PostAsync("api/jsonrpc", request_body, cancellationToken);
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
        /// <param name="connectionConfiguration">Connection Configuration which should contains the base address, username, passwort etc.</param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        public HttpClientAndWebAppCookie GetHttpClient(HttpClientConnectionConfiguration connectionConfiguration, bool include_web_application_cookie)
            => GetHttpClientBaseAsync(connectionConfiguration, null, include_web_application_cookie).GetAwaiter().GetResult();

        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection Configuration which should contains the base address, username, passwort etc.</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        public HttpClientAndWebAppCookie GetHttpClient(HttpClientConnectionConfiguration connectionConfiguration, ApiAuthenticationMode loginMode, bool include_web_application_cookie)
            => GetHttpClientBaseAsync(connectionConfiguration, loginMode, include_web_application_cookie).GetAwaiter().GetResult();

        /// <summary>
        /// Get an httpclient using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection Configuration which should contains the base address, username, passwort etc.</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
        public HttpClient GetHttpClient(HttpClientConnectionConfiguration connectionConfiguration)
            => (GetHttpClientBaseAsync(connectionConfiguration).GetAwaiter().GetResult()).HttpClient;
        /// <summary>
        /// Get an httpclient using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection Configuration which should contains the base address, username, passwort etc.</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
        public HttpClient GetHttpClient(HttpClientConnectionConfiguration connectionConfiguration, ApiAuthenticationMode loginMode)
            => (GetHttpClientBaseAsync(connectionConfiguration, loginMode).GetAwaiter().GetResult()).HttpClient;

        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using standard values for <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public async Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(string baseAddress, string username, string password, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpClient = await GetHttpClientAsync(baseAddress, username, password, cancellationToken);
            return new ApiHttpClientRequestHandler(httpClient, _apiRequestFactory, _apiResponseChecker, _logger, _dataProtector);
        }

        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using standard values for <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public async Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(string baseAddress, string username, string password, ApiAuthenticationMode loginMode, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpClient = await GetHttpClientAsync(baseAddress, username, password, loginMode, cancellationToken);
            return new ApiHttpClientRequestHandler(httpClient, _apiRequestFactory, _apiResponseChecker, _logger, _dataProtector);
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
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using standard values for <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public IApiRequestHandler GetApiHttpClientRequestHandler(string baseAddress, string username, string password, ApiAuthenticationMode loginMode)
            => GetApiHttpClientRequestHandlerAsync(baseAddress, username, password, loginMode).GetAwaiter().GetResult();

        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public async Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(HttpClientConnectionConfiguration connectionConfiguration)
        {
            var httpClient = (await GetHttpClientBaseAsync(connectionConfiguration)).HttpClient;
            return new ApiHttpClientRequestHandler(httpClient, _apiRequestFactory, _apiResponseChecker, _logger, _dataProtector);
        }

        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public async Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(HttpClientConnectionConfiguration connectionConfiguration, ApiAuthenticationMode loginMode)
        {
            var httpClient = (await GetHttpClientBaseAsync(connectionConfiguration, loginMode)).HttpClient;
            return new ApiHttpClientRequestHandler(httpClient, _apiRequestFactory, _apiResponseChecker, _logger, _dataProtector);
        }

        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public async Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(HttpClientConnectionConfiguration connectionConfiguration, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpClient = (await GetHttpClientBaseAsync(connectionConfiguration, cancellationToken: cancellationToken)).HttpClient;
            return new ApiHttpClientRequestHandler(httpClient, _apiRequestFactory, _apiResponseChecker, _logger, _dataProtector);
        }

        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public async Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(HttpClientConnectionConfiguration connectionConfiguration, ApiAuthenticationMode loginMode, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpClient = (await GetHttpClientBaseAsync(connectionConfiguration, loginMode, cancellationToken: cancellationToken)).HttpClient;
            return new ApiHttpClientRequestHandler(httpClient, _apiRequestFactory, _apiResponseChecker, _logger, _dataProtector);
        }

        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public IApiRequestHandler GetApiHttpClientRequestHandler(HttpClientConnectionConfiguration connectionConfiguration)
            => GetApiHttpClientRequestHandlerAsync(connectionConfiguration).GetAwaiter().GetResult();
        
        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <param name="loginMode">The mode defines where the login shall be performed. All available modes supported by API method Api.GetAuthenticationMode can be passed. </param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        public IApiRequestHandler GetApiHttpClientRequestHandler(HttpClientConnectionConfiguration connectionConfiguration, ApiAuthenticationMode loginMode)
            => GetApiHttpClientRequestHandlerAsync(connectionConfiguration, loginMode).GetAwaiter().GetResult();

        /// <summary>
        /// Get A apiPlcProgramHandler with the given requestHandler and the set apiRequestFactory
        /// </summary>
        /// <param name="requestHandler">Request Handler the plcProgramHandler shall use</param>
        /// <returns>an <see cref="ApiPlcProgramHandler"/></returns>
        public IApiPlcProgramHandler GetPlcProgramHandler(IApiRequestHandler requestHandler)
        {
            return new ApiPlcProgramHandler(requestHandler, _apiRequestFactory, _logger);
        }

        /// <summary>
        /// Get A resourceHandler with the given requestHandler and the set apiResourceBuilder
        /// </summary>
        /// <param name="requestHandler">Request Handler the Resource Handler shall use</param>
        /// <returns>an <see cref="ApiResourceHandler"/></returns>
        public IApiResourceHandler GetApiResourceHandler(IApiRequestHandler requestHandler)
        {
            return new ApiResourceHandler(requestHandler, _apiWebAppResourceBuilder, GetApiTicketHandler(requestHandler));
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

        /// <summary>
        /// Get A WebAppResourceBuilder
        /// </summary>
        /// <returns>WebAppResourceBuilder</returns>
        public IApiWebAppResourceBuilder GetApiWebAppResourceBuilder()
        {
            return new ApiWebAppResourceBuilder();
        }

        /// <summary>
        /// Get a RequestFactory
        /// </summary>
        /// <returns>A RequestFactory</returns>
        public IApiRequestFactory GetApiRequestFactory()
        {
            return _apiRequestFactory;
        }

        /// <summary>
        /// Get A RequestGenerator
        /// </summary>
        /// <returns>A RequestGenerator</returns>
        public IIdGenerator GetIdGenerator()
        {
            return new GUIDGenerator();
        }

        /// <summary>
        /// Get a ApiFileHandler
        /// </summary>
        /// <param name="requestHandler">Request handler to send the api requests with.</param>
        /// <returns>A ApiFileHandler</returns>
        public IApiFileHandler GetApiFileHandler(IApiRequestHandler requestHandler)
        {
            return new ApiFileHandler(requestHandler, GetApiTicketHandler(requestHandler));
        }

        /// <summary>
        /// Get a ApiFileHandler
        /// </summary>
        /// <param name="requestHandler">Request handler to send the api requests with.</param>
        /// <returns>A ApiFileHandler</returns>
        public IApiDirectoryHandler GetApiDirectoryHandler(IApiRequestHandler requestHandler)
        {
            return new ApiDirectoryHandler(requestHandler, GetApiFileHandler(requestHandler));
        }

        /// <summary>
        /// Get a ApiBackupHandler
        /// </summary>
        /// <param name="requestHandler">Request handler to send the api requests with.</param>
        /// <returns>A ApiBackupHandler</returns>
        public IApiBackupHandler GetApiBackupHandler(IApiRequestHandler requestHandler)
        {
            return new ApiBackupHandler(requestHandler, GetApiTicketHandler(requestHandler));
        }

        /// <summary>
        /// Get A Tickethandler with the given requestHandler
        /// </summary>
        /// <param name="requestHandler">Request Handler the Ticketing Handler shall use</param>
        /// <returns>an <see cref="ApiTicketHandler"/></returns>
        public IApiTicketHandler GetApiTicketHandler(IApiRequestHandler requestHandler)
        {
            return new ApiTicketHandler(requestHandler);
        }

        /// <summary>
        /// Get an ApiFileResourceBuilder
        /// </summary>
        /// <returns>an ApiFileResourceBuilder</returns>
        public IApiFileResourceBuilder GetApiFileResourceBuilder()
        {
            return new ApiFileResourceBuilder();
        }

        /// <summary>
        /// Get an ApiDirectoryBuilder
        /// </summary>
        /// <param name="pathToLocalDirectory">path to the local directory</param>
        /// <returns>an ApiDirectoryBuilder</returns>
        public IApiDirectoryBuilder GetApiDirectoryBuilder(string pathToLocalDirectory)
        {
            return new ApiDirectoryBuilder(pathToLocalDirectory, GetApiFileResourceBuilder(), false);
        }

        /// <summary cref ="IApiServiceFactory.GetHttpClientAsync(HttpClientConnectionConfiguration, CancellationToken)" />
        public async Task<HttpClient> GetHttpClientAsync(HttpClientConnectionConfiguration connectionConfiguration, CancellationToken cancellationToken = default)
        {
            return (await GetHttpClientBaseAsync(connectionConfiguration, cancellationToken: cancellationToken)).HttpClient;
        }

        /// <summary cref ="IApiServiceFactory.GetHttpClientAsync(HttpClientConnectionConfiguration, ApiAuthenticationMode,  CancellationToken)" />
        public async Task<HttpClient> GetHttpClientAsync(HttpClientConnectionConfiguration connectionConfiguration, ApiAuthenticationMode loginMode, CancellationToken cancellationToken = default)
        {
            return (await GetHttpClientBaseAsync(connectionConfiguration, loginMode, cancellationToken: cancellationToken)).HttpClient;
        }

        /// <summary cref ="IApiServiceFactory.GetHttpClient(string, string, string, bool)" />
        public HttpClientAndWebAppCookie GetHttpClient(string baseAddress, string username, string password, bool include_web_application_cookie)
        {
            return (GetHttpClientBaseAsync(GetConnectionConfiguration(baseAddress, username, password), include_web_application_cookie: include_web_application_cookie).GetAwaiter().GetResult());
        }

        /// <summary cref ="IApiServiceFactory.GetHttpClientAsync(HttpClientConnectionConfiguration, bool, CancellationToken)" />
        public async Task<HttpClientAndWebAppCookie> GetHttpClientAsync(HttpClientConnectionConfiguration connectionConfiguration, bool include_web_application_cookie, CancellationToken cancellationToken = default)
        {
            return await GetHttpClientBaseAsync(connectionConfiguration, include_web_application_cookie: include_web_application_cookie, cancellationToken: cancellationToken);
        }

        /// <summary cref ="IApiServiceFactory.GetHttpClientAsync(HttpClientConnectionConfiguration,ApiAuthenticationMode, bool, CancellationToken)" />
        public async Task<HttpClientAndWebAppCookie> GetHttpClientAsync(HttpClientConnectionConfiguration connectionConfiguration, ApiAuthenticationMode loginMode, bool include_web_application_cookie, CancellationToken cancellationToken = default)
        {
            return await GetHttpClientBaseAsync(connectionConfiguration, loginMode, include_web_application_cookie, cancellationToken);
        }
    }
}
