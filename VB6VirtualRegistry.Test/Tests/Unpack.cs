using System.IO;
using Xunit;

namespace VB6VirtualRegistry.Test
{
    public class Unpack
    {
        private readonly string _msixApp = "unsignedmyvb6app.msix";

        [Fact]        
        public void FullPath()
        {

            string unpackagedDir = "C:\\MSIX\\UnpackTest\\files";

            if (Directory.Exists(unpackagedDir))
            {
                Directory.Delete(unpackagedDir, true);
            }

            string[] args = {  "unpack",
                              $"{Vb6VirtualRegistry.CurrentDirectoryHelper.GetCurrentDirectory()}\\{_msixApp}",
                              unpackagedDir
                             };

            Vb6VirtualRegistry.Program.Main(args);

            Assert.True(Directory.Exists(unpackagedDir));

            Assert.True(Directory.GetFiles(unpackagedDir).Length >0);

            Assert.True(File.Exists($"{unpackagedDir}\\AppxManifest.xml"));

            Assert.True(Directory.Exists($"{unpackagedDir}\\VFS"));
            Assert.True(Directory.Exists($"{unpackagedDir}\\Assets"));

        }

        [Fact]
        public void RelativePath()
        {

            string unpackagedDir = "files";

            if (Directory.Exists(unpackagedDir))
            {
                Directory.Delete(unpackagedDir, true);
            }

            string[] args = { "unpack",
                              $"{_msixApp}",
                              unpackagedDir
                             };

            Vb6VirtualRegistry.Program.Main(args);

            Assert.True(Directory.Exists(unpackagedDir));

            Assert.True(Directory.GetFiles(unpackagedDir).Length > 0);

            Assert.True(File.Exists($"{unpackagedDir}\\AppxManifest.xml"));

            Assert.True(Directory.Exists($"{unpackagedDir}\\VFS"));
            Assert.True(Directory.Exists($"{unpackagedDir}\\Assets"));

        }
    }
}
