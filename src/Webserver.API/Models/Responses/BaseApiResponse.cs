// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// Generally an ApiResponse contains "Id" and "JsonRpc"
    /// </summary>
    public abstract class BaseApiResponse
    {
        /// <summary>
        /// Id of the Response (Matches the Id of the according Request)
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// JsonRpc of the Response (Matches the Id of the according Request)
        /// </summary>
        public string JsonRpc { get; set; }
    }
}
