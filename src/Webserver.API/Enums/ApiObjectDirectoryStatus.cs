// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// The status of the entry. Either incoming or outgoing.
    /// </summary>
    public enum ApiObjectDirectoryStatus
    {
        /// <summary>
        /// The entry is incoming
        /// </summary>
        Incoming = 1,
        /// <summary>
        /// The entry is outgoing
        /// </summary>
        Outgoing = 2
    }
}