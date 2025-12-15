// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.RedundancySyncupProgress
{
    /// <summary>
    /// This object provides information about syncup state 'minimizing_delay'.
    /// </summary>
    public class ApiRedundancySyncupProgress_MinimizingDelay
    {
        /// <summary>
        /// The hypothetical cycle time with millisecond resolution.
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Hypothetical_cycle_time { get; set; }

        /// <summary>
        /// The tolerable cycle time with millisecond resolution.
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Tolerable_cycle_time { get; set; }

        /// <summary>
        /// Check whether properties match
        /// </summary>
        /// <param name="obj">ApiRedundancySyncupProgress_MinimizingDelay</param>
        /// <returns>Returns true if the ApiRedundancySyncupProgress_MinimizingDelay are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiRedundancySyncupProgress_MinimizingDelay;
            if (structure == null) { return false; }
            return structure.Hypothetical_cycle_time == this.Hypothetical_cycle_time &&
                   structure.Tolerable_cycle_time == this.Tolerable_cycle_time;
        }
        /// <summary>
        /// GetHashCode for ApiRedundancySyncupProgress_MinimizingDelay
        /// </summary>
        /// <returns>hashcode for the ApiRedundancySyncupProgress_MinimizingDelay</returns>
        public override int GetHashCode()
        {
            return (Hypothetical_cycle_time, Tolerable_cycle_time).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiRedundancySyncupProgress_MinimizingDelay
        /// </summary>
        /// <returns>ApiRedundancySyncupProgress_MinimizingDelay as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiRedundancySyncupProgress_MinimizingDelay
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiRedundancySyncupProgress_MinimizingDelay as a multiline string</returns>
        public string ToString(NullValueHandling nullValueHandling)
        {
            JsonSerializerSettings options = new JsonSerializerSettings()
            {
                NullValueHandling = nullValueHandling,
            };
            string result = JsonConvert.SerializeObject(this, Formatting.Indented, options);
            return result;
        }
    }
}
