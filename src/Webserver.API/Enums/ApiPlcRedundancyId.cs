// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Represents which PLC with id 1 or 2 is used in an R/H system.
    /// </summary>
    public enum ApiPlcRedundancyId
    {
        /// <summary>
        /// Standard PLC
        /// </summary>
        StandardPLC = 0,
        /// <summary>
        /// Redundancy ID 1
        /// </summary>
        RedundancyId_1 = 1,
        /// <summary>
        /// Redundancy ID 2
        /// </summary>
        RedundancyId_2 = 2
    }
}
