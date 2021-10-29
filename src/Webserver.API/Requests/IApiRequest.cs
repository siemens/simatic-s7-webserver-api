// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
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
    public interface IApiRequest
    {
        /// <summary>
        /// Dictionary to contain the according request Parameters (string:key, object:value)
        /// </summary>
        Dictionary<string, object> Params { get; set; }
        /// <summary>
        /// Request Id
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Request JsonRpc Protocol version (prob. 2.0)
        /// </summary>
        string JsonRpc { get; set; }
        /// <summary>
        /// Request Method (e.g. Api.Browse)
        /// </summary>
        string Method { get; set; }
    }
}
