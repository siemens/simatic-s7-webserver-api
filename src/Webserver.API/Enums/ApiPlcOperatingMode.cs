// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Stop_fwupdate = 6
    }
}
