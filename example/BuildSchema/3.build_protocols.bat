set IDL=..\..\bin\IDL.exe

..\..\bin\IDL.exe -cs  .\Protocols\GameProtocol.idl  .\Defines\Common.def
copy /Y   *.cs    ..\CSharpServers\GameServer\Generated\Protocols\
move /Y   *.cs    ..\UnityClient\Assets\Example\Scripts\Generated\Protocols\

..\..\bin\IDL.exe -js  .\Protocols\AuthProtocol.idl  .\Defines\Common.def
move /Y   *.js    ..\NodeServers\Protocols\

..\..\bin\IDL.exe -cs  .\Protocols\GameProtocol.idl  .\Defines\Common.def
copy /Y   *.cs    ..\CSharpServers\GameServer\Generated\Protocols\
move /Y   *.cs    ..\UnityClient\Assets\Example\Scripts\Generated\Protocols\

..\..\bin\IDL.exe -js  .\Protocols\GameProtocol.idl  .\Defines\Common.def
move /Y   *.js    ..\NodeServers\Protocols\

pause