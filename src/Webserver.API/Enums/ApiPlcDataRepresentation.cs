// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Determines the response format for various methods that return data from the PLC.
    /// </summary>
    public enum ApiPlcDataRepresentation
    {
        /// <summary>
        /// Should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// "Simple", comfortable format -- represented in a semi-human-readable format.
        /// </summary>
        Simple = 1,
        /// <summary>
        /// The data is represented in big endian format, formatted as a JSON byte array.
        /// </summary>
        Raw = 2
    }
}
