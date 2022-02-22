using OffregLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Vb6VirtualRegistry
{
    public sealed class Regasm : IPackageAction
    {
        string _virtualRegistryPath;

        string _temporaryVirtualRegistryKeyName = $"MSIXTool_{Guid.NewGuid()}";

        List<string> _regasmComponents = new ();

        const string _regasmPath = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319";

        public void Run(string pParameter, string pVirtualRegistryPath)

        {
            pParameter = CurrentDirectoryHelper.GetAbsolutePathFromRelative(pParameter);

            _virtualRegistryPath = CurrentDirectoryHelper.GetAbsolutePathFromRelative(pVirtualRegistryPath);

            
            LoadComponents(pParameter);

            if (_regasmComponents == null || _regasmComponents.Count ==0)
            {
                Console.WriteLine("Please inform at least one component to be registered through regasm");
                return;
            }

            if (File.Exists(_virtualRegistryPath) == false)
                CreateVirtualRegistryFile(_virtualRegistryPath);

            LoadVirtualHiveToRealRegistry();

            ExportRegasmKeys();

            UnloadVirtualHiveToRealRegistry();

        }

        private void LoadComponents(string pComponents)
        {
            foreach (var item in pComponents.Split(","))
            {
                _regasmComponents.Add(item);
            }
        }

        private void CreateVirtualRegistryFile(string pPackagedRegistry)
        {
            using var virtualHive = OffregHive.Create();
            using var virtualRegistryEntry = virtualHive.Root.CreateSubKey("REGISTRY");
            using var virtualMachineEntry = virtualRegistryEntry.CreateSubKey("MACHINE");
            using var virtualSoftwareEntry = virtualMachineEntry.CreateSubKey("SOFTWARE");
            using var virtualClassesEntry = virtualSoftwareEntry.CreateSubKey("Classes");
            
            virtualHive.SaveHive(pPackagedRegistry, 5, 1);
            Console.WriteLine($"Saving file {pPackagedRegistry}");
            
        }

        private void LoadVirtualHiveToRealRegistry()
        {
            using var p = new Process();
            
            p.StartInfo.FileName = "REG.exe";
            p.StartInfo.Arguments = $"LOAD HKLM\\{_temporaryVirtualRegistryKeyName} {_virtualRegistryPath}";

            Console.WriteLine($"{p.StartInfo.FileName} {p.StartInfo.Arguments}");

            p.Start();
            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                Console.WriteLine($"LoadVirtualHiveToRealRegistry: ExitCode {p.ExitCode}");
                Console.WriteLine("Probably you need run this App elevated to generate the keys files!");
                return;
            }
            

            Console.WriteLine($"Virtual registry {_virtualRegistryPath} successfuly loaded.");
            
        }

        private void UnloadVirtualHiveToRealRegistry()
        {
            using (var p = new Process())
            {
                p.StartInfo.FileName = "reg.exe";
                p.StartInfo.Arguments = $"unload HKLM\\{_temporaryVirtualRegistryKeyName}";
                p.Start();
                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    Console.WriteLine("UnloadVirtualHiveToRealRegistry: ExitCode == 1");
                    
                    Console.WriteLine($"Could not unload the {_temporaryVirtualRegistryKeyName} hive.");
                    return;
                }

            }

            Console.WriteLine($"Virtual registry {_virtualRegistryPath} successfuly loaded.");
            //reg.exe load HKLM\MSIX C:\\MSIX\\Registry.dat
        }

        private void ExportRegasmKeys()
        {
            var regasmExitCode = 0;

            foreach (var item in _regasmComponents)
            {
                RunRegasm(item);

                regasmExitCode = GenerateRegFile(item);

                var regFile = item.Replace(".dll", ".reg");

                if (regasmExitCode == 0 && File.Exists(regFile))
                {
                    var clsIds = GetClsIdFromRegFile(regFile);

                    foreach (var clsId in clsIds)
                    {
                        CreateVirtualRegistryEntries(item, clsId, "x86",
                                                    "HKEY_CLASSES_ROOT\\CLSID",
                                                    $"HKEY_LOCAL_MACHINE\\{_temporaryVirtualRegistryKeyName}\\REGISTRY\\MACHINE\\SOFTWARE\\Classes\\CLSID");

                        CreateVirtualRegistryEntries(item, clsId, "x64",
                                                     "HKEY_CLASSES_ROOT\\CLSID",
                                                     $"HKEY_LOCAL_MACHINE\\{_temporaryVirtualRegistryKeyName}\\REGISTRY\\MACHINE\\SOFTWARE\\Classes\\WOW6432Node\\CLSID");

                    }

                    ImportRegistryFile(regFile,
                                       "HKEY_CLASSES_ROOT",
                                       $"HKEY_LOCAL_MACHINE\\{_temporaryVirtualRegistryKeyName}\\REGISTRY\\MACHINE\\SOFTWARE\\Classes");
                }

            }


           



        }

        private void RunRegasm(string item)
        {


            using var p = new Process();

            p.StartInfo.FileName = $"{_regasmPath}\\regasm.exe";
            p.StartInfo.Arguments = $"{item} /codebase";

            p.Start();
            p.WaitForExit();
            Console.WriteLine($"Exporting regasm keys for: {item}");


            if (p.ExitCode != 0)
            {
                Console.WriteLine($"RunRegasm: ExitCode {p.ExitCode}");

                Console.WriteLine("Probably you need run this App elevated!");
                return;
            }

            
        }

        private int GenerateRegFile(string item)
        {
            int regasmExitCode;
            using (var p = new Process())
            {
                p.StartInfo.FileName = $"{_regasmPath}\\regasm.exe";
                p.StartInfo.Arguments = $" /regfile {item}";

                p.Start();
                p.WaitForExit();
                Console.WriteLine($"Exporting regasm keys for: {item}");

                
                regasmExitCode = p.ExitCode;
            }

            return regasmExitCode;
        }

        private static List<string> GetClsIdFromRegFile(string regFile)
        {
            var list = new List<string>();

            foreach (var line in File.ReadAllLines(regFile))
            {
                if (line.Contains("[HKEY_CLASSES_ROOT\\CLSID"))
                {
                    var index = line.IndexOf(@"[HKEY_CLASSES_ROOT\CLSID\");

                    var clsId = line.Substring(25, 38);

                    if (list.Contains(clsId) == false)
                        list.Add(clsId);

                }

            }

            return list;
        }

        private static void CreateVirtualRegistryEntries(string item, string clsId, string pProcessorArchitecture,
                                                 string pPhysicalHive, string pVirtualHive)
        {
            string fullClsidPath = ExportClsidFromRegistry(item, clsId, pProcessorArchitecture);

            List<string> codeBases = GetCodebasesFromRegFile(fullClsidPath);

            ReplaceCodeBaseValueByVirtualVFS(item, fullClsidPath, codeBases);

            ImportRegistryFile(fullClsidPath, pPhysicalHive, pVirtualHive);
        }

        private static string ExportClsidFromRegistry(string item, string clsId, string pProcessorArchitecture)
        {
            var fullClsidPath = $"{item.Substring(0, item.LastIndexOf("\\"))}\\{clsId}_{pProcessorArchitecture}.reg";

            using (var reg = new Process())
            {
                reg.StartInfo.FileName = "reg.exe";
                reg.StartInfo.Arguments = $"export HKEY_CLASSES_ROOT\\CLSID\\{clsId} {fullClsidPath}";
                reg.Start();
                reg.WaitForExit();

                if (reg.ExitCode != 0)
                {
                    Console.WriteLine($"ExportClsidFromRegistry: ExitCode {reg.ExitCode}");

                    Console.WriteLine("Probably you need run this App elevated!");
                    
                }
            }

            return fullClsidPath;
        }

        private static List<string> GetCodebasesFromRegFile(string fullClsidPath)
        {
            var codeBases = new List<string>();
            foreach (var line in File.ReadAllLines(fullClsidPath))
            {
                if (line.Contains("CodeBase"))
                {
                    if (codeBases.Contains(line) == false)
                        codeBases.Add(line);
                }
            }

            return codeBases;
        }

        private static void ReplaceCodeBaseValueByVirtualVFS(string item, string fullClsidPath, List<string> codeBases)
        {
            var fileRegContent = File.ReadAllText(fullClsidPath);

            foreach (var codeBase in codeBases)
            {
                fileRegContent = fileRegContent.Replace(codeBase,
                                     $"\"CodeBase\"=\"{ReplacePhysicalPathForVirtualOne(item)}\"");
            }

            File.WriteAllText(fullClsidPath, fileRegContent);
        }

        private static string ReplacePhysicalPathForVirtualOne(string pValue)
        {

            List<String> directoryValues = new (){  "AppVPackageDrive",
                                                                "Common Desktop",
                                                                "Common Programs",
                                                                "Local AppData",
                                                                "LocalAppDataLow",
                                                                "ProgramFilesX86",
                                                                "ProgramFilesX64",
                                                                "System",
                                                                "SystemX86",
                                                                "SystemX64",
                                                                "SYSTEM~1",
                                                                "SYSTEM~2",
                                                                "Windows",
                                                                "Fonts"
                                                             };

            var resultList = directoryValues.FindLast(s => pValue.IndexOf(s) > 0);

            if (!string.IsNullOrEmpty(resultList))
            {
                pValue = ReplaceKeyEntryValues(pValue, resultList);
            }

            return pValue;

        }
              

        private static string ReplaceKeyEntryValues(string pValue, string pDirectory)
        {
            string msixValue;

            //replace
            //C:\MSIX\TAP\extracted\VFS\ProgramFilesX64\Componentes\Integrada.dll
            //by
            //[{ProgramFilesX64}]\Componentes\Integrada.dll

            //it is a bug that can occur (c:\ciudad\files3\VFS\SYSTEM~2\Threed32.ocx)
            pValue = pValue.Replace("SYSTEM~1", "SystemX86").Replace("SYSTEM~2", "SystemX86");
            pDirectory = pDirectory.Replace("SYSTEM~1", "SystemX86").Replace("SYSTEM~2", "SystemX86");


            var index = pValue.IndexOf($"{pDirectory}");

            msixValue = pValue.Substring(index).Replace($"{pDirectory}", $"[{{{pDirectory}}}]");
            Console.WriteLine($"Replaced {pValue} by {msixValue}");
            
            return msixValue.Replace("\\", "\\\\"); ;

        }

     

        private static void ImportRegistryFile(string pRegFile, string pPhysicalHive, string pVirtualHive)
        {
            string value = File.ReadAllText(pRegFile).Replace(pPhysicalHive, pVirtualHive);

            File.WriteAllText(pRegFile, value);

            using (var p = new Process())
            {
                p.StartInfo.FileName = "reg.exe";
                p.StartInfo.Arguments = $"import {pRegFile}";
                p.Start();
                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    Console.WriteLine($"ImportRegistryFile: ExitCode {p.ExitCode}");

                    Console.WriteLine("Probably you need run this App elevated!");
                    return;
                }
            }

            File.Delete(pRegFile);

        }

    }
}
