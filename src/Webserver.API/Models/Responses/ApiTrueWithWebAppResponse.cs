// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// Response for ApiMethods that change a WebApp
    /// </summary>
    public class ApiTrueWithWebAppResponse
    {
        /// <summary>
        /// TrueOnSuccesResponse by the PLC
        /// </summary>
        public ApiTrueOnSuccessResponse TrueOnSuccesResponse { get; set; }

        /// <summary>
        /// WebAppData adjusted(created) by the Implementation of the Methods
        /// </summary>
        public ApiWebAppData NewWebApp { get; set; }
    }
}
