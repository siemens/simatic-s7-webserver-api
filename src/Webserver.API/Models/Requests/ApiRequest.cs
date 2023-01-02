// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Models.Requests
{
    /// <summary>
    /// ApiRequest: Containing the required JsonRpc, Id and Method and optional Params for a Request to a PLC1500
    /// </summary>
    public class ApiRequest : ApiBaseRequest<string>, IApiRequest, IApiBaseRequest<string>
    {
        /// <summary>
        /// Get the ApiRequest (e.g. Api.Browse) with the given parameters
        /// </summary>
        /// <param name="method">The name of the request (e.g. Api.Browse)</param>
        /// <param name="jsonRpc">The required JsonRpc</param>
        /// <param name="id">The required ID</param>
        /// <param name="requestParams">Optional parameters for those Api Request where it is possible (e.g. Files.Rename)</param>
        public ApiRequest(string method, string jsonRpc, string id, Dictionary<string, object> requestParams = null) 
            : base(method, jsonRpc, id, requestParams)
        {
        }
    }

    /// <summary>
    /// ApiRequest: Containing the required JsonRpc, Id and Method and optional Params for a Request to a PLC1500
    /// </summary>
    public class ApiRequestIntId : ApiBaseRequest<int>, IApiRequestIntId, IApiBaseRequest<int>
    {

        /// <summary>
        /// Get the ApiRequest (e.g. Api.Browse) with the given parameters
        /// </summary>
        /// <param name="method">The name of the request (e.g. Api.Browse)</param>
        /// <param name="jsonRpc">The required JsonRpc</param>
        /// <param name="id">The required ID as an Integer</param>
        /// <param name="requestParams">Optional parameters for those Api Request where it is possible (e.g. Files.Rename)</param>
        public ApiRequestIntId(string method, string jsonRpc, int id, Dictionary<string, object> requestParams = null) 
            : base(method, jsonRpc, id, requestParams)
        {
        }
    }

    /// <summary>
    /// ApiRequest: Containing the required JsonRpc, Id and Method and optional Params for a Request to a PLC1500
    /// </summary>
    public class ApiBaseRequest<T> : IApiBaseRequest<T>
    {
        /// <summary>
        /// Request JsonRpc Protocol version (prob. 2.0)
        /// </summary>
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; }
        /// <summary>
        /// Request Id
        /// </summary>
        public T Id { get; set; }
        /// <summary>
        /// Request Method (e.g. Api.Browse)
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// Dictionary to contain the according request Parameters (string:key, object:value)
        /// </summary>
        public Dictionary<string, object> Params { get; set; }

        /// <summary>
        /// Create a new Request with the given parameters
        /// </summary>
        /// <param name="method">Api Request Method (e.g. Api.Browse)</param>
        /// <param name="jsonRpc">jsonrpc: JavaScript Object Notation Remote Procedure Call protocol version</param>
        /// <param name="id">request Id</param>
        /// <param name="requestParams">Request parameters (can also be null)</param>
        public ApiBaseRequest(string method, string jsonRpc, T id, Dictionary<string, object> requestParams = null)
        {
            this.Method = method ?? throw new ArgumentNullException(nameof(method));
            this.Params = requestParams;
            this.JsonRpc = jsonRpc;
            this.Id = id;
        }
    }

}
