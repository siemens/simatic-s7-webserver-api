// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Represents information of a node accessible using its HWID
    /// </summary>
    public class ModulesParameters
    {
        /// <summary>
        /// General attributes of the node
        /// </summary>
        public ModulesNodeGeneral General { get; set; }

        /// <summary>
        /// Comment of the node
        /// </summary>
        public ModulesNodeComment Comment { get; set; }

        /// <summary>
        /// Geo address of the node
        /// </summary>
        [JsonProperty("geo_address")]
        public ModulesNodeGeoAddress GeoAddress { get; set; }

        /// <summary>
        /// Various parameters of the node
        /// </summary>
        public ModulesNodeParameters Parameters { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesParameters modulesParameters &&
                EqualityComparer<ModulesNodeGeneral>.Default.Equals(General, modulesParameters.General) &&
                EqualityComparer<ModulesNodeComment>.Default.Equals(Comment, modulesParameters.Comment) &&
                EqualityComparer<ModulesNodeGeoAddress>.Default.Equals(GeoAddress, modulesParameters.GeoAddress) &&
                EqualityComparer<ModulesNodeParameters>.Default.Equals(Parameters, modulesParameters.Parameters);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (General, Comment, GeoAddress, Parameters).GetHashCode();
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
