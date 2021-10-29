// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Requests
{
    /// <summary>
    /// ApiRequest: Containing the required JsonRpc, Id and Method and optional Params for a Request to a PLC1500
    /// </summary>
    public class ApiRequest : IApiRequest
    {
        /// <summary>
        /// Request JsonRpc Protocol version (prob. 2.0)
        /// </summary>
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; }
        /// <summary>
        /// Request Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Request Method (e.g. Api.Browse)
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// Dictionary to contain the according request Parameters (string:key, object:value)
        /// </summary>
        public Dictionary<string,object> Params { get; set; }

        /// <summary>
        /// Create a new Request with the given parameters
        /// </summary>
        /// <param name="method">Api Request Method (e.g. Api.Browse)</param>
        /// <param name="jsonRpc">jsonrpc: JavaScript Object Notation Remote Procedure Call protocol version</param>
        /// <param name="id">request Id</param>
        /// <param name="requestParams">Request parameters (can also be null)</param>
        public ApiRequest(string method, string jsonRpc, string id, Dictionary<string, object> requestParams = null)
        {
            this.Method = method ?? throw new ArgumentNullException(nameof(method));
            this.Params = requestParams;
            this.JsonRpc = jsonRpc;
            this.Id = id;
        }
    }
}
