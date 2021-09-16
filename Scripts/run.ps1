
. "msixFunctions.ps1"

Unpack-MSIX "C:\Banco Ciudad\Cobis_1.0.0.0_x86__eamkjkkbw7e1a.msix" "c:\msix\cobis\files"

C:\msix\vb6registrytool\Vb6VirtualRegistry.exe C:\msix\cobis\files\VFS C:\msix\cobis\files\registry.dat

Pack-MSIX "c:\msix\cobis\files" "C:\MSIX\cobis\CobisNavegador.msix"

Sign-MSIX "C:\MSIX\cobis\CobisNavegador.msix" "C:\Users\luisdem\OneDrive - Microsoft\PowerShell\AppConsult.pfx"

Add-AppxPackage "C:\MSIX\cobis\CobisNavegador.msix"