// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Not accepted;\r\n" +
    /// "In login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.\r\n" +
    /// "For WebApp.SetUrlRedirectMode: ​The method cannot be executed because this method of the application is not supported either for this application type or for the loaded project version.";
    /// </summary>
    public class ApiNotAcceptedException : Exception
    {
        private static readonly string message = "Not accepted;\r\n" +
            "In login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.\r\n" +
            "For WebApp.SetUrlRedirectMode: ​The method cannot be executed because this method of the application is not supported either for this application type or for the loaded project version.";
        /// <summary>
        /// Not accepted;\r\n" +
        /// "In login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.\r\n" +
        /// "For WebApp.SetUrlRedirectMode: ​The method cannot be executed because this method of the application is not supported either for this application type or for the loaded project version.";
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNotAcceptedException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Not accepted;\r\n" +
        /// "In login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.\r\n" +
        /// "For WebApp.SetUrlRedirectMode: ​The method cannot be executed because this method of the application is not supported either for this application type or for the loaded project version.";
        /// </summary>
        public ApiNotAcceptedException() : base(message) { }
        /// <summary>
        /// Not accepted;\r\n" +
        /// "In login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.\r\n" +
        /// "For WebApp.SetUrlRedirectMode: ​The method cannot be executed because this method of the application is not supported either for this application type or for the loaded project version.";
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiNotAcceptedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Not accepted;\r\n" +
        /// "In login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.\r\n" +
        /// "For WebApp.SetUrlRedirectMode: ​The method cannot be executed because this method of the application is not supported either for this application type or for the loaded project version.";
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNotAcceptedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
