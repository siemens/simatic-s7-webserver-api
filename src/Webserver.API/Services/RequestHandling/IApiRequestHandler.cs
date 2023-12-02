// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.AlarmsBrowse;
using Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using Siemens.Simatic.S7.Webserver.API.Models.TimeSettings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Interface for AsyncApiRequestHandler
    /// </summary>
    public interface IApiRequestHandler
    {
        /// <summary>
        /// appilication/json for requests to the jsonrpc api endpoint
        /// </summary>
        string ContentType { get; }
        /// <summary>
        /// Encoding.UTF8
        /// </summary>
        Encoding Encoding { get; }
        /// <summary>
        /// api/jsonrpc endpoint of plc
        /// </summary>
        string JsonRpcApi { get; }

        /// <summary>
        /// Send an ApiBulk Request
        /// </summary>
        /// <returns>an array of ApiResponses</returns>
        /// <param name="apiRequests">api Requests to send</param>
        Task<ApiBulkResponse> ApiBulkAsync(IEnumerable<IApiRequest> apiRequests);
        /// <summary>
        /// Send an Api.Browse Request
        /// </summary>
        /// <returns>An Array of ApiClass (and Id,Jsonrpc)</returns>
        Task<ApiArrayOfApiClassResponse> ApiBrowseAsync(CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Send an Api.BrowseTickets Request  
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        Task<ApiBrowseTicketsResponse> ApiBrowseTicketsAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Send an Api.BrowseTickets Request  
        /// </summary>
        /// <param name="ticketId">ticket id (28 chars)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        Task<ApiBrowseTicketsResponse> ApiBrowseTicketsAsync(string ticketId = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Send an Api.BrowseTickets Request  
        /// </summary>
        /// <param name="ticket">ticket to be browsed (null to browse all)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        Task<ApiBrowseTicketsResponse> ApiBrowseTicketsAsync(ApiTicket ticket, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Send an Api.BrowseTickets Request
        /// </summary>
        /// <param name="ticketId">ticket id (28 chars)</param>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        ApiBrowseTicketsResponse ApiBrowseTickets(string ticketId);
        /// <summary>
        /// Send an Api.BrowseTickets Request  
        /// </summary>
        /// <param name="ticket">ticket to be browsed (null to browse all)</param>
        /// <returns>BrowseTickets Response containing: Max_Tickets:uint, Tickets:Array of Ticket</returns>
        ApiBrowseTicketsResponse ApiBrowseTickets(ApiTicket ticket);
        /// <summary>
        /// Send an Api.CloseTicket Request  
        /// </summary>
        /// <param name="ticketId">ticket id (28 chars)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>True to indicate Success</returns>
        Task<ApiTrueOnSuccessResponse> ApiCloseTicketAsync(string ticketId, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Send an Api.CloseTicket Request  
        /// </summary>
        /// <param name="ticket">ticket containing ticket id (28 chars)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>True to indicate Success</returns>
        Task<ApiTrueOnSuccessResponse> ApiCloseTicketAsync(ApiTicket ticket, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Send an Api.ChangePassword request
        /// </summary>
        /// <param name="username">The user account for which the password shall be changed</param>
        /// <param name="currentPassword">The current password for the user</param>
        /// <param name="newPassword">The new password for the user</param>
        /// <returns>True if changing password for the user was successful</returns>
        Task<ApiTrueOnSuccessResponse> ApiChangePasswordAsync(string username, string currentPassword, string newPassword);
        /// <summary>
        /// Send an Api.ChangePassword request
        /// </summary>
        /// <param name="username">The user account for which the password shall be changed</param>
        /// <param name="currentPassword">The current password for the user</param>
        /// <param name="newPassword">The new password for the user</param>
        /// <returns>True if changing password for the user was successful</returns>
        ApiTrueOnSuccessResponse ApiChangePassword(string username, string currentPassword, string newPassword);
        /// <summary>
        /// Send an Api.GetCertificateUrl Request 
        /// </summary>
        /// <returns>ApiSingleStringResponse that contians the URL to the certificate</returns>
        Task<ApiSingleStringResponse> ApiGetCertificateUrlAsync();
        /// <summary>
        /// Send an Api.GetPermissions Request  
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>Array of ApiClass (in this case permissions)</returns>
        Task<ApiArrayOfApiClassResponse> ApiGetPermissionsAsync(CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Send an Api.GetQuantityStructures Request  
        /// </summary>
        /// <returns>A QuantityStructure object</returns>
        Task<ApiGetQuantityStructuresResponse> ApiGetQuantityStructuresAsync();
        /// <summary>
        /// Send an Api.GetQuantityStructures Request  
        /// </summary>
        /// <returns>A QuantityStructure object</returns>
        ApiGetQuantityStructuresResponse ApiGetQuantityStructures();
        /// <summary>
        /// Send a Api.Login Request 
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <returns>ApiLoginResponse: contains ApiTokenResult: Token(auth token string) and if requested Web_application_cookie</returns>
        Task<ApiLoginResponse> ApiLoginAsync(string userName, string password, bool? includeWebApplicationCookie = null);
        /// <summary>
        /// Send an Api.Logout Request 
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>True to indicate success</returns>
        Task<ApiTrueOnSuccessResponse> ApiLogoutAsync(CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Send an Api.Ping Request 
        /// </summary>
        /// <returns>ApiSingleStringResponse - an Id that'll stay the same for the users session</returns>
        Task<ApiSingleStringResponse> ApiPingAsync();
        /// <summary>
        /// Send an Api.Version Request 
        /// </summary>
        /// <returns>a double that contains the value for the current ApiVersion</returns>
        Task<ApiDoubleResponse> ApiVersionAsync();
        /// <summary>
        /// Function to get the ByteArray Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>Bytearray given from the PLC</returns>
        Task<byte[]> DownloadTicketAsync(string ticketId, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Function to get the ByteArray Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>HTTp response </returns>
        Task<HttpResponseMessage> DownloadTicketAndGetResponseAsync(string ticketId, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Send a PlcProgram.Browse Request 
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
        Task<ApiPlcProgramBrowseResponse> PlcProgramBrowseAsync(ApiPlcProgramBrowseMode plcProgramBrowseMode, string var = null);
        /// <summary>
        /// Send a PlcProgram.Browse Request 
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
        Task<ApiPlcProgramBrowseResponse> PlcProgramBrowseAsync(ApiPlcProgramBrowseMode plcProgramBrowseMode, ApiPlcProgramData var);
        /// <summary>
        /// Send a PlcProgram.Read Request 
        /// </summary>
        /// <param name="var">Name of the variable to be read
        /// 
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramReadMode">
        /// "simple" will get the variable values according to the presentation of the manual - "supported Datatypes"
        /// "raw" : will get the variable values according to the presentation of the manual "raw"
        ///
        /// Aufzählung, die das Antwortformat für diese Methode festlegt:
        /// • "simple": liefert Variablenwerte gemäß der Darstellung
        /// "simple" in Kapitel "Unterstützte Datentypen (Seite 162)"
        /// • "raw": liefert Variablenwerte gemäß der Darstellung "raw"
        /// in Kapitel "Unterstützte Datentypen"</param>
        /// <returns>ApiPlcProgramReadResponse: object with the value for the variables value to be read</returns>
        Task<ApiResultResponse<T>> PlcProgramReadAsync<T>(ApiPlcProgramData var, ApiPlcProgramReadOrWriteMode? plcProgramReadMode = null);
        /// <summary>
        /// Send a PlcProgram.Read Request 
        /// </summary>
        /// <param name="var">Name of the variable to be read
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramReadMode">
        /// "simple" will get the variable values according to the presentation of the manual - "supported Datatypes"
        /// "raw" : will get the variable values according to the presentation of the manual "raw"
        ///
        /// Aufzählung, die das Antwortformat für diese Methode festlegt:
        /// • "simple": liefert Variablenwerte gemäß der Darstellung
        /// "simple" in Kapitel "Unterstützte Datentypen (Seite 162)"
        /// • "raw": liefert Variablenwerte gemäß der Darstellung "raw"
        /// in Kapitel "Unterstützte Datentypen"</param>
        /// <returns>ApiPlcProgramReadResponse: object with the value for the variables value to be read</returns>
        Task<ApiResultResponse<T>> PlcProgramReadAsync<T>(string var, ApiPlcProgramReadOrWriteMode? plcProgramReadMode = null);
        /// <summary>
        /// Send a PlcProgram.Write Request 
        /// This function will build up the name with quotes from the parents given with the ApiPlcProgramDataand call PlcProgramWrite
        /// </summary>
        /// <param name="var">
        /// Name of the variable to be read
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramWriteMode"></param>
        /// <param name="valueToBeSet"></param>
        /// <returns>true to indicate success</returns>
        Task<ApiTrueOnSuccessResponse> PlcProgramWriteAsync(ApiPlcProgramData var, object valueToBeSet, ApiPlcProgramReadOrWriteMode? plcProgramWriteMode = null);
        /// <summary>
        /// Send a PlcProgram.Write Request 
        /// </summary>
        /// <param name="var">
        /// Name of the variable to be read
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramWriteMode"></param>
        /// <param name="valueToBeSet"></param>
        /// <returns>true to indicate success</returns>
        Task<ApiTrueOnSuccessResponse> PlcProgramWriteAsync(string var, object valueToBeSet, ApiPlcProgramReadOrWriteMode? plcProgramWriteMode = null);
        /// <summary>
        /// Send a Plc.ReadOperatingMode Request 
        /// </summary>
        /// <returns>The current Plc OperatingMode</returns>
        Task<ApiReadOperatingModeResponse> PlcReadOperatingModeAsync();
        /// <summary>
        /// Send a Plc.RequestChangeOperatingMode Request 
        /// Method to change the plc operating mode
        /// valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.
        /// </summary>
        /// <returns>valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.</returns>
        Task<ApiTrueOnSuccessResponse> PlcRequestChangeOperatingModeAsync(ApiPlcOperatingMode plcOperatingMode);
        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// </summary>
        /// <param name="apiRequest">Api Request to send to the plc</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>string: response from thePLC</returns>
        Task<string> SendPostRequestAsync(IApiRequest apiRequest, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// </summary>
        /// <param name="apiRequest">Api Request to send to the plc</param>
        /// <returns>string: response from thePLC</returns>
        string SendPostRequest(IApiRequest apiRequest);
        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <param name="apiRequestWithIntId">Api Request to send to the plc (Json Serialized - null properties are deleted)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>string: response from thePLC</returns>
        Task<string> SendPostRequestAsync(IApiRequestIntId apiRequestWithIntId, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <param name="apiRequestWithIntId">Api Request to send to the plc (Json Serialized - null properties are deleted)</param>
        /// <returns>string: response from thePLC</returns>
        string SendPostRequest(IApiRequestIntId apiRequestWithIntId);
        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <param name="apiRequestString">further information about the Api requeest the user tried to send (or was trying to send)</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>string: response from thePLC</returns>
        Task<string> SendPostRequestAsync(string apiRequestString, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// only use this function if you know how to build up apiRequests on your own!
        /// will remove those Params that have the value Null and send the request using the HttpClient.
        /// </summary>
        /// <returns>string: response from thePLC</returns>
        string SendPostRequest(string apiRequestString);
        /// <summary>
        /// Function to send the ByteArrayContent for a Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="data">ByteArray that should be sent to the plc Ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        Task UploadTicketAsync(string ticketId, ByteArrayContent data);
        /// <summary>
        /// Function to Read and send the ByteArrayContent for a file with the Ticketing Endpoint Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="pathToFile">File Bytes will be Read and saved into ByteArrayContent - then sent to the ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        Task UploadTicketAsync(string ticketId, string pathToFile);
        /// <summary>
        /// Send a WebApp.Browse Request 
        /// </summary>
        /// <param name="webAppName">webapp name in case only one is requested</param>
        /// <returns>ApiWebAppBrowseResponse: Containing WebAppBrowseResult: Max_Applications:uint, Applications: Array of ApiWebAppdata</returns>
        Task<ApiWebAppBrowseResponse> WebAppBrowseAsync(string webAppName = null);
        /// <summary>
        /// Send a WebApp.Browse Request 
        /// </summary>
        /// <param name="webAppData">webappdata that should be requested</param>
        /// <returns>ApiWebAppBrowseResponse: Containing WebAppBrowseResult: Max_Applications:uint, Applications: Array of ApiWebAppdata containing one element: the webappdata that has been requested</returns>
        Task<ApiWebAppBrowseResponse> WebAppBrowseAsync(ApiWebAppData webAppData);
        /// <summary>
        /// Send a WebApp.BrowseResources Request 
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webApp">WebApp.Name to browse resources of</param>
        /// <param name="resourceName">If given only that resource will be inside the array (in case it exists)</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,Resources</returns>
        Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(ApiWebAppData webApp, string resourceName = null);
        /// <summary>
        /// Send a WebApp.BrowseResources Request 
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webAppName">WebApp Name to browse resources of</param>
        /// <param name="resource">resource.Name to browse for</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,Resources</returns>
        Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.BrowseResources Request 
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webApp">webApp.Name to browse resources of</param>
        /// <param name="resource">resource.Name to browse for</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.BrowseResources Request 
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webAppName">WebApp name to browse resources of</param>
        /// <param name="resourceName">If given only that resource will be inside the array (in case it exists)</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,Resources</returns>
        Task<ApiWebAppBrowseResourcesResponse> WebAppBrowseResourcesAsync(string webAppName, string resourceName = null);
        /// <summary>
        /// Send a WebApp.Create Request 
        /// </summary>
        /// <param name="webApp">containing information about name and state for the app to be created</param>
        /// <returns>true to indicate success</returns>
        Task<ApiTrueOnSuccessResponse> WebAppCreateAsync(ApiWebAppData webApp);
        /// <summary>
        /// Send a WebApp.Create Request 
        /// </summary>
        /// <param name="webAppName">webapp name for the app to be created</param>
        /// <param name="apiWebAppState">optional parameter: state the webapp should be in</param>
        /// <returns>true to indicate success</returns>
        Task<ApiTrueOnSuccessResponse> WebAppCreateAsync(string webAppName, ApiWebAppState? apiWebAppState = null);
        /// <summary>
        /// Send a WebApp.CreateResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to create the resource on</param>
        /// <param name="resource">resource containing all the information: 
        /// Name:           name to be created with (typically provided with extension)
        /// Media_type:     resource media type - see MIMEType.mapping
        /// Last_modified:  Be sure to provide the RFC3339 format!
        /// Visibility:     resource visibility (protect your confidential data)
        /// Etag:           you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        Task<ApiTicketIdResponse> WebAppCreateResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.CreateResource Request 
        /// </summary>
        /// <param name="webAppName">name of the webapp to create the resource on</param>
        /// <param name="resource">resource containing all the information: 
        /// Name:           name to be created with (typically provided with extension)
        /// Media_type:     resource media type - see MIMEType.mapping
        /// Last_modified:  Be sure to provide the RFC3339 format!
        /// Visibility:     resource visibility (protect your confidential data)
        /// Etag:           you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        Task<ApiTicketIdResponse> WebAppCreateResourceAsync(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.CreateResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to create the resource on</param>
        /// <param name="resourceName">resource name to be created with (typically provided with extension)</param>
        /// <param name="media_type">resource media type - see MIMEType.mapping</param>
        /// <param name="last_modified">Be sure to provide the RFC3339 format!</param>
        /// <param name="apiWebAppResourceVisibility">resource visibility (protect your confidential data)</param>
        /// <param name="etag">you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        Task<ApiTicketIdResponse> WebAppCreateResourceAsync(ApiWebAppData webApp, string resourceName, string media_type, string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null);
        /// <summary>
        /// Send a WebApp.CreateResource Request 
        /// </summary>
        /// <param name="webAppName">name of the webapp to create the resource on</param>
        /// <param name="resourceName">resource name to be created with (typically provided with extension)</param>
        /// <param name="media_type">resource media type - see MIMEType.mapping</param>
        /// <param name="last_modified">Be sure to provide the RFC3339 format!</param>
        /// <param name="apiWebAppResourceVisibility">resource visibility (protect your confidential data)</param>
        /// <param name="etag">you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        Task<ApiTicketIdResponse> WebAppCreateResourceAsync(string webAppName, string resourceName, string media_type, string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null);
        /// <summary>
        /// Send a WebApp.Delete Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to delete</param>
        /// <returns>true to indicate success</returns>
        Task<ApiTrueOnSuccessResponse> WebAppDeleteAsync(ApiWebAppData webApp);
        /// <summary>
        /// Send a WebApp.Delete Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp to delete</param>
        /// <returns>true to indicate success</returns>
        Task<ApiTrueOnSuccessResponse> WebAppDeleteAsync(string webAppName);
        /// <summary>
        /// Send a WebApp.DeleteRespource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.DeleteRespource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.DeleteRespource Request 
        /// </summary>
        /// <param name="webApp">webapp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(ApiWebAppData webApp, string resourceName);
        /// <summary>
        /// Send a WebApp.DeleteRespource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        Task<ApiTrueOnSuccessResponse> WebAppDeleteResourceAsync(string webAppName, string resourceName);
        /// <summary>
        /// Send a WebApp.DownloadResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.DownloadResource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.DownloadResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(ApiWebAppData webApp, string resourceName);
        /// <summary>
        /// Send a WebApp.DownloadResource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        Task<ApiTicketIdResponse> WebAppDownloadResourceAsync(string webAppName, string resourceName);
        /// <summary>
        /// Send a WebApp.Rename Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that should be renamed</param>
        /// <param name="newWebAppName">New name for the WebApp</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the given WebApp that has the change:
        /// name which equals the newname</returns>
        Task<ApiTrueWithWebAppResponse> WebAppRenameAsync(ApiWebAppData webApp, string newWebAppName);
        /// <summary>
        /// Send a WebApp.Rename Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that should be renamed</param>
        /// <param name="newWebAppName">New name for the WebApp</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a WebApp that only has the information: 
        /// name which equals the newname</returns>
        Task<ApiTrueWithWebAppResponse> WebAppRenameAsync(string webAppName, string newWebAppName);
        /// <summary>
        /// Send a WebApp.RenameResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the Resource given that has the following change: 
        /// name which equals the newname</returns>
        Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource, string newResourceName);
        /// <summary>
        /// Send a WebApp.RenameResource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the Resource given that has the following change: 
        /// name which equals the newname</returns>
        Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(string webAppName, ApiWebAppResource resource, string newResourceName);
        /// <summary>
        /// Send a WebApp.RenameResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a Resource that only has the information: 
        /// name which equals the newname</returns>
        Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(ApiWebAppData webApp, string resourceName, string newResourceName);
        /// <summary>
        /// Send a WebApp.RenameResource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a Resource that only has the information: 
        /// name which equals the newname</returns>
        Task<ApiTrueWithResourceResponse> WebAppRenameResourceAsync(string webAppName, string resourceName, string newResourceName);
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the default page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Default_Page:   which equals the resource.Name
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the default page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Default_Page:   which equals the resourceName
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the default page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Default_Page:   which equals the resourceName
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(ApiWebAppData webApp, string resourceName);
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the default page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Default_Page:   which equals the resourceName
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetDefaultPageAsync(string webAppName, string resourceName);
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:                   which equals webAppName
        /// Not_authorized_page:    which equals the resource.Name
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(ApiWebAppData webApp, string resourceName);
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:                   which equals webAppName
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetNotAuthorizedPageAsync(string webAppName, string resourceName);
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not found page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_found_page:   which equals the resource.Name
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not found page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Not_found_page: which equals the resource.Name
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not found page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_found_page:   which equals the resourceName
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(ApiWebAppData webApp, string resourceName);
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not found page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Not_found_page: which equals the resourceName
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetNotFoundPageAsync(string webAppName, string resourceName);
        /// <summary>
        /// Send a WebApp.SetResourceETag Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Etag: which equals the newEtagValue
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(ApiWebAppData webApp, ApiWebAppResource resource, string newETagValue);
        /// <summary>
        /// Send a WebApp.SetResourceETag Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Etag: which equals the newEtagValue
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(string webAppName, ApiWebAppResource resource, string newETagValue);
        /// <summary>
        /// Send a WebApp.SetResourceETag Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Etag: which equals the newEtagValue
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(ApiWebAppData webApp, string resourceName, string newETagValue);
        /// <summary>
        /// Send a WebApp.SetResourceETag Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Etag: which equals the newEtagValue
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceETagAsync(string webAppName, string resourceName, string newETagValue);
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// MediaType: which equals the newMediaType
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(ApiWebAppData webApp, ApiWebAppResource resource, string newMediaType);
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// MediaType: which equals the newMediaType
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(string webAppName, ApiWebAppResource resource, string newMediaType);
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:       which equals the resourceName
        /// MediaType:  which equals the newMediaType
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(ApiWebAppData webApp, string resourceName, string newMediaType);
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:       which equals the resourceName
        /// MediaType:  which equals the newMediaType
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceMediaTypeAsync(string webAppName, string resourceName, string newMediaType);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, ApiWebAppResource resource, string newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, ApiWebAppResource resource, string newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, string resourceName, string newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, string resourceName, string newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, string resourceName, DateTime newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(string webAppName, ApiWebAppResource resource, DateTime newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, string resourceName, DateTime newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceModificationTimeAsync(ApiWebAppData webApp, ApiWebAppResource resource, DateTime newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(ApiWebAppData webApp, ApiWebAppResource resource, ApiWebAppResourceVisibility newResourceVisibility);
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(string webAppName, ApiWebAppResource resource, ApiWebAppResourceVisibility newResourceVisibility);
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(ApiWebAppData webApp, string resourceName, ApiWebAppResourceVisibility newResourceVisibility);
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Visibility: which equals the newVisibility
        /// </returns>
        Task<ApiTrueWithResourceResponse> WebAppSetResourceVisibilityAsync(string webAppName, string resourceName, ApiWebAppResourceVisibility newResourceVisibility);
        /// <summary>
        /// Send a WebApp.SetState Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that state should be set for</param>
        /// <param name="apiWebAppState">State the WebApp should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// State: which equals the state
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetStateAsync(ApiWebAppData webApp, ApiWebAppState apiWebAppState);

        /// <summary>
        /// Send a WebApp.SetState Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the state should be set for</param>
        /// <param name="apiWebAppState">State the WebApp should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:  which equals the webAppName
        /// State: which equals the state
        /// </returns>
        Task<ApiTrueWithWebAppResponse> WebAppSetStateAsync(string webAppName, ApiWebAppState apiWebAppState);

        /// <summary>
        /// Send a WebServer.ReadDefaultPage request
        /// </summary>
        /// <returns>Returns an ApiWebServerGetReadDefaultPageResponse object</returns>
        ApiWebServerGetReadDefaultPageResponse WebServerGetReadDefaultPage();

        /// <summary>
        /// Send a WebServer.ReadDefaultPage request
        /// </summary>
        /// <returns>Returns an ApiWebServerGetReadDefaultPageResponse object</returns>
        Task<ApiWebServerGetReadDefaultPageResponse> WebServerGetReadDefaultPageAsync();

        /// <summary>
        /// Send a WebServer.SetDefaultPage request
        /// </summary>
        /// <param name="defaultPage">Name of the default page</param>
        /// <returns>Returns a TrueOnSuccessResponse</returns>
        Task<ApiTrueOnSuccessResponse> WebServerSetDefaultPageAsync(string defaultPage);

        /// <summary>
        /// Send a WebServer.SetDefaultPage request
        /// </summary>
        /// <param name="defaultPage">Name of the default page</param>
        /// <returns>Returns a TrueOnSuccessResponse</returns>
        ApiTrueOnSuccessResponse WebServerSetDefaultPage(string defaultPage);

        /// <summary>
        /// Send an ApiBulk Request
        /// </summary>
        /// <returns>an array of ApiResponses</returns>
        /// <param name="apiRequests">api Requests to send</param>
        ApiBulkResponse ApiBulk(IEnumerable<IApiRequest> apiRequests);
        /// <summary>
        /// Send an Api.Browse Request
        /// </summary>
        /// <returns>An Array of ApiClass (and Id,Jsonrpc)</returns>
        ApiArrayOfApiClassResponse ApiBrowse();

        /// <summary>
        /// Send an Api.CloseTicket Request  
        /// </summary>
        /// <param name="ticketId">ticket id (28 chars)</param>
        /// <returns>True to indicate Success</returns>
        ApiTrueOnSuccessResponse ApiCloseTicket(string ticketId);
        /// <summary>
        /// Send an Api.CloseTicket Request  
        /// </summary>
        /// <param name="ticket">ticket containing ticket id (28 chars)</param>
        /// <returns>True to indicate Success</returns>
        ApiTrueOnSuccessResponse ApiCloseTicket(ApiTicket ticket);
        /// <summary>
        /// Send an Api.GetCertificateUrl Request 
        /// </summary>
        /// <returns>ApiSingleStringResponse that contians the URL to the certificate</returns>
        ApiSingleStringResponse ApiGetCertificateUrl();
        /// <summary>
        /// Send an Api.GetPermissions Request  
        /// </summary>
        /// <returns>Array of ApiClass (in this case permissions)</returns>
        ApiArrayOfApiClassResponse ApiGetPermissions();
        /// <summary>
        /// Send a Api.Login Request 
        /// </summary>
        /// <param name="userName">Username to login with</param>
        /// <param name="password">Password for the user to login with</param>
        /// <param name="includeWebApplicationCookie">Used to determine wether or not a WebApplicationCookie should be included in the Response (Result)</param>
        /// <returns>ApiLoginResponse: contains ApiTokenResult: Token(auth token string) and if requested Web_application_cookie</returns>
        ApiLoginResponse ApiLogin(string userName, string password, bool? includeWebApplicationCookie = null);
        /// <summary>
        /// Send an Api.Logout Request 
        /// </summary>
        /// <returns>True to indicate success</returns>
        ApiTrueOnSuccessResponse ApiLogout();
        /// <summary>
        /// Send an Api.Ping Request 
        /// </summary>
        /// <returns>ApiSingleStringResponse - an Id that'll stay the same for the users session</returns>
        ApiSingleStringResponse ApiPing();
        /// <summary>
        /// Send an Api.Version Request 
        /// </summary>
        /// <returns>a double that contains the value for the current ApiVersion</returns>
        ApiDoubleResponse ApiVersion();
        /// <summary>
        /// Function to get the ByteArray Requested by a Ticket (e.g. DownloadResource)
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=+ticketId</param>
        /// <returns>Bytearray given from the PLC</returns>
        byte[] DownloadTicket(string ticketId);
        /// <summary>
        /// Send a Project.ReadLanguages Request
        /// </summary>
        /// <returns>Languages Response containing a list of languages</returns>
        Task<ApiReadLanguagesResponse> ProjectReadLanguagesAsync();
        /// <summary>
        /// Send a Project.ReadLanguages Request
        /// </summary>
        /// <returns>Languages Response containing a list of languages</returns>
        ApiReadLanguagesResponse ProjectReadLanguages();
        /// <summary>
        /// Perform a service data download on the corresponding module with hwid
        /// </summary>
        /// <param name="hwid">The HWID of a node (module) for which a service data file can be downloaded</param>
        /// <returns>Ticket to use for downloading the service data</returns>
        Task<ApiTicketIdResponse> ModulesDownloadServiceDataAsync(ApiPlcHwId hwid);
        /// <summary>
        /// Perform a service data download on the corresponding module with hwid
        /// </summary>
        /// <param name="hwid">The HWID of a node (module) for which a service data file can be downloaded</param>
        /// <returns>Ticket to use for downloading the service data</returns>
        ApiTicketIdResponse ModulesDownloadServiceData(ApiPlcHwId hwid);
        /// <summary>
        /// Send a PlcProgram.Browse Request 
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
        ApiPlcProgramBrowseResponse PlcProgramBrowse(ApiPlcProgramBrowseMode plcProgramBrowseMode, string var = null);
        /// <summary>
        /// Send a PlcProgram.Browse Request 
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
        ApiPlcProgramBrowseResponse PlcProgramBrowse(ApiPlcProgramBrowseMode plcProgramBrowseMode, ApiPlcProgramData var);

        /// <summary>
        /// Send a PlcProgram.Browse request for the code blocks.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>ApiPlcProgramBrowseCodeBlocksResponse: A collection of ApiPlcProgramBrowseCodeBlocksData objects.</returns>
        Task<ApiPlcProgramBrowseCodeBlocksResponse> PlcProgramBrowseCodeBlocksAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Send a PlcProgram.Browse request for the code blocks.
        /// </summary>
        /// <returns>ApiPlcProgramBrowseCodeBlocksResponse: A collection of ApiPlcProgramBrowseCodeBlocksData objects.</returns>
        ApiPlcProgramBrowseCodeBlocksResponse PlcProgramBrowseCodeBlocks();

        /// <summary>
        /// Send a PlcProgram.DownloadProfilingData request.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>ApiSingleStringResponse: Object containing the ticket ID for the data download.</returns>
        Task<ApiSingleStringResponse> PlcProgramDownloadProfilingDataAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Send a PlcProgram.DownloadProfilingData request.
        /// </summary>
        /// <returns>ApiSingleStringResponse: Object containing the ticket ID for the data download.</returns>
        ApiSingleStringResponse PlcProgramDownloadProfilingData();

        /// <summary>
        /// Send a PlcProgram.Read Request 
        /// </summary>
        /// <param name="var">Name of the variable to be read
        /// 
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramReadMode"></param>        
        /// "simple" will get the variable values according to the presentation of the manual - "supported Datatypes"
        /// "raw" : will get the variable values according to the presentation of the manual "raw"
        ///
        /// Aufzählung, die das Antwortformat für diese Methode festlegt:
        /// • "simple": liefert Variablenwerte gemäß der Darstellung
        /// "simple" in Kapitel "Unterstützte Datentypen (Seite 162)"
        /// • "raw": liefert Variablenwerte gemäß der Darstellung "raw"
        /// in Kapitel "Unterstützte Datentypen"
        /// <returns>ApiPlcProgramReadResponse: object with the value for the variables value to be read</returns>
        ApiResultResponse<T> PlcProgramRead<T>(ApiPlcProgramData var, ApiPlcProgramReadOrWriteMode? plcProgramReadMode = null);
        /// <summary>
        /// Send a PlcProgram.Read Request 
        /// </summary>
        /// <param name="var">Name of the variable to be read
        /// 
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramReadMode"></param>        
        /// "simple" will get the variable values according to the presentation of the manual - "supported Datatypes"
        /// "raw" : will get the variable values according to the presentation of the manual "raw"
        ///
        /// Aufzählung, die das Antwortformat für diese Methode festlegt:
        /// • "simple": liefert Variablenwerte gemäß der Darstellung
        /// "simple" in Kapitel "Unterstützte Datentypen (Seite 162)"
        /// • "raw": liefert Variablenwerte gemäß der Darstellung "raw"
        /// in Kapitel "Unterstützte Datentypen"
        /// <returns>ApiPlcProgramReadResponse: object with the value for the variables value to be read</returns>
        ApiResultResponse<T> PlcProgramRead<T>(string var, ApiPlcProgramReadOrWriteMode? plcProgramReadMode = null);
        /// <summary>
        /// Send a PlcProgram.Write Request 
        /// This function will build up the name with quotes from the parents given with the ApiPlcProgramDataand call PlcProgramWrite
        /// </summary>
        /// <param name="var">
        /// Name of the variable to be read
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramWriteMode"></param>
        /// <param name="valueToBeSet"></param>
        /// <returns>true to indicate success</returns>
        ApiTrueOnSuccessResponse PlcProgramWrite(ApiPlcProgramData var, object valueToBeSet, ApiPlcProgramReadOrWriteMode? plcProgramWriteMode = null);
        /// <summary>
        /// Send a PlcProgram.Write Request 
        /// </summary>
        /// <param name="var">
        /// Name of the variable to be read
        /// Name der zu lesenden Variable</param>
        /// <param name="plcProgramWriteMode"></param>
        /// <param name="valueToBeSet"></param>
        /// <returns>true to indicate success</returns>
        ApiTrueOnSuccessResponse PlcProgramWrite(string var, object valueToBeSet, ApiPlcProgramReadOrWriteMode? plcProgramWriteMode = null);
        /// <summary>
        /// Send a Plc.ReadOperatingMode Request 
        /// </summary>
        /// <returns>The current Plc OperatingMode</returns>
        ApiReadOperatingModeResponse PlcReadOperatingMode();
        /// <summary>
        /// Send a Plc.RequestChangeOperatingMode Request 
        /// Method to change the plc operating mode
        /// valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.
        /// </summary>
        /// <returns>valid plcOperatingModes are: "run", "stop" - others will lead to an invalid params exception.</returns>
        ApiTrueOnSuccessResponse PlcRequestChangeOperatingMode(ApiPlcOperatingMode plcOperatingMode);
        /// <summary>
        /// Send a Plc.ReadModeSelectorState request
        /// </summary>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <returns>Mode Selector state</returns>
        Task<ApiPlcReadModeSelectorStateResponse> PlcReadModeSelectorStateAsync(ApiPlcRedundancyId rhid);
        /// <summary>
        /// Send a Plc.ReadModeSelectorState request
        /// </summary>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <returns>Mode Selector state</returns>
        ApiPlcReadModeSelectorStateResponse PlcReadModeSelectorState(ApiPlcRedundancyId rhid);
        /// <summary>
        /// Function to send the ByteArrayContent for a Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="data">ByteArray that should be sent to the plc Ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        void UploadTicket(string ticketId, ByteArrayContent data);
        /// <summary>
        /// Function to Read and send the ByteArrayContent for a file with the Ticketing Endpoint Ticket (e.g. CreateResource)
        /// MediaTypeHeaderValue: application/octet-stream
        /// </summary>
        /// <param name="ticketId">Id of the Ticket - will be used to send the request to the endpoint /api/ticket?id=ticketId</param>
        /// <param name="pathToFile">File Bytes will be Read and saved into ByteArrayContent - then sent to the ticketing Endpoint</param>
        /// <returns>Task/void</returns>
        void UploadTicket(string ticketId, string pathToFile);
        /// <summary>
        /// Send a WebApp.Browse Request 
        /// </summary>
        /// <param name="webAppName">webapp name in case only one is requested</param>
        /// <returns>ApiWebAppBrowseResponse: Containing WebAppBrowseResult: Max_Applications:uint, Applications: Array of ApiWebAppdata</returns>
        ApiWebAppBrowseResponse WebAppBrowse(string webAppName = null);
        /// <summary>
        /// Send a WebApp.Browse Request 
        /// </summary>
        /// <param name="webAppData">webappdata that should be requested</param>
        /// <returns>ApiWebAppBrowseResponse: Containing WebAppBrowseResult: Max_Applications:uint, Applications: Array of ApiWebAppdata containing one element: the webappdata that has been requested</returns>
        ApiWebAppBrowseResponse WebAppBrowse(ApiWebAppData webAppData);
        /// <summary>
        /// Send a WebApp.BrowseResources Request 
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webApp">WebApp.Name to browse resources of</param>
        /// <param name="resourceName">If given only that resource will be inside the array (in case it exists)</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,Resources</returns>
        ApiWebAppBrowseResourcesResponse WebAppBrowseResources(ApiWebAppData webApp, string resourceName = null);
        /// <summary>
        /// Send a WebApp.BrowseResources Request 
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webAppName">WebApp Name to browse resources of</param>
        /// <param name="resource">resource.Name to browse for</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,Resources</returns>
        ApiWebAppBrowseResourcesResponse WebAppBrowseResources(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.BrowseResources Request 
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webApp">webApp.Name to browse resources of</param>
        /// <param name="resource">resource.Name to browse for</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,Resources:Array of ApiWebAppResource (only 1 if one is requested)</returns>
        ApiWebAppBrowseResourcesResponse WebAppBrowseResources(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.BrowseResources Request 
        /// Will return the Api Response "straight away"
        /// A user can use the List of ApiWebAppResources to set those to an ApiWebAppData (care to also add those who are protected to the protected resources in case you want to do that)
        /// </summary>
        /// <param name="webAppName">WebApp name to browse resources of</param>
        /// <param name="resourceName">If given only that resource will be inside the array (in case it exists)</param>
        /// <returns>ApiWebAppBrowseResourcesResponse:containing ApiWebAppBrowseResourcesResult: Max_Resources:uint,Resources</returns>
        ApiWebAppBrowseResourcesResponse WebAppBrowseResources(string webAppName, string resourceName = null);
        /// <summary>
        /// Send a WebApp.Create Request 
        /// </summary>
        /// <param name="webApp">containing information about name and state for the app to be created</param>
        /// <returns>true to indicate success</returns>
        ApiTrueOnSuccessResponse WebAppCreate(ApiWebAppData webApp);
        /// <summary>
        /// Send a WebApp.Create Request 
        /// </summary>
        /// <param name="webAppName">webapp name for the app to be created</param>
        /// <param name="apiWebAppState">optional parameter: state the webapp should be in</param>
        /// <returns>true to indicate success</returns>
        ApiTrueOnSuccessResponse WebAppCreate(string webAppName, ApiWebAppState? apiWebAppState = null);
        /// <summary>
        /// Send a WebApp.CreateResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to create the resource on</param>
        /// <param name="resource">resource containing all the information: 
        /// Name:           name to be created with (typically provided with extension)
        /// Media_type:     resource media type - see MIMEType.mapping
        /// Last_modified:  Be sure to provide the RFC3339 format!
        /// Visibility:     resource visibility (protect your confidential data)
        /// Etag:           you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        ApiTicketIdResponse WebAppCreateResource(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.CreateResource Request 
        /// </summary>
        /// <param name="webAppName">name of the webapp to create the resource on</param>
        /// <param name="resource">resource containing all the information: 
        /// Name:           name to be created with (typically provided with extension)
        /// Media_type:     resource media type - see MIMEType.mapping
        /// Last_modified:  Be sure to provide the RFC3339 format!
        /// Visibility:     resource visibility (protect your confidential data)
        /// Etag:           you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        ApiTicketIdResponse WebAppCreateResource(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.CreateResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to create the resource on</param>
        /// <param name="resourceName">resource name to be created with (typically provided with extension)</param>
        /// <param name="media_type">resource media type - see MIMEType.mapping</param>
        /// <param name="last_modified">Be sure to provide the RFC3339 format!</param>
        /// <param name="apiWebAppResourceVisibility">resource visibility (protect your confidential data)</param>
        /// <param name="etag">you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        ApiTicketIdResponse WebAppCreateResource(ApiWebAppData webApp, string resourceName, string media_type, string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null);
        /// <summary>
        /// Send a WebApp.CreateResource Request 
        /// </summary>
        /// <param name="webAppName">name of the webapp to create the resource on</param>
        /// <param name="resourceName">resource name to be created with (typically provided with extension)</param>
        /// <param name="media_type">resource media type - see MIMEType.mapping</param>
        /// <param name="last_modified">Be sure to provide the RFC3339 format!</param>
        /// <param name="apiWebAppResourceVisibility">resource visibility (protect your confidential data)</param>
        /// <param name="etag">you can provide an etag as identification,... for your resource</param>
        /// <returns>TicketId for the Ticketing Endpoint to perform the Upload on</returns>
        ApiTicketIdResponse WebAppCreateResource(string webAppName, string resourceName, string media_type, string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null);
        /// <summary>
        /// Send a WebApp.Delete Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp to delete</param>
        /// <returns>true to indicate success</returns>
        ApiTrueOnSuccessResponse WebAppDelete(ApiWebAppData webApp);
        /// <summary>
        /// Send a WebApp.Delete Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp to delete</param>
        /// <returns>true to indicate success</returns>
        ApiTrueOnSuccessResponse WebAppDelete(string webAppName);
        /// <summary>
        /// Send a WebApp.DeleteRespource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        ApiTrueOnSuccessResponse WebAppDeleteResource(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.DeleteRespource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        ApiTrueOnSuccessResponse WebAppDeleteResource(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.DeleteRespource Request 
        /// </summary>
        /// <param name="webApp">webapp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        ApiTrueOnSuccessResponse WebAppDeleteResource(ApiWebAppData webApp, string resourceName);
        /// <summary>
        /// Send a WebApp.DeleteRespource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to delete</param>
        /// <returns>true to indicate success</returns>
        ApiTrueOnSuccessResponse WebAppDeleteResource(string webAppName, string resourceName);
        /// <summary>
        /// Send a WebApp.DownloadResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        ApiTicketIdResponse WebAppDownloadResource(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.DownloadResource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        ApiTicketIdResponse WebAppDownloadResource(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.DownloadResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        ApiTicketIdResponse WebAppDownloadResource(ApiWebAppData webApp, string resourceName);
        /// <summary>
        /// Send a WebApp.DownloadResource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource to download</param>
        /// <returns>Ticket id for Ticketing Endpoint to trigger the download on</returns>
        ApiTicketIdResponse WebAppDownloadResource(string webAppName, string resourceName);
        /// <summary>
        /// Send a WebApp.Rename Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that should be renamed</param>
        /// <param name="newWebAppName">New name for the WebApp</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the given WebApp that has the change:
        /// name which equals the newname</returns>
        ApiTrueWithWebAppResponse WebAppRename(ApiWebAppData webApp, string newWebAppName);
        /// <summary>
        /// Send a WebApp.Rename Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that should be renamed</param>
        /// <param name="newWebAppName">New name for the WebApp</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a WebApp that only has the information: 
        /// name which equals the newname</returns>
        ApiTrueWithWebAppResponse WebAppRename(string webAppName, string newWebAppName);
        /// <summary>
        /// Send a WebApp.RenameResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the Resource given that has the following change: 
        /// name which equals the newname</returns>
        ApiTrueWithResourceResponse WebAppRenameResource(ApiWebAppData webApp, ApiWebAppResource resource, string newResourceName);
        /// <summary>
        /// Send a WebApp.RenameResource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the Resource given that has the following change: 
        /// name which equals the newname</returns>
        ApiTrueWithResourceResponse WebAppRenameResource(string webAppName, ApiWebAppResource resource, string newResourceName);
        /// <summary>
        /// Send a WebApp.RenameResource Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a Resource that only has the information: 
        /// name which equals the newname</returns>
        ApiTrueWithResourceResponse WebAppRenameResource(ApiWebAppData webApp, string resourceName, string newResourceName);
        /// <summary>
        /// Send a WebApp.RenameResource Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that contains the resource</param>
        /// <param name="resourceName">Name of the resource that should be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a Resource that only has the information: 
        /// name which equals the newname</returns>
        ApiTrueWithResourceResponse WebAppRenameResource(string webAppName, string resourceName, string newResourceName);
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the default page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Default_Page:   which equals the resource.Name
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetDefaultPage(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the default page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Default_Page:   which equals the resourceName
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetDefaultPage(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the default page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Default_Page:   which equals the resourceName
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetDefaultPage(ApiWebAppData webApp, string resourceName);
        /// <summary>
        /// Send a WebApp.SetDefaultPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the default page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps default page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Default_Page:   which equals the resourceName
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetDefaultPage(string webAppName, string resourceName);
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetNotAuthorizedPage(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:                   which equals webAppName
        /// Not_authorized_page:    which equals the resource.Name
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetNotAuthorizedPage(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetNotAuthorizedPage(ApiWebAppData webApp, string resourceName);
        /// <summary>
        /// Send a WebApp.SetNotAuthorizedPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not authorized page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not authorized page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:                   which equals webAppName
        /// Not_authorized_page:   which equals the resourceName
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetNotAuthorizedPage(string webAppName, string resourceName);
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not found page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_found_page:   which equals the resource.Name
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetNotFoundPage(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not found page should be set for</param>
        /// <param name="resource">resource.Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Not_found_page: which equals the resource.Name
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetNotFoundPage(string webAppName, ApiWebAppResource resource);
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that the not found page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// Not_found_page:   which equals the resourceName
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetNotFoundPage(ApiWebAppData webApp, string resourceName);
        /// <summary>
        /// Send a WebApp.SetNotFoundPage Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the not found page should be set for</param>
        /// <param name="resourceName">Name of the resource that should be the webapps not found page</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:           which equals the webAppName
        /// Not_found_page: which equals the resourceName
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetNotFoundPage(string webAppName, string resourceName);
        /// <summary>
        /// Send a WebApp.SetResourceETag Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Etag: which equals the newEtagValue
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceETag(ApiWebAppData webApp, ApiWebAppResource resource, string newETagValue);
        /// <summary>
        /// Send a WebApp.SetResourceETag Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Etag: which equals the newEtagValue
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceETag(string webAppName, ApiWebAppResource resource, string newETagValue);
        /// <summary>
        /// Send a WebApp.SetResourceETag Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Etag: which equals the newEtagValue
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceETag(ApiWebAppData webApp, string resourceName, string newETagValue);
        /// <summary>
        /// Send a WebApp.SetResourceETag Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the etag should be set for </param>
        /// <param name="newETagValue">Etag value the resource should have
        /// new value for the resource etag - "" for "null"/"no etag", also null can be given for null!</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Etag: which equals the newEtagValue
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceETag(string webAppName, string resourceName, string newETagValue);
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// MediaType: which equals the newMediaType
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceMediaType(ApiWebAppData webApp, ApiWebAppResource resource, string newMediaType);
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// MediaType: which equals the newMediaType
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceMediaType(string webAppName, ApiWebAppResource resource, string newMediaType);
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:       which equals the resourceName
        /// MediaType:  which equals the newMediaType
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceMediaType(ApiWebAppData webApp, string resourceName, string newMediaType);
        /// <summary>
        /// Send a WebApp.SetResourceMediaType Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Media_type should be set for </param>
        /// <param name="newMediaType">MediaType value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:       which equals the resourceName
        /// MediaType:  which equals the newMediaType
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceMediaType(string webAppName, string resourceName, string newMediaType);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceModificationTime(ApiWebAppData webApp, ApiWebAppResource resource, string newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceModificationTime(string webAppName, ApiWebAppResource resource, string newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceModificationTime(ApiWebAppData webApp, string resourceName, string newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceModificationTime(string webAppName, string resourceName, string newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceModificationTime(string webAppName, string resourceName, DateTime newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceModificationTime(string webAppName, ApiWebAppResource resource, DateTime newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name:           which equals the resourceName
        /// Last_Modified:  which equals the newModificationTime
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceModificationTime(ApiWebAppData webApp, string resourceName, DateTime newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceModificationTime Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Last_modified should be set for </param>
        /// <param name="newModificationTime">ModificationTime - Last_modified value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Last_Modified: which equals the newModificationTime
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceModificationTime(ApiWebAppData webApp, ApiWebAppResource resource, DateTime newModificationTime);
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceVisibility(ApiWebAppData webApp, ApiWebAppResource resource, ApiWebAppResourceVisibility newResourceVisibility);
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resource">resource.Name of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the resource given containing only the change: 
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceVisibility(string webAppName, ApiWebAppResource resource, ApiWebAppResourceVisibility newResourceVisibility);
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Visibility: which equals the newResourceVisibility
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceVisibility(ApiWebAppData webApp, string resourceName, ApiWebAppResourceVisibility newResourceVisibility);
        /// <summary>
        /// Send a WebApp.SetResourceVisibility Request 
        /// </summary>
        /// <param name="webAppName">webAppName of the webapp that contains the resource</param>
        /// <param name="resourceName">resourceName of the resource that the Visibility should be set for </param>
        /// <param name="newResourceVisibility">Visibility value the resource should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a a resource containing only the information: 
        /// Name: which equals the resourceName
        /// Visibility: which equals the newVisibility
        /// </returns>
        ApiTrueWithResourceResponse WebAppSetResourceVisibility(string webAppName, string resourceName, ApiWebAppResourceVisibility newResourceVisibility);
        /// <summary>
        /// Send a WebApp.SetState Request 
        /// </summary>
        /// <param name="webApp">webApp.Name of the webapp that state should be set for</param>
        /// <param name="newApiWebAppState">State the WebApp should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and a copy of the webapp given containing only the change: 
        /// State: which equals the state
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetState(ApiWebAppData webApp, ApiWebAppState newApiWebAppState);
        /// <summary>
        /// Send a WebApp.SetState Request 
        /// </summary>
        /// <param name="webAppName">Name of the webapp that the state should be set for</param>
        /// <param name="newApiWebAppState">State the WebApp should have</param>
        /// <returns>This function will return the TrueOnSuccessResponse and webapp containing only the information: 
        /// Name:  which equals the webAppName
        /// State: which equals the state
        /// </returns>
        ApiTrueWithWebAppResponse WebAppSetState(string webAppName, ApiWebAppState newApiWebAppState);

        /// <summary>
        /// Send a Plc.ReadSystemTime Request
        /// </summary>
        /// <returns>Current Plc Utc System Time</returns>
        Task<ApiPlcReadSystemTimeResponse> PlcReadSystemTimeAsync();

        /// <summary>
        /// Send a Plc.ReadSystemTime Request
        /// </summary>
        /// <returns>Current Plc Utc System Time</returns>
        ApiPlcReadSystemTimeResponse PlcReadSystemTime();

        /// <summary>
        /// Send an Plc.SetSystemTime Request
        /// </summary>
        /// <param name="timestamp">The timestamp of the system time to be set</param>
        /// <returns>True if time was set successfully</returns>
        Task<ApiTrueOnSuccessResponse> PlcSetSystemTimeAsync(DateTime timestamp);
        /// <summary>
        /// Send an Plc.SetSystemTime Request
        /// </summary>
        /// <param name="timestamp">The timestamp of the system time to be set</param>
        /// <returns>True if time was set successfully</returns>
        ApiTrueOnSuccessResponse PlcSetSystemTime(DateTime timestamp);

        /// <summary>
        /// Send a Plc.ReadTimeSettings Request
        /// </summary>
        /// <returns>Current Plc Time Settings</returns>
        Task<ApiPlcReadTimeSettingsResponse> PlcReadTimeSettingsAsync();

        /// <summary>
        /// Send a Plc.ReadTimeSettings Request
        /// </summary>
        /// <returns>Current Plc Time Settings</returns>
        ApiPlcReadTimeSettingsResponse PlcReadTimeSettings();
        /// <summary>
        /// Send a Plc.SetTimeSettings Request with parameters
        /// </summary>
        /// <param name="utcOffset">The time zone offset from the UTC time in hours</param>
        /// <param name="daylightSavings">(Optional) Represents the settings for daylight-savings. If there is no daylight-savings rule configured, the utcOffset is applied to calculate the local time</param>
        /// <returns>True if the settings are applied successfully</returns>
        Task<ApiTrueOnSuccessResponse> PlcSetTimeSettingsAsync(TimeSpan utcOffset, DaylightSavingsRule daylightSavings = null);
        /// <summary>
        /// Send a Plc.SetTimeSettings Request with parameters
        /// </summary>
        /// <param name="utcOffset">The time zone offset from the UTC time in hours</param>
        /// <param name="daylightSavings">(Optional) Represents the settings for daylight-savings. If there is no daylight-savings rule configured, the utcOffset is applied to calculate the local time</param>
        /// <returns>True if the settings are applied successfully</returns>
        ApiTrueOnSuccessResponse PlcSetTimeSettings(TimeSpan utcOffset, DaylightSavingsRule daylightSavings = null);
        /// <summary>
        /// Send a Files.Browse Request
        /// </summary>
        /// <param name="resource">Path of the directory or file relative to the memory card root to fetch the entry list. 
        /// The resource name must start with a "/". The parameter may be omitted.In that case, it will default to "/".</param>
        /// <returns>Browsed resources (files/dir/...)</returns>
        Task<ApiBrowseFilesResponse> FilesBrowseAsync(string resource = null);

        /// <summary>
        /// Send a Files.Browse Request
        /// </summary>
        /// <param name="resource">Path of the directory or file relative to the memory card root to fetch the entry list. 
        /// The resource name must start with a "/". The parameter may be omitted.In that case, it will default to "/".</param>
        /// <returns>Browsed resources (files/dir/...)</returns>
        ApiBrowseFilesResponse FilesBrowse(string resource = null);

        /// <summary>
        /// Send a Files.Browse Request
        /// </summary>
        /// <param name="resource">resource to browse.</param>
        /// <returns>Browsed resources (files/dir/...)</returns>
        Task<ApiBrowseFilesResponse> FilesBrowseAsync(ApiFileResource resource);

        /// <summary>
        /// Send a Files.Browse Request
        /// </summary>
        /// <param name="resource">resource to browse</param>
        /// <returns>Browsed resources (files/dir/...)</returns>
        ApiBrowseFilesResponse FilesBrowse(ApiFileResource resource);

        /// <summary>
        /// Send a Files.Download Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>Ticket ID</returns>
        Task<ApiTicketIdResponse> FilesDownloadAsync(string resource);


        /// <summary>
        /// Send a Files.Download Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>Ticket ID</returns>
        ApiTicketIdResponse FilesDownload(string resource);

        /// <summary>
        /// Send a Files.Create Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>Ticket ID</returns>
        Task<ApiTicketIdResponse> FilesCreateAsync(string resource);

        /// <summary>
        /// Send a Files.Create Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>Ticket ID</returns>
        ApiTicketIdResponse FilesCreate(string resource);


        /// <summary>
        /// Send a Files.Create Request
        /// </summary>
        /// <param name="resource">FileInfo for informations about the file to the memory card root.</param>
        /// <returns>Ticket ID</returns>
        Task<ApiTicketIdResponse> FilesCreateAsync(FileInfo resource);

        /// <summary>
        /// Send a Files.Create Request
        /// </summary>
        /// <param name="resource">FileInfo for informations about the file to the memory card root.</param>
        /// <returns>Ticket ID</returns>
        ApiTicketIdResponse FilesCreate(FileInfo resource);

        /// <summary>
        /// Send Files.Rename request
        /// </summary>
        /// <param name="resource">Current path of file/folder</param>
        /// <param name="new_resource">New path of file/folder</param>
        /// <returns>True if the file or folder is renamed successfully</returns>
        Task<ApiTrueOnSuccessResponse> FilesRenameAsync(string resource, string new_resource);


        /// <summary>
        /// Send Files.Rename request
        /// </summary>
        /// <param name="resource">Current path of file/folder</param>
        /// <param name="new_resource">New path of file/folder</param>
        /// <returns>True if the file or folder is renamed successfully</returns>
        ApiTrueOnSuccessResponse FilesRename(string resource, string new_resource);

        /// <summary>
        /// Send a Files.Delete Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>True if the file is deleted successfully</returns>
        Task<ApiTrueOnSuccessResponse> FilesDeleteAsync(string resource);

        /// <summary>
        /// Send a Files.Delete Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>True if the file is deleted successfully</returns>
        ApiTrueOnSuccessResponse FilesDelete(string resource);

        /// <summary>
        /// Send a Files.Delete Request
        /// </summary>
        /// <param name="resource">the resource that shall be deleted.</param>
        /// <returns>True if the file is deleted successfully</returns>
        Task<ApiTrueOnSuccessResponse> FilesDeleteAsync(ApiFileResource resource);

        /// <summary>
        /// Send a Files.Delete Request
        /// </summary>
        /// <param name="resource">the resource that shall be deleted.</param>
        /// <returns>True if the file is deleted successfully</returns>
        ApiTrueOnSuccessResponse FilesDelete(ApiFileResource resource);

        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>True if the directory is created successfully</returns>
        Task<ApiTrueOnSuccessResponse> FilesCreateDirectoryAsync(string resource);


        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>True if the directory is created successfully</returns>
        ApiTrueOnSuccessResponse FilesCreateDirectory(string resource);


        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>True if the directory is created successfully</returns>
        Task<ApiTrueOnSuccessResponse> FilesCreateDirectoryAsync(DirectoryInfo resource);


        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>True if the directory is created successfully</returns>
        ApiTrueOnSuccessResponse FilesCreateDirectory(DirectoryInfo resource);

        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">The resource to create</param>
        /// <returns>True if the directory is created successfully</returns>
        Task<ApiTrueOnSuccessResponse> FilesCreateDirectoryAsync(ApiFileResource resource);


        /// <summary>
        /// Send a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">The resource to create.</param>
        /// <returns>True if the directory is created successfully</returns>
        ApiTrueOnSuccessResponse FilesCreateDirectory(ApiFileResource resource);

        /// <summary>
        /// Send a Files.DeleteDirectory Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>True if the directory is deleted successfully</returns>
        Task<ApiTrueOnSuccessResponse> FilesDeleteDirectoryAsync(string resource);

        /// <summary>
        /// Send a Files.DeleteDirectory Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>True if the directory is deleted successfully</returns>
        ApiTrueOnSuccessResponse FilesDeleteDirectory(string resource);

        /// <summary>
        /// Send a Files.DeleteDirectory Request
        /// </summary>
        /// <param name="resource">the directory to delete.</param>
        /// <returns>True if the directory is deleted successfully</returns>
        Task<ApiTrueOnSuccessResponse> FilesDeleteDirectoryAsync(ApiFileResource resource);

        /// <summary>
        /// Send a Files.DeleteDirectory Request
        /// </summary>
        /// <param name="resource">the directory to delete.</param>
        /// <returns>True if the directory is deleted successfully</returns>
        ApiTrueOnSuccessResponse FilesDeleteDirectory(ApiFileResource resource);

        /// <summary>
        /// Send a Datalogs.DownloadAndClear Request
        /// </summary>
        /// <param name="resource">Resource name of data log to retrieve, including the path.</param>
        /// <returns>True if the resource is downloaded and deleted successfully</returns>
        Task<ApiTicketIdResponse> DatalogsDownloadAndClearAsync(string resource);


        /// <summary>
        /// Send a Datalogs.DownloadAndClear Request
        /// </summary>
        /// <param name="resource">Resource name of data log to retrieve, including the path.</param>
        /// <returns>True if the resource is downloaded and deleted successfully</returns>
        ApiTicketIdResponse DatalogsDownloadAndClear(string resource);

        /// <summary>
        /// Send a Plc.CreateBackup request
        /// </summary>
        /// <returns>ticket id</returns>
        Task<ApiTicketIdResponse> PlcCreateBackupAsync();


        /// <summary>
        /// Send a Plc.CreateBackup request
        /// </summary>
        /// <returns>ticket id</returns>
        ApiTicketIdResponse PlcCreateBackup();

        /// <summary>
        /// Send a Plc.RestoreBackup request
        /// </summary>
        /// <returns>ticket id</returns>
        Task<ApiTicketIdResponse> PlcRestoreBackupAsync(string password = "");

        /// <summary>
        /// Send a Plc.RestoreBackup request
        /// </summary>
        /// <returns>ticket id</returns>
        ApiTicketIdResponse PlcRestoreBackup(string password = "");

        /// <summary>
        /// Re-login to the plc, set the header again in the connected service (e.g.HttpClient)!
        /// </summary>
        Task<ApiLoginResponse> ReLoginAsync(string userName, string password, bool? includeWebApplicationCookie = null);

        /// <summary>
        /// Re-login to the plc, set the header again in the connected service (e.g.HttpClient)!
        /// </summary>
        ApiLoginResponse ReLogin(string userName, string password, bool? includeWebApplicationCookie = null);

        /// <summary>
        /// Send a Failsafe.ReadParameters request
        /// </summary>
        /// <param name="hwid">The hardware identifier from which the parameters shall be read</param>
        /// <returns>Response with Failsafe parameters</returns>
        Task<ApiFailsafeReadParametersResponse> FailsafeReadParametersAsync(uint hwid);

        /// <summary>
        /// Send a Failsafe.ReadParameters request
        /// </summary>
        /// <param name="hwid">The hardware identifier from which the parameters shall be read</param>
        /// <returns>Response with Failsafe parameters</returns>
        ApiFailsafeReadParametersResponse FailsafeReadParameters(uint hwid);

        /// <summary>
        /// Send a Failsafe.ReadRuntimeGroups request
        /// </summary>
        /// <returns>Response with Runtime Groups</returns>
        Task<ApiFailsafeReadRuntimeGroupsResponse> FailsafeReadRuntimeGroupsAsync();

        /// <summary>
        /// Send a Failsafe.ReadRuntimeGroups request
        /// </summary>
        /// <returns>Response with Runtime Groups</returns>
        ApiFailsafeReadRuntimeGroupsResponse FailsafeReadRuntimeGroups();
        /// <summary>
        /// Send an Api.GetPasswordPolicy request
        /// </summary>
        /// <returns>ApiGetPasswordPolicy response</returns>
        Task<ApiGetPasswordPolicyResponse> ApiGetPasswordPolicyAsync();

        /// <summary>
        /// Send an Api.GetPasswordPolicy request
        /// </summary>
        /// <returns>ApiGetPasswordPolicy response</returns>
        ApiGetPasswordPolicyResponse ApiGetPasswordPolicy();

        /// <summary>
        /// Send an Api.GetAuthenticationMode request
        /// </summary>
        /// <returns>A response containing the authentication modes</returns>
        Task<ApiGetAuthenticationModeResponse> ApiGetAuthenticationModeAsync();

        /// <summary>
        /// Send an Api.GetAuthenticationMode request
        /// </summary>
        /// <returns>A response containing the authentication modes</returns>
        ApiGetAuthenticationModeResponse ApiGetAuthenticationMode();

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
        Task<ApiSyslogBrowseResponse> ApiSyslogBrowseAsync(ApiPlcRedundancyId? redundancy_id = null, uint? count = null, uint? first = null);

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
        ApiSyslogBrowseResponse ApiSyslogBrowse(ApiPlcRedundancyId? redundancy_id = null, uint? count = null, uint? first = null);

        /// <summary>
        /// This method allows the user to acknowledge a single alarm. <br/>
        /// This method will always return true, even when nothing is acknowledged.
        /// </summary>
        /// <param name="id">The Acknowledgement ID of the alarm which shall be acknowledged. <br/>
        /// The acknowledgement ID can be found in the alarm object that was returned by method Alarms.Browse.</param>
        /// <returns>ApiTrueOnSuccessResponse</returns>
        Task<ApiTrueOnSuccessResponse> AlarmsAcknowledgeAsync(string id);

        /// <summary>
        /// This method allows the user to acknowledge a single alarm. <br/>
        /// This method will always return true, even when nothing is acknowledged.
        /// </summary>
        /// <param name="id">The Acknowledgement ID of the alarm which shall be acknowledged. <br/>
        /// The acknowledgement ID can be found in the alarm object that was returned by method Alarms.Browse.</param>
        /// <returns>ApiTrueOnSuccessResponse</returns>
        ApiTrueOnSuccessResponse AlarmsAcknowledge(string id);

        /// <summary>
        /// Send a Alarms.Browse request
        /// </summary>
        /// <returns>ApiAlarmsBrowseResponse</returns>
        /// <param name="language">The language in which the texts should be returned. 
        ///                        If the language is valid, then the response must contain the texts in the requested language. <br/>
        ///                        An empty string shall be treated the same as an invalid language string.
        ///                        </param>
        /// <param name="count">(optional) The maximum number of alarm entries to be requested. Default value: 50 <br/>
        ///                     A count of 0 must omit any alarm entries from the response and must only return the attributes last_modified, count_max and count_current. 
        ///                     </param>
        /// <param name="alarm_id">(optional) The CPU alarm ID for which the user wants to return the data. If this is provided, no other parameters can be provided as filter.</param>
        /// <param name="filters">(optional) Optional object that contains parameters to filter the response.</param>
        Task<ApiAlarmsBrowseResponse> ApiAlarmsBrowseAsync(CultureInfo language, int? count = null, string alarm_id = null, ApiAlarms_RequestFilters filters = null);

        /// <summary>
        /// Send a Alarms.Browse request
        /// </summary>
        /// <returns>ApiAlarmsBrowseResponse</returns>
        /// <param name="language">The language in which the texts should be returned. 
        ///                        If the language is valid, then the response must contain the texts in the requested language. <br/>
        ///                        An empty string shall be treated the same as an invalid language string.
        ///                        </param>
        /// <param name="count">(optional) The maximum number of alarm entries to be requested. Default value: 50 <br/>
        ///                     A count of 0 must omit any alarm entries from the response and must only return the attributes last_modified, count_max and count_current. 
        ///                     </param>
        /// <param name="alarm_id">(optional) The CPU alarm ID for which the user wants to return the data. If this is provided, no other parameters can be provided as filter.</param>
        /// <param name="filters">(optional) Optional object that contains parameters to filter the response.</param>
        ApiAlarmsBrowseResponse ApiAlarmsBrowse(CultureInfo language, int? count = null, string alarm_id = null, ApiAlarms_RequestFilters filters = null);

        /// <summary>
        /// Send a DiagnosticBuffer.Browse request
        /// </summary>
        /// <param name="language">The language in which the texts should be returned. If the language is valid, then the response must contain the texts in the requested language.An empty string shall be treated the same as an invalid language string.</param>
        /// <param name="count">(optional) The maximum number of diagnostic buffer entries to be requested. Default value: 50. A count of 0 will omit any diagnostic buffer entries from the response</param>
        /// <param name="filters">(optional) ApiDiagnosticBufferBrowse_RequestFilters representing various filtering possibilities.</param>
        /// <returns>ApiDiagnosticBufferBrowseResponse</returns>
        Task<ApiDiagnosticBufferBrowseResponse> ApiDiagnosticBufferBrowseAsync(CultureInfo language,
                                                                               uint? count = null,
                                                                               ApiDiagnosticBuffer_RequestFilters filters = null);
        /// <summary>
        /// Send a DiagnosticBuffer.Browse request
        /// </summary>
        /// <param name="language">The language in which the texts should be returned. If the language is valid, then the response must contain the texts in the requested language.An empty string shall be treated the same as an invalid language string.</param>
        /// <param name="count">(optional) The maximum number of diagnostic buffer entries to be requested. Default value: 50. A count of 0 will omit any diagnostic buffer entries from the response</param>
        /// <param name="filters">(optional) ApiDiagnosticBufferBrowse_RequestFilters representing various filtering possibilities.</param>
        /// <returns>ApiDiagnosticBufferBrowseResponse</returns>
        ApiDiagnosticBufferBrowseResponse ApiDiagnosticBufferBrowse(CultureInfo language, uint? count = null, ApiDiagnosticBuffer_RequestFilters filters = null);

        /// <summary>
        /// Cancel the outstanding Requests
        /// </summary>
        void CancelPendingRequests();
    }
}