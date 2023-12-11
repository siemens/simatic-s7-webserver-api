// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT

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
    }
}
