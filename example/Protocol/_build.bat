set IDL=..\..\bin\IDL.exe

..\..\bin\IDL.exe -cs  AuthProtocol.idl  Defines.idl
move /Y   *.cs    ..\..\src\Devarc.UnityClient\Assets\Scripts\Example\Protocols\

..\..\bin\IDL.exe -js  AuthProtocol.idl  Defines.idl
move /Y   *.js    ..\NodeServer\Protocols\

..\..\bin\IDL.exe -cs  GameProtocol.idl  Defines.idl
move /Y   *.cs    ..\..\src\Devarc.UnityClient\Assets\Scripts\Example\Protocols\

..\..\bin\IDL.exe -js  GameProtocol.idl  Defines.idl
move /Y   *.js    ..\NodeServer\Protocols\

pause