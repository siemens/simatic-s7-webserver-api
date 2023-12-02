// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The provided rule is invalid. Check the DaylightSavingsRule object. If the Rule object is present, both DST and STD is required.
    /// </summary>
    public class ApiInvalidTimeRuleException : Exception
    {
        private static string message = "The provided rule is invalid. Check the DaylightSavingsRule object. If the Rule object is present, both DST and STD is required.";
        /// <summary>
        /// The provided rule is invalid. Check the DaylightSavingsRule object. If the Rule object is present, both DST and STD is required.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidTimeRuleException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The provided rule is invalid. Check the DaylightSavingsRule object. If the Rule object is present, both DST and STD is required.
        /// </summary>
        public ApiInvalidTimeRuleException() : base(message) { }
        /// <summary>
        /// The provided rule is invalid. Check the DaylightSavingsRule object. If the Rule object is present, both DST and STD is required.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidTimeRuleException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The provided rule is invalid. Check the DaylightSavingsRule object. If the Rule object is present, both DST and STD is required.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidTimeRuleException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
