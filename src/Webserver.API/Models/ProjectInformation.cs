// Copyright (c) 2026, Siemens AG
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
    public class ProjectInformation
    {
        /// <summary>
        /// The TIA project name
        /// </summary>
        [JsonProperty("project_name")]
        public string ProjectName { get; set; }

        /// <summary>
        /// The list of version strings for TIA Portal or STEP 7 Safety
        /// </summary>
        public List<ProjectInformationVersion> Versions { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ProjectInformation policy &&
                   ProjectName == policy.ProjectName &&
                   Versions.Count == policy.Versions.Count &&
                   Versions.SequenceEqual(policy.Versions);
            ;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (ProjectName, Versions).GetHashCode();
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
