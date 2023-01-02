// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The Ticket is not in Completed State but was expected to be so
    /// </summary>
    public class ApiTicketNotInCompletedStateException : Exception
    {
        /// <summary>
        /// The Ticket is not in Completed State but was expected to be so => give further information about the Ticket
        /// </summary>
        /// <param name="ticket"></param>
        public ApiTicketNotInCompletedStateException(ApiTicket ticket) :
            base($"Ticket: {ticket.Id + Environment.NewLine} is not in completed state! " +
                $"instead: {ticket.State.ToString() + Environment.NewLine} " +
                $"further ticket Information:{Environment.NewLine}" +
                $"ticket Provider: { ticket.Provider.ToString() + Environment.NewLine}" +
                $"date created: { ticket.Date_created }" +
                ((ticket.Data != null && ticket.Data.ToString() != "{}") ? $"ticket data: {ticket.Data.ToString()}":"")) { }
    }
}
