using System;
using System.Collections.Generic;
using System.Text;

namespace Vb6VirtualRegistry
{
    public class PackageFactory
    {
        public static string Run(string pAction, string pParameter, string pVirtualRegistry)
        {

            string returnMessage = string.Empty;

            IPackageAction package = null;

            string virtualRegistry = pVirtualRegistry;

            if (string.IsNullOrEmpty(virtualRegistry))
            {
                string result = System.Reflection.Assembly.GetExecutingAssembly().Location;
                int index = result.LastIndexOf("\\");
                virtualRegistry = $"{result.Substring(0, index)}\\registry.dat";
            }

            switch (pAction)
            {
             
                case "regasm":
                    package = new Regasm();
                    returnMessage = "Regasm registry file successfuly imported.";
                    break;


                case "regsvr32":
                    package = new Regsvr32();
                    returnMessage = "Registry file successfuly created.";
                    break;

                case "pack":
                    package = new Pack();
                    returnMessage = "Application successfuly packaged.";
                    break;

                case "unpack":
                    package = new Unpack();
                    returnMessage = "Packaged successfuly extracted.";
                    break;

            }

            if (package != null)
                package.Run(pParameter, virtualRegistry);


            return returnMessage;
        }
    }
}
