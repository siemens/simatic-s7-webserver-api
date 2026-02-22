// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// ApiResponse (Jsonrpc,id) with an Api Quantity Structure object
    /// </summary>
    public class ApiGetQuantityStructuresResponse : ApiResultResponse<ApiQuantityStructure>
    {
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
