// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters
{
    /// <summary>
    /// Converts string "enabled" (true) or "disabled" (false) into a bool, or vice versa
    /// </summary>
    public class EnabledDisabledConverter : JsonConverter<bool?>
    {
        /// <summary>
        /// Deserializes string "enabled" (true) or "disabled" (false) into a bool
        /// </summary>
        /// <returns>Deserialized bool value</returns>
        public override bool? ReadJson(JsonReader reader, Type objectType, bool? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string value = reader.Value.ToString().ToLower();
                if (value == "enabled")
                {
                    return true;
                }
                else if (value == "disabled")
                {
                    return false;
                }
            }
            return null;
        }

        /// <summary>
        /// Serializes bool into "enabled" (true) or "disabled" (false)
        /// </summary>
        public override void WriteJson(JsonWriter writer, bool? value, JsonSerializer serializer)
        {
            if (value.HasValue)
            {
                string boolAsString = value.Value ? "enabled" : "disabled";
                writer.WriteValue(boolAsString);
            }
            else writer.WriteNull();
        }
    }
}
