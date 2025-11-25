// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Aging information of the memory card
    /// </summary>
    public class PlcLoadMemoryAging
    {
        /// <summary>
        /// Used percentage of the overall service life (between 0 and 100)
        /// </summary>
        [JsonProperty("service_life_used_percentage")]
        public uint ServiceLifeUsedPercentage { get; set; }

        /// <summary>
        /// Configured percentage when a diagnostics event shall be raised.(between 0 and 100)
        /// </summary>
        [JsonProperty("threshold_percentage")]
        public uint ThresholdPercentage { get; set; }

        /// <summary>
        /// True when the objects are equal
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>True when the objects are equal</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcLoadMemoryAging aging &&
                   ServiceLifeUsedPercentage == aging.ServiceLifeUsedPercentage &&
                   ThresholdPercentage == aging.ThresholdPercentage;
        }

        /// <summary>
        /// GetHashCode Implementation for comparison
        /// </summary>
        public override int GetHashCode() => (ServiceLifeUsedPercentage, ThresholdPercentage).GetHashCode();

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
