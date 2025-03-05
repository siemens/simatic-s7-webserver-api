// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Data Class for parameterization of HttpClient configuration
    /// </summary>
    public class HttpClientConnectionConfiguration
    {
        /// <summary>
        /// Function to MemberwiseClone a HttpClientConnectionConfiguration to another Object.
        /// </summary>
        /// <returns></returns>
        public HttpClientConnectionConfiguration ShallowCopy()
        {
            return (HttpClientConnectionConfiguration)this.MemberwiseClone();
        }

        /// <summary>
        /// Mandatory
        /// PLC base Address/DNS name
        /// </summary>
        public readonly string BaseAddress;

        /// <summary>
        /// Mandatory
        /// Plc User Management username to login with
        /// </summary>
        public readonly string Username;

        /// <summary>
        /// Mandatory
        /// Plc User Management password for Username
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Optional
        /// defaults to false
        /// </summary>
        public readonly bool ConnectionClose;

        /// <summary>
        /// Optional 
        /// defaults to ten minutes
        /// </summary>
        public readonly TimeSpan TimeOut;

        /// <summary>
        /// Optional
        /// defaults to false
        /// </summary>
        public readonly bool AllowAutoRedirect;

        /// <summary>
        /// Optional defaults to true
        /// </summary>
        public readonly bool DiscardPasswordAfterConnect;

        /// <summary>
        /// c'tor
        /// </summary>
        /// <param name="baseAddress">PLC base Address/DNS name</param>
        /// <param name="username">Plc User Management username to login with</param>
        /// <param name="password">Plc User Management password for Username</param>
        /// <param name="timeOut">timeout for the waithandler => plc to be up again after reboot, etc.</param>
        /// <param name="connectionClose">Defaults to false</param>
        /// <param name="allowAutoRedirect">Defaults to false</param>
        /// <param name="discardPasswordAfterConnect">Defaults to true</param>
        public HttpClientConnectionConfiguration(string baseAddress, string username, string password,
            TimeSpan timeOut, bool connectionClose, bool allowAutoRedirect, bool discardPasswordAfterConnect)
        {
            this.BaseAddress = baseAddress;
            this.Username = username;
            this.Password = password;
            this.TimeOut = timeOut;
            this.ConnectionClose = connectionClose;
            this.AllowAutoRedirect = allowAutoRedirect;
            this.DiscardPasswordAfterConnect = discardPasswordAfterConnect;
        }

        /// <summary>
        /// Method to discard password
        /// </summary>
        public void DiscardPassword()
        {
            this.Password = null;
        }

        /// <summary>
        /// (Name, Has_children, Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only) Equal!;
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as HttpClientConnectionConfiguration);

        /// <summary>
        /// Equals => ()
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(HttpClientConnectionConfiguration obj)
        {
            if (obj is null)
                return false;
            var toReturn = this.BaseAddress == obj.BaseAddress;
            toReturn &= this.Username == obj.Username;
            toReturn &= this.Password == obj.Password;
            toReturn &= this.ConnectionClose == obj.ConnectionClose;
            toReturn &= this.AllowAutoRedirect == obj.AllowAutoRedirect;
            toReturn &= this.DiscardPasswordAfterConnect == obj.DiscardPasswordAfterConnect;
            toReturn &= this.TimeOut.Equals(obj.TimeOut);
            return toReturn;
        }

        /// <summary>
        /// GetHashCode for SequenceEqual etc.
        /// </summary>
        /// <returns>hashcode of the connectionConfiguration</returns>
        public override int GetHashCode()
        {
            var hashCode = 939475008;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BaseAddress);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Username);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Password);
            hashCode = hashCode * -1521134295 + ConnectionClose.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(TimeOut);
            hashCode = hashCode * -1521134295 + AllowAutoRedirect.GetHashCode();
            hashCode = hashCode * -1521134295 + DiscardPasswordAfterConnect.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
