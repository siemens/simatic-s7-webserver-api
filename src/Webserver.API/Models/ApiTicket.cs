// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// ApiTicket - id will always have 28 characters, can be used to get further information about a ticket - like
    /// State and for example if state is failed (planned:) reason or message => detailed error information what went wrong
    /// </summary>
    public class ApiTicket
    {

        private string id;
        /// <summary>
        /// 28 byte string!
        /// </summary>
        public string Id
        {
            get { return id; }
            set
            {
                if (value.Length != 28)
                    throw new ApiInvalidTicketIdValueException($"- given string:{value}");
                else
                    id = value;
            }
        }

        private ApiTicketState state;

        /// <summary>
        /// ApiTicketState - None is not a valid State! there are no Stateless Tickets (so far)
        /// </summary>
        public ApiTicketState State
        {
            get
            {
                return state;
            }
            set
            {
                if (value == ApiTicketState.None)
                {
                    throw new Exception($"{value.ToString()} is not an expected value for ApiTicketState!");
                }
                state = value;
            }
        }

        /// <summary>
        /// Date Created 
        /// </summary>
        public DateTime Date_created { get; set; }

        private ApiTicketProvider provider;

        /// <summary>
        /// Ticket Provider - e.g. WebApp.DownloadResource, WebApp.CreateResource (ticket "origin")
        /// </summary>
        public ApiTicketProvider Provider
        {
            get
            {
                return provider;
            }
            set
            {
                if (value == ApiTicketProvider.None)
                {
                    //throw new ApiException(new Responses.ApiErrorModel() { Error = new ApiError() {  } })
                    throw new Exception($"{value.ToString()} is not an expected value for ApiTicketProvider!");
                }
                provider = value;
            }
        }
        /// <summary>
        /// Ticket Data
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// File that has been downloaded
        /// </summary>
        public FileInfo File_Downloaded { get; set; }

        /// <summary>
        /// Check wether properties match
        /// </summary>
        /// <param name="obj">to compare</param>
        /// <returns>true if properties match</returns>
        public override bool Equals(object obj)
        {
            var ticket = obj as ApiTicket;
            return ticket != null &&
                   Id == ticket.Id &&
                   State == ticket.State &&
                   Date_created == ticket.Date_created &&
                   Provider == ticket.Provider &&
                   EqualityComparer<object>.Default.Equals(Data, ticket.Data);
        }

        /// <summary>
        /// GetHashCode for SequenceEqual etc.
        /// </summary>
        /// <returns>hashcode of the ApiTicket</returns>
        public override int GetHashCode()
        {
            var hashCode = -577061235;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + State.GetHashCode();
            hashCode = hashCode * -1521134295 + Date_created.GetHashCode();
            hashCode = hashCode * -1521134295 + Provider.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Data);
            return hashCode;
        }
    }

}
