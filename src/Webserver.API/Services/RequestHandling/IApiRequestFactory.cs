// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models.AlarmsBrowse;
using Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Models.TimeSettings;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Api Request Factory => Will perform the according ParameterChecks (RequestParameterChecker) and return the requested IApiRequest
    /// </summary>
    public interface IApiRequestFactory
    {
        /// <summary>
        /// In case you want to "build up" an IApiRequest on your own
        /// </summary>
        /// <param name="method">Api method to be called</param>
        /// <param name="parameters">Api method parameters to be provided</param>
        /// <param name="jsonRpc">jsonrpc version </param>
        /// <param name="id">request id </param>
        /// <returns>An IApiRequest to be sent to the plc</returns>
        IApiRequest GetIApiRequest(string method, Dictionary<string, object> parameters = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.Browse Request without parameters. 
        /// </summary>
        /// <returns>ApiBrowseRequest without parameters. </returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiBrowseRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.BrowseTickets request - if a (valid - 28chars) ticketid is provided - return ApiBrowseTickets request with parameter "id" : ticketid
        /// </summary>
        /// <param name="ticketId">optional - if set: has to be 28chars long! - otherwise: InvalidParams!</param>
        /// <returns>ApiBrowseTickets request - if a (valid - 28chars) ticketid is provided - 
        /// return ApiBrowseTickets request with parameter "id" : ticketid</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiBrowseTicketsRequest(string ticketId = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.CloseTicket Request - if a (valid - 28chars) ticketid is provided - return Api.CloseTicket request with parameter "id" : ticketid
        /// </summary>
        /// <param name="ticketId">has to be 28chars long! - otherwise: InvalidParams!</param>
        /// <returns>Api.CloseTicket Request - if a (valid - 28chars) ticketid is provided - return Api.CloseTicket request with parameter "id" : ticketid</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiCloseTicketRequest(string ticketId, string jsonRpc = null, string id = null);
        /// <summary>
        /// Get an Api.ChangePassword request with parameters
        /// </summary>
        /// <param name="username">The user account for which the password shall be changed</param>
        /// <param name="currentPassword">The current password for the user</param>
        /// <param name="newPassword">The new password for the user</param>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>an Api.ChangePassword request</returns>
        IApiRequest GetApiChangePasswordRequest(string username, string currentPassword, string newPassword, string jsonRpc = null, string id = null);
        /// <summary>
        /// Get an Api.GetPasswordPolicy request
        /// </summary>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>An Api.GetPasswordPolicy request</returns>
        IApiRequest GetApiGetPasswordPolicyRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// Get an Api.GetAuthenticationMode request
        /// </summary>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>An Api.GetAuthenticationMode request</returns>
        IApiRequest GetApiGetAuthenticationModeRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.GetCertificateUrl Request without parameters
        /// </summary>
        /// <returns>Api.GetCertificateUrl Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiGetCertificateUrlRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.Login Request with the given "user":userName, "password": password,  "include_web_application_cookie" : include_web_application_cookie (might be null)
        /// </summary>
        /// <param name="userName">username for login</param>
        /// <param name="password">password for login</param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <returns>ApiLoginRequest with the given "user":userName, "password": password,  "include_web_application_cookie" : include_web_application_cookie (might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiLoginRequest(string userName, string password, bool? include_web_application_cookie = null,
           string jsonRpc = null, string id = null);

        /// <summary>
        /// get an Api.Logout Request without parameters
        /// </summary>
        /// <returns>Api.Logout Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiLogoutRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.GetPermissions Request without parameters
        /// </summary>
        /// <returns>Api.GetPermissions Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiGetPermissionsRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// Get an Api.GetQuantityStructures Request without parameters
        /// </summary>
        /// <returns>Api.GetQuantityStructures Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiGetQuantityStructuresRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.Ping Request without parameters
        /// </summary>
        /// <returns>Api.Ping Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiPingRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// Get a Project.ReadLanguages request without parameters
        /// </summary>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>Project.ReadLanguages request without parameters</returns>
        IApiRequest GetApiProjectReadLanguagesRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an PlcProgram.Browse Request with parameter "mode": apiPlcProgramBrowseMode, "var" : var (might be null)
        /// </summary>
        /// <param name="apiPlcProgramBrowseMode">Var or children</param>
        /// <param name="var">variable to be browsed</param>
        /// <returns>PlcProgram.Browse Request with parameter "mode": apiPlcProgramBrowseMode, "var" : var (might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiPlcProgramBrowseRequest(ApiPlcProgramBrowseMode apiPlcProgramBrowseMode, string var = null, string jsonRpc = null, string id = null);

        /// <summary>
        /// Get a PlcProgram.Browse request for the code blocks with parameters "mode":"Children", and "type"=["code_blocks"]).
        /// </summary>
        /// <returns>PlcProgram.Browse request for the code blocks with parameters "mode":"Children", and "type"=["code_blocks"]).</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiPlcProgramBrowseCodeBlocksRequest(string jsonRpc = null, string id = null);

        /// <summary>
        /// Get a PlcProgram.DownloadProfilingData request.
        /// </summary>
        /// <returns>PlcProgram.DownloadProfilingData request.</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiPlcProgramDownloadProfilingDataRequest(string jsonRpc = null, string id = null);

        /// <summary>
        /// get an PlcProgram.Read Request with parameter "var" : var, "mode": apiPlcProgramReadMode (might be null) - 
        /// </summary>
        /// <param name="var">Variable name requested (including "Parents" seperated by dots)</param>
        /// <param name="apiPlcProgramReadMode">mode to be read</param>
        /// <returns>PlcProgram.Read Request with parameter "var" : var, "mode": apiPlcProgramReadMode (might be null) - </returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiPlcProgramReadRequest(string var, ApiPlcProgramReadOrWriteMode? apiPlcProgramReadMode = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an PlcProgram.Write Request with parameter "var" : var, "value":valueToBeSet, "mode": apiPlcProgramReadMode (might be null) - 
        /// </summary>
        /// <param name="var">Variable name of the var to be written (including "Parents" seperated by dots)</param>
        /// <param name="valueToBeSet">value the "var" should have</param>
        /// <param name="apiPlcProgramWriteMode">mode to be written</param>
        /// <returns>PlcProgram.Write Request with parameter "var" : var, "value":valueToBeSet, "mode": apiPlcProgramReadMode (might be null) - </returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiPlcProgramWriteRequest(string var, object valueToBeSet, ApiPlcProgramReadOrWriteMode? apiPlcProgramWriteMode = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// "Comfort" function to get the according Type object for a value the user Wants depending on the apiPlcProgramData
        /// </summary>
        /// <param name="apiPlcProgramData">ApiPlcProgramDataType of the valueWanted</param>
        /// <param name="valueWanted">value the user wants</param>
        /// <returns>the value in the correct format for the api (8bytes and string: string, otherwise: object - e.g. int)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        object GetApiPlcProgramWriteValueToBeSet(ApiPlcProgramDataType apiPlcProgramData, object valueWanted, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Plc.ReadOperatingMode Request without parameters
        /// </summary>
        /// <returns>Plc.ReadOperatingMode Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiPlcReadOperatingModeRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Plc.CheckPlcRequestChangeOperatingMode Request with parameter "mode": apiPlcOperatingMode
        /// </summary>
        /// <param name="apiPlcOperatingMode">Plc Operating mode wanted</param>
        /// <returns>Plc.CheckPlcRequestChangeOperatingMode Request with parameter "mode": apiPlcOperatingMode</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiPlcRequestChangeOperatingModeRequest(ApiPlcOperatingMode apiPlcOperatingMode, string jsonRpc = null, string id = null);
        /// <summary>
        /// Get a Plc.ReadModeSelectorState Request with redundancy id parameter
        /// </summary>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>Plc.ReadModeSelectorState request with redundancy id parameter</returns>
        IApiRequest GetApiPlcReadModeSelectorStateRequest(ApiPlcRedundancyId rhid, string jsonRpc = null, string id = null);
        /// <summary>
        /// check new Etag value
        /// get an WebApp.SetResourceETag Request with parameter "app_name" : webAppName,"name": resourceName, "etag" : newETagValue
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource the new Etag value should be set for</param>
        /// <param name="newETagValue">New Etag value for the resource</param>
        /// <returns>WebApp.SetResourceETag Request with parameter "app_name" : webAppName,"name": resourceName, "etag" : newETagValue</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiSetResourceETagRequest(string webAppName, string resourceName, string newETagValue, string jsonRpc = null, string id = null);
        /// <summary>
        /// check new Media Type name
        /// get an WebApp.SetResourceMediaType Request with parameter "app_name" : webAppName,"name": resourceName, "media_type" : newMediaType
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource the new Mediatype value should be set for</param>
        /// <param name="newMediaType">New Mediatype value for the resource</param>
        /// <returns>WebApp.SetResourceMediaType Request with parameter "app_name" : webAppName,"name": resourceName, "media_type" : newMediaType</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiSetResourceMediaTypeRequest(string webAppName, string resourceName, string newMediaType, string jsonRpc = null, string id = null);
        /// <summary>
        /// check new lastmodified value (rfc3339)
        /// get an WebApp.SetResourceModificationTime Request with parameter "app_name" : webAppName,"name": resourceName, "last_modified" : newLastModified
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource the new Lastmodified value should be set for</param>
        /// <param name="newLastModified">New Lastmodified value for the resource</param>
        /// <returns>WebApp.SetResourceModificationTime Request with parameter "app_name" : webAppName,"name": resourceName, "last_modified" : newLastModified</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiSetResourceModificationTimeRequest(string webAppName, string resourceName, string newLastModified, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.SetResourceVisibility Request with parameter "app_name" : webAppName,"name": resourceName, "visibility" : apiWebAppResourceVisibility
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource the new Visibility value should be set for</param>
        /// <param name="apiWebAppResourceVisibility">New Visibility value for the resource</param>
        /// <returns>WebApp.SetResourceVisibility Request with parameter "app_name" : webAppName,"name": resourceName, "visibility" : apiWebAppResourceVisibility</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiSetResourceVisibilityRequest(string webAppName, string resourceName, ApiWebAppResourceVisibility apiWebAppResourceVisibility, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.Version Request without parameters
        /// </summary>
        /// <returns>Api.Version Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiVersionRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.Browse Request with parameter "name" : webAppName (optional, might be null)
        /// </summary>
        /// <param name="webAppName">OPTIONAL: name of the webapp you want to browse</param>
        /// <returns>WebApp.Browse Request with parameter "name" : webAppName (optional, might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppBrowseRequest(string webAppName = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.BrowseResources Request with parameter "app_name" : webAppName, "name":resourceName (optional, might be null)
        /// </summary>
        /// <param name="webAppName">Name of the Webapp you want to browse the resources of</param>
        /// <param name="resourceName">OPTIONAL: name of the resource you want to browse</param>
        /// <returns>WebApp.BrowseResources Request with parameter "app_name" : webAppName, "name":resourceName (optional, might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppBrowseResourcesRequest(string webAppName, string resourceName = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// check the request parameters: webAppName, if given WebAppState
        /// get an WebApp.Create Request with parameter "name" : webAppName, "state":apiWebAppState (optional, might be null)
        /// </summary>
        /// <param name="webAppName">webappname of the webapp to create</param>
        /// <param name="apiWebAppState">OPTIONAL: state the webapp should be in after creation</param>
        /// <returns>WebApp.Create Request with parameter "name" : webAppName, "state":apiWebAppState (optional, might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppCreateRequest(string webAppName, ApiWebAppState? apiWebAppState = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// Check resourceName, MediaType, LastModified, if given: Visibility (None is invalid) - if all are valid:
        /// get an WebApp.CreateResource Request with parameter "app_name" : webAppName, "name":resourceName, "media_type":media_type, "last_modified" : last_modified, "visibility":ApiWebAppResourceVisibility (optional, might be null), "etag":etag (optional, might be null)
        /// </summary>
        /// <param name="webAppName">webappname of the webapp the resource should be created in</param>
        /// <param name="resourceName">resourcename of the resource to be created</param>
        /// <param name="media_type">mediatype of the resource to be created</param>
        /// <param name="last_modified">lastmodified of the resource to be created</param>
        /// <param name="apiWebAppResourceVisibility">visibility of the resource to be created</param>
        /// <param name="etag">etag of the resource to be created</param>
        /// <returns>WebApp.CreateResource Request with parameter "app_name" : webAppName, "name":resourceName, "media_type":media_type, "last_modified" : last_modified, "visibility":ApiWebAppResourceVisibility (optional, might be null), "etag":etag (optional, might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppCreateResourceRequest(string webAppName, string resourceName, string media_type,
            string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.Delete Request with parameter "name" : webAppName
        /// </summary>
        /// <param name="webAppName">Name of the webapp that should be deleted</param>
        /// <returns>WebApp.Delete Request with parameter "name" : webAppName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppDeleteRequest(string webAppName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.DeleteResource Request with parameter "app_name" : webAppName, "name": resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be deleted</param>
        /// <returns>WebApp.DeleteResource Request with parameter "app_name" : webAppName, "name": resourceName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppDeleteResourceRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.DownloadResource Request with parameter "app_name" : webAppName, "name": resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be downloaded</param>
        /// <returns>WebApp.DownloadResource Request with parameter "app_name" : webAppName, "name": resourceName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppDownloadResourceRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.Rename Request with parameter "name" : webAppName, "new_name": newWebAppName
        /// </summary>
        /// <param name="webAppName">Current name of the Webapp that is to be renamed</param>
        /// <param name="newWebAppName">New name for the Webapp</param>
        /// <returns>WebApp.Rename Request with parameter "name" : webAppName, "new_name": newWebAppName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppRenameRequest(string webAppName, string newWebAppName, string jsonRpc = null, string id = null);
        /// <summary>
        /// check new resource name, if valid:
        /// get an WebApp.RenameResource Request with parameter "app_name" : webAppName, "name" : resourceName, "new_name": newResourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Current name of the resource that is to be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>WebApp.RenameResource Request with parameter "app_name" : webAppName, "name" : resourceName, "new_name": newResourceName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppRenameResourceRequest(string webAppName, string resourceName, string newResourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get a WebApp.SetDefaultPage Request with parameter "name" : webAppName, "resource_name" : resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be the Defaultpage</param>
        /// <returns>WebApp.SetDefaultPage Request with parameter "name" : webAppName, "resource_name" : resourceName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppSetDefaultPageRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.SetNotAuthorizedPage Request with parameter "name" : webAppName, "resource_name" : resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be the Notauthorizedpage</param>
        /// <returns>WebApp.SetNotAuthorizedPage Request with parameter "name" : webAppName, "resource_name" : resourceName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppSetNotAuthorizedPageRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.SetNotFoundPage Request with parameter "name" : webAppName, "resource_name" : resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be the Notfoundpage</param>
        /// <returns>WebApp.SetNotFoundPage Request with parameter "name" : webAppName, "resource_name" : resourceName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppSetNotFoundPageRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// check apiwebappstate (none isnt valid)
        /// get an WebApp.SetState Request with parameter "name" : webAppName, "state" : apiWebAppState
        /// </summary>
        /// <param name="webAppName">Name of the Webapp the state should be changed for</param>
        /// <param name="apiWebAppState">New state for the Webapp</param>
        /// <returns>WebApp.SetState Request with parameter "name" : webAppName, "state" : apiWebAppState</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebAppSetStateRequest(string webAppName, ApiWebAppState apiWebAppState, string jsonRpc = null, string id = null);

        /// <summary>
        /// Method to make sure all requests in the ApiBulk have a unique Id
        /// </summary>
        /// <param name="IApiRequests">Api Requests to make sure of that the ids are unique</param>
        /// <param name="timeOut">timeout for the waithandler => plc to be up again after reboot, etc.</param>
        /// <returns>A list of api Requests containing unique Ids</returns>
        IEnumerable<IApiRequest> GetApiBulkRequestWithUniqueIds(IEnumerable<IApiRequest> IApiRequests, TimeSpan? timeOut = null);

        /// <summary>
        /// get an Plc.ReadSystemTime Request
        /// </summary>
        /// <returns>Plc.ReadSystemTime Request</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiPlcReadSystemTimeRequest(string jsonRpc = null, string id = null);

        /// <summary>
        /// get an Plc.SetSystemTime Request
        /// </summary>
        /// <returns>Plc.SetSystemTime Request</returns>
        /// <param name="timestamp">The timestamp of the system time to be set</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to JsonRpcVersion</param>
        IApiRequest GetApiPlcSetSystemTimeRequest(DateTime timestamp, string jsonRpc = null, string id = null);

        /// <summary>
        /// get an Plc.ReadTimeSettings Request
        /// </summary>
        /// <returns>Plc.ReadTimeSettings Request</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiPlcReadTimeSettingsRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// Get an Plc.SetTimeSettings request
        /// </summary>
        /// <param name="utcOffset">The time zone offset from the UTC time in hours</param>
        /// <param name="daylightSavings">(Optional) Represents the settings for daylight-savings. If there is no daylight-savings rule configured, the utcOffset is applied to calculate the local time</param>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>Plc.SetTimeSettings Request</returns>
        IApiRequest GetApiPlcSetTimeSettingsRequest(TimeSpan utcOffset, DaylightSavingsRule daylightSavings = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Files.Browse Request
        /// </summary>
        /// <returns>Files.Browse Request</returns>
        /// <param name="resource">directory or file to be browsed</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiFilesBrowseRequest(string resource, string jsonRpc = null, string id = null);


        /// <summary>
        /// get an Files.Download Request
        /// </summary>
        /// <returns>Files.Download Request</returns>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiFilesDownloadRequest(string resource, string jsonRpc = null, string id = null);


        /// <summary>
        /// get an Files.Create Request
        /// </summary>
        /// <returns>Files.Create Request</returns>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiFilesCreateRequest(string resource, string jsonRpc = null, string id = null);


        /// <summary>
        /// get an Files.Rename Request
        /// </summary>
        /// <returns>Files.Rename Request</returns>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="new_resource">New path of file/folder</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiFilesRenameRequest(string resource, string new_resource, string jsonRpc = null, string id = null);


        /// <summary>
        /// get an Files.Delete Request
        /// </summary>
        /// <returns>Files.Delete Request</returns>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiFilesDeleteRequest(string resource, string jsonRpc = null, string id = null);


        /// <summary>
        /// get an Files.Delete Request
        /// </summary>
        /// <returns>Files.Delete Request</returns>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiFilesCreateDirectoryRequest(string resource, string jsonRpc = null, string id = null);


        /// <summary>
        /// get an Files.DeleteDirectory Request
        /// </summary>
        /// <returns>Files.Delete Request</returns>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiFilesDeleteDirectoryRequest(string resource, string jsonRpc = null, string id = null);


        /// <summary>
        /// get a DataLogs.DownloadAndClear Request
        /// </summary>
        /// <returns>DataLogs.DownloadAndClear Request</returns>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiDatalogsDownloadAndClearRequest(string resource, string jsonRpc = null, string id = null);

        /// <summary>
        /// get an Plc.CreateBackup Request
        /// </summary>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>Plc.CreateBackup Request</returns>
        IApiRequest GetPlcCreateBackupRequest(string jsonRpc = null, string id = null);

        /// <summary>
        /// get an Plc.RestoreBackup Request
        /// </summary>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <param name="password">Password the Plc.RestoreBackup should be called with</param>
        /// <returns>Plc.CreateBackup Request</returns>
        IApiRequest GetPlcRestoreBackupRequest(string jsonRpc = null, string id = null, string password = null);

        /// <summary>
        /// Get a Failsafe.ReadParameters Request
        /// </summary>
        /// <param name="hwid">The hardware identifier from which the parameters shall be read</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>Failsafe.ReadParameters Request</returns>
        IApiRequest GetFailsafeReadParametersRequest(uint hwid, string jsonRpc = null, string id = null);

        /// <summary>
        /// Get a Failsafe.ReadRuntimeGroups Request
        /// </summary>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>Failsafe.ReadRuntimeGroups Request</returns>
        IApiRequest GetFailsafeReadRuntimeGroupsRequest(string jsonRpc = null, string id = null);

        /// <summary>
        /// Get a Modules.DownloadServiceData request
        /// </summary>
        /// <param name="hwid">The HWID of a node (module) for which a service data file can be downloaded</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <returns>Modules.DownloadServiceData request</returns>
        IApiRequest GetModulesDownloadServiceData(ApiPlcHwId hwid, string jsonRpc = null, string id = null);

        /// <summary>
        /// Get a WebServer.ReadDefaultPage Request
        /// </summary>
        /// <param name="jsonRpc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IApiRequest GetApiWebserverReadDefaultPageRequest(string jsonRpc = null, string id = null);

        /// <summary>
        /// get a WebServer.SetDefaultPage Request with parameter "default_page" : default page name
        /// examples: "default_page" : "index.html" OR "default_page" : "/~webapp1/index.html" OR "default_page" : ""
        /// </summary>
        /// <param name="defaultPage">Name of the desired default page</param>
        /// <returns>WebApp.SetDefaultPage Request with parameter "default_page" : default page name</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        IApiRequest GetApiWebserverSetDefaultPageRequest(string defaultPage, string jsonRpc = null, string id = null);

        /// <summary>
        /// Get a Syslog.Browse request
        /// </summary>
        /// <param name="redundancy_id">(optional) The Redundancy ID parameter must be present when the request is executed on an R/H PLC. <br/> 
        ///                             In this case it must either have a value of 1 or 2, otherwise it is null.</param>
        /// <param name="count">(optional) The maximum number of syslog entries to be requested. Default value: 50 <br/>
        ///                     A count of 0 will omit any syslog entries from the response and only return the attributes last_modified, count_total and count_lost.</param>
        /// <param name="first">(optional) Allows the user to provide the id of an entry as a starting point for the returned entries array. <br/>
        ///                     This allows the user to traverse through the syslog buffer using multiple API calls.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>ApiSyslogBrowse request</returns>
        IApiRequest GetApiSyslogBrowseRequest(ApiPlcRedundancyId? redundancy_id = null, uint? count = null, uint? first = null, string jsonRpc = null, string id = null);

        /// <summary>
        /// Get a Alarms.Acknowledge request
        /// </summary>
        /// <returns>ApiAlarmsAcknowledgeRequest</returns>
        /// <param name="alarmId">Specifies the id of the alarm</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiAlarmsAcknowledgeRequest(string alarmId, string jsonRpc = null, string id = null);

        /// <summary>
        /// Get a Alarms.Browse request
        /// </summary>
        /// <returns>ApiAlarmsBrowseRequest</returns>
        /// <param name="language">The language in which the texts should be returned. 
        ///                        If the language is valid, then the response must contain the texts in the requested language. <br/>
        ///                        An empty string shall be treated the same as an invalid language string.
        ///                        </param>
        /// <param name="count">(optional) The maximum number of alarm entries to be requested. Default value: 50 <br/>
        ///                     A count of 0 must omit any alarm entries from the response and must only return the attributes last_modified, count_max and count_current. 
        ///                     </param>
        /// <param name="alarm_id">(optional) The CPU alarm ID for which the user wants to return the data. If this is provided, no other parameters can be provided as filter.</param>
        /// <param name="filters">(optional) Optional object that contains parameters to filter the response.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        IApiRequest GetApiAlarmsBrowseRequest(CultureInfo language, int? count = null, string alarm_id = null, ApiAlarms_RequestFilters filters = null, string jsonRpc = null, string id = null);

        /// <summary>
        /// Get a DiagnosticBuffer.Browse request
        /// </summary>
        /// <param name="language">Specifies the language of the response</param>
        /// <param name="count">Specifies maximum how many diagnosticbuffer entry you will get back</param>
        /// <param name="filters">ApiDiagnosticBufferBrowse_RequestFilters representing various filtering possibilities.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>ApiDiagnosticBufferBrowseRequest</returns>  
        IApiRequest GetApiDiagnosticBufferBrowseRequest(CultureInfo language,
                                                        uint? count = null,
                                                        ApiDiagnosticBuffer_RequestFilters filters = null,
                                                        string jsonRpc = null,
                                                        string id = null);
    }

}