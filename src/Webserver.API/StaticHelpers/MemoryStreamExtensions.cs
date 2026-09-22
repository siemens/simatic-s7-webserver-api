// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.IO;

namespace Siemens.Simatic.S7.Webserver.API.StaticHelpers
{
    /// <summary>
    /// https://stackoverflow.com/questions/4015602/equivalent-of-stringbuilder-for-byte-arrays
    /// </summary>
    public static class MemoryStreamExtensions
    {
        /// <summary>
        /// Append Method for comfort mimicing the StringBuilder for a byte[].
        /// This function was adapted from a Stack Overflow answer by Fredrik Mörl 
        /// returned the stream itself since this further mimics the 'StringBuilder style'
        /// https://stackoverflow.com/users/93623/fredrik-m%c3%b6rk on Stackoverflow:
        /// https://stackoverflow.com/questions/4015602/equivalent-of-stringbuilder-for-byte-arrays
        /// </summary>
        /// <param name="stream">Memory stream to be appended (byte[])</param>
        /// <param name="values">byte array to append the stream with</param>
        public static MemoryStream Append(this MemoryStream stream, byte[] values)
        {
            stream.Write(values, 0, values.Length);
            return stream;
        }
    }
}
