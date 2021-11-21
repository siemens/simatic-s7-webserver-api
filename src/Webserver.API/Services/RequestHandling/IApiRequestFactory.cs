// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using System;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Api Request Factory => Will perform the according ParameterChecks (RequestParameterChecker) and return the requested ApiRequest
    /// </summary>
    public interface IApiRequestFactory
    {
        /// <summary>
        /// In case you want to "build up" an ApiRequest on your own
        /// </summary>
        /// <param name="method">Api method to be called</param>
        /// <param name="parameters">Api method parameters to be provided</param>
        /// <param name="jsonRpc">jsonrpc version </param>
        /// <param name="id">request id </param>
        /// <returns>An ApiRequest to be sent to the plc</returns>
        ApiRequest GetApiRequest(string method, Dictionary<string, object> parameters = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.Browse Request without parameters. 
        /// </summary>
        /// <returns>ApiBrowseRequest without parameters. </returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiBrowseRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.BrowseTickets request - if a (valid - 28chars) ticketid is provided - return ApiBrowseTickets request with parameter "id" : ticketid
        /// </summary>
        /// <param name="ticketId">optional - if set: has to be 28chars long! - otherwise: InvalidParams!</param>
        /// <returns>ApiBrowseTickets request - if a (valid - 28chars) ticketid is provided - 
        /// return ApiBrowseTickets request with parameter "id" : ticketid</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiBrowseTicketsRequest(string ticketId = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.CloseTicket Request - if a (valid - 28chars) ticketid is provided - return Api.CloseTicket request with parameter "id" : ticketid
        /// </summary>
        /// <param name="ticketId">has to be 28chars long! - otherwise: InvalidParams!</param>
        /// <returns>Api.CloseTicket Request - if a (valid - 28chars) ticketid is provided - return Api.CloseTicket request with parameter "id" : ticketid</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiCloseTicketRequest(string ticketId, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.GetCertificateUrl Request without parameters
        /// </summary>
        /// <returns>Api.GetCertificateUrl Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiGetCertificateUrlRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.Login Request with the given "user":userName, "password": password,  "include_web_application_cookie" : include_web_application_cookie (might be null)
        /// </summary>
        /// <param name="userName">username for login</param>
        /// <param name="password">password for login</param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <returns>ApiLoginRequest with the given "user":userName, "password": password,  "include_web_application_cookie" : include_web_application_cookie (might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiLoginRequest(string userName, string password, bool? include_web_application_cookie = null,
           string jsonRpc = null, string id = null);

        /// <summary>
        /// get an Api.Logout Request without parameters
        /// </summary>
        /// <returns>Api.Logout Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiLogoutRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.GetPermissions Request without parameters
        /// </summary>
        /// <returns>Api.GetPermissions Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiGetPermissionsRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.Ping Request without parameters
        /// </summary>
        /// <returns>Api.Ping Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiPingRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an PlcProgram.Browse Request with parameter "mode": apiPlcProgramBrowseMode, "var" : var (might be null)
        /// </summary>
        /// <param name="apiPlcProgramBrowseMode">Var or children</param>
        /// <param name="var">variable to be browsed</param>
        /// <returns>PlcProgram.Browse Request with parameter "mode": apiPlcProgramBrowseMode, "var" : var (might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiPlcProgramBrowseRequest(ApiPlcProgramBrowseMode apiPlcProgramBrowseMode, string var = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an PlcProgram.Read Request with parameter "var" : var, "mode": apiPlcProgramReadMode (might be null) - 
        /// </summary>
        /// <param name="var">Variable name requested (including "Parents" seperated by dots)</param>
        /// <param name="apiPlcProgramReadMode">mode to be read</param>
        /// <returns>PlcProgram.Read Request with parameter "var" : var, "mode": apiPlcProgramReadMode (might be null) - </returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiPlcProgramReadRequest(string var, ApiPlcProgramReadOrWriteMode? apiPlcProgramReadMode = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an PlcProgram.Write Request with parameter "var" : var, "value":valueToBeSet, "mode": apiPlcProgramReadMode (might be null) - 
        /// </summary>
        /// <param name="var">Variable name of the var to be written (including "Parents" seperated by dots)</param>
        /// <param name="valueToBeSet">value the "var" should have</param>
        /// <param name="apiPlcProgramWriteMode">mode to be written</param>
        /// <returns>PlcProgram.Write Request with parameter "var" : var, "value":valueToBeSet, "mode": apiPlcProgramReadMode (might be null) - </returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiPlcProgramWriteRequest(string var, object valueToBeSet, ApiPlcProgramReadOrWriteMode? apiPlcProgramWriteMode = null, string jsonRpc = null, string id = null);
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
        ApiRequest GetApiPlcReadOperatingModeRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Plc.CheckPlcRequestChangeOperatingMode Request with parameter "mode": apiPlcOperatingMode
        /// </summary>
        /// <param name="apiPlcOperatingMode">Plc Operating mode wanted</param>
        /// <returns>Plc.CheckPlcRequestChangeOperatingMode Request with parameter "mode": apiPlcOperatingMode</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiPlcRequestChangeOperatingModeRequest(ApiPlcOperatingMode apiPlcOperatingMode, string jsonRpc = null, string id = null);
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
        ApiRequest GetApiSetResourceETagRequest(string webAppName, string resourceName, string newETagValue, string jsonRpc = null, string id = null);
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
        ApiRequest GetApiSetResourceMediaTypeRequest(string webAppName, string resourceName, string newMediaType, string jsonRpc = null, string id = null);
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
        ApiRequest GetApiSetResourceModificationTimeRequest(string webAppName, string resourceName, string newLastModified, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.SetResourceVisibility Request with parameter "app_name" : webAppName,"name": resourceName, "visibility" : apiWebAppResourceVisibility
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource the new Visibility value should be set for</param>
        /// <param name="apiWebAppResourceVisibility">New Visibility value for the resource</param>
        /// <returns>WebApp.SetResourceVisibility Request with parameter "app_name" : webAppName,"name": resourceName, "visibility" : apiWebAppResourceVisibility</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiSetResourceVisibilityRequest(string webAppName, string resourceName, ApiWebAppResourceVisibility apiWebAppResourceVisibility, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an Api.Version Request without parameters
        /// </summary>
        /// <returns>Api.Version Request without parameters</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiVersionRequest(string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.Browse Request with parameter "name" : webAppName (optional, might be null)
        /// </summary>
        /// <param name="webAppName">OPTIONAL: name of the webapp you want to browse</param>
        /// <returns>WebApp.Browse Request with parameter "name" : webAppName (optional, might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiWebAppBrowseRequest(string webAppName = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.BrowseResources Request with parameter "app_name" : webAppName, "name":resourceName (optional, might be null)
        /// </summary>
        /// <param name="webAppName">Name of the Webapp you want to browse the resources of</param>
        /// <param name="resourceName">OPTIONAL: name of the resource you want to browse</param>
        /// <returns>WebApp.BrowseResources Request with parameter "app_name" : webAppName, "name":resourceName (optional, might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiWebAppBrowseResourcesRequest(string webAppName, string resourceName = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// check the request parameters: webAppName, if given WebAppState
        /// get an WebApp.Create Request with parameter "name" : webAppName, "state":apiWebAppState (optional, might be null)
        /// </summary>
        /// <param name="webAppName">webappname of the webapp to create</param>
        /// <param name="apiWebAppState">OPTIONAL: state the webapp should be in after creation</param>
        /// <returns>WebApp.Create Request with parameter "name" : webAppName, "state":apiWebAppState (optional, might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiWebAppCreateRequest(string webAppName, ApiWebAppState? apiWebAppState = null, string jsonRpc = null, string id = null);
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
        ApiRequest GetApiWebAppCreateResourceRequest(string webAppName, string resourceName, string media_type,
            string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.Delete Request with parameter "name" : webAppName
        /// </summary>
        /// <param name="webAppName">Name of the webapp that should be deleted</param>
        /// <returns>WebApp.Delete Request with parameter "name" : webAppName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiWebAppDeleteRequest(string webAppName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.DeleteResource Request with parameter "app_name" : webAppName, "name": resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be deleted</param>
        /// <returns>WebApp.DeleteResource Request with parameter "app_name" : webAppName, "name": resourceName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiWebAppDeleteResourceRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.DownloadResource Request with parameter "app_name" : webAppName, "name": resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be downloaded</param>
        /// <returns>WebApp.DownloadResource Request with parameter "app_name" : webAppName, "name": resourceName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiWebAppDownloadResourceRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.Rename Request with parameter "name" : webAppName, "new_name": newWebAppName
        /// </summary>
        /// <param name="webAppName">Current name of the Webapp that is to be renamed</param>
        /// <param name="newWebAppName">New name for the Webapp</param>
        /// <returns>WebApp.Rename Request with parameter "name" : webAppName, "new_name": newWebAppName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiWebAppRenameRequest(string webAppName, string newWebAppName, string jsonRpc = null, string id = null);
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
        ApiRequest GetApiWebAppRenameResourceRequest(string webAppName, string resourceName, string newResourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get a WebApp.SetDefaultPage Request with parameter "name" : webAppName, "resource_name" : resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be the Defaultpage</param>
        /// <returns>WebApp.SetDefaultPage Request with parameter "name" : webAppName, "resource_name" : resourceName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiWebAppSetDefaultPageRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.SetNotAuthorizedPage Request with parameter "name" : webAppName, "resource_name" : resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be the Notauthorizedpage</param>
        /// <returns>WebApp.SetNotAuthorizedPage Request with parameter "name" : webAppName, "resource_name" : resourceName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiWebAppSetNotAuthorizedPageRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// get an WebApp.SetNotFoundPage Request with parameter "name" : webAppName, "resource_name" : resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be the Notfoundpage</param>
        /// <returns>WebApp.SetNotFoundPage Request with parameter "name" : webAppName, "resource_name" : resourceName</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiWebAppSetNotFoundPageRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null);
        /// <summary>
        /// check apiwebappstate (none isnt valid)
        /// get an WebApp.SetState Request with parameter "name" : webAppName, "state" : apiWebAppState
        /// </summary>
        /// <param name="webAppName">Name of the Webapp the state should be changed for</param>
        /// <param name="apiWebAppState">New state for the Webapp</param>
        /// <returns>WebApp.SetState Request with parameter "name" : webAppName, "state" : apiWebAppState</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        ApiRequest GetApiWebAppSetStateRequest(string webAppName, ApiWebAppState apiWebAppState, string jsonRpc = null, string id = null);

        /// <summary>
        /// Method to make sure all requests in the ApiBulk have a unique Id
        /// </summary>
        /// <param name="apiRequests">Api Requests to make sure of that the ids are unique</param>
        /// <returns>A list of api Requests containing unique Ids</returns>
        IEnumerable<ApiRequest> GetApiBulkRequestWithUniqueIds(IEnumerable<ApiRequest> apiRequests, TimeSpan? timeOut = null);
    }

}