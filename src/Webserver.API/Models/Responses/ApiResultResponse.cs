// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// Generally an ApiResponse contains "Id" and "JsonRpc"
    /// </summary>
    public class ApiResultResponse<T> : BaseApiResponse
    {
        /// <summary>
        /// The (requested) Result the Api has responded with
        /// </summary>
        public virtual T Result { get; set; }
    }
}
