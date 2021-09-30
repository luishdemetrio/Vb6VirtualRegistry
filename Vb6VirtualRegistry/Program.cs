using System;
using System.IO;
using System.Linq;

namespace Vb6VirtualRegistry
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("VB6 Registry Tool");
            Console.WriteLine("Virtual Registry tool utility for Visual Basic 6.0 applications");
            Console.WriteLine("https://github.com/luishdemetrio/Vb6VirtualRegistry");


            string arg3 = string.Empty;

            if (args.Length < 2)
            {
                Console.WriteLine("This project helps to automate the Visual Basic 6.0 packaging by generating a virtual registry file to be added on the VB6 MSIX package with the components (OCXs and DLLs) used by the VB6 application.");

                Console.WriteLine("\nUsage:");
                Console.WriteLine("    vb6registrytool.exe [action] [componentsPath] [destinationPath]:");

                Console.WriteLine("\nActions:");
                Console.WriteLine("\n    regsvr32");
                Console.WriteLine("\n    regasm");

                Console.WriteLine("\nSample:");
                Console.WriteLine("    vb6registrytool.exe regsvr32 c:\\VB6\\dlls c:\\vb6\\registry.dat");
                Console.WriteLine("    vb6registrytool.exe regasm c:\\VB6\\dlls\\CrystalReport.ocx c:\\vb6\\registry.dat");

                return;
            } else if (args.Length == 3)
            {
                arg3 = args[2];
            }
        
            PackageFactory.Run(args[0], args[1], arg3) ;
        }
    }
}
