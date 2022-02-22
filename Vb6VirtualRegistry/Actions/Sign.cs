using System;
using System.Diagnostics;
using System.Text;

namespace Vb6VirtualRegistry
{
    public sealed class Sign : IPackageAction
    {
        private string _certificatePassword = string.Empty;

        
        public Sign(string pCertificatePassword)
        {
            _certificatePassword = pCertificatePassword;
        }

        public void Run(string pMSIXPackage, string pCertificatePath)
        {
            try
            {

                pMSIXPackage = CurrentDirectoryHelper.GetAbsolutePathFromRelative(pMSIXPackage);

                pCertificatePath = CurrentDirectoryHelper.GetAbsolutePathFromRelative(pCertificatePath);

                SignPackage(pMSIXPackage, pCertificatePath);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string GetCertificatePassword()
        {
            StringBuilder sb = new ();
            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (cki.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        Console.Write("\b\0\b");
                        sb.Length--;
                    }

                    continue;
                }

                Console.Write('*');
                sb.Append(cki.KeyChar);
            }

            return sb.ToString();
        }

        private void SignPackage(string pMSIXPackage, string pCertificatePath)
        {

            
            try
            {
                Console.WriteLine("Enter the certificate password: ");

                if (String.IsNullOrEmpty(_certificatePassword))
                    _certificatePassword = GetCertificatePassword();

                if (string.IsNullOrEmpty(_certificatePassword))
                {
                    Console.WriteLine("Invalid certificate password!");
                    return;

                }

                using var makeappx = new Process();
                
                var fileName = $"{CurrentDirectoryHelper.GetCurrentDirectory()}\\SDK\\signtool.exe";

                var args = $"sign /a /debug /v /fd SHA256 /td SHA256 /f \"{pCertificatePath}\" /p \"{_certificatePassword}\" {pMSIXPackage} ";

                makeappx.StartInfo.FileName = fileName;
                makeappx.StartInfo.Arguments = args;

                makeappx.Start();

                makeappx.WaitForExit();

                Console.WriteLine(fileName + args);
                
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message.Replace(_certificatePassword, "Secret"));
            }
        }

     
    }
}
