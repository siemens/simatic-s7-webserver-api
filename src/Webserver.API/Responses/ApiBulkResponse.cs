// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Responses
{
    /// <summary>
    /// Response to an Api Bulk Request
    /// </summary>
    public class ApiBulkResponse
    {
        /// <summary>
        /// Error Responses - empty List if there are none
        /// </summary>
        public IEnumerable<ApiErrorModel> ErrorResponses;

        /// <summary>
        /// Successful Responses - empty List if there are none
        /// </summary>
        public IEnumerable<ApiResultResponse<object>> SuccessfulResponses;

        /// <summary>
        /// Boolean that indicates whether there have been Errors or not
        /// </summary>
        public bool ContainsErrors => ErrorResponses.Count() != 0;

        /// <summary>
        /// Default c'tor: initialize the ErrorResponses and Successfulresponses to an Empty List
        /// </summary>
        public ApiBulkResponse()
        {
            this.ErrorResponses = new List<ApiErrorModel>();
            this.SuccessfulResponses = new List<ApiResultResponse<object>>();
        }
    }
}
