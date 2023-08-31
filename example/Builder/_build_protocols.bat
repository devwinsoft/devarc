set IDL=..\..\bin\IDL.exe

..\..\bin\IDL.exe -cs  .\Protocols\AuthProtocol.idl  .\Defines\Common.def
move /Y   *.cs    ..\..\src\Devarc.UnityClient\Assets\Example\Scripts\Protocols\

..\..\bin\IDL.exe -js  .\Protocols\AuthProtocol.idl  .\Defines\Common.def
move /Y   *.js    ..\NodeServer\Protocols\

..\..\bin\IDL.exe -cs  .\Protocols\GameProtocol.idl  .\Defines\Common.def
move /Y   *.cs    ..\..\src\Devarc.UnityClient\Assets\Example\Scripts\Protocols\

..\..\bin\IDL.exe -js  .\Protocols\GameProtocol.idl  .\Defines\Common.def
move /Y   *.js    ..\NodeServer\Protocols\

pause