// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// System state of the R/H system.
    /// </summary>
    public class ApiRedundancySystemState
    {
        /// <summary>
        /// Contains the current system state of the R/H system.
        /// </summary>
        public ApiPlcRedundancySystemState State { get; set; }
        /// <summary>
        /// Check whether properties match
        /// </summary>
        /// <param name="obj">ApiRedundancySystemInfo_Plc</param>
        /// <returns>Returns true if the ApiRedundancySystemInfo_Plc are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiRedundancySystemState;
            if (structure == null) { return false; }
            return structure.State == this.State;
        }
        /// <summary>
        /// GetHashCode for ApiRedundancySystemInfo_Plc
        /// </summary>
        /// <returns>hashcode for the ApiRedundancySystemInfo_Plc</returns>
        public override int GetHashCode()
        {
            return (State).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiRedundancySystemInfo_Plc
        /// </summary>
        /// <returns>ApiRedundancySystemInfo_Plc as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiRedundancySystemInfo_Plc
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiRedundancySystemInfo_Plc as a multiline string</returns>
        public string ToString(NullValueHandling nullValueHandling)
        {
            JsonSerializerSettings options = new JsonSerializerSettings()
            {
                NullValueHandling = nullValueHandling,
            };
            string result = JsonConvert.SerializeObject(this, Formatting.Indented, options);
            return result;
        }
    }
}
