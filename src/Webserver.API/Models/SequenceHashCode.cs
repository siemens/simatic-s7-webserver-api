// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    internal static class SequenceHashCode
    {
        public static int GetSequenceHashCode<T>(IEnumerable<T> values)
        {
            if (values == null)
            {
                return 0;
            }

            unchecked
            {
                int hashCode = 17;
                foreach (T value in values)
                {
                    hashCode = hashCode * 31 + EqualityComparer<T>.Default.GetHashCode(value);
                }
                return hashCode;
            }
        }
    }
}