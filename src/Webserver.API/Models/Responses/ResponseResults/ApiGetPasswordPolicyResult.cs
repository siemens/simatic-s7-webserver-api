// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Result of Api.GetPasswordPolicy
    /// </summary>
    public class ApiGetPasswordPolicyResult
    {
        /// <summary>
        /// Currently configured password policy
        /// </summary>
        public ApiPasswordPolicy Password_policy { get; set; }

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
