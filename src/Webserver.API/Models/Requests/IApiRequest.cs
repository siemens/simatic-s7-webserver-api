// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Requests
{
    /// <summary>
    /// ApiRequest: Containing the required JsonRpc, Id and Method and optional Params for a Request to a PLC1500
    /// </summary>
    public interface IApiRequest : IApiBaseRequest<string>
    {
    }

    /// <summary>
    /// ApiRequest: Containing the required JsonRpc, Id and Method and optional Params for a Request to a PLC1500
    /// </summary>
    public interface IApiRequestIntId : IApiBaseRequest<int>
    {
    }

    /// <summary>
    /// ApiRequest: Containing the required JsonRpc, Id and Method and optional Params for a Request to a PLC1500
    /// </summary>
    public interface IApiBaseRequest<T>
    {
        /// <summary>
        /// Dictionary to contain the according request Parameters (string:key, object:value)
        /// </summary>
        Dictionary<string, object> Params { get; set; }
        /// <summary>
        /// Request Id
        /// </summary>
        T Id { get; set; }
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
