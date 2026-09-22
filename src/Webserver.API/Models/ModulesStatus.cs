// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// The status information of the given HWID
    /// </summary>
    public class ModulesStatusDetails
    {
        /// <summary>
        /// The own state of the given HWID
        /// </summary>
        public ApiModulesNodeState Own { get; set; }

        /// <summary>
        /// The subordinate state of the given HWID
        /// </summary>
        public ApiModulesNodeState Subordinate { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesStatusDetails details &&
                   Own == details.Own &&
                   Subordinate == details.Subordinate;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Own, Subordinate).GetHashCode();
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

    /// <summary>
    /// A single diagnostic message of the given HWID
    /// </summary>
    public class ModulesStatusText
    {
        /// <summary>
        /// A single diagnostic message of the given HWID
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesStatusText text &&
                   Text == text.Text;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Text).GetHashCode();
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

    /// <summary>
    /// Status information of a requested HWID
    /// </summary>
    public class ModulesStatus
    {
        /// <summary>
        /// The status information of the given HWID
        /// </summary>
        public ModulesStatusDetails Status { get; set; }

        /// <summary>
        /// The diagnostic messages of the given HWID
        /// </summary>
        public List<ModulesStatusText> Messages { get; set; }

        /// <summary>
        /// The language in which the diagnostic messages were returned
        /// </summary>
        [JsonConverter(typeof(SafeCultureInfoConverter))]
        public CultureInfo Language { get; set; }

        /// <summary>
        /// Status information of a requested HWID
        /// </summary>
        public ModulesStatus()
        {
            Messages = new List<ModulesStatusText>();
        }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesStatus status &&
                EqualityComparer<ModulesStatusDetails>.Default.Equals(Status, status.Status) &&
                EqualityComparer<CultureInfo>.Default.Equals(Language, status.Language) &&
                   Messages.Count == status.Messages.Count &&
                   Messages.SequenceEqual(status.Messages);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Status, Messages, Language).GetHashCode();
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
