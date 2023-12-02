// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models.FailsafeParameters;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Result containing Failsafe.ReadParameters objects
    /// </summary>
    public class ApiFailsafeReadParametersResult
    {
        /// <summary>
        /// The status if the safety mode is enabled or not.
        /// </summary>
        [JsonConverter(typeof(EnabledDisabledConverter))]
        public bool Safety_mode { get; set; }
        /// <summary>
        /// The type defines if the requested hardware identifier represents either the safety PLC itself or another failsafe module.
        /// </summary>
        public ApiFailsafeHardwareType Type { get; set; }
        /// <summary>
        /// If the requested hardware identifier represents the safety PLC itself and the PLC has a safety program,this object must be present using data structure 'FailsafeCPU' <br/>
        /// If the requested hardware identifier represents any other safety module than the Safety PLC itself, this object must be present using data structure 'FailsafeModule'
        /// </summary>
        public FailsafeHardware Parameters { get; set; }
    }
}
