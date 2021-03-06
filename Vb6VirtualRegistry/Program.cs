using System;

namespace Vb6VirtualRegistry
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("MSIXTool - VB6 Registry Tool");
            Console.WriteLine("Virtual Registry tool utility for Visual Basic 6.0 applications");
            Console.WriteLine("https://github.com/luishdemetrio/Vb6VirtualRegistry");

            bool appendVirtualRegistry= false;
            string certificatePassword = string.Empty;

            if (args.Length < 3)
            {
                Console.WriteLine("This project helps to automate the Visual Basic 6.0 packaging by generating a virtual registry file to be added on the VB6 MSIX package with the components (OCXs and DLLs) used by the VB6 application.");

                Console.WriteLine("\nUsage:");
                Console.WriteLine("    Vb6VirtualRegistry.exe [action] [args] [args]:");

                Console.WriteLine("\nActions:");
                Console.WriteLine("    pack");
                Console.WriteLine("    unpack");
                Console.WriteLine("    regsvr32");
                Console.WriteLine("    regasm");
                Console.WriteLine("    sign");


                Console.WriteLine("\nCreate virtual registry for VB6 components:");
                Console.WriteLine("    Vb6VirtualRegistry.exe regsvr32 c:\\MyApp\\unpackagedFiles c:\\MyApp\\unpackagedFiles\\registry.dat");
                Console.WriteLine("    Vb6VirtualRegistry.exe regsvr32 c:\\MyApp\\unpackagedFiles c:\\MyApp\\unpackagedFiles\\registry.dat --append");

                Console.WriteLine("\nCreate virtual registry for .NET components:");
                Console.WriteLine("    Vb6VirtualRegistry.exe regasm c:\\MyApp\\unpackagedFiles\\VFS\\SystemX86\\CrystalReport13.dll c:\\vb6\\registry.dat");

                Console.WriteLine("\nPackage the folder");
                Console.WriteLine("    Vb6VirtualRegistry.exe pack c:\\MyApp\\unpackagedFiles c:\\MyApp\\myapp.msix");

                Console.WriteLine("\nUnpack a MSIX file");
                Console.WriteLine("    Vb6VirtualRegistry.exe unpack c:\\MyApp\\myapp.msix c:\\MyApp\\unpackagedFiles");

                Console.WriteLine("\nSign a MSIX file");
                Console.WriteLine("    Vb6VirtualRegistry.exe sign c:\\MyApp\\myapp.msix c:\\MyApp\\mycertificate.pfx");

                return;
            }
            
            if(args.Length == 4)
            {
                if (args[0] == "regsvr32")
                    appendVirtualRegistry = true;
                else
                    certificatePassword = args[3];
            }
            
            PackageFactory.Run(args[0], args[1], args[2], appendVirtualRegistry, certificatePassword) ;
        }
    }
}
