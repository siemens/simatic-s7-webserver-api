// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models.TimeSettings;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Check Request Parameters before sending the request(s) to the plc
    /// </summary>
    public interface IApiRequestParameterChecker
    {
        /// <summary>
        /// an etag has got a max. amount of characters of 128
        /// </summary>
        /// <param name="etag">etag (of resource) to be checked</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckETag(string etag, bool performCheck);
        /// <summary>
        /// Check the Last modified string on its regex
        /// </summary>
        /// <param name="lastModified">LastModified that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckLastModified(string lastModified, bool performCheck);
        /// <summary>
        /// Not checking anything currently!
        /// </summary>
        /// <param name="mediaType">MediaType that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckMediaType(string mediaType, bool performCheck);
        /// <summary>
        /// None isnt valid!
        /// </summary>
        /// <param name="plcProgramBrowseMode">PlcProgramBrowseMode that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckPlcProgramBrowseMode(ApiPlcProgramBrowseMode plcProgramBrowseMode, bool performCheck);
        /// <summary>
        /// None isnt valid!
        /// </summary>
        /// <param name="apiPlcProgramReadMode">PlcProgramReadMode  that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckPlcDataRepresentationMode(ApiPlcDataRepresentation? apiPlcProgramReadMode, bool performCheck);
        /// <summary>
        /// None isnt valid (unsupported type = -1)
        /// </summary>
        /// <param name="apiPlcProgramData">PlcProgramData</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckPlcProgramReadOrWriteDataType(ApiPlcProgramDataType apiPlcProgramData, bool performCheck);
        /// <summary>
        /// Only run or stop are valid!
        /// </summary>
        /// <param name="plcOperatingMode">Operating mode that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckPlcRequestChangeOperatingMode(ApiPlcOperatingMode plcOperatingMode, bool performCheck);
        /// <summary>
        /// valid charset: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.+()|,/.*!\"'"
        /// </summary>
        /// <param name="resourceName">Name of the resource that should be checked for the valid charset</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckResourceName(string resourceName, bool performCheck);
        /// <summary>
        /// Check ApiWebAppState => None isnt valid!
        /// </summary>
        /// <param name="apiWebAppState">Web Application State</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckWebAppState(ApiWebAppState apiWebAppState, bool performCheck);
        /// <summary>
        /// 28 Chars is the only accepted length for the ticketId!
        /// </summary>
        /// <param name="ticketId">TicketId that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckTicket(string ticketId, bool performCheck);
        /// <summary>
        /// None isnt valid!
        /// </summary>
        /// <param name="apiWebAppResourceVisibility">ResourceVisibility that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckWebAppResourceVisibility(ApiWebAppResourceVisibility apiWebAppResourceVisibility, bool performCheck);
        /// <summary>
        /// valid charset: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.+\""
        /// </summary>
        /// <param name="webAppName">Name of the Web Application</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckWebAppName(string webAppName, bool performCheck);
        /// <summary>
        /// Checks username for Api.ChangePassword (can't be the Anonymous user)
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckUsername(string username, bool performCheck);
        /// <summary>
        /// Checks whether current and new password matches
        /// </summary>
        /// <param name="currentPassword">Current password of the user</param>
        /// <param name="newPassword">New password for the user</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckPasswords(string currentPassword, string newPassword, bool performCheck);
        /// <summary>
        /// DateTime is only supported from 1970-01-01 to 2554-07-21 23:34:33.709551615
        /// </summary>
        /// <param name="timestamp">Timestamp to check</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckSystemTimeStamp(DateTime timestamp, bool performCheck);
        /// <summary>
        /// Checks whether the TimeSetting parameters are correct
        /// </summary>
        /// <param name="utcOffset">The time zone offset from the UTC time in hours</param>
        /// <param name="daylightSavings">Represents the settings for daylight-savings.</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        void CheckTimeSettings(TimeSpan utcOffset, DaylightSavingsRule daylightSavings, bool performCheck);
    }
}