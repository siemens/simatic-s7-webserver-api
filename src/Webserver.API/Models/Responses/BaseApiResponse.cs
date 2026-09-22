// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

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
