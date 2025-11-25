// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// A set of criteria defined for passwords.
    /// </summary>
    public class ModulesLeds
    {
        /// <summary>
        /// Available LEDs with their status information
        /// </summary>
        public List<ModulesLed> Leds { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesLeds modulesLeds &&
                Leds.Count == modulesLeds.Leds.Count &&
                Leds.SequenceEqual(modulesLeds.Leds);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Leds).GetHashCode();
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
