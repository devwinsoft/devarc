set IDL=..\..\bin\IDL.exe

..\..\bin\IDL.exe -cs-def  .\Defines\Common.def
move /Y   *.cs    ..\UnityClient\Assets\Example\Scripts\Defines\

..\..\bin\IDL.exe -js-def  .\Defines\Common.def
move /Y   *.js    ..\NodeServers\Protocols\

pause