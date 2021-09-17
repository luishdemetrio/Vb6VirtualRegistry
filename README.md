[![.NET](https://github.com/luishdemetrio/Vb6VirtualRegistry/actions/workflows/dotnet.yml/badge.svg)](https://github.com/luishdemetrio/Vb6VirtualRegistry/actions/workflows/dotnet.yml)

# VB6 Registry Tool
This is a very specific project created to assist developers who want to publish their Visual Basic 6.0 applications, which still demand periodic updates/implementations, through the  [MSIX](https://docs.microsoft.com/en-us/windows/msix/overview "What is MSIX?") format.

This project helps to automate the Visual Basic 6.0 packaging by generating a virtual registry file to be added on the VB6 MSIX package with the components (OCXs and DLLs) used by the VB6 application.

## Purpose
I work for Microsoft in a team called **App Consult** that is responsible to assist customers to modernize their Desktop applications through technologies like Windows App SDK and MSIX.

During my engagements, I noticed some companies that still have legacy applications like Visual Basic 6.0 that still working well and that don't require modifications. Some of them no longer have the source code of those VB6 applications. For this scenario, the [MSIX Packaging Tool](https://docs.microsoft.com/en-us/windows/msix/packaging-tool/tool-overview "MSIX Packaging Tool") is very effective for packaging the VB6 application to the MSIX format which simplifies the installation of those applications by removing the need for administrative rights for installing or registering components. 


But there are also companies that still have applications developed in Visual Basic 6.0 that often need updates. Unlike other applications, when a VB6 application is modified, in certain situations, the CLSID of its components can be changed during the new build. For this specific scenario, due to the amount of builds and deployments, having to manually perform the entire packaging process to ensure that the registry keys are updated is time consuming.

The tool **VB6RegistryTool** creates the virtual registry of the application components through a single command line, allowing to automate and simplify the packaging of VB6 applications for MSIX.



### This project uses the OffregLib project available on GitHub
As the original [OffregLib](https://github.com/LordMike/OffregLib "OffregLib repo on GitHub") project was developed in .NET Framework 4 at the time when VB6RegistryTool was created, I cloned their repository, migrated it to .NET Core 3.1 and included here in this project.

More about the official OffregLib here: http://nuget.org/packages/OffregLib

