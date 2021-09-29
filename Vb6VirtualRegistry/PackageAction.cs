using System;
using System.Collections.Generic;
using System.Text;

namespace Vb6VirtualRegistry
{
    abstract class PackageAction : IPackageAction
    {
        public void Run(string pParameter, string pVirtualRegistryPath)
        {
            throw new NotImplementedException();
        }

        private static string ReplaceKeyEntryValues(string pValue, string pDirectory)
        {
            string msixValue = String.Empty;

            //replace
            //C:\MSIX\TAP\extracted\VFS\ProgramFilesX64\Componentes\ArrecadacaoIntegrada.dll
            //by
            //[{ProgramFilesX64}]\Componentes\ArrecadacaoIntegrada.dll

            //it is a bug that can occur (c:\ciudad\files3\VFS\SYSTEM~2\Threed32.ocx)
            pValue = pValue.Replace("SYSTEM~1", "SystemX86").Replace("SYSTEM~2", "SystemX86");
            pDirectory = pDirectory.Replace("SYSTEM~1", "SystemX86").Replace("SYSTEM~2", "SystemX86");


            var index = pValue.IndexOf($"{pDirectory}");

            msixValue = pValue.Substring(index).Replace($"{pDirectory}", $"[{{{pDirectory}}}]");

            return msixValue;

        }
    }
}
