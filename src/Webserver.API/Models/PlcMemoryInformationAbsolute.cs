// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Response of PlcProgram.ReadMemoryInformation
    /// </summary>
    public class PlcMemoryInformationAbsolute
    {
        /// <summary>
        /// Number of free bytes
        /// </summary>
        [JsonProperty("free_bytes")]
        public uint FreeBytes { get; set; }
        /// <summary>
        /// Total number of bytes
        /// </summary>
        [JsonProperty("total_bytes")]
        public uint TotalBytes { get; set; }
        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcMemoryInformationAbsolute absolute &&
                   FreeBytes == absolute.FreeBytes &&
                   TotalBytes == absolute.TotalBytes;
        }
        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (FreeBytes, TotalBytes).GetHashCode();
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
