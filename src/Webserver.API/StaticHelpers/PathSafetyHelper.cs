// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using System;
using System.IO;

namespace Siemens.Simatic.S7.Webserver.API.StaticHelpers
{
    internal static class PathSafetyHelper
    {
        public static string EnsureTrailingDirectorySeparator(string path)
        {
            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()) ||
                path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                return path;
            }

            return path + Path.DirectorySeparatorChar;
        }

        public static StringComparison GetPathComparison()
            => Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        public static string GetNormalizedDirectoryRoot(string path)
            => EnsureTrailingDirectorySeparator(Path.GetFullPath(path));

        public static bool IsPathContainedInRoot(string rootDirectory, string candidatePath)
            => candidatePath.StartsWith(rootDirectory, GetPathComparison());
    }
}