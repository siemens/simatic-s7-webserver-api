// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Exceptions;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// TrueOnSuccessResponse - Api Responses with "true" to indicate success - never false so an errorcode is given in case something goes wrong
    /// </summary>
    public class ApiTrueOnSuccessResponse : ApiResultResponse<bool>
    {
        /// <summary>
        /// True on SuccessResponse: Only Accept "true"
        /// </summary>
        public override bool Result
        {
            get => base.Result; set
            {
                if (value)
                {
                    base.Result = value;
                }
                else
                {
                    throw new ApiInvalidResponseException($"Server responded with \"{value}\" for a true on success response!?");
                }

            }
        }
    }
}
