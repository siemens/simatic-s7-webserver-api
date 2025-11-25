// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// container for IM data
    /// </summary>
    public class ModulesIdentificationMaintenance_IM2_Data
    {
        /// <summary>
        /// holds the IM2 installation date
        /// </summary>
        [JsonProperty("installation_date")]
        public DateTime InstallationDate { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesIdentificationMaintenance_IM2_Data data &&
                   InstallationDate == data.InstallationDate;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (InstallationDate).GetHashCode();
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
