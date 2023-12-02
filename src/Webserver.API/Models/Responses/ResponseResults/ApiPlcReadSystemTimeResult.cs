// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Plc.ReadSystemTime result containing the current Utc PLC Timestamp
    /// </summary>
    public class ApiPlcReadSystemTimeResult
    {
        /// <summary>
        /// Current Utc PLC Timestamp
        /// </summary>
        public DateTime Timestamp;
    }
}
