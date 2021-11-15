// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Check Request Parameters before sending the request(s) to the plc
    /// </summary>
    public interface IRequestParameterChecker
    {
        /// <summary>
        /// an etag has got a max. amount of characters of 128
        /// </summary>
        /// <param name="etag">etag (of resource) to be checked</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckETag(string etag, bool performCheck);
        /// <summary>
        /// regex used: Regex regex = new Regex(@"\d{4}(-\d{2})(-\d{2})T(\d{2}):(\d{2}):(\d{2})(\.[0-9]{1,3})*Z"); string has to match!
        /// </summary>
        /// <param name="lastModified">LastModified that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckLastModified(string lastModified, bool performCheck);
        /// <summary>
        /// Not checking anything currently!
        /// </summary>
        /// <param name="mediaType">MediaType that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckMediaType(string mediaType, bool performCheck);
        /// <summary>
        /// None isnt valid!
        /// </summary>
        /// <param name="plcProgramBrowseMode">PlcProgramBrowseMode that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckPlcProgramBrowseMode(ApiPlcProgramBrowseMode plcProgramBrowseMode, bool performCheck);
        /// <summary>
        /// None isnt valid!
        /// </summary>
        /// <param name="apiPlcProgramReadMode">PlcProgramReadMode  that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckPlcProgramReadOrWriteMode(ApiPlcProgramReadOrWriteMode? apiPlcProgramReadMode, bool performCheck);
        /// <summary>
        /// None isnt valid (unsupported type = -1)
        /// </summary>
        /// <param name="apiPlcProgramData">PlcProgramData</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckPlcProgramReadOrWriteDataType(ApiPlcProgramDataType apiPlcProgramData, bool performCheck);
        /// <summary>
        /// Only run or stop are valid!
        /// </summary>
        /// <param name="plcOperatingMode">Operating mode that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckPlcRequestChangeOperatingMode(ApiPlcOperatingMode plcOperatingMode, bool performCheck);
        /// <summary>
        /// valid charset: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.+()|,/.*!\"'"
        /// </summary>
        /// <param name="resourceName">Name of the resource that should be checked for the valid charset</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckResourceName(string resourceName, bool performCheck);
        /// <summary>
        /// Check ApiWebAppState => None isnt valid!
        /// </summary>
        /// <param name="apiWebAppState">Web Application State</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckWebAppState(ApiWebAppState apiWebAppState, bool performCheck);
        /// <summary>
        /// 28 Chars is the only accepted length for the ticketId!
        /// </summary>
        /// <param name="ticketId">TicketId that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckTicket(string ticketId, bool performCheck);
        /// <summary>
        /// None isnt valid!
        /// </summary>
        /// <param name="apiWebAppResourceVisibility">ResourceVisibility that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckWebAppResourceVisibility(ApiWebAppResourceVisibility apiWebAppResourceVisibility, bool performCheck);
        /// <summary>
        /// valid charset: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.+\""
        /// </summary>
        /// <param name="webAppName">Name of the Web Application</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        void CheckWebAppName(string webAppName, bool performCheck);
    }
}