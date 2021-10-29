// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.ResponseResults
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
