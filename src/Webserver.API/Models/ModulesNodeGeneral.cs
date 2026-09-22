// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Represents general of a node accessible using its HWID
    /// </summary>
    public class ModulesNodeGeneral
    {
        /// <summary>
        /// The name of the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The class of the node
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// The type of the node
        /// </summary>
        public ApiModulesNodeType Type { get; set; }

        /// <summary>
        /// The sub type of the node
        /// </summary>
        public ApiModulesNodeSubType Sub_Type { get; set; }

        /// <summary>
        /// List of attributes describing node capabilities
        /// </summary>
        public List<ApiModulesNodeAttribute> Attributes { get; set; }

        /// <summary>
        /// Represents general of a node accessible using its HWID
        /// </summary>
        public ModulesNodeGeneral()
        {
            Attributes = new List<ApiModulesNodeAttribute>();
        }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesNodeGeneral general &&
                   Name == general.Name &&
                   Class == general.Class &&
                   Type == general.Type &&
                   Sub_Type == general.Sub_Type &&
                   Attributes.Count == general.Attributes.Count &&
                   Attributes.SequenceEqual(general.Attributes);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Name, Class, Type, Sub_Type, Attributes).GetHashCode();
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
