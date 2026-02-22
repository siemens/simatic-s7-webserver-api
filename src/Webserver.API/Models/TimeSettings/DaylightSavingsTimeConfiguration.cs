// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.TimeSettings
{
    /// <summary>
    /// Daylight savings time configuration containing the start date of the daylight savings time and the offset during the daylight savings time
    /// </summary>
    public class DaylightSavingsTimeConfiguration : StandardTimeConfiguration
    {
        private TimeSpan _offset;
        /// <summary>
        /// Offset during the daylight savings time
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                if (value.TotalMinutes >= -180 && value.TotalMinutes <= 180)
                {
                    _offset = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Valid values of {nameof(Offset)} are between -180 and 180 minutes. Input value was {value}");
                }
            }
        }

        /// <summary>
        /// Constructor to create a DST object
        /// </summary>
        /// <param name="start">The starting time of the daylight-savings time</param>
        /// <param name="timeOffset">The offset of daylight savings time from the standard time</param>
        public DaylightSavingsTimeConfiguration(PlcDate start, TimeSpan timeOffset) : base(start)
        {
            this.Offset = timeOffset;
        }

        /// <summary>
        /// Checks whether incoming object is same as this DST
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they're the same</returns>
        public override bool Equals(object obj)
        {
            return obj is DaylightSavingsTimeConfiguration configuration &&
                   base.Equals(obj) &&
                   Start.Equals(configuration.Start) &&
                   Offset.Equals(configuration.Offset);
        }

        /// <summary>
        /// (Offset, Start).GetHashCode()
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Offset, Start).GetHashCode();
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
