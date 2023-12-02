// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System;
using System.Xml;

namespace Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters
{
    /// <summary>
    /// A JsonConverter that converts a TimeSpan into an ISO8601 duration
    /// </summary>
    public class TimeSpanISO8601Converter : JsonConverter<TimeSpan>
    {
        /// <summary>
        /// Writes to Json
        /// </summary>
        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToISO8601Duration());
        }
        /// <summary>
        /// Reads from json
        /// </summary>
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var durationString = reader.Value.ToString();
                return TimeSpanExtensions.ISO8601DurationToTimeSpan(durationString);
            }

            throw new JsonSerializationException("Expected string value for TimeSpan.");
        }
        /// <summary>
        /// Can it read from json
        /// </summary>
        public override bool CanRead => true;
        /// <summary>
        /// Can it write to json
        /// </summary>
        public override bool CanWrite => true;
    }
    /// <summary>
    /// Extensions for TimeSpan object
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Converts TimeSpan into ISO8601 duration string
        /// </summary>
        /// <param name="ts">TimeSpan to convert</param>
        /// <returns>Duration as ISO8601 string</returns>
        public static string ToISO8601Duration(this TimeSpan ts)
        {
            return XmlConvert.ToString(ts);
        }
        /// <summary>
        /// Converts an ISO8601 duration string into a TimeSpan object
        /// </summary>
        /// <param name="iso8601Duration">A string that is an ISO8601 duration, eg.: PT3H</param>
        /// <returns>New TimeSpan object with the input duration</returns>
        public static TimeSpan ISO8601DurationToTimeSpan(string iso8601Duration)
        {
            return XmlConvert.ToTimeSpan(iso8601Duration);
        }
    }
}
