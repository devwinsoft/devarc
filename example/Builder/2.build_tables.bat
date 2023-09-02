..\..\bin\TableBuilder.exe -json .\Tables\GameDB.xlsx
copy /Y   *.json    ..\..\src\Devarc.UnityClient\Assets\Example\Bundles\Tables\
move /Y   *.json    ..\UnityClient\Assets\Example\Bundles\Tables\

..\..\bin\TableBuilder.exe -sql .\Tables\GameDB.xlsx
move /Y   *.ddl    ..\Database\Tables\
move /Y   *.sql    ..\Database\Tables\

..\..\bin\TableBuilder.exe -cs .\Tables\GameDB.xlsx
copy /Y   *.cs    ..\CSharpServers\GameServer\Generated\Tables\
copy /Y   *.cs    ..\..\src\Devarc.UnityClient\Assets\Example\Generated\Tables\
move /Y   *.cs    ..\UnityClient\Assets\Example\Generated\Tables\

..\..\bin\TableBuilder.exe -unity .\Tables\GameDB.xlsx -path Example/Bundles/Tables/
copy /Y   *.Table.cs    ..\..\src\Devarc.UnityClient\Assets\Example\Generated\Tables\
move /Y   *.Table.cs    ..\UnityClient\Assets\Example\Generated\Tables\
copy /Y   *.Editor.cs    ..\..\src\Devarc.UnityClient\Assets\Example\Generated\Tables\Editor\
move /Y   *.Editor.cs    ..\UnityClient\Assets\Example\Generated\Tables\Editor\

pause