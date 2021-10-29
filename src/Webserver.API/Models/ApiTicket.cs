// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;

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
        public string Id { get { return id; }
            set {
                if (value.Length != 28)
                    throw new ApiInvalidTicketIdValueException($"- given string:{value}");
                else
                    id = value;
            } }

        private ApiTicketState state;

        /// <summary>
        /// ApiTicketState - None is not a valid State! there are no Stateless Tickets (so far)
        /// </summary>
        public ApiTicketState State {
            get
            {
                return state;
            }
            set
            {
                if(value == ApiTicketState.None)
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
                if(value == ApiTicketProvider.None)
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

    }

}
