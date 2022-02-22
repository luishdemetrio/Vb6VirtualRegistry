using Xunit;
using System.IO;
using System.Diagnostics;

namespace VB6VirtualRegistry.Test
{
    public class Pack
    {
        [Fact]
        public void FullPath() 
        { 

            var testDirectory = "C:\\MSIX\\PackTest";
            var unpackagedDir = "unpackaged";
            var resultMSIX = $"{testDirectory}\\myvb6app.msix";

            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
            
            Directory.CreateDirectory(testDirectory);

            using (var p = new Process())
            {
                p.StartInfo.FileName = "xcopy";
                p.StartInfo.Arguments = $"/E /I {unpackagedDir} {testDirectory}\\{unpackagedDir}";
                p.Start();
                p.WaitForExit();
            }

            string[] args = {"pack", $"{testDirectory}\\{unpackagedDir}", resultMSIX };

            Vb6VirtualRegistry.Program.Main(args);

            Assert.True(File.Exists(resultMSIX));


        }

        [Fact]
        public void RelativePath()
        {

            string[] args = { "pack", "unpackaged", "result.msix" };

            Vb6VirtualRegistry.Program.Main(args);

            Assert.True(File.Exists("result.msix"));


        }
    }
}