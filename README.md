# Weld-Data-Setter-for-Fronius-TPSi  [![DOI](https://zenodo.org/badge/701179267.svg)](https://zenodo.org/doi/10.5281/zenodo.10039267)

This software can be used to send identifying part information and part serial number to welding machine so that components traceability is possible to full fill. Works with at least Fronius TPSi welding machines for collecting welding parameters to Fronius WeldCube Premium welding data manager. Allows one to set welding machines IP address and port number (4714 as default) where entered information is about to be sent. Works as is in small scale production and in research purposes.

The software is developed with Visual Studio using .Net Core 3.1 framework, which is compatible with Windows, Linux and Mac. A Runtime package may be needed to run the software, if OS does not iclude it by default (Windows). Runtime can be downloaded from https://dotnet.microsoft.com/en-us/download/dotnet/3.1 

Steps to run the software: (Windows)
1. Download compiled software package from https://github.com/SavoniaUAS/Weld-Data-Setter-for-Fronius-TPSi/releases/download/Release/WeldDataSetter.zip
2. Unzip a content of the software package to a folder.
3. Open the folder and run WeldDataSetter.exe application. If the application does not run, try to install .Net Core 3.1 Runtime package.
4. Set a IP address and a port (Default 4714) of the welding machine to server settings.
5. Now a part information of welding machine can be updated by filling a item number, a serial number and clicking "Send message" button.

   
