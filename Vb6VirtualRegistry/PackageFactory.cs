
namespace Vb6VirtualRegistry
{
    public class PackageFactory
    {
        public static string Run(string pAction, string pParameter, string pVirtualRegistry, 
                                 bool appendVirtualRegistryFile, string pCertificatePassword)
        {

            string returnMessage = string.Empty;

            IPackageAction package = null;

            string virtualRegistry = pVirtualRegistry;

          
            switch (pAction)
            {
             
                case "regasm":
                    package = new Regasm();
                    returnMessage = "Regasm registry file successfuly imported.";
                    break;


                case "regsvr32":
                    
                    if(appendVirtualRegistryFile)
                        package = new Regsvr32(true);
                    else
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

                case "sign":
                    package = new Sign(pCertificatePassword);

                    returnMessage = "Packaged successfuly signed.";
                    break;

            }

            if (package != null)
                package.Run(pParameter, virtualRegistry);


            return returnMessage;
        }
    }
}
