// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
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

        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
