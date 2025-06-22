// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.TimeSettings
{
    /// <summary>
    /// Daylight savings rule
    /// </summary>
    public class DaylightSavingsRule
    {
        /// <summary>
        /// Start of Standard time
        /// </summary>
        public StandardTimeConfiguration Std { get; set; }
        /// <summary>
        /// Start of Daylight saving time
        /// </summary>
        public DaylightSavingsTimeConfiguration Dst { get; set; }

        /// <summary>
        /// Constructor to create a daylight-savings rule using STD and DST objects
        /// </summary>
        /// <param name="std">Represents the standard time configuration</param>
        /// <param name="dst">Represents the daylight-savings time configuration</param>
        [JsonConstructor]
        public DaylightSavingsRule(StandardTimeConfiguration std, DaylightSavingsTimeConfiguration dst)
        {
            this.Std = std;
            this.Dst = dst;
        }

        /// <summary>
        /// Constructor to create a daylight-savings rule -- this will also create the STD and DST objects
        /// </summary>
        /// <param name="stdStart">The starting time of the standard time</param>
        /// <param name="dstStart">The starting time of the daylight-savings time</param>
        /// <param name="dstTimeOffset">The offset of daylight savings time from the standard time</param>
        public DaylightSavingsRule(PlcDate stdStart, PlcDate dstStart, TimeSpan dstTimeOffset)
        {
            this.Std = new StandardTimeConfiguration(stdStart);
            this.Dst = new DaylightSavingsTimeConfiguration(dstStart, dstTimeOffset);
        }

        /// <summary>
        /// Mixed constructor to create a daylight-savings rule: needs an STD object, but creates the DST object on it's own.
        /// </summary>
        /// <param name="std">The standard time configuration</param>
        /// <param name="dstStart">The starting time of the daylight-savings time</param>
        /// <param name="dstTimeOffset">The offset of daylight savings time from the standard time</param>
        public DaylightSavingsRule(StandardTimeConfiguration std, PlcDate dstStart, TimeSpan dstTimeOffset)
        {
            this.Std = std;
            this.Dst = new DaylightSavingsTimeConfiguration(dstStart, dstTimeOffset);
        }

        /// <summary>
        /// Mixed constructor to create a daylight-savings rule: creates the STD object on it's own, but expects a DST object.
        /// </summary>
        /// <param name="stdStart">The starting time of the standard time</param>
        /// <param name="dst">The daylight-savings time configuration</param>
        public DaylightSavingsRule(PlcDate stdStart, DaylightSavingsTimeConfiguration dst)
        {
            this.Std = new StandardTimeConfiguration(stdStart);
            this.Dst = dst;
        }

        /// <summary>
        /// Get HashCode of DaylightSavingsRule
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Std, Dst).GetHashCode();
        }

        /// <summary>
        /// Checks if the input object is the same as this DaylightSavingsRule
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if this and obj is the same</returns>
        public override bool Equals(object obj)
        {
            return obj is DaylightSavingsRule rule &&
                   Std.Equals(rule.Std) &&
                   Dst.Equals(rule.Dst);
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
