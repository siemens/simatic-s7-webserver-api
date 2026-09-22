// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// A set of criteria defined for passwords.
    /// </summary>
    public class PlcLoadMemory
    {
        /// <summary>
        /// number of free bytes of the load memory
        /// </summary>
        [JsonProperty("free_bytes")]
        public uint FreeBytes { get; set; }

        /// <summary>
        /// Total number of bytes of the load memory
        /// </summary>
        [JsonProperty("total_bytes")]
        public uint TotalBytes { get; set; }

        /// <summary>
        /// Aging information of the memory card
        /// </summary>
        public PlcLoadMemoryAging Aging { get; set; }

        /// <summary>
        /// True when the objects are equal
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>True when the objects are equal</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcLoadMemory memory &&
                   FreeBytes == memory.FreeBytes &&
                   TotalBytes == memory.TotalBytes &&
                   EqualityComparer<PlcLoadMemoryAging>.Default.Equals(Aging, memory.Aging);
        }

        /// <summary>
        /// GetHashCode Implementation for comparison
        /// </summary>
        public override int GetHashCode()
        {
            return (FreeBytes, TotalBytes, Aging).GetHashCode();
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
