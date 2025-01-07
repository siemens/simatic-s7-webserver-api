// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// The acknowledgement state of the alarm
    /// </summary>
    public enum ApiAlarmAcknowledgementState
    {
        /// <summary>
        /// Alarm is not acknowledged
        /// </summary>
        Not_Acknowledged = 0,
        /// <summary>
        /// Alarm is acknowledged
        /// </summary>
        Acknowledged = 1
    }
}
