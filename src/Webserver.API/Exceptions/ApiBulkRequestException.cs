// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using System;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Exception to get ahead of multiple error messages in response to a bulk request
    /// </summary>
    public class ApiBulkRequestException : Exception
    {
        private static string GetErrorMessage(ApiBulkResponse bulkResponse)
        {
            int allMessages = bulkResponse.ErrorResponses.Count() + bulkResponse.SuccessfulResponses.Count();
            int errorMessages = bulkResponse.ErrorResponses.Count();
            string message = $"During Bulk request for {allMessages} there have been {errorMessages} Errors:" +
                $"{Environment.NewLine}For details: Check the Property {nameof(BulkResponse)}";
            return message;
        }

        /// <summary>
        /// Bulk Response from PLC
        /// </summary>
        public ApiBulkResponse BulkResponse { get; private set; }
        /// <summary>
        /// Bulk Request Exceptions
        /// </summary>
        /// <param name="bulkResponse">bulk Response</param>
        public ApiBulkRequestException(ApiBulkResponse bulkResponse) : base(GetErrorMessage(bulkResponse))
        {
            BulkResponse = bulkResponse;
        }
    }
}
