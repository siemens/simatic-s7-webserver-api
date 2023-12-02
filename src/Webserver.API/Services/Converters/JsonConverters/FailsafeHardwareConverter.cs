// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Siemens.Simatic.S7.Webserver.API.Models.FailsafeParameters;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters
{
    /// <summary>
    /// Decides which derived class to use when deserializing/serializing parameters of Failsafe.ReadParameters
    /// </summary>
    public class FailsafeHardwareConverter : JsonConverter<FailsafeHardware>
    {
        /// <summary>
        /// Read FailsafeHardware from json
        /// </summary>
        /// <returns>A FailsafeHardware object: either a CPU or a Module if the parameters are correct</returns>
        public override FailsafeHardware ReadJson(JsonReader reader, Type objectType, FailsafeHardware existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            if (jObject["collective_signature"] != null)
            {
                var fCPU = new FailsafeCPU();
                serializer.Populate(jObject.CreateReader(), fCPU);
                return fCPU;
            }
            else if (jObject["f_par_crc"] != null)
            {
                var fModule = new FailsafeModule();
                serializer.Populate(jObject.CreateReader(), fModule);
                return fModule;
            }

            var baseHW = new FailsafeHardware();
            serializer.Populate(jObject.CreateReader(), baseHW);
            return baseHW;
        }
        /// <summary>
        /// Writes to json
        /// </summary>
        public override void WriteJson(JsonWriter writer, FailsafeHardware value, JsonSerializer serializer)
        {
            JObject jObject = JObject.FromObject(value);
            if (value is FailsafeCPU CPU)
            {
                jObject = JObject.FromObject(CPU);
            }
            else if (value is FailsafeModule module)
            {
                jObject = JObject.FromObject(module);
            }
            jObject.WriteTo(writer);
        }
    }
}
