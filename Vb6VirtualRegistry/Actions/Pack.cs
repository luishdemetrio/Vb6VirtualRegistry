using System;
using System.Diagnostics;
using System.IO;

namespace Vb6VirtualRegistry
{
    public sealed class Pack : IPackageAction
    {
       
        public void Run(string pUnpackagedPath, string pDestinationMsixPath)
        {

            try
            {
                pUnpackagedPath = CurrentDirectoryHelper.GetAbsolutePathFromRelative(pUnpackagedPath);

                pDestinationMsixPath = CurrentDirectoryHelper.GetAbsolutePathFromRelative(pDestinationMsixPath);

                //remove existing msix
                if (File.Exists(pDestinationMsixPath))
                {
                    File.Delete(pDestinationMsixPath);
                    Console.WriteLine($"{pDestinationMsixPath} deleted!");
                }

                if (!Directory.Exists(pUnpackagedPath))
                {
                    Console.WriteLine($"The path {pUnpackagedPath} is invalid!");
                    return;
                }

                var priconfigFile = pUnpackagedPath + "\\priconfig.xml";

                if (File.Exists(priconfigFile))
                {
                    File.Delete(priconfigFile);
                    Console.WriteLine($"{priconfigFile} deleted!");
                }

                var resourcesFile = pUnpackagedPath + "\\resources.pri";

                if (File.Exists(resourcesFile))
                {
                    File.Delete(resourcesFile);
                    Console.WriteLine($"{resourcesFile} deleted!");
                }

                CreateConfigXml(pUnpackagedPath);

                CreateResourceFile(pUnpackagedPath);

                MakeAppx(pUnpackagedPath, pDestinationMsixPath);



            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex);
            }
        }

        private static void MakeAppx(string pUnpackagedPath, string pDestinationMsixPath)
        {
            try
            {
                using var makeappx = new Process();
                
                var fileName= $"{CurrentDirectoryHelper.GetCurrentDirectory()}\\SDK\\makeappx.exe";

                var args = $"pack /d {pUnpackagedPath} /p {pDestinationMsixPath} /l";

                makeappx.StartInfo.FileName = fileName;
                makeappx.StartInfo.Arguments = args;

                makeappx.Start();

                makeappx.WaitForExit();

                Console.WriteLine(fileName + args);
                
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

       

        private static void CreateConfigXml(string pUnpackagedPath)
        {
            try
            {
                using var makepri = new Process();
                
                var makepriFile = $"{CurrentDirectoryHelper.GetCurrentDirectory()}\\SDK\\makepri.exe";

                var makepriArgs = $"createconfig /cf \"{pUnpackagedPath}\\priconfig\" /dq en-us /pv 10.0.0";

                Console.WriteLine($"{makepriFile} {makepriArgs}");

                    
                makepri.StartInfo.FileName = makepriFile;
                makepri.StartInfo.Arguments = makepriArgs;
                    

                makepri.Start();

                makepri.WaitForExit();

                
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

        private static void CreateResourceFile(string pUnpackagedPath)
        {
            try
            {
                using var makepri = new Process();
                
                var makepriFile = $"{CurrentDirectoryHelper.GetCurrentDirectory()}\\SDK\\makepri.exe";

                var makepriArgs = $"new /pr '{pUnpackagedPath}' /ConfigXml '{pUnpackagedPath}\\priconfig.xml' /OutputFile '{pUnpackagedPath}\\resources.pri' /Verbose /Overwrite";


                Console.WriteLine(makepriFile + makepriArgs);
                makepri.StartInfo.FileName = makepriFile;
                makepri.StartInfo.Arguments = @"new /pr """ + pUnpackagedPath + @""" /cf """ + pUnpackagedPath + @"\priconfig.xml"" /of """ + pUnpackagedPath + @"\resources.pri"" /v /o"  ;

                makepri.Start();

                makepri.WaitForExit();

                    
                
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }
    }
}
