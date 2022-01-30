using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Vb6VirtualRegistry
{
    public sealed class Unpack : IPackageAction
    {
        public void Run(string pMSIXPackage, string pUnpackagedPath)
        {
            try
            {

                UnPackMsix(pMSIXPackage, pUnpackagedPath);



            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

        private void UnPackMsix(string pMSIXPackage, string pUnpackagedPath)
        {
            try
            {
                using (var makeappx = new Process())
                {
                    var fileName = $"{GetCurrentDirectory()}\\SDK\\makeappx.exe";

                    var args = $"unpack /v /p {pMSIXPackage} /d {pUnpackagedPath} /l";

                    makeappx.StartInfo.FileName = fileName;
                    makeappx.StartInfo.Arguments = args;

                    makeappx.Start();

                    makeappx.WaitForExit();

                    Console.WriteLine(fileName + args);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        private string GetCurrentDirectory()
        {
            // System.Reflection.Assembly.GetExecutingAssembly().Location;
            //https://github.com/dotnet/runtime/issues/13051#issuecomment-510267727

            string result = Process.GetCurrentProcess().MainModule.FileName; 
            int index = result.LastIndexOf("\\");
            return $"{result.Substring(0, index)}";

        }
    }

}
