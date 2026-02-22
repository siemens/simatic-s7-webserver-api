// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Models.TimeSettings;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Result of Plc.ReadTimeSettings
    /// </summary>
    public class ApiPlcReadTimeSettingsResult
    {
        /// <summary>
        /// Offset to the current time => Utc_offset + daylight savings time offset if currently applied
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Current_offset { get; set; }

        /// <summary>
        /// fixed offset to utc
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Utc_offset { get; set; }

        /// <summary>
        /// (Optional) Current Daylight savings rule
        /// </summary>
        public DaylightSavingsRule Rule { get; set; }

        /// <summary>
        /// Can be used to comfortly get the current time for a given timestamp of a Plc.ReadSystemTime result
        /// </summary>
        /// <param name="plcReadSystemTimeResult">Timestamp of result of Plc.ReadSystemTime (UTC)</param>
        /// <returns>Return current time based on an input plc time, plus the current offset from this time setting.</returns>
        public DateTime GetCurrentTime(DateTime plcReadSystemTimeResult)
        {
            return (plcReadSystemTimeResult + Current_offset);
        }

        /// <summary>
        /// Can be used to comfortly get the current time for a given Plc.ReadSystemTime result
        /// </summary>
        /// <param name="plcReadSystemTimeResult">Result of Plc.ReadSystemTime (UTC)</param>
        /// <returns>Return current time based on a read system time result, plus the current offset from this time setting.</returns>
        public DateTime GetCurrentTime(ApiPlcReadSystemTimeResult plcReadSystemTimeResult)
        {
            return (plcReadSystemTimeResult.Timestamp + Current_offset);
        }

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