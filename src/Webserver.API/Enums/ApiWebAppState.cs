// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible api Webapp states => determines wether (get) requests (e.g. browser: access plc webapp) to the endpoint are responded with the requested resources or not
    /// </summary>
    public enum ApiWebAppState
    {
        /// <summary>
        /// Should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// The WebApp is enabled => When sending (GET) requests to the according endpoint the resource requested will be returned in the response
        /// </summary>
        Enabled = 1,
        /// <summary>
        /// The WebApp is enabled => When sending (GET) requests to the according endpoint 503 FORBIDDEN will be responded
        /// </summary>
        Disabled = 2
    }
}
