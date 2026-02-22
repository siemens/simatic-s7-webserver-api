// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// ApiPlcProgramBrowseCodeBlocksResponse (Jsonrpc,id) with a List of ApiPlcProgramBrowseCodeBlocksData
    /// </summary>
    public class ApiPlcProgramBrowseCodeBlocksResponse : ApiResultResponse<List<ApiPlcProgramBrowseCodeBlocksData>>
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
