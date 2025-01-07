// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// ApiResponse (Jsonrpc,id) with the plcOperatingMode
    /// </summary>
    public class ApiReadOperatingModeResponse : ApiResultResponse<ApiPlcOperatingMode>
    {
        /// <summary>
        /// ApiPlcOperatingMode Result => Only accept valid Plc Operating Modes!
        /// </summary>
        public override ApiPlcOperatingMode Result
        {
            get => base.Result;
            set
            {
                if (value == ApiPlcOperatingMode.None || value == ApiPlcOperatingMode.Unknown)
                    throw new ApiInvalidResponseException($"ApiPlcOperatingmode returned from api was:{value.ToString()} - which is not valid! contact Siemens");
                base.Result = value;
            }
        }
    }
}
