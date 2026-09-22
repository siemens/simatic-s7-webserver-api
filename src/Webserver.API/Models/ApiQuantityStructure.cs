// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// This method returns a variety of quantity structure information of the webserver.
    /// </summary>
    public class ApiQuantityStructure
    {
        /// <summary>
        /// The size of the HTTP request body of a JSON-RPC request in bytes.
        /// </summary>
        public long Webapi_Max_Http_Request_Body_Size { get; set; }
        /// <summary>
        /// The number of parallel requests to the JSON-RPC endpoint.
        /// </summary>
        public int Webapi_Max_Parallel_Requests { get; set; }
        /// <summary>
        /// The number of parallel user sessions using the JSON-RPC endpoint.
        /// </summary>
        public int Webapi_Max_Parallel_User_Sessions { get; set; }

        /// <summary>
        /// Minimum Quantity Structure of Plc(s)
        /// </summary>
        public static ApiQuantityStructure MinimumQuantityStructure
        {
            get
            {
                return new ApiQuantityStructure
                {
                    Webapi_Max_Http_Request_Body_Size = 64 * 1024,
                    Webapi_Max_Parallel_Requests = 4,
                    Webapi_Max_Parallel_User_Sessions = 30
                };
            }
        }

        /// <summary>
        /// Check whether properties match
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>Returns true if the ApiQuantityStructures are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiQuantityStructure;
            return structure != null &&
                   structure.Webapi_Max_Parallel_User_Sessions == this.Webapi_Max_Parallel_User_Sessions &&
                   structure.Webapi_Max_Parallel_Requests == this.Webapi_Max_Parallel_Requests &&
                   structure.Webapi_Max_Http_Request_Body_Size == this.Webapi_Max_Http_Request_Body_Size;
        }

        /// <summary>
        /// GetHashCode for SequenceEqual etc.
        /// </summary>
        /// <returns>hashcode for the ApiQuantityStructures</returns>
        public override int GetHashCode()
        {
            return (Webapi_Max_Http_Request_Body_Size, Webapi_Max_Parallel_Requests, Webapi_Max_Parallel_User_Sessions).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiQuantityStructures
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
