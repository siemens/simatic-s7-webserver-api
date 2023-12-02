// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// ApiErrors always contain ErrorCodes and ErrorMessages
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// ErrorCode sent in the Response
        /// </summary>
        public ApiErrorCode Code { get; set; }

        /// <summary>
        /// Message sent in the Response
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// true if properties match
        /// </summary>
        /// <param name="obj">to compare</param>
        /// <returns>true if properties match</returns>
        public override bool Equals(object obj)
        {
            var error = obj as ApiError;
            return error != null &&
                   Code == error.Code &&
                   Message == error.Message;
        }

        /// <summary>
        /// GetHashCode for SequenceEqual etc.
        /// </summary>
        /// <returns>hashcode of the ApiError</returns>
        public override int GetHashCode()
        {
            var hashCode = -1809243720;
            hashCode = hashCode * -1521134295 + Code.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            return hashCode;
        }
    }
}
