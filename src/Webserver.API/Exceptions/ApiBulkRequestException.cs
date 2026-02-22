// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using System;
using System.Linq;
using System.Text;

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
            StringBuilder message = new StringBuilder($"During Bulk request for {allMessages} there have been {errorMessages} Errors:" +
                $"{Environment.NewLine}For details: Check the Property {nameof(BulkResponse)}, errors:");
            foreach (var error in bulkResponse.ErrorResponses)
            {
                message.AppendLine($"{JsonConvert.SerializeObject(error.Error)}");
            }
            return message.ToString();
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
