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
            
            

            if (args.Length == 0)
            {
                Console.WriteLine("This project helps to automate the Visual Basic 6.0 packaging by generating a virtual registry file to be added on the VB6 MSIX package with the components (OCXs and DLLs) used by the VB6 application.");

                Console.WriteLine("\nUsage:");

                Console.WriteLine("    vb6registrytool.exe [componentsPath] [destinationPath]:");

                Console.WriteLine("\nSample:");
                Console.WriteLine("    vb6registrytool.exe c:\\VB6\\dlls c:\\vb6\\registry.dat");

                return;
            }
            Console.WriteLine("\n");

            var files = Directory.GetFiles(args[0] , 
                                          "*.*", SearchOption.AllDirectories)
                                 .Where(p => p.ToLower().EndsWith("ocx") || p.ToLower().EndsWith("dll")).ToList();

          


            var registry = new Regsvr32();

            string virtualRegistryPath = string.Empty;

            if (args.Length > 1)
            {
                virtualRegistryPath = args[1];      
            }

            if (virtualRegistryPath == string.Empty)
            {
                string result = System.Reflection.Assembly.GetExecutingAssembly().Location; 
                int index = result.LastIndexOf("\\");
                virtualRegistryPath = $"{result.Substring(0, index)}\\registry.dat";
            }

            if (registry.Run(files, virtualRegistryPath))
                Console.WriteLine("Virtual Registry successfully created!");


        }
    }
}
