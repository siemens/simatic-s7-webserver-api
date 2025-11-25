// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Representation of a version number of a module, component, etc.
    /// </summary>
    public class ModuleVersion
    {
        /// <summary>
        /// The versino type
        /// </summary>
        public char Type { get; set; }

        /// <summary>
        /// The major versino
        /// </summary>
        public uint Major { get; set; }

        /// <summary>
        /// The minor version
        /// </summary>
        public uint Minor { get; set; }

        /// <summary>
        /// The patch versino
        /// </summary>
        public uint Patch { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModuleVersion version &&
                   Type == version.Type &&
                   Major == version.Major &&
                   Minor == version.Minor &&
                   Patch == version.Patch;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Type, Major, Minor, Patch).GetHashCode();
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
