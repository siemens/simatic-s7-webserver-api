// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible Api ticket states
    /// </summary>
    public enum ApiTicketState
    {
        /// <summary>
        /// Should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// The Ticket has been created and no further functions have been executed for the ticket
        /// </summary>
        Created = 1,
        /// <summary>
        /// The Ticket is currently active (e.g. during download/upload)
        /// </summary>
        Active = 2,
        /// <summary>
        /// Currently never the case
        /// </summary>
        Busy = 3,
        /// <summary>
        /// After successful handling of the Ticket functions the Ticket will be in Completed state
        /// </summary>
        Completed = 4,
        /// <summary>
        /// The Ticket is in Failed state => something went wrong during Ticket execution 
        /// => Upload/Download has not been successful etc.
        /// </summary>
        Failed = 5

    }
}
