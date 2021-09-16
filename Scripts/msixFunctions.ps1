$global:windowsSDKPath = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86" 
$global:certificatePassword = "AppConsult@FY22!"

function CreateCertificate($certificateName, $certificatePath)
{

    #more details at: https://docs.microsoft.com/en-us/powershell/module/pkiclient/new-selfsignedcertificate?view=win10-ps

    if (  (Test-Path $certificatePath ) -eq $false)
    {
        New-Item -Path $certificatePath -ItemType Directory -Force
    }

    Set-Location Cert:\LocalMachine\My

    #creates the a self-signed certificate that should be use for test only

    New-SelfSignedCertificate -Type CodeSigning -Subject "CN=$certificateName" -KeyUsage DigitalSignature `
                              -FriendlyName $certificateName -CertStoreLocation "Cert:\LocalMachine\My" `
                              -NotAfter (Get-Date).AddYears(10)  

    $cert = Get-ChildItem "Cert:\LocalMachine\My" | Where Subject -eq "CN=$certificateName"

    $pwd = ConvertTo-SecureString -String $certificatePassword -Force -AsPlainText 

    Export-PfxCertificate -cert $cert.Thumbprint -FilePath "$certificatePath\$certificateName.pfx" -Password $pwd
    
    Export-Certificate -Cert $cert -FilePath "$certificateName\$certificateName.cer"
     
    Move-Item -Path $cert.PSPath -Destination "Cert:\LocalMachine\TrustedPeople" 

}

#CreateCertificate "AppConsult" "C:\Users\luisdem\"


function Unpack-MSIX($msixPath, $destinationPath)
{
     #Windows 10 SDK Path
    set-location $windowsSDKPath
    
    .\MakeAppx.exe unpack /v /p $msixPath /d $destinationPath /l

}

#Unpack-MSIX  "C:\msix\app.msix" "C:\msix\files\"

function Pack-MSIX($unpackedPath, $newMsixFile)
{
    set-location $windowsSDKPath

    Remove-Item $newMsixFile -ErrorAction Ignore

    Remove-Item $unpackedPath\priconfig.xml -ErrorAction Ignore
    
    .\MakePri.exe createconfig /cf $unpackedPath\priconfig.xml /dq en-US /pv 10.0.0

    Remove-Item $unpackedPath\resources.pri -ErrorAction Ignore

    .\MakePri.exe New -ProjectRoot $unpackedPath -ConfigXml $unpackedPath\priconfig.xml `
                      -OutputFile $unpackedPath\resources.pri `
                      -Verbose -Overwrite

    #comando para reempaquetar
    .\MakeAppx.exe pack /d $unpackedPath /p $newMsixFile /l
}

#Pack-MSIX  "C:\msix\files\" "C:\msix\new.msix"


function Sign-MSIX($msixPath, $certificatePath)
{
    set-location $windowsSDKPath

    .\signtool.exe sign /a /debug /v /fd SHA256 /td SHA256 /f $certificatePath /p $certificatePassword $msixPath 

}
#Sign-MSIX "C:\msix\new.msix" "C:\Users\luisdem\luisdem.pfx" 