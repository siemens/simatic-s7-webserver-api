using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Services.FileHandling;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Assert.AreEqual(localTmpDirectory, res.PathToLocalDirectory);
                Assert.AreEqual(0, res.Resources.Count);
                Assert.AreEqual(ApiFileResourceState.Active, res.State);
                var dirInf = new DirectoryInfo(localTmpDirectory);
                Assert.AreEqual(dirInf.LastWriteTime, res.Last_Modified);
                Assert.AreEqual(dirInf.Name, res.Name);
                Assert.IsNull(res.Size);
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
                Assert.AreEqual(1, res.Resources.Count);
                Assert.IsNull(res.Size);
                var firstRes = res.Resources.First();
                Assert.AreEqual(ApiFileResourceType.File, firstRes.Type);
                Assert.AreEqual(ApiFileResourceState.Active, firstRes.State);
                var fileInfo = new FileInfo(filePath);
                Assert.AreEqual(fileInfo.Length, firstRes.Size);
                Assert.AreEqual(fileInfo.LastWriteTime, firstRes.Last_Modified);
                Assert.AreEqual(localTmpDirectory, firstRes.PathToLocalDirectory);
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
                Assert.AreEqual(1, res.Resources.Count);
                Assert.IsNull(res.Size);
                var firstRes = res.Resources.First();
                Assert.AreEqual(ApiFileResourceType.Dir, firstRes.Type);
                Assert.AreEqual(ApiFileResourceState.Active, firstRes.State);
                Assert.IsNull(firstRes.Size);
                Assert.AreEqual(dirInfo.LastWriteTime, firstRes.Last_Modified);
                Assert.AreEqual(dirPath, firstRes.PathToLocalDirectory);
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
                Assert.AreEqual(2, res.Resources.Count);
                var dirRes = res.Resources.First(el => el.Type == ApiFileResourceType.Dir);
                var fileRes = res.Resources.First(el => el.Type == ApiFileResourceType.File);
                Assert.AreEqual(dirInfo2.Name, dirRes.Name);
                Assert.AreEqual(2, dirRes.Resources.Count);
                Assert.AreEqual(1, dirRes.Parents.Count);
                Assert.AreEqual(dirInfo2.FullName, dirRes.PathToLocalDirectory);

                Assert.AreEqual(fileInfo.Name, fileRes.Name);
                Assert.AreEqual(filePath, Path.Combine(fileRes.PathToLocalDirectory, fileRes.Name));
                Assert.AreEqual(1, fileRes.Parents.Count);
                Assert.IsNull(fileRes.Resources);

                var subRes = dirRes.Resources;
                Assert.AreEqual(2, subRes.Count);
                var dirRes2 = subRes.First(el => el.Type == ApiFileResourceType.Dir);
                var fileRes2 = subRes.First(el => el.Type == ApiFileResourceType.File);
                Assert.AreEqual(dirInfo3.Name, dirRes2.Name);
                Assert.AreEqual(1, dirRes2.Resources.Count);
                Assert.AreEqual(2, dirRes2.Parents.Count);
                Assert.AreEqual(dirPath3, dirRes2.PathToLocalDirectory);

                Assert.AreEqual(fileInfo2.Name, fileRes2.Name);
                Assert.AreEqual(filePath2, Path.Combine(fileRes2.PathToLocalDirectory, fileRes2.Name));
                Assert.AreEqual(2, fileRes2.Parents.Count);
                Assert.IsNull(fileRes2.Resources);

                var fileRes3 = dirRes2.Resources.First();
                Assert.AreEqual(fileInfo3.Name, fileRes3.Name);
                Assert.AreEqual(filePath3, Path.Combine(fileRes3.PathToLocalDirectory, fileRes3.Name));
                Assert.IsNull(fileRes3.Resources);
                Assert.AreEqual(3, fileRes3.Parents.Count);
            }
            finally
            {
                Directory.Delete(localTmpDirectory, true);
            }
        }
    }
}
