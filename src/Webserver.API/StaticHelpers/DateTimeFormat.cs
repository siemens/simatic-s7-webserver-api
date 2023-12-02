// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.StaticHelpers
{
    /// <summary>
    /// Used for RFC3339 Matching UTC Time
    /// </summary>
    public static class DateTimeFormatting
    {
        /// <summary>
        /// APIDateTimeFormat : "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"
        /// </summary>
        public const string ApiDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
    }
}
