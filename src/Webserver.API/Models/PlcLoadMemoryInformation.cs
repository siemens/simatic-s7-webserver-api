// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// A set of information about the load memory
    /// </summary>
    public class PlcLoadMemoryInformation
    {
        /// <summary>
        /// Object representing load memory information
        /// </summary>
        [JsonProperty("load_memory")]
        public PlcLoadMemory LoadMemory { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcLoadMemoryInformation loadMemoryInformation && LoadMemory == loadMemoryInformation.LoadMemory;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (LoadMemory).GetHashCode();
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
