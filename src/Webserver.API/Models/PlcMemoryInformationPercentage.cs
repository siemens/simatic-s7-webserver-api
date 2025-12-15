// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Response of Plc.ReadMemoryInformation
    /// </summary>
    public class PlcMemoryInformationPercentage
    {
        /// <summary>
        /// allowed tolerance for float comparison
        /// </summary>
        private const float Tolerance = 1e-6f;

        /// <summary>
        /// Percentage of free memory
        /// </summary>
        [JsonProperty("free_percentage")]
        public float FreePercentage { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcMemoryInformationPercentage percentage &&
                   Math.Abs(FreePercentage - percentage.FreePercentage) < Tolerance;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (FreePercentage).GetHashCode();
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
