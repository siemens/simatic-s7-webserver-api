// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.Backup;
using Siemens.Simatic.S7.Webserver.API.Services.FileHandling;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using Siemens.Simatic.S7.Webserver.API.Services.PlcProgram;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.Services.WebApp;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services
{
    /// <summary>
    /// Factory to create services
    /// </summary>
    public interface IApiServiceFactory
    {
        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        IApiRequestHandler GetApiHttpClientRequestHandler(HttpClientConnectionConfiguration connectionConfiguration);
        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        IApiRequestHandler GetApiHttpClientRequestHandler(string baseAddress, string username, string password);
        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(HttpClientConnectionConfiguration connectionConfiguration);
        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/> using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(HttpClientConnectionConfiguration connectionConfiguration, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get an httpclient using the given <see cref="HttpClientConnectionConfiguration"/>
        /// </summary>
        /// <param name="connectionConfiguration">Connection Configuration which should contains the base address, username, passwort etc.</param>
        /// <param name="cancellationToken">Token used to cancel requests without waiting for the response</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
        Task<HttpClient> GetHttpClientAsync(HttpClientConnectionConfiguration connectionConfiguration, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get an <see cref="ApiHttpClientRequestHandler"/>
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>A usable and authenticated <see cref="ApiHttpClientRequestHandler"/></returns>
        Task<IApiRequestHandler> GetApiHttpClientRequestHandlerAsync(string baseAddress, string username, string password, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get A resourceHandler with the given requestHandler and the set resourcebuilder
        /// </summary>
        /// <param name="requestHandler">Request Handler the Resource Handler shall use</param>
        /// <returns>a resourcehandler</returns>
        IApiResourceHandler GetApiResourceHandler(IApiRequestHandler requestHandler);
        /// <summary>
        /// Get A apiWebAppDeployer with the given requestHandler and an apiResourceHandler <see cref="GetApiResourceHandler(IApiRequestHandler)"/>
        /// </summary>
        /// <param name="requestHandler">Request Handler the Resource Handler shall use</param>
        /// <returns>an <see cref="ApiWebAppDeployer"/></returns>
        IApiWebAppDeployer GetApiWebAppDeployer(IApiRequestHandler requestHandler);
        /// <summary>
        /// Get a <see cref="HttpClientConnectionConfiguration"/>
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>A HttpClientConnectionConfiguration with standard values</returns>
        HttpClientConnectionConfiguration GetConnectionConfiguration(string baseAddress, string username, string password);
        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        HttpClient GetHttpClient(HttpClientConnectionConfiguration connectionConfiguration);
        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection configuration to use</param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        HttpClientAndWebAppCookie GetHttpClient(HttpClientConnectionConfiguration connectionConfiguration, bool include_web_application_cookie);
        /// <summary>
        /// Get an httpclient
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
        HttpClient GetHttpClient(string baseAddress, string username, string password);
        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) 
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        HttpClientAndWebAppCookie GetHttpClient(string baseAddress, string username, string password, bool include_web_application_cookie);
        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages) using the given <see cref="HttpClientConnectionConfiguration"/> 
        /// </summary>
        /// <param name="connectionConfiguration">Connection Configuration which should contains the base address, username, passwort etc.</param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        Task<HttpClientAndWebAppCookie> GetHttpClientAsync(HttpClientConnectionConfiguration connectionConfiguration, bool include_web_application_cookie, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get an httpclient
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set)</returns>
        Task<HttpClient> GetHttpClientAsync(string baseAddress, string username, string password, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get an httpclient and a webappcookie (for accessing userdefined web pages)
        /// </summary>
        /// <param name="baseAddress">ip address or dns name of your plc</param>
        /// <param name="username">username to login with</param>
        /// <param name="password">password to login with</param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>an authorized httpclient (client with header value x-auth-token set) and the according webappcookie</returns>
        Task<HttpClientAndWebAppCookie> GetHttpClientAsync(string baseAddress, string username, string password, bool include_web_application_cookie, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get A apiPlcProgramHandler with the given requestHandler and the set apiRequestFactory
        /// </summary>
        /// <param name="requestHandler">Request Handler the plcProgramHandler shall use</param>
        /// <returns>an apiPlcProgramHandler/></returns>
        IApiPlcProgramHandler GetPlcProgramHandler(IApiRequestHandler requestHandler);

        /// <summary>
        /// Get A WebAppResourceBuilder
        /// </summary>
        /// <returns>WebAppResourceBuilder</returns>
        IApiWebAppResourceBuilder GetApiWebAppResourceBuilder();

        /// <summary>
        /// Get A RequestFactory
        /// </summary>
        /// <returns>A RequestFactory</returns>
        IApiRequestFactory GetApiRequestFactory();

        /// <summary>
        /// Get A RequestGenerator
        /// </summary>
        /// <returns>A RequestGenerator</returns>
        IIdGenerator GetIdGenerator();

        /// <summary>
        /// Get an ApiFileHandler
        /// </summary>
        /// <param name="apiRequestHandler">Request handler to send the api requests with.</param>
        /// <returns>A ApiFileHandler</returns>
        IApiFileHandler GetApiFileHandler(IApiRequestHandler apiRequestHandler);

        /// <summary>
        /// Get an ApiBackupHandler
        /// </summary>
        /// <param name="apiRequestHandler">Request handler to send the api requests with.</param>
        /// <returns>A ApiBackupHandler</returns>
        IApiBackupHandler GetApiBackupHandler(IApiRequestHandler apiRequestHandler);

        /// <summary>
        /// Get a ApiFileHandler
        /// </summary>
        /// <param name="requestHandler">Request handler to send the api requests with.</param>
        /// <returns>A ApiFileHandler</returns>
        IApiDirectoryHandler GetApiDirectoryHandler(IApiRequestHandler requestHandler);

        /// <summary>
        /// Get an ApiFileResourceBuilder
        /// </summary>
        /// <returns>an ApiFileResourceBuilder</returns>
        IApiFileResourceBuilder GetApiFileResourceBuilder();

        /// <summary>
        /// Get an ApiDirectoryBuilder
        /// </summary>
        /// <param name="pathToLocalDirectory">path to the local directory</param>
        /// <returns>an ApiDirectoryBuilder</returns>
        IApiDirectoryBuilder GetApiDirectoryBuilder(string pathToLocalDirectory);
    }
}