
. "C:\github\Vb6VirtualRegistry\Scripts\msixFunctions.ps1"

# CreateCertificate "Vb6VirtualRegistry" "C:\github\Vb6VirtualRegistry\Scripts\Sample"

# Unpack-MSIX "C:\github\Vb6VirtualRegistry\Scripts\Sample\sample.msix" C:\github\Vb6VirtualRegistry\Scripts\Sample\msix

# C:\github\vb6registrytool\Vb6VirtualRegistry.exe C:\github\Vb6VirtualRegistry\Scripts\Sample\msix C:\github\Vb6VirtualRegistry\Scripts\Sample\msix\registry.dat

Pack-MSIX C:\github\Vb6VirtualRegistry\Scripts\Sample\msix C:\github\Vb6VirtualRegistry\Scripts\Sample.msix

Sign-MSIX C:\github\Vb6VirtualRegistry\Scripts\Sample.msix "C:\github\Vb6VirtualRegistry\Scripts\Sample\Vb6VirtualRegistry.pfx"

Add-AppxPackage C:\github\Vb6VirtualRegistry\Scripts\Sample.msix