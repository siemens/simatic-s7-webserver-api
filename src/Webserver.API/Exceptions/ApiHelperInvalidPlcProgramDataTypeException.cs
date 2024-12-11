// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// InvalidPlcProgramDataType 
    /// </summary>
    public class ApiHelperInvalidPlcProgramDataTypeException : Exception
    {
        /// <summary>
        /// InvalidPlcProgramDataType 
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ApiHelperInvalidPlcProgramDataTypeException(string message) : base(message) { }
    }
}
