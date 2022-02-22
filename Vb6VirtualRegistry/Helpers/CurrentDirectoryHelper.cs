using System;
using System.Diagnostics;
using System.IO;

namespace Vb6VirtualRegistry
{
    public static class CurrentDirectoryHelper
    {

        public static string GetCurrentDirectory()
        {

            // System.Reflection.Assembly.GetExecutingAssembly().Location;
            //https://github.com/dotnet/runtime/issues/13051#issuecomment-510267727

            string result = Process.GetCurrentProcess().MainModule.FileName; // Environment.ProcessPath;//System.Reflection.Assembly.GetExecutingAssembly().Location;

            int index = result.LastIndexOf("\\");

            return $"{result.Substring(0, index)}";

        }

        public static string GetAbsolutePathFromRelative(string pRelativePath)
        {
            string relativePath = pRelativePath;

            if (!Path.IsPathFullyQualified(pRelativePath))
            {
                relativePath= Path.Join(Environment.CurrentDirectory, pRelativePath);
            }

            return relativePath;    
        }

      
    }
}
