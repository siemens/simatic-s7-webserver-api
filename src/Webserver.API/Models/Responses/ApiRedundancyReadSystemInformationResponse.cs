// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.RedundancySystemInformation;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// Response to an Redundancy.ReadSystemInformation request
    /// </summary>
    public class ApiRedundancyReadSystemInformationResponse : ApiResultResponse<ApiRedundancySystemInfo>
    {
    }
}
