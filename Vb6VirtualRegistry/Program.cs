using System;
using System.IO;
using System.Linq;

namespace Vb6VirtualRegistry
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MSIXTool - VB6 Registry Tool");
            Console.WriteLine("Virtual Registry tool utility for Visual Basic 6.0 applications");
            Console.WriteLine("https://github.com/luishdemetrio/Vb6VirtualRegistry");


            string arg3 = string.Empty;

            if (args.Length < 2)
            {
                Console.WriteLine("This project helps to automate the Visual Basic 6.0 packaging by generating a virtual registry file to be added on the VB6 MSIX package with the components (OCXs and DLLs) used by the VB6 application.");

                Console.WriteLine("\nUsage:");
                Console.WriteLine("    Vb6VirtualRegistry.exe [action] [args] [args]:");

                Console.WriteLine("\nActions:");
                Console.WriteLine("\n    pack");
                Console.WriteLine("\n    unpack");
                Console.WriteLine("\n    regsvr32");
                Console.WriteLine("\n    regasm");
                Console.WriteLine("\n    sign");


                Console.WriteLine("\nCreate virtual registry for VB6 components:");
                Console.WriteLine("    Vb6VirtualRegistry.exe regsvr32 c:\\MyApp\\unpackagedFiles c:\\MyApp\\unpackagedFiles\\registry.dat");

                Console.WriteLine("\nCreate virtual registry for .NET components:");
                Console.WriteLine("    Vb6VirtualRegistry.exe regasm c:\\MyApp\\unpackagedFiles\\VFS\\SystemX86\\CrystalReport13.dll c:\\vb6\\registry.dat");

                Console.WriteLine("\nPackage the folder");
                Console.WriteLine("    Vb6VirtualRegistry.exe pack c:\\MyApp\\unpackagedFiles c:\\MyApp\\myapp.msix");

                Console.WriteLine("\nUnpack a MSIX file");
                Console.WriteLine("    Vb6VirtualRegistry.exe unpack c:\\MyApp\\myapp.msix c:\\MyApp\\unpackagedFiles");

                Console.WriteLine("\nSign a MSIX file");
                Console.WriteLine("    Vb6VirtualRegistry.exe sign c:\\MyApp\\myapp.msix c:\\MyApp\\mycertificate.pfx");

                return;
            } else if (args.Length == 3)
            {
                arg3 = args[2];
            }
        
            PackageFactory.Run(args[0], args[1], arg3) ;
        }
    }
}
