// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Services.IdGenerator
{
    /// <summary>
    /// GUID Generator: use Guid implementation to create (request) id(s)
    /// </summary>
    public class GUIDGenerator : IIdGenerator
    {
        /// <summary>
        /// Length for the id(s) generated
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// Default value for Length
        /// </summary>
        public readonly int DefaultLength;

        /// <summary>
        /// GUID Generator: use Guid implementation to create (request) id(s)
        /// </summary>
        public GUIDGenerator()
        {
            DefaultLength = Guid.NewGuid().ToString().Length;
            Length = DefaultLength;
        }

        /// <summary>
        /// GUID Generator: use Guid implementation to create (request) id(s)
        /// </summary>
        /// <param name="length"></param>
        public GUIDGenerator(int length) : this()
        {
            if (length <= 0 || length > DefaultLength)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
            Length = length;
        }

        /// <summary>
        /// Generate a new 
        /// </summary>
        /// <returns></returns>
        public string Generate()
        {
            var result = Guid.NewGuid().ToString();
            return result.Substring(0, Length);
        }

        /// <summary>
        /// (Name, Has_children, Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only) Equal!;
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as GUIDGenerator);

        /// <summary>
        /// Equals => ()
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(GUIDGenerator obj)
        {
            if (obj is null)
                return false;
            var toReturn = this.Length == obj.Length;
            return toReturn;
        }

        /// <summary>
        /// GetHasCode => Length.GetHashCode()
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return -2130075011 + Length;
        }
    }
}
