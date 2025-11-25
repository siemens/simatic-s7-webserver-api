// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Represents a LED with its status information
    /// </summary>
    public class ModulesLed
    {
        /// <summary>
        /// LED type
        /// </summary>
        public ApiLedType Type { get; set; }

        /// <summary>
        /// The period describes how long each color is shown. If only one color is defined, it will toggle between an off an on period, otherwise colors are alternated
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Period { get; set; }

        /// <summary>
        /// List of colors that are active on the LED
        /// </summary>
        public List<ApiLedColor> Colors { get; set; }

        /// <summary>
        /// LED status
        /// </summary>
        public ApiLedStatus Status { get; set; }

        /// <summary>
        /// Represents a LED with its status information
        /// </summary>
        public ModulesLed()
        {
            Colors = new List<ApiLedColor>();
        }

        /// <summary>
        /// Returns true when the object is equal to the compared object
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>true when the object is equal to the compared object</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is ModulesLed modulesLed)
            {
                return Type == modulesLed.Type &&
                   Period.Equals(modulesLed.Period) &&
                   Colors.Count == modulesLed.Colors.Count &&
                   Colors.SequenceEqual(modulesLed.Colors) &&
                   Status == modulesLed.Status;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// GetHashCode -> Equality -> Used for comparing two objects
        /// </summary>
        /// <returns>Hashcode for the Object</returns>
        public override int GetHashCode()
        {
            int hashCode = -1337457871;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Period.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<ApiLedColor>>.Default.GetHashCode(Colors);
            hashCode = hashCode * -1521134295 + Status.GetHashCode();
            return hashCode;
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
