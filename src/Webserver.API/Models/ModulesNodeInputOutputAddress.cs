// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Represents the actual and configured geo addresses of a node accessible using its HWID
    /// </summary>
    public class ModulesNodeInputOutputAddress
    {
        /// <summary>
        /// Direction of the address, whether it is an input or output address
        /// </summary>
        public ApiModulesNodeAddressDirection Direction { get; set; }

        /// <summary>
        /// The start offset of the address range
        /// </summary>
        public uint Address { get; set; }

        /// <summary>
        /// The lenght of the address range in bytes
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesNodeInputOutputAddress address &&
                   Direction == address.Direction &&
                   Address == address.Address &&
                   Length == address.Length;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Direction, Address, Length).GetHashCode();
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
