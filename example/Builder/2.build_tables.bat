..\..\bin\TableBuilder.exe -json .\Tables\GameDB.xlsx
move /Y   *.json    ..\UnityClient\Assets\Example\Bundles\Tables\

..\..\bin\TableBuilder.exe -cs .\Tables\GameDB.xlsx
copy /Y   *.cs    ..\CSharpServers\GameServer\Generated\Tables\
move /Y   *.cs    ..\UnityClient\Assets\Example\Generated\Tables\

pause