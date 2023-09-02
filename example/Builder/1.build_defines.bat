set IDL=..\..\bin\IDL.exe

..\..\bin\IDL.exe -cs-def  .\Defines\Common.def
copy /Y   *.cs    ..\CSharpServers\GameServer\Generated\Defines\
copy /Y   *.cs    ..\..\src\Devarc.UnityClient\Assets\Example\Generated\Defines\
move /Y   *.cs    ..\UnityClient\Assets\Example\Generated\Defines\

..\..\bin\IDL.exe -js-def  .\Defines\Common.def
move /Y   *.js    ..\NodeServers\Protocols\

pause