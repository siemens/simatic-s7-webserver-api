// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using System;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Represents a Failsafe Runtime Group on the PLC
    /// </summary>
    public class ApiFailsafeRuntimeGroup
    {
        /// <summary>
        /// The name of the F-runtime group.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The signature of the runtime group, encoded as array of 4 elements to represent the 32-bit signature.
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// The current cycle time.
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Cycle_time_current { get; set; }
        /// <summary>
        /// The maximum cycle time.
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Cycle_time_Max { get; set; }
        /// <summary>
        /// The current runtime.
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Runtime_current { get; set; }
        /// <summary>
        /// The maximum runtime.
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Runtime_max { get; set; }

        /// <summary>
        /// Compares object and this
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if input object and this match</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiFailsafeRuntimeGroup group &&
                   Name == group.Name &&
                   Enumerable.SequenceEqual(Signature, group.Signature) &&
                   Cycle_time_current.Equals(group.Cycle_time_current) &&
                   Cycle_time_Max.Equals(group.Cycle_time_Max) &&
                   Runtime_current.Equals(group.Runtime_current) &&
                   Runtime_max.Equals(group.Runtime_max);
        }

        /// <summary>
        /// GetHashCode()
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return (Name, Signature, Cycle_time_current, Cycle_time_Max, Runtime_current, Runtime_max).GetHashCode();
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
