// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using Siemens.Simatic.S7.Webserver.API.Models.TimeSettings;
using System;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Check Request Parameters before sending the request(s) to the plc
    /// </summary>
    public class ApiRequestParameterChecker : IApiRequestParameterChecker
    {
        /// <summary>
        /// Check ApiWebAppState => None isnt valid!
        /// </summary>
        /// <param name="apiWebAppState">Web Application State</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckWebAppState(ApiWebAppState apiWebAppState, bool performCheck)
        {
            if (performCheck)
            {
                if (apiWebAppState == ApiWebAppState.None)
                {
                    throw new ApiInvalidParametersException($"WebApp function shall not be called with state None:{Environment.NewLine + apiWebAppState.ToString()}" +
                    $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidParams, Message = "Invalid Params" } }));
                }
            }
        }

        /// <summary>
        /// valid charset: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.+\""
        /// </summary>
        /// <param name="webAppName">Name of the Web Application</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckWebAppName(string webAppName, bool performCheck)
        {
            if (performCheck)
            {
                if (webAppName.Length == 0)
                {
                    throw new ApiInvalidParametersException($"the webapp name cannot be an empty string! :{Environment.NewLine + webAppName + Environment.NewLine}is not valid!",
                        new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidParams, Message = "Invalid Params" } }));
                }
                if (webAppName.Length > 100)
                {
                    throw new ApiInvalidApplicationNameException($"the max. allowed length for a webapp is 100 chars! - therefor :{Environment.NewLine + webAppName + Environment.NewLine}is not valid!",
                        new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidApplicationName, Message = "Invalid application name" } }));
                }
                string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.+\"";
                if (!webAppName.All(c => validChars.Contains(c)))
                {
                    throw new ApiInvalidApplicationNameException($"Invalid characters found in:{Environment.NewLine + webAppName + Environment.NewLine} correct chars:{Environment.NewLine + validChars}",
                         new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidApplicationName, Message = "Invalid application name" } }));
                }
            }
        }

        /// <summary>
        /// None isnt valid (unsupported type = -1)
        /// </summary>
        /// <param name="apiPlcProgramData">PlcProgramData</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckPlcProgramReadOrWriteDataType(ApiPlcProgramDataType apiPlcProgramData, bool performCheck)
        {
            if (performCheck)
            {
                if (apiPlcProgramData == ApiPlcProgramDataType.None)
                {
                    throw new ApiHelperInvalidPlcProgramDataTypeException($"PlcProgram Read or Write Comfort functions are not available without the DataType!(Given:{apiPlcProgramData.ToString()})" +
                        $"{Environment.NewLine}Browse for the PlcProgramData first, set it and then use comfort functionality with given DataType!");
                }
                var bytesOfDataType = apiPlcProgramData.GetBytesOfDataType();
                // unsupported: -1
                if (bytesOfDataType == -1)
                {
                    throw new ApiUnsupportedAddressException(new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.UnsupportedAddress, Message = "Unsupported Address" } }));
                }
            }
        }

        /// <summary>
        /// valid charset: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.+()|,/.*!\"'"
        /// </summary>
        /// <param name="resourceName">Name of the resource that should be checked for the valid charset</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckResourceName(string resourceName, bool performCheck)
        {
            if (performCheck)
            {
                if (resourceName.Length == 0)
                {
                    throw new ApiInvalidParametersException($"the resource name cannot be an empty string! :{Environment.NewLine + resourceName + Environment.NewLine}is not valid!",
                        new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidParams, Message = "Invalid Params" } }));
                }
                if (resourceName.Length > 200)
                {
                    throw new ApiInvalidResourceNameException($"the max. allowed length for a resource is 200 chars! - therefor :{Environment.NewLine + resourceName + Environment.NewLine}is not valid!",
                        new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidResourceName, Message = "Invalid resource name" } }));
                }
                string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.+()|,/.*!\"'";
                if (!resourceName.All(c => validChars.Contains(c)))
                {
                    throw new ApiInvalidResourceNameException($"Invalid characters found in:{Environment.NewLine + resourceName + Environment.NewLine} correct chars:{Environment.NewLine + validChars}",
                         new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidResourceName, Message = "Invalid resource name" } }));
                }
            }
        }

        /// <summary>
        /// Only run or stop are valid!
        /// </summary>
        /// <param name="plcOperatingMode">Operating mode that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckPlcRequestChangeOperatingMode(ApiPlcOperatingMode plcOperatingMode, bool performCheck)
        {
            if (performCheck)
            {
                if (plcOperatingMode == ApiPlcOperatingMode.Run || plcOperatingMode == ApiPlcOperatingMode.Stop)
                    return;
                throw new ApiInvalidParametersException($"Plc.RequestChangeOperatingMode shall not be called with {Environment.NewLine + plcOperatingMode.ToString().ToLower()}" +
                    $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidParams, Message = "Invalid Params" } }));
            }
        }

        /// <summary>
        /// None isnt valid!
        /// </summary>
        /// <param name="plcProgramBrowseMode">PlcProgramBrowseMode that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckPlcProgramBrowseMode(ApiPlcProgramBrowseMode plcProgramBrowseMode, bool performCheck)
        {
            if (performCheck)
            {
                if (plcProgramBrowseMode == ApiPlcProgramBrowseMode.None)
                {
                    throw new ApiInvalidParametersException($"PlcProgram.Browse shall not be called with {Environment.NewLine + plcProgramBrowseMode.ToString().ToLower()}" +
                    $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidParams, Message = "Invalid Params" } }));
                }
            }
        }

        /// <summary>
        /// None isnt valid!
        /// </summary>
        /// <param name="apiPlcProgramReadMode">Determines the response format for various methods that return data from the PLC</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckPlcDataRepresentationMode(ApiPlcDataRepresentation? apiPlcProgramReadMode, bool performCheck)
        {
            if (performCheck)
            {
                if (apiPlcProgramReadMode != null)
                {
                    if (apiPlcProgramReadMode == ApiPlcDataRepresentation.None)
                    {
                        throw new ApiInvalidParametersException($"PlcProgram.Read or Write shall not be called with {Environment.NewLine + apiPlcProgramReadMode.ToString().ToLower()}" +
                        $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidParams, Message = "Invalid Params" } }));
                    }
                }
            }
        }

        /// <summary>
        /// 28 Chars is the only accepted length for the ticketId!
        /// </summary>
        /// <param name="ticketId">TicketId that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckTicket(string ticketId, bool performCheck)
        {
            if (performCheck)
            {
                if (ticketId?.Length != 28 && ticketId?.Length != 36)
                {
                    throw new ApiInvalidParametersException($"Api Tickets cannot have a length other than 28 bytes!{Environment.NewLine + ticketId + Environment.NewLine}provide a valid ticket!" +
                                        $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidParams, Message = "Invalid Params" } }));
                }
            }
        }

        /// <summary>
        /// an etag has got a max. amount of characters of 128
        /// </summary>
        /// <param name="etag">etag (of resource) to be checked</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckETag(string etag, bool performCheck)
        {
            if (performCheck)
            {
                if (string.IsNullOrEmpty(etag))
                {
                    // maybe throw here but for now let "the plc decide"
                }
                else if (etag.Length > 128)
                {
                    throw new ApiInvalidETagException($"WebApp.CreateResource shall not be called with \"etag\" {Environment.NewLine + etag} because the value is too long!-max 128 bytes(chars)" +
                        $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidParams, Message = "Invalid Params" } }));
                }
            }
        }

        /// <summary>
        /// Not checking anything currently!
        /// </summary>
        /// <param name="mediaType">MediaType that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckMediaType(string mediaType, bool performCheck)
        {
            if (performCheck)
            {
                ;
            }
            // could provide a insanely long list of possible mediaTypes look: https://www.iana.org/assignments/media-types/media-types.xhtml - will not do it until requested!
        }

        /// <summary>
        /// None isnt valid!
        /// </summary>
        /// <param name="apiWebAppResourceVisibility">ResourceVisibility that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckWebAppResourceVisibility(ApiWebAppResourceVisibility apiWebAppResourceVisibility, bool performCheck)
        {
            if (performCheck)
            {
                if (apiWebAppResourceVisibility == ApiWebAppResourceVisibility.None)
                    throw new ApiInvalidParametersException($"WebApp.CreateResource shall not be called with {Environment.NewLine + apiWebAppResourceVisibility.ToString().ToLower()}" +
                $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidParams, Message = "Invalid Params" } }));
            }
        }

        /// <summary>
        /// Currently don't check - last use:
        /// regex used: Regex regex = new Regex(@"\d{4}(-\d{2})(-\d{2})T(\d{2}):(\d{2}):(\d{2})(\.[0-9]{1,3})*Z"); string has to match!
        /// </summary>
        /// <param name="lastModified">LastModified that should be checked for being valid</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckLastModified(string lastModified, bool performCheck)
        {
            if (performCheck)
            {
                //Regex regex = new Regex(@"\d{4}(-\d{2})(-\d{2})T(\d{2}):(\d{2}):(\d{2})(\.[0-9]{1,3})*Z");
                //if (!regex.IsMatch(lastModified))
                //{
                //    throw new ApiInvalidParametersException($"the datetime provided does not match the pattern:{Environment.NewLine + lastModified + Environment.NewLine + DateTimeFormatting.ApiDateTimeFormat + Environment.NewLine}used Regex:{regex.ToString()}"/*, formatException*//*);
                //}
                // for now not 100% sure the regex check is fine => let the plc perform the check for now/wait for a request to check locally
                ;
            }
        }

        /// <summary>
        /// DateTime is only supported from 1970-01-01 to 2554-07-21 23:34:33.709551615
        /// </summary>
        /// <param name="timestamp">Timestamp to check</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckSystemTimeStamp(DateTime timestamp, bool performCheck)
        {
            if (performCheck)
            {
                var dtMin = new DateTime(1970, 1, 1);
                var dtMax = new DateTime(2554, 7, 21, 23, 34, 33, 709);
                if (timestamp < dtMin || timestamp > dtMax)
                {
                    throw new ApiTimestampOutOfRangeException($"The given date is unsupported -- it's out of range! Supported range is between 1970-01-01T00:00:00Z and 2554-07-21T23:34:33.709551615Z" +
                    $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.TimestampOutOfRange, Message = "The timestamp is not within the allowed range of timestamps" } }));
                }
            }
        }

        /// <summary>
        /// Override equals - will always be true since there are no Properties to compare
        /// </summary>
        /// <param name="obj">obj to compare to</param>
        /// <returns>true if the obj is an ApiRequestParameterChecker</returns>
        public override bool Equals(object obj) => Equals(obj as ApiRequestParameterChecker);

        /// <summary>
        /// Always true as there are no Properties to compare
        /// </summary>
        /// <param name="obj">obj to compare to</param>
        /// <returns>true if the obj is an ApiRequestParameterChecker</returns>
        public bool Equals(ApiRequestParameterChecker obj)
        {
            return true;
        }

        /// <summary>
        /// base.GetHashCode()
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Checks username for Api.ChangePassword (can't be the Anonymous user)
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        public void CheckUsername(string username, bool performCheck)
        {
            if (performCheck)
            {
                if (username == null || username == "")
                {
                    throw new ApiInvalidParametersException($"Username can't be empty! {Environment.NewLine}" +
                            $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidParams, Message = "Invalid Params" } }));
                }
                if (username == "Anonymous")
                {
                    throw new ApiNotAcceptedException($"Password can not be changed for the Anonymous user! {Environment.NewLine}" +
                           $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.NotAccepted, Message = "The password change cannot be performed" } }));
                }
            }
        }
        /// <summary>
        /// Checks whether current and new password matches
        /// </summary>
        /// <param name="currentPassword">Current password of the user</param>
        /// <param name="newPassword">New password for the user</param>
        /// <param name="performCheck">Bool to determine whether to really perform the check or not</param>
        public void CheckPasswords(string currentPassword, string newPassword, bool performCheck)
        {
            if (performCheck)
            {
                if (currentPassword == newPassword)
                {
                    throw new ApiNewPasswordMatchesOldPasswordException($"New password can not be the same as current password! {Environment.NewLine}" +
                          $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.NewPasswordMatchesOldPassword, Message = "The provided new password is identical with a former password" } }));
                }
            }
        }

        /// <summary>
        /// Checks whether the TimeSetting parameters are correct
        /// </summary>
        /// <param name="utcOffset">The time zone offset from the UTC time in hours</param>
        /// <param name="daylightSavings">Represents the settings for daylight-savings.</param>
        /// <param name="performCheck">Bool to determine wether to really perform the check or not</param>
        public void CheckTimeSettings(TimeSpan utcOffset, DaylightSavingsRule daylightSavings, bool performCheck)
        {
            if (performCheck)
            {
                //the time range of UTC offsets is between UTC+13 and UTC-12, this may change in the future.
                //If it contains a precision higher than minutes, it must be considered invalid.
                if (utcOffset.Seconds != 0 || utcOffset.Milliseconds != 0 ||
                    utcOffset.TotalHours > 13 || utcOffset.TotalHours < -12)
                {
                    throw new ApiInvalidUtcOffsetException($"The UTC offset must be between +13hrs and -12hrs. Please do not use seconds and milliseconds." +
                    $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidUtcOffset, Message = "Invalid UTC offset" } }));
                }
                //allowed value range must be between -180 and +180 (inclusive).
                //If it contains a precision higher than minutes, it must be considered invalid.
                if (daylightSavings != null)
                {
                    if (daylightSavings.Dst == null || daylightSavings.Std == null ||
                        daylightSavings.Dst.Start == null || daylightSavings.Std.Start == null ||
                        daylightSavings.Dst.Offset.TotalMinutes > 180 || daylightSavings.Dst.Offset.TotalMinutes < -180 ||
                        daylightSavings.Dst.Offset.Seconds != 0 || daylightSavings.Dst.Offset.Milliseconds != 0)
                    {
                        throw new ApiInvalidTimeRuleException($"The DaylightSavingsTimeConfiguration TimeOffset must be between +180mins and -180mins. Please do not use seconds and milliseconds." +
                        $"{Environment.NewLine}Probably Api would send: ", new ApiException(new ApiErrorModel() { Error = new ApiError() { Code = ApiErrorCode.InvalidTimeRule, Message = "Invalid time rule" } }));
                    }
                }

            }
        }
    }
}
