// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// A set of criteria defined for passwords.
    /// </summary>
    public class ApiPasswordPolicy
    {
        /// <summary>
        /// Minimum length of passwords
        /// </summary>
        public int Min_password_length { get; set; }
        /// <summary>
        /// Maximum length of passwords
        /// </summary>
        public int Max_password_length { get; set; }
        /// <summary>
        /// Minimum number of digits required in a password
        /// </summary>
        public int Min_digits { get; set; }
        /// <summary>
        /// Minimum number of special characters required in a password
        /// </summary>
        public int Min_special_characters { get; set; }
        /// <summary>
        /// If true, the password must contain uppercase letters.
        /// </summary>
        public bool Requires_uppercase_characters { get; set; }
        /// <summary>
        /// If true, the password must contain lowercase letters.
        /// </summary>
        public bool Requires_lowercase_characters { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiPasswordPolicy policy &&
                   Min_password_length == policy.Min_password_length &&
                   Max_password_length == policy.Max_password_length &&
                   Min_digits == policy.Min_digits &&
                   Min_special_characters == policy.Min_special_characters &&
                   Requires_uppercase_characters == policy.Requires_uppercase_characters &&
                   Requires_lowercase_characters == policy.Requires_lowercase_characters;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Min_special_characters, Min_password_length, Min_digits, Max_password_length, Requires_uppercase_characters, Requires_lowercase_characters).GetHashCode();
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
