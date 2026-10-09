using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.IO;

namespace Webserver.API.UnitTests
{
    public class PathSafetyHelperTests
    {
        [TestCase("nested/file.txt")]
        [TestCase("nested\\file.txt")]
        [TestCase("nested/../nested/file.txt")]
        public void GetContainedFilePath_NestedRelativePath_ReturnsNormalizedContainedPath(string relativePath)
        {
            var rootDirectory = Path.Combine(Path.GetTempPath(), "root");

            var result = PathSafetyHelper.GetContainedFilePath(rootDirectory, relativePath);

            Assert.That(result, Is.EqualTo(Path.GetFullPath(Path.Combine(rootDirectory, "nested", "file.txt"))));
        }

        [TestCase("../root-other/file.txt")]
        [TestCase("..\\root-other\\file.txt")]
        [TestCase("nested\\../../root-other/file.txt")]
        [TestCase("../file.txt")]
        [TestCase("..")]
        public void GetContainedFilePath_PathEscapesRoot_ThrowsIOException(string relativePath)
        {
            var rootDirectory = Path.Combine(Path.GetTempPath(), "root");

            Assert.Throws<IOException>(() => PathSafetyHelper.GetContainedFilePath(rootDirectory, relativePath));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetContainedFilePath_EmptyRelativePath_ThrowsArgumentException(string relativePath)
        {
            Assert.Throws<ArgumentException>(() => PathSafetyHelper.GetContainedFilePath(Path.GetTempPath(), relativePath));
        }
    }
}