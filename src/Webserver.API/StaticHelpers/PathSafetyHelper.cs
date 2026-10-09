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

        public static string GetContainedFilePath(string rootDirectory, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException("Relative file path must not be null or empty.", nameof(relativePath));
            }

            var normalizedRelativePath = relativePath.Replace('\\', '/').Replace('/', Path.DirectorySeparatorChar);
            if (Path.IsPathRooted(normalizedRelativePath))
            {
                throw new IOException($"The file path '{relativePath}' must not be rooted.");
            }

            var normalizedRoot = GetNormalizedDirectoryRoot(rootDirectory);
            var candidatePath = Path.GetFullPath(Path.Combine(normalizedRoot, normalizedRelativePath));
            if (!IsPathContainedInRoot(normalizedRoot, candidatePath))
            {
                throw new IOException($"The file path '{relativePath}' escapes the selected directory '{normalizedRoot}'.");
            }

            return candidatePath;
        }

        public static bool IsPathContainedInRoot(string rootDirectory, string candidatePath)
            => candidatePath.StartsWith(rootDirectory, GetPathComparison());
    }
}