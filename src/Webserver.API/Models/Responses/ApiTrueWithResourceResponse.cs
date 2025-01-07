// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// Response for ApiMethods that change a resource
    /// </summary>
    public class ApiTrueWithResourceResponse
    {
        /// <summary>
        /// TrueOnSuccesResponse by the PLC
        /// </summary>
        public ApiTrueOnSuccessResponse TrueOnSuccesResponse;

        /// <summary>
        /// ResourceData adjusted(created) by the Implementation of the Methods
        /// </summary>
        public ApiWebAppResource NewResource;
    }
}
