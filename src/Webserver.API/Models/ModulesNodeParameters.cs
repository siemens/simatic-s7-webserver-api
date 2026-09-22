// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Represents various parameters of a node accessible using its HWID
    /// </summary>
    public class ModulesNodeParameters
    {
        /// <summary>
        /// Represents the input and output addresses of the node
        /// </summary>
        [JsonProperty("input_output")]
        public List<ModulesNodeInputOutputAddress> InputOutput { get; set; }

        /// <summary>
        /// Represents version information of the node
        /// </summary>
        public ModulesNodeVersions Versions { get; set; }

        /// <summary>
        /// Represents submodule information of the node
        /// </summary>
        public ModulesNodeSubmodule Submodule { get; set; }

        /// <summary>
        /// Represents various parameters of a node accessible using its HWID
        /// </summary>
        public ModulesNodeParameters()
        {
            InputOutput = new List<ModulesNodeInputOutputAddress>();
        }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesNodeParameters parameters &&
                   EqualityComparer<ModulesNodeVersions>.Default.Equals(Versions, parameters.Versions) &&
                   EqualityComparer<ModulesNodeSubmodule>.Default.Equals(Submodule, parameters.Submodule) &&
                   InputOutput.Count == parameters.InputOutput.Count &&
                   InputOutput.SequenceEqual(parameters.InputOutput);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (InputOutput, Versions, Submodule).GetHashCode();
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
