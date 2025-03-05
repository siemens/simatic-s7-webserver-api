// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Plc.ReadModeSelectorState result containing the mode selector enum
    /// </summary>
    public class ApiPlcReadModeSelectorStateResult
    {
        /// <summary>
        /// The state of the mode selector switch of the PLC
        /// </summary>
        public ApiPlcModeSelectorState Mode_Selector { get; set; }

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
