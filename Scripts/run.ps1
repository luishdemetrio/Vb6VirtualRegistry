
# it loads the methods defined on msixFunctions.ps1
. "C:\github\Vb6VirtualRegistry\Scripts\msixFunctions.ps1"


# it creates a self-signed certificate for CN=VB6VirtualRegistry 
# and it installs the certificate on Local Machine \ Trusted People
# notice that the name must be the same defined on the app manifest Publisher="CN=Vb6VirtualRegistry" 
CreateCertificate "MeuCertificado" "C:\msix"

# Unpack-MSIX "C:\github\Vb6VirtualRegistry\Scripts\Sample\sample.msix" C:\github\Vb6VirtualRegistry\Scripts\Sample\msix

# C:\github\vb6registrytool\Vb6VirtualRegistry.exe regsvr32 C:\github\Vb6VirtualRegistry\Scripts\Sample\msix C:\github\Vb6VirtualRegistry\Scripts\Sample\msix\registry.dat


# it is used to package the folder
Pack-MSIX C:\github\Vb6VirtualRegistry\Sample\unpackaged C:\github\Vb6VirtualRegistry\Sample\myvb6app.msix

# it signs the application with our test certification
Sign-MSIX C:\github\Vb6VirtualRegistry\Sample\myvb6app.msix "C:\github\Vb6VirtualRegistry\Sample\Vb6VirtualRegistry.pfx"

#Add-AppxPackage C:\github\Vb6VirtualRegistry\Sample\myvb6app.msix