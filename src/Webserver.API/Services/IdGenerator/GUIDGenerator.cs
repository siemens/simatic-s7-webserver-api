// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public int Length { get; set; }

        /// <summary>
        /// Default value for Length
        /// </summary>
        public int DefaultLength { get; }

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
            Length = length;
        }

        /// <summary>
        /// Generate a new 
        /// </summary>
        /// <returns></returns>
        public string Generate()
        {
            if(Length < 0)
            {
                throw new InvalidOperationException($"{nameof(Length)} must be greater than 0 (was: {Length})!");
            }
            var result = Guid.NewGuid().ToString();
            if(result.Length < Length)
            {
                throw new InvalidOperationException($"{nameof(Length)} must be smaller than {result.Length} (was: {Length})!");
            }
            return result.Substring(0, Length);
        }
    }
}
