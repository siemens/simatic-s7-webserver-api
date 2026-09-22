// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// various memory information of the PLC
    /// </summary>
    public class PlcMemoryInformation
    {
        /// <summary>
        /// PLC code work memory
        /// </summary>
        [JsonProperty("code_work_memory")]
        public PlcMemoryInformationAbsolute CodeWorkMemory { get; set; }
        /// <summary>
        /// PLC data work memory
        /// </summary>
        [JsonProperty("data_work_memory")]
        public PlcMemoryInformationAbsolute DataWorkMemory { get; set; }
        /// <summary>
        /// PLC retentive memory
        /// </summary>
        [JsonProperty("retentive_memory")]
        public PlcMemoryInformationAbsolute RetentiveMemory { get; set; }
        /// <summary>
        /// PLC data type memory
        /// </summary>
        [JsonProperty("datatype_memory")]
        public PlcMemoryInformationPercentage DataTypeMemory { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            if (obj is PlcMemoryInformation plcMemoryInformation)
            {
                return CodeWorkMemory.Equals(plcMemoryInformation.CodeWorkMemory) &&
                    DataWorkMemory.Equals(plcMemoryInformation.DataWorkMemory) &&
                    RetentiveMemory.Equals(plcMemoryInformation.RetentiveMemory) &&
                    DataTypeMemory.Equals(plcMemoryInformation.DataTypeMemory);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (CodeWorkMemory, DataWorkMemory, RetentiveMemory, DataTypeMemory).GetHashCode();
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
