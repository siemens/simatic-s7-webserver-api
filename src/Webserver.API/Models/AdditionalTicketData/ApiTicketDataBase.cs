// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.AdditionalTicketData
{
    /// <summary>
    /// Base class for the different providers
    /// </summary>
    public class ApiTicketDataBase
    {
        /// <summary>
        /// The amount of data downloaded/uploaded from/to the according Ticket - will be updated with every call to Api.BrowseTickets.
        /// </summary>
        public uint Bytes_processed;

        /// <summary>
        /// TBD!
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public ApiTicketDataBase()
        {
            throw new NotImplementedException("!!");
        }
    }
}
