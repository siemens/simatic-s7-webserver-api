// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// The ApiticketIdResponse is an ApiSingleStringResponse that contains a string that must match 28 characters 
    /// </summary>
    public class ApiTicketIdResponse : ApiSingleStringResponse
    {
        /// <summary>
        /// The ApiticketIdResponse is an ApiSingleStringResponse that contains a string that must match 28 characters 
        /// </summary>
        public ApiTicketIdResponse() : base()
        {

        }

        /// <summary>
        /// Create a TicketIdResponse from an ApiSingleStringResponse (copy the strings to the strings of the TicketIdResponse)
        /// </summary>
        /// <param name="singleStringResponse"></param>
        public ApiTicketIdResponse(ApiSingleStringResponse singleStringResponse) : base()
        {
            this.Id = string.Copy(singleStringResponse.Id);
            this.JsonRpc = string.Copy(singleStringResponse.JsonRpc);
            this.Result = string.Copy(singleStringResponse.Result);
        }

        /// <summary>
        /// Check Validity => RequestParameterChecker.CheckTicket(this.Result); => Does the TicketId match 28 characters length?
        /// </summary>
        public void CheckValidity(IApiRequestParameterChecker requestParameterChecker)
        {
            requestParameterChecker.CheckTicket(this.Result, true);
        }
    }
}
