// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;

namespace Siemens.Simatic.S7.Webserver.API.Models.RedundancySystemInformation
{
    /// <summary>
    /// This object represents one of the PLC in the redundant system
    /// </summary>
    public class ApiRedundancySystemInfo_Plc
    {
        /// <summary>
        /// The Redundancy ID of the represented PLC. 
        /// For object plc_1 this attribute always contains value 1.
        /// For object plc_2 this attribute always contains value 2.
        /// </summary>
        public ApiPlcRedundancyId Redundancy_id { get; set; }

        /// <summary>
        /// The role of the represented PLC.
        /// For the local PLC, it must always contain a valid value, either containing role primary or backup. 
        /// For the tree of the partner PLC, the role may be unknown in case that the partner PLC is not properly paired.
        /// </summary>
        public ApiPlcRedundancyRole Role { get; set; }

        /// <summary>
        /// The hardware identifier of the represented PLC.
        /// This can contain either value 65149 or 65349.
        /// For object plc_1 this attribute always contains value 65149.
        /// For object plc_2 this attribute always contains value 65349.
        /// </summary>
        public uint Hwid { get; set; }

        /// <summary>
        /// Check whether properties match
        /// </summary>
        /// <param name="obj">ApiRedundancySystemInfo_Plc</param>
        /// <returns>Returns true if the ApiRedundancySystemInfo_Plc are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiRedundancySystemInfo_Plc;
            if (structure == null) { return false; }
            return structure.Redundancy_id == this.Redundancy_id &&
                   structure.Role == this.Role &&
                   structure.Hwid == this.Hwid;
        }
        /// <summary>
        /// GetHashCode for ApiRedundancySystemInfo_Plc
        /// </summary>
        /// <returns>hashcode for the ApiRedundancySystemInfo_Plc</returns>
        public override int GetHashCode()
        {
            return (Redundancy_id, Role, Hwid).GetHashCode();
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
