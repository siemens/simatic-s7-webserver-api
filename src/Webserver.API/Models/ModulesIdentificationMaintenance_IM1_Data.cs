// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// container for IM data
    /// </summary>
    public class ModulesIdentificationMaintenance_IM1_Data
    {
        /// <summary>
        /// holds the plant designation string
        /// </summary>
        [JsonProperty("plant_designation")]
        public string PlantDesignation { get; set; }

        /// <summary>
        /// holds the location identifier string
        /// </summary>
        [JsonProperty("location_identifier")]
        public string LocationIdentifier { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesIdentificationMaintenance_IM1_Data data &&
                   PlantDesignation == data.PlantDesignation &&
                   LocationIdentifier == data.LocationIdentifier;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (PlantDesignation, LocationIdentifier).GetHashCode();
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
