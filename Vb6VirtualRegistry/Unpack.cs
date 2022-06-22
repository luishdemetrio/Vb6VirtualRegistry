using System;
using System.Diagnostics;
using System.IO;

namespace Vb6VirtualRegistry
{
    public sealed class Unpack : IPackageAction
    {
        public void Run(string pMSIXPackage, string pUnpackagedPath)
        {
            try
            {


                if (Directory.Exists(pUnpackagedPath))
                {
                    Console.WriteLine($"{pUnpackagedPath} destination folder already exist. Deleting the same before proceeding...");
                    Directory.Delete(pUnpackagedPath, true);

                }
                    

                UnPackMsix(pMSIXPackage, pUnpackagedPath);


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

        private static void UnPackMsix(string pMSIXPackage, string pUnpackagedPath)
        {
            try
            {
                using var makeappx = new Process();
                
                var fileName = $"{CurrentDirectoryHelper.GetCurrentDirectory()}\\SDK\\makeappx.exe";

                var args = $" unpack /v /p {pMSIXPackage} /d {pUnpackagedPath} /l";

                makeappx.StartInfo.FileName = fileName;
                makeappx.StartInfo.Arguments = args;

                makeappx.Start();

                makeappx.WaitForExit();

                Console.WriteLine($"{fileName} {args}");
                
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

       
    }

}
