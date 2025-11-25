// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// ApiWebAppResource => Data container for WebApp Resources, configurations can be made using Api functions
    /// </summary>
    public class ProjectInformationVersion
    {
        /// <summary>
        /// The Version string
        /// </summary>
        public string Version { get; set; }

        private ApiVersionSource source;
        /// <summary>
        /// The source of the version
        /// </summary>
        public ApiVersionSource Source
        {
            get
            {
                return source;
            }
            set
            {
                if (value == ApiVersionSource.None)
                {
                    throw new ApiInvalidResponseException($"Returned from API was:{value.ToString()} - which is not valid! contact Siemens");
                }
                source = value;
            }
        }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ProjectInformationVersion versionObject &&
                   source == versionObject.Source &&
                   Version == versionObject.Version;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => (Version, Source).GetHashCode();

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
