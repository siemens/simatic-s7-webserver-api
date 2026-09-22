// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Represents the actual and configured geo addresses of a node accessible using its HWID
    /// </summary>
    public class ModulesNodeGeoAddress
    {
        /// <summary>
        /// Actual geo address of the node
        /// </summary>
        public ModulesNodeGeoAddressActual Actual { get; set; }

        /// <summary>
        /// Configured geo address of the node
        /// </summary>
        public ModulesNodeGeoAddressConfigured Configured { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesNodeGeoAddress address &&
                   EqualityComparer<ModulesNodeGeoAddressActual>.Default.Equals(Actual, address.Actual) &&
                   EqualityComparer<ModulesNodeGeoAddressConfigured>.Default.Equals(Configured, address.Configured);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Actual, Configured).GetHashCode();
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
