// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System;
using System.Globalization;
namespace Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters
{
    /// <summary>
    /// Converter that will accept "invalid" as CultureInfo and just make it be null in that case.
    /// </summary>
    public class SafeCultureInfoConverter : JsonConverter<CultureInfo>
    {
        /// <summary>
        /// WriteNull if its null, value when it has value
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">CultureInfo value</param>
        /// <param name="serializer">Json serializer</param>
        public override void WriteJson(JsonWriter writer, CultureInfo value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.Name);
            }
        }

        /// <summary>
        /// Read Json -> Also allow 'invalid' for cultureInfo -> then just set to null
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="objectType">Object Type</param>
        /// <param name="existingValue">existing value</param>
        /// <param name="hasExistingValue">wheter the object has an existing value</param>
        /// <param name="serializer">Json serializer</param>
        /// <returns>CultureInfo - null for 'invalid'</returns>
        /// <exception cref="JsonException">For Unexpected token types</exception>
        public override CultureInfo ReadJson(JsonReader reader, Type objectType, CultureInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                string cultureString = (string)reader.Value;

                if (string.IsNullOrEmpty(cultureString) || cultureString == "invalid")
                {
                    return null;
                }

                try
                {
                    return new CultureInfo(cultureString);
                }
                catch (CultureNotFoundException)
                {
                    // Return null for any invalid culture strings
                    return null;
                }
                catch (ArgumentException)
                {
                    // Return null for any invalid culture strings
                    return null;
                }
            }

            throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }
    }
}
