using OffregLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Runtime.InteropServices.ComTypes;
using System.IO;
using System.Linq;

namespace Vb6VirtualRegistry
{
    public sealed class Regsvr32 : IPackageAction
    {

        [DllImport("oleaut32.dll", PreserveSig = false)]
        public static extern ITypeLib LoadTypeLib([In, MarshalAs(UnmanagedType.LPWStr)] string typelib);


        private List<string> _components;

        private bool _appendFile;

        public Regsvr32()
        {
            _appendFile = false;
        }

        public Regsvr32(bool pAppendFile)
        {
            _appendFile = pAppendFile;  
        }



        public void Run(string pParameter, string pVirtualRegistryPath)
        {

            pParameter = CurrentDirectoryHelper.GetAbsolutePathFromRelative(pParameter);

            pVirtualRegistryPath = CurrentDirectoryHelper.GetAbsolutePathFromRelative(pVirtualRegistryPath);

            _components = GetComponents(pParameter);

            RegistryComponentsToRealRegistry();

            ParseTypeLib(pVirtualRegistryPath);

            
        }

       private static List<string> GetComponents(string pPath)
        {


            return Directory.GetFiles(pPath,
                                          "*.*", SearchOption.AllDirectories)
                                 .Where(p => p.ToLower().EndsWith("ocx") || p.ToLower().EndsWith("dll")).ToList();
        }
        

        private void RegistryComponentsToRealRegistry()
        {
            foreach (string item in _components.AsParallel())
            {
                try
                {
                    using var process = new Process();
                    
                    process.StartInfo.FileName = "regsvr32.exe";
                    process.StartInfo.Arguments = $"/s {item}";
                    process.Start();
                    Console.WriteLine($"{process.StartInfo.FileName} {process.StartInfo.Arguments}");
                    process.WaitForExit();
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"RegistryComponentsToRealRegistry {ex}");
                }

            }
        }


        public void ParseTypeLib(string pVirtualRegistryPath)
        {

            Console.WriteLine("Generating the Registry virtual keys");

            OffregHive virtualHive;
            OffregKey virtualRegistryEntry;
            OffregKey virtualMachineEntry;
            OffregKey virtualSoftwareEntry;
            OffregKey virtualClassesEntry = null;

            if (_appendFile)
            {

                try
                {
                    //I am assuming that at least the structure REGISTRY\MACHINE\SOFTWARE\ exists
                    //once it is pre-req for the msix virtual registry

                    using (virtualHive = OffregHive.Open(pVirtualRegistryPath))
                    using (virtualRegistryEntry = virtualHive.Root.OpenSubKey("REGISTRY"))
                    using (virtualMachineEntry = virtualRegistryEntry.OpenSubKey("MACHINE"))
                    using (virtualSoftwareEntry = virtualMachineEntry.OpenSubKey("SOFTWARE"))
                    {
                        
                        if (virtualSoftwareEntry.SubkeyExist("Classes"))
                        {
                            virtualClassesEntry = virtualSoftwareEntry.OpenSubKey("Classes");
                        }
                        else
                        {
                            virtualClassesEntry = virtualSoftwareEntry.CreateSubKey("Classes");                            
                        }

                        GenerateVirtualRegistryClsId(pVirtualRegistryPath, virtualHive, virtualClassesEntry);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Invalid virtual registry: {pVirtualRegistryPath}");
                    Console.WriteLine(ex);
                }           
                
            }
            else
            {
                //The first step is to create the hive REGISTRY\MACHINE\SOFTWARE\Classes that is common to all scenarios
                using (virtualHive = OffregHive.Create())
                using (virtualRegistryEntry = virtualHive.Root.CreateSubKey("REGISTRY"))
                using (virtualMachineEntry = virtualRegistryEntry.CreateSubKey("MACHINE"))
                using (virtualSoftwareEntry = virtualMachineEntry.CreateSubKey("SOFTWARE"))
                using (virtualClassesEntry = virtualSoftwareEntry.CreateSubKey("Classes"))
                {
                    GenerateVirtualRegistryClsId(pVirtualRegistryPath, virtualHive, virtualClassesEntry);
                }
            }

        }

        private void GenerateVirtualRegistryClsId(string pVirtualRegistryPath, OffregHive virtualHive, OffregKey virtualClassesEntry)
        {
            foreach (var file in _components)
            {
                ITypeLib typeLib;

                try
                {
                    typeLib = LoadTypeLib(file);
                    Console.WriteLine($"Extracting keys for {file}");
                }
                catch
                {
                    continue;
                }

                int typeInfoCount = typeLib.GetTypeInfoCount();
                IntPtr ipLibAtt = IntPtr.Zero;
                typeLib.GetLibAttr(out ipLibAtt);

                var typeLibAttr = (System.Runtime.InteropServices.ComTypes.TYPELIBATTR)
                    Marshal.PtrToStructure(ipLibAtt, typeof(System.Runtime.InteropServices.ComTypes.TYPELIBATTR));
                Guid tlbId = typeLibAttr.guid;


                for (int i = 0; i < typeInfoCount; i++)
                {
                    typeLib.GetTypeInfo(i, out ITypeInfo typeInfo);

                    //figure out what guids, typekind, and names of the thing we're dealing with
                    IntPtr ipTypeAttr = IntPtr.Zero;
                    try
                    {

                        typeInfo.GetTypeAttr(out ipTypeAttr);

                        //unmarshal the pointer into a structure into something we can read
                        var typeattr = (System.Runtime.InteropServices.ComTypes.TYPEATTR)
                            Marshal.PtrToStructure(ipTypeAttr, typeof(System.Runtime.InteropServices.ComTypes.TYPEATTR));

                        System.Runtime.InteropServices.ComTypes.TYPEKIND typeKind = typeattr.typekind;
                        Guid typeId = typeattr.guid;

                        //get the name of the type
                        typeLib.GetDocumentation(i, out string strName, out string strDocString, out int dwHelpContext, out string strHelpFile);



                        using (var typeLibEntry = virtualClassesEntry.CreateSubKey("TypeLib"))
                        {
                            //read from:
                            //HKEY_CLASSES_ROOT\TypeLib\{DFF71238-4555-42EA-AC07-679B142559B5}

#pragma warning disable CA1416 // Validate platform compatibility
                            RegistryKey localTypeLibKey = Registry.ClassesRoot.OpenSubKey($"TypeLib\\{tlbId.ToString("B").ToUpper()}");
#pragma warning restore CA1416 // Validate platform compatibility

                            if (localTypeLibKey != null)
                            {
                                //save at:
                                //\REGISTRY\MACHINE\SOFTWARE\Classes\TypeLib\{DFF71238-4555-42EA-AC07-679B142559B5}
                                Console.WriteLine($"TypeLib\\{tlbId.ToString("B").ToUpper()}");
                                
                                var typeLibEntrySubKey = typeLibEntry.CreateSubKey(tlbId.ToString("B").ToUpper());
                                
                                ExportKey(localTypeLibKey, typeLibEntrySubKey);
                                

                            }
                        }

                        if (typeKind == System.Runtime.InteropServices.ComTypes.TYPEKIND.TKIND_COCLASS)
                        {
                            //read from:
                            //HKEY_CLASSES_ROOT\WOW6432Node\CLSID\{E29387D7-20AD-4F47-B17F-AEEB2B1D1D2F}

                            using var virtualWOW6432CLSIDKey = virtualClassesEntry.CreateSubKey("WOW6432Node");
                            using var virtualCLSID = virtualWOW6432CLSIDKey.CreateSubKey("CLSID");
                            
                            var localWOW6432CLSIDKey = Registry.ClassesRoot.OpenSubKey($"WOW6432Node\\CLSID\\{typeId.ToString("B").ToUpper()}");

                            if (localWOW6432CLSIDKey != null)
                            {
                                //save at:
                                //REGISTRY\MACHINE\SOFTWARE\Classes\WOW6432Node\CLSID\{E29387D7-20AD-4F47-B17F-AEEB2B1D1D2F}
                                Console.WriteLine($"WOW6432Node\\CLSID\\{typeId.ToString("B").ToUpper()}");
                                using (var virtualCLSIDEntrySubKey = virtualCLSID.CreateSubKey(typeId.ToString("B").ToUpper()))
                                {
                                    ExportKey(localWOW6432CLSIDKey, virtualCLSIDEntrySubKey);
                                }

                                //INSIDE DEFAULT KEY
                                //PRODID: HKEY_CLASSES_ROOT\ARRECINTEGRA.ClsT_ANU_USO_INDE

                                var localProgID = Registry.ClassesRoot.OpenSubKey($"WOW6432Node\\CLSID\\{typeId.ToString("B").ToUpper()}\\ProgID");

                                if (localProgID != null)
                                {
                                    //saves at:
                                    //REGISTRY\MACHINE\SOFTWARE\Classes\ARRECINTEGRA.ClsT_ANU_USO_INDE

                                    var ProgIDValue = localProgID.GetValue("").ToString();

                                    if (ProgIDValue != null)
                                    {
                                        using var virtualProgID = virtualClassesEntry.CreateSubKey(ProgIDValue);
                                        
                                        var localProgIDKey = Registry.ClassesRoot.OpenSubKey(ProgIDValue);

                                        if (localProgIDKey != null)
                                        {

                                            ExportKey(localProgIDKey, virtualProgID);

                                        }
                                        
                                    }

                                }

                            }
                        }

                        else if (typeKind == System.Runtime.InteropServices.ComTypes.TYPEKIND.TKIND_INTERFACE)
                        {

                        }

                        else if (typeKind == System.Runtime.InteropServices.ComTypes.TYPEKIND.TKIND_DISPATCH)
                        {

                            //HKCR\WOW6432Node\Interface\{26995163-87A0-4667-82B1-D304274EEE98}
                            //HKCR\Interface\{26995163-87A0-4667-82B1-D304274EEE98}

                            if (!virtualClassesEntry.SubkeyExist("WOW6432Node"))
                                virtualClassesEntry.CreateSubKey("WOW6432Node");
                                
                            
                            using var virtualWOW6432CLSIDKey = virtualClassesEntry.OpenSubKey("WOW6432Node");
                            using var virtualInterface = virtualWOW6432CLSIDKey.CreateSubKey("Interface");
                            
                            var localInterfaceKey = Registry.ClassesRoot.OpenSubKey($"WOW6432Node\\Interface\\{typeId.ToString("B").ToUpper()}");

                            if (localInterfaceKey != null)
                            {
                                //save at:
                                //REGISTRY\MACHINE\SOFTWARE\Classes\WOW6432Node\Interface\{E29387D7-20AD-4F47-B17F-AEEB2B1D1D2F}
                                Console.WriteLine($"WOW6432Node\\Interface\\{typeId.ToString("B").ToUpper()}");

                                using var virtualCLSIDEntrySubKey = virtualInterface.CreateSubKey(typeId.ToString("B").ToUpper());
                                
                                ExportKey(localInterfaceKey, virtualCLSIDEntrySubKey);


                                //REGISTRY\MACHINE\SOFTWARE\Classes\Interface\{E29387D7-20AD-4F47-B17F-AEEB2B1D1D2F}
                                using var virtualInterfaceRoot = virtualClassesEntry.CreateSubKey("Interface");
                                
                                var localInterfaceRootKey = Registry.ClassesRoot.OpenSubKey($"Interface\\{typeId.ToString("B").ToUpper()}");

                                if (localInterfaceRootKey != null)
                                {
                                    //save at:
                                    //REGISTRY\MACHINE\SOFTWARE\Classes\Interface\{E29387D7-20AD-4F47-B17F-AEEB2B1D1D2F}
                                    Console.WriteLine($"Classes\\Interface\\{typeId.ToString("B").ToUpper()}");
                                    using var virtualInterfaceRootEntrySubKey = virtualInterfaceRoot.CreateSubKey(typeId.ToString("B").ToUpper());
                                    
                                    ExportKey(localInterfaceRootKey, virtualInterfaceRootEntrySubKey);
                                    

                                }
                                
                            }
                            
                        }



                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"System.Runtime.InteropServices.ComTypes.ITypeInfo.GetTypeAttr: {ex.Message}"); ;
                    }
                }
            }


            if (File.Exists(pVirtualRegistryPath))
            {
                File.Delete(pVirtualRegistryPath);
                Console.WriteLine($"Deleting file {pVirtualRegistryPath}");
            }


            virtualHive.SaveHive(pVirtualRegistryPath, 5, 1);
            Console.WriteLine($"Saving file {pVirtualRegistryPath}");
        }

        private static string ReplaceKeyEntryValues(string pValue)
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
                                                    "Windows"
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
            string msixValue = String.Empty;

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
            return msixValue;

        }

        private static void ExportKey(RegistryKey localKey, OffregKey virtualKey)
        {

            foreach (var valueName in localKey.GetValueNames())
            {
                var vn = localKey.GetValue(valueName);

                if (vn != null)
                {

                    virtualKey.SetValue(valueName, ReplaceKeyEntryValues(vn.ToString()));
                }

            }

            foreach (var subkey in localKey.GetSubKeyNames())
            {


                var sk = localKey.OpenSubKey(subkey);

                if (sk != null)
                {
                    using var newSubKey = virtualKey.CreateSubKey(subkey);

                    foreach (var valueName in sk.GetValueNames())
                    {
                        var vn = sk.GetValue(valueName);

                        if (vn != null)
                        {
                            newSubKey.SetValue(valueName, ReplaceKeyEntryValues(vn.ToString()));
                        }

                    }

                    ExportKey(sk, newSubKey);
                    

                }

            }

        }


    }
}
