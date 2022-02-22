using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace VB6VirtualRegistry.Test.Tests
{

    
    public class Sign
    {
        
        public void FullPath()
        {
            string signDir = "C:\\MSIX\\SignTest";
            string msixApp = "unsignedmyvb6app.msix";
            string testCert = "Vb6VirtualRegistry.pfx";

            if (Directory.Exists(signDir))
            {
                Directory.Delete(signDir, true);
            }

            Directory.CreateDirectory(signDir);

            using (var p = new Process())
            {
                p.StartInfo.FileName = "xcopy";
                p.StartInfo.Arguments = $"/E /I {msixApp} {signDir}";
                p.Start();
                p.WaitForExit();
            }

            using (var p = new Process())
            {
                p.StartInfo.FileName = "xcopy";
                p.StartInfo.Arguments = $"/E /I {testCert} {signDir}";
                p.Start();
                p.WaitForExit();
            }

            string[] args = {  "sign",
                              $"{signDir}\\{msixApp}",
                              $"{signDir}\\{testCert}",
                              @"AppConsult@PleaseChangeit!"
                             };

            Vb6VirtualRegistry.Program.Main(args);

            string output = String.Empty;

            using (var p = new Process())
            {
                p.StartInfo.FileName = "signtool.exe";
                p.StartInfo.Arguments = $"verify /v {signDir}\\{msixApp}";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.EnableRaisingEvents = true;
                p.Start();
                p.WaitForExit();

                output = p.StandardOutput.ReadToEnd();
            }

            Assert.Contains("Signing Certificate Chain:", output);


        }

       
        public void RelativePath()
        {
            string msixApp = "unsignedmyvb6app.msix";
            string testCert = "Vb6VirtualRegistry.pfx";
                      
            string[] args = {  "sign",
                              $"{msixApp}",
                              $"{testCert}",
                              @"AppConsult@PleaseChangeit!"
                             };

            Vb6VirtualRegistry.Program.Main(args);

            string output = String.Empty;

            using (var p = new Process())
            {
                p.StartInfo.FileName = "signtool.exe";
                p.StartInfo.Arguments = $"verify /v {msixApp}";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.EnableRaisingEvents = true;
                p.Start();
                p.WaitForExit();

                output = p.StandardOutput.ReadToEnd();
            }

            Assert.Contains("Signing Certificate Chain:", output);

        }
    }
}
