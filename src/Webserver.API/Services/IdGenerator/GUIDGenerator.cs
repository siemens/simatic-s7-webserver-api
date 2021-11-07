// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Siemens.Simatic.S7.Webserver.API.Services.IdGenerator
{
    class GUIDGenerator : IIdGenerator
    {
        public string Generate(int length)
        {
            if(length>0)
            {
                throw new ArgumentException($"{nameof(length)} must be greater than 0 (was: {length})!");
            }
            var result = Guid.NewGuid().ToString();
            if(result.Length < length)
            {
                throw new ArgumentException($"{nameof(length)} must be smaller than {result.Length} (was: {length})!");
            }
            return result.Substring(0, length);
        }
    }
}
