// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models.RedundancySystemInformation
{
    /// <summary>
    /// This object represents the R/H system and its both PLCs.
    /// </summary>
    public class ApiRedundancySystemInfo_Plcs
    {
        /// <summary>
        /// This object represents the PLC with Redundancy ID 1. 
        /// </summary>
        public ApiRedundancySystemInfo_Plc Plc_1 { get; set; }

        /// <summary>
        /// This object represents the PLC with Redundancy ID 2. 
        /// </summary>
        public ApiRedundancySystemInfo_Plc Plc_2 { get; set; }

        /// <summary>
        /// Check whether properties match
        /// </summary>
        /// <param name="obj">ApiRedundancySystemInfo_Plcs</param>
        /// <returns>Returns true if the ApiRedundancySystemInfo_Plcs are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiRedundancySystemInfo_Plcs;
            if (structure == null) { return false; }
            return structure.Plc_1.Equals(this.Plc_1) &&
                   structure.Plc_2.Equals(this.Plc_2);
        }
        /// <summary>
        /// GetHashCode for ApiRedundancySystemInfo_Plcs
        /// </summary>
        /// <returns>hashcode for the ApiRedundancySystemInfo_Plcs</returns>
        public override int GetHashCode()
        {
            return (Plc_1.GetHashCode(), Plc_2.GetHashCode()).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiRedundancySystemInfo_Plcs
        /// </summary>
        /// <returns>ApiRedundancySystemInfo_Plcs as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiRedundancySystemInfo_Plcs
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiRedundancySystemInfo_Plcs as a multiline string</returns>
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
