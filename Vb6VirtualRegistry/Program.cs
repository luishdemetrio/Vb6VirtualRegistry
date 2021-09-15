using System;
using System.IO;
using System.Linq;

namespace Vb6VirtualRegistry
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {

                Console.WriteLine("Example: vb6virtualregistry c:\\msix\\files  c:\\temp\\registry.dat");
                return;

            }

           var files = Directory.GetFiles(args[0] , 
                                          "*.*", SearchOption.AllDirectories)
                                 .Where(p => p.EndsWith("ocx") || p.EndsWith("dll")).ToList();

          


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
