// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Contains the station name of the PLC on which the request was executed.
    /// </summary>
    public class ApiPlcReadStationNameResult
    {
        /// <summary>
        /// The station name of the PLC on which the request was executed.
        /// </summary>
        public string Station_Name { get; set; }

        /// <summary>
        /// Compares this to input object
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if objects are the same</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiPlcReadStationNameResult stationName &&
                   Station_Name == stationName.Station_Name;
        }

        /// <summary>
        /// Get Hash Code of PasswordExpiraton object
        /// </summary>
        /// <returns>Hash Code</returns>
        public override int GetHashCode()
        {
            return Station_Name.GetHashCode();
        }

        /// <summary>
        /// ToString for ApiPlcCpuType
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Station_Name)}: {Station_Name}";
        }
    }
}
