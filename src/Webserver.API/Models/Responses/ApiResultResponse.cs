// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

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
        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
