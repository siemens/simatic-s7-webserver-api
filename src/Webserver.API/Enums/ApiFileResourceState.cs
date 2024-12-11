// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// State of the file resource (datalog)
    /// </summary>
    public enum ApiFileResourceState
    {
        /// <summary>None state of the resource (datalog)</summary>
        None = 0,
        /// <summary>Active state of the resource (datalog)</summary>
        Active = 1,
        /// <summary>Inactive state of the resource (datalog)</summary>
        Inactive = 2
    }
}
