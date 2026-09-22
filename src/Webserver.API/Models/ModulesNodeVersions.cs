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
    /// The structure represents motion package information
    /// </summary>
    public class ModulesNodeVersions_Motion_Package
    {
        /// <summary>
        /// The name of the motion package
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The internal version string of the motion package
        /// </summary>
        public string Internal { get; set; }

        /// <summary>
        /// The external version string of the motion package
        /// </summary>
        public string External { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesNodeVersions_Motion_Package package &&
                   Name == package.Name &&
                   Internal == package.Internal &&
                   External == package.External;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Name, Internal, External).GetHashCode();
        }
    }

    /// <summary>
    /// The structure represents a list of motion packages
    /// </summary>
    public class ModulesNodeVersions_Motion
    {
        /// <summary>
        /// The list of motion packages
        /// </summary>
        public List<ModulesNodeVersions_Motion_Package> Packages { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesNodeVersions_Motion motion &&
                   Packages.Count == motion.Packages.Count &&
                   Packages.SequenceEqual(motion.Packages);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Packages).GetHashCode();
        }
    }

    /// <summary>
    /// Represents submodule information of a node accessible using its HWID
    /// </summary>
    public class ModulesNodeVersions
    {
        /// <summary>
        /// The bootloader version of the module
        /// </summary>
        public ModuleVersion Bootloader { get; set; }

        /// <summary>
        ///  The motion package information of the module
        /// </summary>
        public ModulesNodeVersions_Motion Motion { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesNodeVersions versions &&
                   EqualityComparer<ModuleVersion>.Default.Equals(Bootloader, versions.Bootloader) &&
                   EqualityComparer<ModulesNodeVersions_Motion>.Default.Equals(Motion, versions.Motion);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Bootloader, Motion).GetHashCode();
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
