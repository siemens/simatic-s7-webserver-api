// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Services.FileHandling;
using System.IO;
using System.Linq;

namespace Webserver.API.UnitTests
{
    public class ApiDirectoryBuilderTests : Base
    {
        public string GetLocalNonexistantTmpDirectory()
        {
            var localTmpDirectory = @"C:\tmp";
            while (Directory.Exists(localTmpDirectory))
            {
                localTmpDirectory = Path.Combine(localTmpDirectory, "tmp");
            }
            return localTmpDirectory;
        }

        [Test]
        public void T001_NonExistantDirectory_ThrowsException()
        {
            var localTmpDirectory = GetLocalNonexistantTmpDirectory();
            // better would be a moq of the fileresourcebuilder
            var builder = new ApiDirectoryBuilder(localTmpDirectory, new ApiFileResourceBuilder(), false);
            var parseConfig = new ApiDirectoryBuilderConfiguration() { };
            Assert.Throws<DirectoryNotFoundException>(() => builder.Build(parseConfig));
        }

        [Test]
        public void T002_NullConfiguration_Works()
        {
            var localTmpDirectory = GetLocalNonexistantTmpDirectory();
            try
            {
                Directory.CreateDirectory(localTmpDirectory);
                var builder = new ApiDirectoryBuilder(localTmpDirectory, new ApiFileResourceBuilder(), false);
                builder.Build(parseConfiguration: null);
            }
            finally
            {
                Directory.Delete(localTmpDirectory, true);
            }
        }

        [Test]
        public void T003_NonExistantConfigFile_ThrowsException()
        {
            var localTmpDirectory = GetLocalNonexistantTmpDirectory();
            try
            {
                Directory.CreateDirectory(localTmpDirectory);
                var configFile = Path.Combine(localTmpDirectory, "configfile.json");
                var builder = new ApiDirectoryBuilder(localTmpDirectory, new ApiFileResourceBuilder(), false);
                Assert.Throws<FileNotFoundException>(() => builder.Build(configFile));
            }
            finally
            {
                Directory.Delete(localTmpDirectory, true);
            }
        }


        [Test]
        public void T004_EmptyDirectory_Built()
        {
            var localTmpDirectory = GetLocalNonexistantTmpDirectory();
            try
            {
                Directory.CreateDirectory(localTmpDirectory);
                var builder = new ApiDirectoryBuilder(localTmpDirectory, new ApiFileResourceBuilder(), false);
                var parseConfig = new ApiDirectoryBuilderConfiguration() { };
                var res = builder.Build(parseConfig);
                Assert.That(res.PathToLocalDirectory, Is.EqualTo(localTmpDirectory));
                Assert.That(res.Resources.Count, Is.EqualTo(0));
                Assert.That(res.State, Is.EqualTo(ApiFileResourceState.Active));
                var dirInf = new DirectoryInfo(localTmpDirectory);
                Assert.That(res.Last_Modified, Is.EqualTo(dirInf.LastWriteTime));
                Assert.That(res.Name, Is.EqualTo(dirInf.Name));
                Assert.That(res.Size, Is.Null);
            }
            finally
            {
                Directory.Delete(localTmpDirectory, true);
            }
        }

        [Test]
        public void T005_DirectoryWithFile_Built()
        {
            var localTmpDirectory = GetLocalNonexistantTmpDirectory();
            try
            {
                Directory.CreateDirectory(localTmpDirectory);
                var filePath = Path.Combine(localTmpDirectory, "file1.txt");
                using (var fs = File.Create(filePath))
                {
                    fs.Close();
                }
                var builder = new ApiDirectoryBuilder(localTmpDirectory, new ApiFileResourceBuilder(), false);
                var parseConfig = new ApiDirectoryBuilderConfiguration() { };
                var res = builder.Build(parseConfig);
                Assert.That(res.Resources.Count, Is.EqualTo(1));
                Assert.That(res.Size, Is.Null);
                var firstRes = res.Resources.First();
                Assert.That(firstRes.Type, Is.EqualTo(ApiFileResourceType.File));
                Assert.That(firstRes.State, Is.EqualTo(ApiFileResourceState.Active));
                var fileInfo = new FileInfo(filePath);
                Assert.That(firstRes.Size, Is.EqualTo(fileInfo.Length));
                Assert.That(firstRes.Last_Modified, Is.EqualTo(fileInfo.LastWriteTime));
                Assert.That(firstRes.PathToLocalDirectory, Is.EqualTo(localTmpDirectory));
            }
            finally
            {
                Directory.Delete(localTmpDirectory, true);
            }
        }

        [Test]
        public void T006_DirectoryWithDir_Built()
        {
            var localTmpDirectory = GetLocalNonexistantTmpDirectory();
            try
            {
                Directory.CreateDirectory(localTmpDirectory);
                var dirPath = Path.Combine(localTmpDirectory, "temp");
                var dirInfo = Directory.CreateDirectory(dirPath);
                var builder = new ApiDirectoryBuilder(localTmpDirectory, new ApiFileResourceBuilder(), false);
                var parseConfig = new ApiDirectoryBuilderConfiguration() { };
                var res = builder.Build(parseConfig);
                Assert.That(res.Resources.Count, Is.EqualTo(1));
                Assert.That(res.Size, Is.Null);
                var firstRes = res.Resources.First();
                Assert.That(firstRes.Type, Is.EqualTo(ApiFileResourceType.Dir));
                Assert.That(firstRes.State, Is.EqualTo(ApiFileResourceState.Active));
                Assert.That(firstRes.Size, Is.Null);
                Assert.That(firstRes.Last_Modified, Is.EqualTo(dirInfo.LastWriteTime));
                Assert.That(firstRes.PathToLocalDirectory, Is.EqualTo(dirPath));
            }
            finally
            {
                Directory.Delete(localTmpDirectory, true);
            }
        }

        [Test]
        public void T007_DirectoryWithDirAndFiles_Built()
        {
            var localTmpDirectory = GetLocalNonexistantTmpDirectory();
            try
            {
                var dirInfo = Directory.CreateDirectory(localTmpDirectory);

                var fileName = "file1.txt";
                var filePath = Path.Combine(localTmpDirectory, fileName);
                using (var fs = File.Create(filePath))
                {
                    fs.Close();
                }
                var fileInfo = new FileInfo(filePath);

                var dirPath2 = Path.Combine(localTmpDirectory, "temp");
                var dirInfo2 = Directory.CreateDirectory(dirPath2);
                var fileName2 = "file2.txt";
                var filePath2 = Path.Combine(dirPath2, fileName2);
                using (var fs = File.Create(filePath2))
                {
                    fs.Close();
                }
                var fileInfo2 = new FileInfo(filePath2);

                var dirPath3 = Path.Combine(localTmpDirectory, "temp", "temp");
                var dirInfo3 = Directory.CreateDirectory(dirPath3);
                var fileName3 = "file3.txt";
                var filePath3 = Path.Combine(dirPath3, fileName3);
                using (var fs = File.Create(filePath3))
                {
                    fs.Close();
                }
                var fileInfo3 = new FileInfo(filePath3);

                var builder = new ApiDirectoryBuilder(localTmpDirectory, new ApiFileResourceBuilder(), false);
                var parseConfig = new ApiDirectoryBuilderConfiguration() { };
                var res = builder.Build(parseConfig);
                Assert.That(res.Resources.Count, Is.EqualTo(2));
                var dirRes = res.Resources.First(el => el.Type == ApiFileResourceType.Dir);
                var fileRes = res.Resources.First(el => el.Type == ApiFileResourceType.File);
                Assert.That(dirRes.Name, Is.EqualTo(dirInfo2.Name));
                Assert.That(dirRes.Resources.Count, Is.EqualTo(2));
                Assert.That(dirRes.Parents.Count, Is.EqualTo(1));
                Assert.That(dirRes.PathToLocalDirectory, Is.EqualTo(dirInfo2.FullName));

                Assert.That(fileRes.Name, Is.EqualTo(fileInfo.Name));
                Assert.That(Path.Combine(fileRes.PathToLocalDirectory, fileRes.Name), Is.EqualTo(filePath));
                Assert.That(fileRes.Parents.Count, Is.EqualTo(1));
                Assert.That(fileRes.Resources.Count, Is.EqualTo(0));

                var subRes = dirRes.Resources;
                Assert.That(subRes.Count, Is.EqualTo(2));
                var dirRes2 = subRes.First(el => el.Type == ApiFileResourceType.Dir);
                var fileRes2 = subRes.First(el => el.Type == ApiFileResourceType.File);
                Assert.That(dirRes2.Name, Is.EqualTo(dirInfo3.Name));
                Assert.That(dirRes2.Resources.Count, Is.EqualTo(1));
                Assert.That(dirRes2.Parents.Count, Is.EqualTo(2));
                Assert.That(dirRes2.PathToLocalDirectory, Is.EqualTo(dirPath3));

                Assert.That(fileRes2.Name, Is.EqualTo(fileInfo2.Name));
                Assert.That(Path.Combine(fileRes2.PathToLocalDirectory, fileRes2.Name), Is.EqualTo(filePath2));
                Assert.That(fileRes2.Parents.Count, Is.EqualTo(2));
                Assert.That(fileRes2.Resources.Count, Is.EqualTo(0));

                var fileRes3 = dirRes2.Resources.First();
                Assert.That(fileRes3.Name, Is.EqualTo(fileInfo3.Name));
                Assert.That(Path.Combine(fileRes3.PathToLocalDirectory, fileRes3.Name), Is.EqualTo(filePath3));
                Assert.That(fileRes3.Resources.Count, Is.EqualTo(0));
                Assert.That(fileRes3.Parents.Count, Is.EqualTo(3));
            }
            finally
            {
                Directory.Delete(localTmpDirectory, true);
            }
        }
    }
}
