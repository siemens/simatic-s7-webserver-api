// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// Response of Modules.ReadParameters
    /// </summary>
    public class ModulesParametersResponse : ApiResultResponse<ModulesParameters>
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
