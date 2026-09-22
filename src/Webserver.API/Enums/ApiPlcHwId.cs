// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Enum containing Hardware Ids for different types of hardware
    /// </summary>
    public enum ApiPlcHwId
    {
        /// <summary>
        /// Central Device
        /// </summary>
        CentralDevice = 32,
        /// <summary>
        /// HwId for a standard, non-redundant plc
        /// </summary>
        StandardPLC = 49,
        /// <summary>
        /// RH Plc Redundancy Id1
        /// </summary>
        RhPlcRedundancyId1 = 65149,
        /// <summary>
        /// RH Plc Redundancy Id2
        /// </summary>
        RhPlcRedundancyId2 = 65349,
    }
}
