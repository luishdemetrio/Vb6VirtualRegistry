using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Vb6VirtualRegistry
{
    public sealed class Sign : IPackageAction
    {
        public void Run(string pMSIXPackage, string pCertificatePath)
        {
            try
            {

               

                SignPackage(pMSIXPackage, pCertificatePath);



            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        private static string GetCertificatePassword()
        {
            StringBuilder sb = new StringBuilder();
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

                var certificatePassword = GetCertificatePassword();

                if (string.IsNullOrEmpty(certificatePassword))
                {
                    Console.WriteLine("Invalid certificate password!");
                    return;

                }

                using (var makeappx = new Process())
                {
                    var fileName = $"{GetCurrentDirectory()}\\SDK\\signtool.exe";

                    var args = $"sign /a /debug /v /fd SHA256 /td SHA256 /f \"{pCertificatePath}\" /p \"{certificatePassword}\" {pMSIXPackage} ";

                    makeappx.StartInfo.FileName = fileName;
                    makeappx.StartInfo.Arguments = args;

                    makeappx.Start();

                    makeappx.WaitForExit();

                    Console.WriteLine(fileName + args);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        private string GetCurrentDirectory()
        {
            string result = System.Reflection.Assembly.GetExecutingAssembly().Location;
            int index = result.LastIndexOf("\\");
            return $"{result.Substring(0, index)}";

        }
    }
}
