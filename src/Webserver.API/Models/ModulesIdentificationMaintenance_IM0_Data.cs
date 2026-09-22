// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// the IM version
    /// </summary>
    public class ModulesIdentificationMaintenance_IM0_ImVersion
    {
        /// <summary>
        /// The major IM versino
        /// </summary>
        public uint Major { get; set; }

        /// <summary>
        /// The minor IM version
        /// </summary>
        public uint Minor { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesIdentificationMaintenance_IM0_ImVersion version &&
                   Major == version.Major &&
                   Minor == version.Minor;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Major, Minor).GetHashCode();
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
    /// container for IM data
    /// </summary>
    public class ModulesIdentificationMaintenance_IM0_Data
    {
        /// <summary>
        /// The standardized Manufacturer ID (PROFIBUS/PROFINET standard)
        /// </summary>
        [JsonProperty("manufacturer_id")]
        public uint ManufacturerId { get; set; }

        /// <summary>
        /// The article number of the module
        /// </summary>
        [JsonProperty("order_number")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// The serial number of the module
        /// </summary>
        [JsonProperty("serial_number")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The hardware revision of the module
        /// </summary>
        [JsonProperty("hardware_revision")]
        public uint HardwareRevision { get; set; }

        /// <summary>
        /// The software revision of the module
        /// </summary>
        [JsonProperty("software_revision")]
        public ModuleVersion SoftwareRevision { get; set; }

        /// <summary>
        /// The revision counter of the module
        /// </summary>
        [JsonProperty("revision_counter")]
        public uint RevisionCounter { get; set; }

        /// <summary>
        /// The profile ID of the module
        /// </summary>
        [JsonProperty("profile_id")]
        public uint ProfileId { get; set; }

        /// <summary>
        /// The profile specific type of the module
        /// </summary>
        [JsonProperty("profile_specific_type")]
        public uint ProfileSpecificType { get; set; }

        /// <summary>
        /// The IM version
        /// </summary>
        [JsonProperty("im_version")]
        public ModulesIdentificationMaintenance_IM0_ImVersion ImVersion { get; set; }

        /// <summary>
        /// The IM supported information
        /// </summary>
        [JsonProperty("im_supported")]
        public uint ImSupported { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesIdentificationMaintenance_IM0_Data data &&
                   ManufacturerId == data.ManufacturerId &&
                   OrderNumber == data.OrderNumber &&
                   SerialNumber == data.SerialNumber &&
                   HardwareRevision == data.HardwareRevision &&
                   EqualityComparer<ModuleVersion>.Default.Equals(SoftwareRevision, data.SoftwareRevision) &&
                   RevisionCounter == data.RevisionCounter &&
                   ProfileId == data.ProfileId &&
                   ProfileSpecificType == data.ProfileSpecificType &&
                   EqualityComparer<ModulesIdentificationMaintenance_IM0_ImVersion>.Default.Equals(ImVersion, data.ImVersion) &&
                   ImSupported == data.ImSupported;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (ManufacturerId, OrderNumber, SerialNumber, HardwareRevision, SoftwareRevision, RevisionCounter, ProfileId, ProfileSpecificType, ImVersion, ImSupported).GetHashCode();
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
