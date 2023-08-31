set IDL=..\..\bin\IDL.exe

..\..\bin\IDL.exe -cs  AuthProtocol.idl  Defines.idl
move /Y   *.cs    ..\Devarc.UnityClient\Assets\Scripts\Example\Protocols\

..\..\bin\IDL.exe -js  AuthProtocol.idl  Defines.idl
move /Y   *.js    ..\Devarc.NodeServer\Protocols\

..\..\bin\IDL.exe -cs  GameProtocol.idl  Defines.idl
move /Y   *.cs    ..\Devarc.UnityClient\Assets\Scripts\Example\Protocols\

..\..\bin\IDL.exe -js  GameProtocol.idl  Defines.idl
move /Y   *.js    ..\Devarc.NodeServer\Protocols\

pause