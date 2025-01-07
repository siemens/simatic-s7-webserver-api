// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Plc Opertaing Modes enum
    /// </summary>
    public enum ApiPlcOperatingMode
    {
        /// <summary>
        /// should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// STOP
        /// </summary>
        Stop = 1,
        /// <summary>
        /// ANLAUF
        /// </summary>
        Startup = 2,
        /// <summary>
        /// RUN
        /// </summary>
        Run = 3,
        /// <summary>
        /// Halt
        /// </summary>
        Hold = 4,
        /// <summary>
        /// should never be the case
        /// </summary>
        Unknown = 5,
        /// <summary>
        /// STOP Firmware-Update
        /// </summary>
        Stop_fwupdate = 6,
        /// <summary>
        /// RH PLCs are in Run-Redundant mode.
        /// </summary>
        Run_redundant = 7,
        /// <summary>
        /// RH PLCs are in syncup state.
        /// </summary>
        Syncup = 8,
        /// <summary>
        /// RH PLCs are synchronizing.
        /// </summary>
        Run_syncup = 9,
        /// <summary>
        /// 
        /// </summary>
        Remote_unknown = 10,
    }
}
