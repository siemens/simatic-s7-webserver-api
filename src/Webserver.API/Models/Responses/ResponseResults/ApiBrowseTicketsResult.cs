// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// BrowseTickets Result - Containing Max Tickets and Tickets (as a List)!
    /// </summary>
    public class ApiBrowseTicketsResult
    {
        /// <summary>
        /// as of 2.9: 4
        /// </summary>
        public uint Max_Tickets { get; set; }
        /// <summary>
        /// Array of current user tickets
        /// </summary>
        public List<ApiTicket> Tickets { get; set; }
    }
}
