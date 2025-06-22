// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models.TimeSettings
{
    /// <summary>
    /// Standard time configuration containing the start date of the standard time
    /// </summary>
    public class StandardTimeConfiguration
    {
        /// <summary>
        /// PlcDate of the StartTime of the standard time or daylight savings time
        /// </summary>
        public PlcDate Start { get; set; }
        /// <summary>
        /// Constructor to create an STD object
        /// </summary>
        /// <param name="start">The starting time of the standard time</param>
        public StandardTimeConfiguration(PlcDate start)
        {
            Start = start;
        }

        /// <summary>
        /// Checks whether incoming object is same as this STD
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they're the same</returns>
        public override bool Equals(object obj)
        {
            return obj is StandardTimeConfiguration configuration &&
                   Start.Equals(configuration.Start);
        }

        /// <summary>
        /// Get HashCode of StandardTime
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return Start.GetHashCode();
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
