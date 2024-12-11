// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Holds password expiration information.
    /// </summary>
    public class ApiPasswordExpiration
    {
        /// <summary>
        /// The UTC timestamp, when the user password will expire. 
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Indicates to the user if the warning threshold for the password has been reached.
        /// </summary>
        public bool Warning { get; set; }

        /// <summary>
        /// Compares this to input object
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if objects are the same</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiPasswordExpiration expiration &&
                   Timestamp == expiration.Timestamp &&
                   Warning == expiration.Warning;
        }

        /// <summary>
        /// Get Hash Code of PasswordExpiraton object
        /// </summary>
        /// <returns>Hash Code</returns>
        public override int GetHashCode()
        {
            return (Timestamp, Warning).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiPasswordExpiration
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Timestamp)}: {Timestamp} | " +
                   $"{nameof(Warning)}: {Warning}";
        }
    }
}
