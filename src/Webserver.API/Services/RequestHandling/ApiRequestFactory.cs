// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models.AlarmsBrowse;
using Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Models.TimeSettings;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Api Request Factory => Will perform the according ParameterChecks (RequestParameterChecker) and return the requested IApiRequest
    /// Will per default provide a Request with jsonrpc 2.0 and an id with 8 random chars, unless set differently
    /// </summary>
    public class ApiRequestFactory : IApiRequestFactory
    {
        private readonly IIdGenerator RequestIdGenerator;

        private readonly IApiRequestParameterChecker RequestParameterChecker;

        /// <summary>
        /// Bool to determine wether to use local checks for Request Parameters or not
        /// </summary>
        public bool PerformCheck { get; set; } = true;

        /// <summary>
        /// JsonRpc Version
        /// </summary>
        public const string JsonRpcVersion = "2.0";

        /// <summary>
        /// C'tor with optional requestGenerator parameter
        /// </summary>
        /// <param name="requestGenerator">RequestGenerator - can be customized</param>
        /// <param name="requestParameterChecker">parameter checker for the requestfactory</param>
        public ApiRequestFactory(IIdGenerator requestGenerator, IApiRequestParameterChecker requestParameterChecker)
        {
            RequestIdGenerator = requestGenerator ?? throw new ArgumentNullException(nameof(requestGenerator));
            RequestParameterChecker = requestParameterChecker ?? throw new ArgumentNullException(nameof(requestParameterChecker));
        }

        /// <summary>
        /// get an Api.Browse Request without parameters. 
        /// </summary>
        /// <returns>ApiBrowseRequest without parameters. </returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiBrowseRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Api.Browse", jsonRpcReq, idReq);
        }
        /// <summary>
        /// get an Api.BrowseTickets request - if a (valid - 28chars) ticketid is provided - return ApiBrowseTickets request with parameter "id" : ticketid
        /// </summary>
        /// <param name="ticketId">optional - if set: has to be 28chars long! - otherwise: InvalidParams!</param>
        /// <returns>ApiBrowseTickets request - if a (valid - 28chars) ticketid is provided - 
        /// return ApiBrowseTickets request with parameter "id" : ticketid</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiBrowseTicketsRequest(string ticketId = null, string jsonRpc = null, string id = null)
        {
            if (!string.IsNullOrEmpty(ticketId))
            {
                RequestParameterChecker.CheckTicket(ticketId, PerformCheck);
            }
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Api.BrowseTickets", jsonRpcReq, idReq, string.IsNullOrEmpty(ticketId) ? null :
                new Dictionary<string, object>() { { "id", ticketId } });
        }
        /// <summary>
        /// get an Api.CloseTicket Request - if a (valid - 28chars) ticketid is provided - return Api.CloseTicket request with parameter "id" : ticketid
        /// </summary>
        /// <param name="ticketId">has to be 28chars long! - otherwise: InvalidParams!</param>
        /// <returns>Api.CloseTicket Request - if a (valid - 28chars) ticketid is provided - return Api.CloseTicket request with parameter "id" : ticketid</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiCloseTicketRequest(string ticketId, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckTicket(ticketId, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Api.CloseTicket", jsonRpcReq, idReq, new Dictionary<string, object>() { { "id", ticketId } });
        }

        /// <summary>
        /// Get an Api.ChangePassword request with parameters
        /// </summary>
        /// <param name="username">The user account for which the password shall be changed</param>
        /// <param name="currentPassword">The current password for the user</param>
        /// <param name="newPassword">The new password for the user</param>
        /// <param name="mode">The mode defines where the password change shall be performed on. If null, the PLC will treat it as local.</param>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>an Api.ChangePassword request</returns>
        public IApiRequest GetApiChangePasswordRequest(string username, string currentPassword, string newPassword, ApiAuthenticationMode? mode = null, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckUsername(username, PerformCheck);
            RequestParameterChecker.CheckPasswords(currentPassword, newPassword, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            Dictionary<string, object> requestParams = new Dictionary<string, object>()
                            {
                                { "username", username },
                                { "password", currentPassword },
                                { "new_password", newPassword }
                            };
            if (mode != null)
            {
                requestParams.Add("mode", mode.ToString().ToLower());
            }
            return new ApiRequest("Api.ChangePassword", jsonRpcReq, idReq, requestParams);
        }
        /// <summary>
        /// Get an Api.GetPasswordPolicy request
        /// </summary>
        /// <param name="mode">The authentication mode that defines where the password policy shall be read from.</param>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>An Api.GetPasswordPolicy request</returns>
        public IApiRequest GetApiGetPasswordPolicyRequest(ApiAuthenticationMode mode = ApiAuthenticationMode.Local, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            if (mode != ApiAuthenticationMode.Local)
            {
                return new ApiRequest("Api.GetPasswordPolicy", jsonRpcReq, idReq,
                                      new Dictionary<string, object>() { { "mode", mode.ToString().ToLower() } });
            }
            else
            {
                return new ApiRequest("Api.GetPasswordPolicy", jsonRpcReq, idReq);
            }
        }

        /// <summary>
        /// Get an Api.GetAuthenticationMode request
        /// </summary>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>An Api.GetAuthenticationMode request</returns>
        public IApiRequest GetApiGetAuthenticationModeRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Api.GetAuthenticationMode", jsonRpcReq, idReq);
        }

        /// <summary>
        /// get an Api.GetCertificateUrl Request without parameters
        /// </summary>
        /// <returns>Api.GetCertificateUrl Request without parameters</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiGetCertificateUrlRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Api.GetCertificateUrl", jsonRpcReq, idReq);
        }

        /// <summary>
        /// get an Api.Login Request with the given "mode":mode, "user":userName, "password": password,  "include_web_application_cookie" : include_web_application_cookie (might be null)
        /// </summary>
        /// <param name="mode">The mode defines where the login shall be performed.
        /// All available modes supported by API method Api.GetAuthenticationMode can be passed, except for umc_sso.</param>
        /// <param name="userName">username for login</param>
        /// <param name="password">password for login</param>
        /// <param name="include_web_application_cookie">bool used to determine if the response should include a valid application cookie value for protected pages access</param>
        /// <returns>ApiLoginRequest with the given "user":userName, "password": password,  "include_web_application_cookie" : include_web_application_cookie (might be null)</returns>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        public virtual IApiRequest GetApiLoginRequest(string userName, string password, bool? include_web_application_cookie = null, ApiAuthenticationMode? mode = null,
           string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            Dictionary<string, object> requestParams = new Dictionary<string, object>()
            {
                { "user", userName },
                { "password", password }
            };
            if (include_web_application_cookie == true)
            {
                requestParams.Add("include_web_application_cookie", include_web_application_cookie);
            }
            if(mode != null)
            {
                requestParams.Add("mode", mode.ToString().ToLower());
            }
            return new ApiRequest("Api.Login", jsonRpcReq, idReq, requestParams);
        }

        /// <summary>
        /// get an Api.Logout Request without parameters
        /// </summary>
        /// <returns>Api.Logout Request without parameters</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiLogoutRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Api.Logout", jsonRpcReq, idReq);
        }
        /// <summary>
        /// get an Api.GetPermissions Request without parameters
        /// </summary>
        /// <returns>Api.GetPermissions Request without parameters</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiGetPermissionsRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Api.GetPermissions", jsonRpcReq, idReq);
        }
        /// <summary>
        /// Get an Api.GetQuantityStructures Request without parameters
        /// </summary>
        /// <returns>Api.GetQuantityStructures Request without parameters</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiGetQuantityStructuresRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Api.GetQuantityStructures", jsonRpcReq, idReq);
        }
        /// <summary>
        /// get an Api.Ping Request without parameters
        /// </summary>
        /// <returns>Api.Ping Request without parameters</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiPingRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Api.Ping", jsonRpcReq, idReq);
        }
        /// <summary>
        /// get an PlcProgram.Browse Request with parameter "mode": apiPlcProgramBrowseMode, "var" : var (might be null) - defaults to ""
        /// </summary>
        /// <param name="apiPlcProgramBrowseMode">Var or children</param>
        /// <param name="var">defaults to ""</param>
        /// <returns>PlcProgram.Browse Request with parameter "mode": apiPlcProgramBrowseMode, "var" : var (might be null) - defaults to ""</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiPlcProgramBrowseRequest(ApiPlcProgramBrowseMode apiPlcProgramBrowseMode, string var = null, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckPlcProgramBrowseMode(apiPlcProgramBrowseMode, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("PlcProgram.Browse", jsonRpcReq, idReq, new Dictionary<string, object>() { { "var", var },
                { "mode", apiPlcProgramBrowseMode.ToString().ToLower() } });
        }

        /// <summary>
        /// Get a PlcProgram.Browse request for the code blocks (i.e. with parameters "mode":"Children", and "type"=["code_blocks"]).
        /// </summary>
        /// <returns>PlcProgram.Browse Request for the code blocks (i.e. with parameters "mode":"Children", and "type"=["code_blocks"]).</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to JsonRpcVersion</param>
        public virtual IApiRequest GetApiPlcProgramBrowseCodeBlocksRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("PlcProgram.Browse", jsonRpcReq, idReq, new Dictionary<string, object>() {
                { "mode", ApiPlcProgramBrowseMode.Children.ToString().ToLower() },
                { "type", new string[] {"code_blocks"} }
            });
        }

        /// <summary>
        /// Get a PlcProgram.DownloadProfilingData request.
        /// </summary>
        /// <returns>PlcProgram.DownloadProfilingData request.</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to JsonRpcVersion</param>
        public virtual IApiRequest GetApiPlcProgramDownloadProfilingDataRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("PlcProgram.DownloadProfilingData", jsonRpcReq, idReq);
        }

        /// <summary>
        /// get an PlcProgram.Read Request with parameter "var" : var, "mode": apiPlcProgramReadMode (might be null) - defaults to "simple"
        /// </summary>
        /// <param name="var">Variable name requested (including "Parents" seperated by dots)</param>
        /// <param name="apiPlcProgramReadMode">defaults to "simple"</param>
        /// <returns>PlcProgram.Read Request with parameter "var" : var, "mode": apiPlcProgramReadMode (might be null) - defaults to "simple"</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiPlcProgramReadRequest(string var, ApiPlcDataRepresentation? apiPlcProgramReadMode = null, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckPlcDataRepresentationMode(apiPlcProgramReadMode, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("PlcProgram.Read", jsonRpcReq, idReq, new Dictionary<string, object>() { { "var", var },
                { "mode", apiPlcProgramReadMode?.ToString().ToLower() } });
        }
        /// <summary>
        /// get an PlcProgram.Write Request with parameter "var" : var, "value":valueToBeSet, "mode": apiPlcProgramReadMode (might be null) - defaults to "simple"
        /// </summary>
        /// <param name="var">Variable name of the var to be written (including "Parents" seperated by dots)</param>
        /// <param name="valueToBeSet">value the "var" should have</param>
        /// <param name="apiPlcProgramWriteMode">defaults to "simple"</param>
        /// <returns>PlcProgram.Write Request with parameter "var" : var, "value":valueToBeSet, "mode": apiPlcProgramReadMode (might be null) - defaults to "simple"</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiPlcProgramWriteRequest(string var, object valueToBeSet, ApiPlcDataRepresentation? apiPlcProgramWriteMode = null, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckPlcDataRepresentationMode(apiPlcProgramWriteMode, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("PlcProgram.Write", jsonRpcReq, idReq, new Dictionary<string, object>() { { "var", var },
                { "mode", apiPlcProgramWriteMode?.ToString().ToLower() }, { "value", valueToBeSet } });
        }
        /// <summary>
        /// "Comfort" function to get the according Type object for a value the user Wants depending on the apiPlcProgramData
        /// </summary>
        /// <param name="apiPlcProgramData">ApiPlcProgramDataType of the valueWanted</param>
        /// <param name="valueWanted">value the user wants</param>
        /// <returns>the value in the correct format for the api (8bytes and string: string, otherwise: object - e.g. int)</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual object GetApiPlcProgramWriteValueToBeSet(ApiPlcProgramDataType apiPlcProgramData, object valueWanted, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckPlcProgramReadOrWriteDataType(apiPlcProgramData, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            var bytesOfDataType = apiPlcProgramData.GetBytesOfDataType();
            var type = apiPlcProgramData.GetAccordingDataType();
            var objToReturn = (bytesOfDataType == 8 && apiPlcProgramData != ApiPlcProgramDataType.Lreal || type == typeof(string)) ? valueWanted.ToString() :
                Convert.ChangeType(valueWanted, type);
            return objToReturn;
        }
        /// <summary>
        /// get an Plc.ReadOperatingMode Request without parameters
        /// </summary>
        /// <returns>Plc.ReadOperatingMode Request without parameters</returns>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiPlcReadOperatingModeRequest(ApiPlcRedundancyId rhid = ApiPlcRedundancyId.StandardPLC, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Plc.ReadOperatingMode", jsonRpcReq, idReq,
                rhid == ApiPlcRedundancyId.StandardPLC ? null : new Dictionary<string, object>()
                {
                    { "redundancy_id", (int)rhid }
                });
        }
        /// <summary>
        /// get an Plc.CheckPlcRequestChangeOperatingMode Request with parameter "mode": apiPlcOperatingMode
        /// </summary>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <param name="apiPlcOperatingMode">Plc Operating mode wanted</param>
        /// <returns>Plc.CheckPlcRequestChangeOperatingMode Request with parameter "mode": apiPlcOperatingMode</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiPlcRequestChangeOperatingModeRequest(ApiPlcOperatingMode apiPlcOperatingMode, ApiPlcRedundancyId rhid = ApiPlcRedundancyId.StandardPLC, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckPlcRequestChangeOperatingMode(apiPlcOperatingMode, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            var dict = new Dictionary<string, object>() { { "mode", apiPlcOperatingMode.ToString().ToLower() } };
            if (rhid != ApiPlcRedundancyId.StandardPLC)
            {
                dict.Add("redundancy_id", (int)rhid);
            }
            return new ApiRequest("Plc.RequestChangeOperatingMode", jsonRpcReq, idReq, dict);
        }
        /// <summary>
        /// Get a Plc.ReadModeSelectorState Request with redundancy id parameter
        /// </summary>
        /// <param name="rhid">In an R/H system, a PLC with ID 1 (primary) or 2 (backup). For standard PLCs, enum value 0 (StandardPLC) is required.</param>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>Plc.ReadModeSelectorState request with redundancy id parameter</returns>
        public IApiRequest GetApiPlcReadModeSelectorStateRequest(ApiPlcRedundancyId rhid = ApiPlcRedundancyId.StandardPLC, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Plc.ReadModeSelectorState", jsonRpcReq, idReq,
                rhid == ApiPlcRedundancyId.StandardPLC ? null : new Dictionary<string, object>()
                {
                    { "redundancy_id", (int)rhid }
                });
        }
        /// <summary>
        /// check new Etag value
        /// get an WebApp.SetResourceETag Request with parameter "app_name" : webAppName,"name": resourceName, "etag" : newETagValue
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource the new Etag value should be set for</param>
        /// <param name="newETagValue">New Etag value for the resource</param>
        /// <returns>WebApp.SetResourceETag Request with parameter "app_name" : webAppName,"name": resourceName, "etag" : newETagValue</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiSetResourceETagRequest(string webAppName, string resourceName, string newETagValue, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckETag(newETagValue, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.SetResourceETag", jsonRpcReq, idReq, new Dictionary<string, object>() { { "app_name", webAppName },
                { "name", resourceName }, { "etag", newETagValue } });
        }
        /// <summary>
        /// check new Media Type name
        /// get an WebApp.SetResourceMediaType Request with parameter "app_name" : webAppName,"name": resourceName, "media_type" : newMediaType
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource the new Mediatype value should be set for</param>
        /// <param name="newMediaType">New Mediatype value for the resource</param>
        /// <returns>WebApp.SetResourceMediaType Request with parameter "app_name" : webAppName,"name": resourceName, "media_type" : newMediaType</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiSetResourceMediaTypeRequest(string webAppName, string resourceName, string newMediaType, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckMediaType(newMediaType, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.SetResourceMediaType", jsonRpcReq, idReq, new Dictionary<string, object>() { { "app_name", webAppName },
                { "name", resourceName }, { "media_type", newMediaType } });
        }
        /// <summary>
        /// check new lastmodified value (rfc3339)
        /// get an WebApp.SetResourceModificationTime Request with parameter "app_name" : webAppName,"name": resourceName, "last_modified" : newLastModified
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource the new Lastmodified value should be set for</param>
        /// <param name="newLastModified">New Lastmodified value for the resource</param>
        /// <returns>WebApp.SetResourceModificationTime Request with parameter "app_name" : webAppName,"name": resourceName, "last_modified" : newLastModified</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiSetResourceModificationTimeRequest(string webAppName, string resourceName, string newLastModified, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckLastModified(newLastModified, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.SetResourceModificationTime", jsonRpcReq, idReq, new Dictionary<string, object>() { { "app_name", webAppName },
                { "name", resourceName }, { "last_modified", newLastModified } });
        }
        /// <summary>
        /// get an WebApp.SetResourceVisibility Request with parameter "app_name" : webAppName,"name": resourceName, "visibility" : apiWebAppResourceVisibility
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource the new Visibility value should be set for</param>
        /// <param name="apiWebAppResourceVisibility">New Visibility value for the resource</param>
        /// <returns>WebApp.SetResourceVisibility Request with parameter "app_name" : webAppName,"name": resourceName, "visibility" : apiWebAppResourceVisibility</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiSetResourceVisibilityRequest(string webAppName, string resourceName, ApiWebAppResourceVisibility apiWebAppResourceVisibility, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckWebAppResourceVisibility(apiWebAppResourceVisibility, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.SetResourceVisibility", jsonRpcReq, idReq, new Dictionary<string, object>() { { "app_name", webAppName },
                { "name", resourceName }, { "visibility", apiWebAppResourceVisibility.ToString().ToLower() } });
        }
        /// <summary>
        /// get an Api.Version Request without parameters
        /// </summary>
        /// <returns>Api.Version Request without parameters</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiVersionRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Api.Version", jsonRpcReq, idReq);
        }
        /// <summary>
        /// get an WebApp.Browse Request with parameter "name" : webAppName (optional, might be null)
        /// </summary>
        /// <param name="webAppName">OPTIONAL: name of the webapp you want to browse</param>
        /// <returns>WebApp.Browse Request with parameter "name" : webAppName (optional, might be null)</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppBrowseRequest(string webAppName = null, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.Browse", jsonRpcReq, idReq, new Dictionary<string, object>() { { "name", webAppName } });
        }
        /// <summary>
        /// get an WebApp.BrowseResources Request with parameter "app_name" : webAppName, "name":resourceName (optional, might be null)
        /// </summary>
        /// <param name="webAppName">Name of the Webapp you want to browse the resources of</param>
        /// <param name="resourceName">OPTIONAL: name of the resource you want to browse</param>
        /// <returns>WebApp.BrowseResources Request with parameter "app_name" : webAppName, "name":resourceName (optional, might be null)</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppBrowseResourcesRequest(string webAppName, string resourceName = null, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.BrowseResources", jsonRpcReq, idReq, new Dictionary<string, object>() { { "app_name", webAppName },
                { "name", resourceName } });
        }
        /// <summary>
        /// check the request parameters: webAppName, if given WebAppState
        /// get an WebApp.Create Request with parameter "name" : webAppName, "state":apiWebAppState (optional, might be null)
        /// </summary>
        /// <param name="webAppName">webappname of the webapp to create</param>
        /// <param name="apiWebAppState">OPTIONAL: state the webapp should be in after creation</param>
        /// <returns>WebApp.Create Request with parameter "name" : webAppName, "state":apiWebAppState (optional, might be null)</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppCreateRequest(string webAppName, ApiWebAppState? apiWebAppState = null, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckWebAppName(webAppName, PerformCheck);
            if (apiWebAppState != null)
            {
                RequestParameterChecker.CheckWebAppState((ApiWebAppState)apiWebAppState, PerformCheck);
            }
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.Create", jsonRpcReq, idReq, new Dictionary<string, object>() { { "name", webAppName },
                { "state", apiWebAppState?.ToString().ToLower() } });
        }
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
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppCreateResourceRequest(string webAppName, string resourceName, string media_type,
            string last_modified, ApiWebAppResourceVisibility? apiWebAppResourceVisibility = null, string etag = null, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckResourceName(resourceName, PerformCheck);
            if (apiWebAppResourceVisibility != null)
            {
                RequestParameterChecker.CheckWebAppResourceVisibility((ApiWebAppResourceVisibility)apiWebAppResourceVisibility, PerformCheck);
            }
            if (etag != null)
            {
                RequestParameterChecker.CheckETag(etag, PerformCheck);
            }
            RequestParameterChecker.CheckMediaType(media_type, PerformCheck);
            RequestParameterChecker.CheckLastModified(last_modified, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.CreateResource", jsonRpcReq, idReq, new Dictionary<string, object>() { { "app_name", webAppName },
                { "name", resourceName }, { "media_type", media_type }, { "last_modified", last_modified },
                { "visibility", apiWebAppResourceVisibility?.ToString().ToLower() } , { "etag", etag } });
        }
        /// <summary>
        /// get an WebApp.Delete Request with parameter "name" : webAppName
        /// </summary>
        /// <param name="webAppName">Name of the webapp that should be deleted</param>
        /// <returns>WebApp.Delete Request with parameter "name" : webAppName</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppDeleteRequest(string webAppName, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.Delete", jsonRpcReq, idReq, new Dictionary<string, object>() { { "name", webAppName } });
        }
        /// <summary>
        /// get an WebApp.DeleteResource Request with parameter "app_name" : webAppName, "name": resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be deleted</param>
        /// <returns>WebApp.DeleteResource Request with parameter "app_name" : webAppName, "name": resourceName</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppDeleteResourceRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.DeleteResource", jsonRpcReq, idReq, new Dictionary<string, object>() { { "app_name", webAppName },
                { "name", resourceName } });
        }
        /// <summary>
        /// get an WebApp.DownloadResource Request with parameter "app_name" : webAppName, "name": resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be downloaded</param>
        /// <returns>WebApp.DownloadResource Request with parameter "app_name" : webAppName, "name": resourceName</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppDownloadResourceRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.DownloadResource", jsonRpcReq, idReq, new Dictionary<string, object>() { { "app_name", webAppName },
                { "name", resourceName } });
        }
        /// <summary>
        /// get an WebApp.Rename Request with parameter "name" : webAppName, "new_name": newWebAppName
        /// </summary>
        /// <param name="webAppName">Current name of the Webapp that is to be renamed</param>
        /// <param name="newWebAppName">New name for the Webapp</param>
        /// <returns>WebApp.Rename Request with parameter "name" : webAppName, "new_name": newWebAppName</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppRenameRequest(string webAppName, string newWebAppName, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckWebAppName(newWebAppName, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.Rename", jsonRpcReq, idReq, new Dictionary<string, object>() { { "name", webAppName },
                { "new_name", newWebAppName } });
        }
        /// <summary>
        /// check new resource name, if valid:
        /// get an WebApp.RenameResource Request with parameter "app_name" : webAppName, "name" : resourceName, "new_name": newResourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Current name of the resource that is to be renamed</param>
        /// <param name="newResourceName">New name for the resource</param>
        /// <returns>WebApp.RenameResource Request with parameter "app_name" : webAppName, "name" : resourceName, "new_name": newResourceName</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppRenameResourceRequest(string webAppName, string resourceName, string newResourceName, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckResourceName(newResourceName, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.RenameResource", jsonRpcReq, idReq, new Dictionary<string, object>() { { "app_name", webAppName },
                { "name", resourceName }, { "new_name", newResourceName } });
        }
        /// <summary>
        /// get a WebApp.SetDefaultPage Request with parameter "name" : webAppName, "resource_name" : resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be the Defaultpage</param>
        /// <returns>WebApp.SetDefaultPage Request with parameter "name" : webAppName, "resource_name" : resourceName</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppSetDefaultPageRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.SetDefaultPage", jsonRpcReq, idReq, new Dictionary<string, object>() { { "name", webAppName },
                { "resource_name", resourceName } });
        }
        /// <summary>
        /// get an WebApp.SetNotAuthorizedPage Request with parameter "name" : webAppName, "resource_name" : resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be the Notauthorizedpage</param>
        /// <returns>WebApp.SetNotAuthorizedPage Request with parameter "name" : webAppName, "resource_name" : resourceName</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppSetNotAuthorizedPageRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.SetNotAuthorizedPage", jsonRpcReq, idReq, new Dictionary<string, object>() { { "name", webAppName },
                { "resource_name", resourceName } });
        }
        /// <summary>
        /// get an WebApp.SetNotFoundPage Request with parameter "name" : webAppName, "resource_name" : resourceName
        /// </summary>
        /// <param name="webAppName">Name of the Webapp containing the resource</param>
        /// <param name="resourceName">Name of the resource that should be the Notfoundpage</param>
        /// <returns>WebApp.SetNotFoundPage Request with parameter "name" : webAppName, "resource_name" : resourceName</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppSetNotFoundPageRequest(string webAppName, string resourceName, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.SetNotFoundPage", jsonRpcReq, idReq, new Dictionary<string, object>() { { "name", webAppName },
                { "resource_name", resourceName } });
        }
        /// <summary>
        /// check apiwebappstate (none isnt valid)
        /// get an WebApp.SetState Request with parameter "name" : webAppName, "state" : apiWebAppState
        /// </summary>
        /// <param name="webAppName">Name of the Webapp the state should be changed for</param>
        /// <param name="apiWebAppState">New state for the Webapp</param>
        /// <returns>WebApp.SetState Request with parameter "name" : webAppName, "state" : apiWebAppState</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiWebAppSetStateRequest(string webAppName, ApiWebAppState apiWebAppState, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckWebAppState(apiWebAppState, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.SetState", jsonRpcReq, idReq, new Dictionary<string, object>() { { "name", webAppName },
                { "state", apiWebAppState.ToString().ToLower() } });
        }

        /// <summary>
        /// Method to make sure all requests in the ApiBulk have a unique Id
        /// </summary>
        /// <param name="IApiRequests">Api Requests to make sure of that the ids are unique</param>
        /// <param name="timeOut">timeout for the waithandler => plc to be up again after reboot, etc.</param>
        /// <returns>A list of api Requests containing unique Ids</returns>
        public virtual IEnumerable<IApiRequest> GetApiBulkRequestWithUniqueIds(IEnumerable<IApiRequest> IApiRequests, TimeSpan? timeOut = null)
        {
            var requestsToReturn = new List<IApiRequest>(IApiRequests.ToList());
            var startTime = DateTime.Now;
            var ignoreTimeOut = false;
            if (timeOut == null)
            {
                ignoreTimeOut = true;
            }
            while (requestsToReturn.GroupBy(el => el.Id).Count() != requestsToReturn.Count
                && (((startTime + timeOut) > DateTime.Now) || ignoreTimeOut))
            {
                IApiRequests.Where(el => IApiRequests.Any(el2 => el.Id == el2.Id))
                    .ToList().ForEach(el =>
                    {
                        el.Id = RequestIdGenerator.Generate();
                    });
            }
            return requestsToReturn;
        }

        /// <summary>
        /// In case you want to "build up" an IApiRequest on your own
        /// </summary>
        /// <param name="method">Api method to be called</param>
        /// <param name="requestParams">Api method parameters to be provided</param>
        /// <param name="jsonRpc">jsonrpc version (defaults to 2.0)</param>
        /// <param name="id">request id (defaults to a new generated id)</param>
        /// <returns>An IApiRequest to be sent to the plc</returns>
        public virtual IApiRequest GetIApiRequest(string method, Dictionary<string, object> requestParams = null, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest(method, jsonRpcReq, idReq, requestParams);
        }

        /// <summary>
        /// get an Plc.ReadSystemTime Request
        /// </summary>
        /// <returns>Plc.ReadSystemTime Request</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiPlcReadSystemTimeRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Plc.ReadSystemTime", jsonRpcReq, idReq);
        }

        /// <summary>
        /// get an Plc.SetSystemTime Request
        /// </summary>
        /// <returns>Plc.SetSystemTime Request</returns>
        /// <param name="timestamp">The timestamp of the system time to be set</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to JsonRpcVersion</param>
        public IApiRequest GetApiPlcSetSystemTimeRequest(DateTime timestamp, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckSystemTimeStamp(timestamp, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Plc.SetSystemTime", jsonRpcReq, idReq, new Dictionary<string, object>() { { "timestamp", timestamp.ToString(DateTimeFormatting.ApiDateTimeFormat) } });
        }
        /// <summary>
        /// get an Plc.ReadTimeSettings Request
        /// </summary>
        /// <returns>Plc.ReadTimeSettings Request</returns>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiPlcReadTimeSettingsRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Plc.ReadTimeSettings", jsonRpcReq, idReq);
        }

        /// <summary>
        /// Get an Plc.SetTimeSettings request
        /// </summary>
        /// <param name="utcOffset">The time zone offset from the UTC time in hours</param>
        /// <param name="daylightSavings">(Optional) Represents the settings for daylight-savings. If there is no daylight-savings rule configured, the utcOffset is applied to calculate the local time</param>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>Plc.SetTimeSettings Request</returns>
        public IApiRequest GetApiPlcSetTimeSettingsRequest(TimeSpan utcOffset, DaylightSavingsRule daylightSavings = null, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckTimeSettings(utcOffset, daylightSavings, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Plc.SetTimeSettings", jsonRpcReq, idReq,
                                      new Dictionary<string, object>()
                                      {
                                          { "utc_offset", utcOffset.ToISO8601Duration() },
                                          { "rule", daylightSavings }
                                      });
        }

        /// <summary>
        /// Call Equals with obj as IApiRequestFactory - check for Properties
        /// </summary>
        /// <param name="obj">to compare</param>
        /// <returns>wether the two are equal or not</returns>
        public override bool Equals(object obj) => Equals(obj as ApiRequestFactory);

        /// <summary>
        /// check for Properties
        /// </summary>
        /// <param name="obj">to compare</param>
        /// <returns>wether the two are equal or not</returns>
        public bool Equals(ApiRequestFactory obj)
        {
            var toReturn = this.PerformCheck == obj.PerformCheck;
            toReturn &= this.RequestIdGenerator.Equals(obj.RequestIdGenerator);
            toReturn &= this.RequestParameterChecker.Equals(obj.RequestParameterChecker);
            return toReturn;
        }

        /// <summary>
        /// GetHashCode for SequenceEqual etc.
        /// </summary>
        /// <returns>hashcode of the ApiTicket</returns>
        public override int GetHashCode()
        {
            var hashCode = 570990538;
            hashCode = hashCode * -1521134295 + EqualityComparer<IIdGenerator>.Default.GetHashCode(RequestIdGenerator);
            hashCode = hashCode * -1521134295 + EqualityComparer<IApiRequestParameterChecker>.Default.GetHashCode(RequestParameterChecker);
            hashCode = hashCode * -1521134295 + PerformCheck.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// get an Files.Browse Request
        /// </summary>
        /// <returns>Files.Browse Request</returns>
        /// <param name="resource">directory or file to be browsed</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>Files.Browse Request</returns>
        public IApiRequest GetApiFilesBrowseRequest(string resource, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Files.Browse", jsonRpcReq, idReq, new Dictionary<string, object>()
            { { "resource", resource } });
        }

        /// <summary>
        /// get an Files.Download Request
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="jsonRpc"></param>
        /// <param name="id"></param>
        /// <returns>Files.Download Request</returns>
        public IApiRequest GetApiFilesDownloadRequest(string resource, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Files.Download", jsonRpcReq, idReq, new Dictionary<string, object>()
            { { "resource", resource } });
        }

        /// <summary>
        /// get a Files.Create Request
        /// </summary>
        /// <param name="resource">Path of file</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <returns>Files.Create Request</returns>
        public IApiRequest GetApiFilesCreateRequest(string resource, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Files.Create", jsonRpcReq, idReq, new Dictionary<string, object>()
            { { "resource", resource } });
        }

        /// <summary>
        /// get a Files.Rename Request
        /// </summary>
        /// <param name="resource">Current path of file/folder</param>
        /// <param name="new_resource">New path of file/folder</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <returns>Files.Rename Request</returns>
        public IApiRequest GetApiFilesRenameRequest(string resource, string new_resource, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Files.Rename", jsonRpcReq, idReq, new Dictionary<string, object>()
            { { "resource", resource }, {"new_resource", new_resource } });
        }

        /// <summary>
        /// get a Files.Delete Request
        /// </summary>
        /// <param name="resource">Path of file</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <returns>Files.Delete Request</returns>
        public IApiRequest GetApiFilesDeleteRequest(string resource, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Files.Delete", jsonRpcReq, idReq, new Dictionary<string, object>()
            { { "resource", resource } });
        }

        /// <summary>
        /// get a Files.CreateDirectory Request
        /// </summary>
        /// <param name="resource">Path of directory</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <returns>Files.CreateDirectory Request</returns>
        public IApiRequest GetApiFilesCreateDirectoryRequest(string resource, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Files.CreateDirectory", jsonRpcReq, idReq, new Dictionary<string, object>()
            { { "resource", resource } });
        }

        /// <summary>
        /// get a Files.DeleteDirectory Request
        /// </summary>
        /// <param name="resource">Path of directory</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <returns>Files.DeleteDirectory Request</returns>
        public IApiRequest GetApiFilesDeleteDirectoryRequest(string resource, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Files.DeleteDirectory", jsonRpcReq, idReq, new Dictionary<string, object>()
            { { "resource", resource } });
        }


        /// <summary>
        /// get a Plc.CreateBackup Request
        /// </summary>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>Plc.CreateBackup Request</returns>
        public IApiRequest GetPlcCreateBackupRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Plc.CreateBackup", jsonRpcReq, idReq);
        }

        /// <summary>
        /// get a Plc.RestoreBackup Request
        /// </summary>
        /// <param name="password">Password for authentication</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>Plc.RetoreBackup Request</returns>
        public IApiRequest GetPlcRestoreBackupRequest(string password, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            var pwd = string.IsNullOrEmpty(password) ? string.Empty : password;
            var reqParams = new Dictionary<string, object>() { { "password", password?.ToString() } };
            return new ApiRequest("Plc.RestoreBackup", jsonRpcReq, idReq, reqParams);
        }

        /// <summary>
        /// get a DataLogs.DownloadAndClear Request
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>DataLogs.DownloadAndClear Request</returns>
        public IApiRequest GetApiDatalogsDownloadAndClearRequest(string resource, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("DataLogs.DownloadAndClear", jsonRpcReq, idReq, new Dictionary<string, object>()
            { { "resource", resource } });
        }

        /// <summary>
        /// Get a Failsafe.ReadParameters Request
        /// </summary>
        /// <param name="hwid">The hardware identifier from which the parameters shall be read</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>Failsafe.ReadParameters Request</returns>
        public IApiRequest GetFailsafeReadParametersRequest(uint hwid, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Failsafe.ReadParameters", jsonRpcReq, idReq, new Dictionary<string, object>()
            { { "hwid", hwid } });
        }

        /// <summary>
        /// Get a Failsafe.ReadRuntimeGroups Request
        /// </summary>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>Failsafe.ReadRuntimeGroups Request</returns>
        public IApiRequest GetFailsafeReadRuntimeGroupsRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Failsafe.ReadRuntimeGroups", jsonRpcReq, idReq);
        }

        /// <summary>
        /// Get a Project.ReadLanguages request without parameters
        /// </summary>
        /// <param name="mode">Determines whether all or only active languages should be returned. Default is 'active'.</param>
        /// <param name="id">Request Id</param>
        /// <param name="jsonRpc">JsonRpc to be used</param>
        /// <returns>Project.ReadLanguages request without parameters</returns>
        public IApiRequest GetApiProjectReadLanguagesRequest(ApiReadLanguagesMode? mode = null, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Project.ReadLanguages", jsonRpcReq, idReq, mode == null ? null : new Dictionary<string, object>() { { "mode", mode } });
        }

        /// <summary>
        /// Get a Modules.DownloadServiceData request
        /// </summary>
        /// <param name="hwid">The HWID of a node (module) for which a service data file can be downloaded</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.GetRandomString(8)</param>
        /// <returns>Modules.DownloadServiceData request</returns>
        public IApiRequest GetModulesDownloadServiceData(ApiPlcHwId hwid, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Modules.DownloadServiceData", jsonRpcReq, idReq, new Dictionary<string, object>()
                                        { { "hwid", hwid } });
        }
        /// <summary>
        /// Get a WebServer.ReadDefaultPage Request
        /// </summary>
        /// <param name="jsonRpc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IApiRequest GetApiWebserverReadDefaultPageRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebServer.ReadDefaultPage", jsonRpcReq, idReq);
        }

        /// <summary>
        /// Get a WebServer.SetDefaultPage Request 
        /// </summary>
        /// <param name="defaultPage"></param>
        /// <param name="jsonRpc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IApiRequest GetApiWebserverSetDefaultPageRequest(string defaultPage, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebServer.SetDefaultPage", jsonRpcReq, idReq, new Dictionary<string, object>() { { "default_page", defaultPage } });
        }

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
        public IApiRequest GetApiSyslogBrowseRequest(ApiPlcRedundancyId? redundancy_id = null, uint? count = null, uint? first = null, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            Dictionary<string, object> requestParams = new Dictionary<string, object>();
            if (redundancy_id != null && redundancy_id != 0)
            {
                requestParams.Add("redundancy_id", redundancy_id);
            }
            if (count != null)
            {
                requestParams.Add("count", count);
            }
            if (first != null)
            {
                requestParams.Add("first", first);
            }
            return new ApiRequest("Syslog.Browse", jsonRpcReq, idReq, requestParams);
        }

        /// <summary>
        /// Get a Alarms.Acknowledge request
        /// </summary>
        /// <returns>ApiAlarmsAcknowledgeRequest</returns>
        /// <param name="alarmId">Specifies the id of the alarm</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiAlarmsAcknowledgeRequest(string alarmId, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            Dictionary<string, object> requestParams = new Dictionary<string, object>() { { "id", alarmId } };
            return new ApiRequest("Alarms.Acknowledge", jsonRpcReq, idReq, requestParams);
        }

        /// <summary>
        /// Get a Alarms.Browse request
        /// </summary>
        /// <returns>ApiAlarmsBrowseRequest</returns>
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
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        public virtual IApiRequest GetApiAlarmsBrowseRequest(CultureInfo language,
                                                             int? count = null,
                                                             string alarm_id = null,
                                                             ApiAlarms_RequestFilters filters = null,
                                                             string jsonRpc = null,
                                                             string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            Dictionary<string, object> requestParams = new Dictionary<string, object>() { { "language", language.Name } };
            if (alarm_id != null)
            {
                requestParams.Add("alarm_id", alarm_id);
            }
            //If alarm_id is provided, count can't be provided as filter.
            if (count != null && alarm_id == null)
            {
                requestParams.Add("count", count);
            }
            if (filters != null)
            {
                requestParams.Add("filters", filters);
            }
            return new ApiRequest("Alarms.Browse", jsonRpcReq, idReq, requestParams);
        }

        /// <summary>
        /// Get a DiagnosticBuffer.Browse request
        /// </summary>
        /// <param name="language">Specifies the language of the response</param>
        /// <param name="count">Specifies maximum how many diagnosticbuffer entry you will get back</param>
        /// <param name="filters">ApiDiagnosticBufferBrowse_RequestFilters representing various filtering possibilities.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>ApiDiagnosticBufferBrowseRequest</returns>        
        public virtual IApiRequest GetApiDiagnosticBufferBrowseRequest(CultureInfo language,
                                                                       uint? count = null,
                                                                       ApiDiagnosticBuffer_RequestFilters filters = null,
                                                                       string jsonRpc = null,
                                                                       string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            Dictionary<string, object> requestParams = new Dictionary<string, object>() { { "language", language.Name } };
            if (count != null)
            {
                requestParams.Add("count", count);
            }
            if (filters != null)
            {
                requestParams.Add("filters", filters);
            }
            return new ApiRequest("DiagnosticBuffer.Browse", jsonRpcReq, idReq, requestParams);
        }

        /// <summary>
        /// Get a Technology.Read request
        /// </summary>
        /// <param name="var">Name of the variable to read. The name must not be empty.</param>
        /// <param name="mode">Enumeration that determines the response format</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>Technology.Read request</returns>
        public IApiRequest GetTechnologyReadRequest(string var, ApiPlcDataRepresentation? mode = null, string jsonRpc = null, string id = null)
        {
            RequestParameterChecker.CheckPlcDataRepresentationMode(mode, PerformCheck);
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            Dictionary<string, object> requestParams = new Dictionary<string, object>() { { "var", var } };
            if (mode != null)
            {
                requestParams.Add("mode", mode);
            }
            return new ApiRequest("Technology.Read", jsonRpcReq, idReq, requestParams);
        }

        /// <summary>
        /// Get a WebServer.ReadResponseHeaders request
        /// </summary>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>ApiWebServerReadResponseHeadersRequest</returns>  
        public virtual IApiRequest GetApiWebServerReadResponseHeadersRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebServer.ReadResponseHeaders", jsonRpcReq, idReq);
        }

        /// <summary>
        /// Get a WebServer.ChangeResponseHeaders request
        /// </summary>
        /// <param name="header">The HTTP response header to be returned when accessing URLs that match the given pattern.</param>
        /// <param name="pattern">The URL pattern for which the header must be returned. For now, this must always be set to /~**/*. Other values are not allowed. </param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>ApiWebServerChangeResponseHeadersRequest</returns>  
        public virtual IApiRequest GetApiWebServerChangeResponseHeadersRequest(string header = null, string pattern = "/~**/*", string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            Dictionary<string, object> requestParams;
            if (header != null)
            {
                requestParams = new Dictionary<string, object>()
                {
                    {"headers",
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"pattern", pattern },
                                {"header", header }
                            }
                        }
                    },
                };
            }
            else
            {
                requestParams = new Dictionary<string, object>()
                {
                    {"headers", new List<Dictionary<string, object>>()}
                };
            }
            return new ApiRequest("WebServer.ChangeResponseHeaders", jsonRpcReq, idReq, requestParams);
        }

        /// <summary>
        /// Get a Redundancy.ReadSyncupProgress request
        /// </summary>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>ApiRedundancyReadSyncupProgressRequest</returns>  
        public virtual IApiRequest GetApiRedundancyReadSyncupProgressRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Redundancy.ReadSyncupProgress", jsonRpcReq, idReq);
        }

        /// <summary>
        /// Get a Technology.BrowseObjects request
        /// </summary>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>Technology.BrowseObjects request</returns>
        public IApiRequest GetTechnologyBrowseObjectsRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Technology.BrowseObjects", jsonRpcReq, idReq);
        }

        /// <summary>
        /// Get a Redundancy.ReadSystemInformation request
        /// </summary>
        public virtual IApiRequest GetApiRedundancyReadSystemInformationRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Redundancy.ReadSystemInformation", jsonRpcReq, idReq);
        }

        /// <summary>
        /// Get a Redundancy.ReadSystemState request
        /// </summary>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>ApiDiagnosticBufferBrowseRequest</returns>  
        public virtual IApiRequest GetApiRedundancyReadSystemStateRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Redundancy.ReadSystemState", jsonRpcReq, idReq);
        }

        /// <summary>
        /// Get a Redundancy.RequestChangeSystemState request
        /// </summary>
        /// <param name="state">The requested system state for the R/H system.</param>
        /// <param name="id">Request Id, defaults to RequestIdGenerator.Generate()</param>
        /// <param name="jsonRpc">JsonRpc to be used - defaults to  JsonRpcVersion</param>
        /// <returns>ApiRedundancyRequestChangeSystemStateRequest</returns>  
        public virtual IApiRequest GetApiRedundancyRequestChangeSystemStateRequest(ApiPlcRedundancySystemState state, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            Dictionary<string, object> requestParams = new Dictionary<string, object>() { { "state", state.ToString().ToLower() } };
            return new ApiRequest("Redundancy.RequestChangeSystemState", jsonRpcReq, idReq, requestParams);
        }

        /// <summary>
        /// Get a WebApp.SetVersion request
        /// </summary>
        /// <param name="webAppName">The application in which the resource is located.</param>
        /// <param name="version">The version of the application. The string may be empty to reset the version string.</param>
        /// <param name="jsonRpc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IApiRequest GetApiWebAppSetVersionRequest(string webAppName, string version, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            Dictionary<string, object> requestParams = new Dictionary<string, object>()
            {
                { "name", webAppName },
                { "version", version }
            };
            return new ApiRequest("WebApp.SetVersion", jsonRpcReq, idReq, requestParams);
        }

        /// <summary>
        /// Get a WebApp.SetUrlRedirectMode request
        /// </summary>
        /// <param name="webAppName">The application for which the redirect mode shall be changed.</param>
        /// <param name="redirect_mode">The redirect mode of the application. </param>
        /// <param name="jsonRpc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IApiRequest GetApiWebAppSetUrlRedirectModeRequest(string webAppName, ApiWebAppRedirectMode redirect_mode, string jsonRpc = null, string id = null)
        {
            Dictionary<string, object> requestParams = new Dictionary<string, object>()
            {
                { "name", webAppName },
                { "redirect_mode", redirect_mode.ToString().ToLower() }
            };
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("WebApp.SetUrlRedirectMode", jsonRpcReq, idReq, requestParams);
        }

        /// <summary>
        /// Get a Plc.ReadCpuType request
        /// </summary>
        /// <param name="jsonRpc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual IApiRequest GetApiPlcReadCpuTypeRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Plc.ReadCpuType", jsonRpcReq, idReq);
        }

        /// <summary>
        /// Get a Plc.ReadStationName request
        /// </summary>
        /// <param name="jsonRpc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IApiRequest GetApiPlcReadStationNameRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Plc.ReadStationName", jsonRpcReq, idReq);
        }

        /// <summary>
        /// Get a Plc.ReadModuleName request
        /// </summary>
        /// <param name="redundancy_id">
        /// The Redundancy ID parameter must be present when the request is executed on an R/H PLC. It must either have a value of 1 or 2. <br/> 
        /// On non-R/H PLCs, the parameter must not be part of the request.</param>
        /// <param name="jsonRpc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IApiRequest GetApiPlcReadModuleNameRequest(uint? redundancy_id = null, string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            if (redundancy_id != null)
            {
                Dictionary<string, object> requestParams = new Dictionary<string, object>() { { "redundancy_id", redundancy_id } };
                return new ApiRequest("Plc.ReadModuleName", jsonRpcReq, idReq, requestParams);
            }
            return new ApiRequest("Plc.ReadModuleName", jsonRpcReq, idReq);
        }

        /// <summary>
        /// Get a GetSessionInfo request
        /// </summary>
        /// <param name="jsonRpc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual IApiRequest GetApiGetSessionInfoRequest(string jsonRpc = null, string id = null)
        {
            string jsonRpcReq = jsonRpc ?? JsonRpcVersion;
            string idReq = id ?? RequestIdGenerator.Generate();
            return new ApiRequest("Api.GetSessionInfo", jsonRpcReq, idReq);
        }
    }
}
