// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.Technology
{
    /// <summary>
    /// A Technology Object
    /// </summary>
    public class ApiTechnologyObject
    {
        /// <summary>
        /// allowed tolerance for float comparison
        /// </summary>
        private const float Tolerance = 1e-6f;
        /// <summary>
        /// The block number of the technology object
        /// </summary>
        public uint Number { get; set; }
        /// <summary>
        /// The name of the technology object
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of the technology object
        /// </summary>
        public ApiTechnologyObjectType Type { get; set; }
        /// <summary>
        /// The major version of the technology object
        /// </summary>
        public float Version { get; set; }

        /// <summary>
        /// Compares input object to this TechnologyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if objects are the same</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiTechnologyObject technologyObject &&
                   Number == technologyObject.Number &&
                   Name == technologyObject.Name &&
                   Type == technologyObject.Type &&
                   Math.Abs(Version - technologyObject.Version) < Tolerance;
        }

        /// <summary>
        /// Get HashCode
        /// </summary>
        /// <returns>The Hashcode</returns>
        public override int GetHashCode()
        {
            return (Number, Name, Type, Version).GetHashCode();
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
