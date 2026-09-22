// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Represents the actual and configured geo addresses of a node accessible using its HWID
    /// </summary>
    public class ModulesNodeGeoAddressActual
    {
        /// <summary>
        /// Configured I/O system number
        /// </summary>
        public uint IoSystem { get; set; }

        /// <summary>
        /// Configured device number
        /// </summary>
        public uint Device { get; set; }

        /// <summary>
        /// Configured slot number
        /// </summary>
        public uint Slot { get; set; }

        /// <summary>
        /// Configured subslot number
        /// </summary>
        public uint Subslot { get; set; }

        /// <summary>
        /// Configured rack number
        /// </summary>
        public uint Rack { get; set; }

        /// <summary>
        /// If this is set to true, the module has been removed by configuration contorl.
        /// All other values will be irrelevant in that case
        /// </summary>
        [JsonProperty("removed_by_configuration_control")]
        public bool RemovedByConfigurationControl { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesNodeGeoAddressActual actual &&
                   IoSystem == actual.IoSystem &&
                   Device == actual.Device &&
                   Slot == actual.Slot &&
                   Subslot == actual.Subslot &&
                   Rack == actual.Rack &&
                   RemovedByConfigurationControl == actual.RemovedByConfigurationControl;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (IoSystem, Device, Slot, Subslot, Rack, RemovedByConfigurationControl).GetHashCode();
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
