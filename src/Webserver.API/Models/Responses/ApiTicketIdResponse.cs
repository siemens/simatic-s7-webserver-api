// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT

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
            this.Id = singleStringResponse.Id;
            this.JsonRpc = singleStringResponse.JsonRpc;
            this.Result = singleStringResponse.Result;
        }
    }
}
