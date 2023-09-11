..\..\bin\TableBuilder.exe -cs .\Tables\GameTable.xlsx
copy /Y   *.cs    ..\CSharpServers\GameServer\Generated\Tables\
copy /Y   *.cs    ..\..\src\Devarc.UnityClient\Assets\Example\Scripts\Generated\Tables\
move /Y   *.cs    ..\UnityClient\Assets\Example\Scripts\Generated\Tables\

..\..\bin\TableBuilder.exe -unity .\Tables\GameTable.xlsx -path Example/Bundles/Tables/
copy /Y   *.Table.cs    ..\..\src\Devarc.UnityClient\Assets\Example\Scripts\Generated\Tables\
move /Y   *.Table.cs    ..\UnityClient\Assets\Example\Scripts\Generated\Tables\
copy /Y   *.Editor.cs    ..\..\src\Devarc.UnityClient\Assets\Example\Scripts\Generated\Tables\Editor\
move /Y   *.Editor.cs    ..\UnityClient\Assets\Example\Scripts\Generated\Tables\Editor\

..\..\bin\TableBuilder.exe -json .\Tables\GameTable.xlsx
copy /Y   *@*.json  ..\..\src\Devarc.UnityClient\Assets\Example\Resources\Tables\
move /Y   *@*.json  ..\UnityClient\Assets\Example\Resources\Tables\
copy /Y   *.json    ..\..\src\Devarc.UnityClient\Assets\Example\Bundles\Tables\
move /Y   *.json    ..\UnityClient\Assets\Example\Bundles\Tables\

..\..\bin\TableBuilder.exe -json .\Tables\SoundTable.xlsx
copy /Y   *@*.json  ..\..\src\Devarc.UnityClient\Assets\Example\Resources\Tables\
move /Y   *@*.json  ..\UnityClient\Assets\Example\Resources\Tables\
copy /Y   *.json    ..\..\src\Devarc.UnityClient\Assets\Example\Bundles\Tables\
move /Y   *.json    ..\UnityClient\Assets\Example\Bundles\Tables\

..\..\bin\TableBuilder.exe -sql .\Tables\GameTable.xlsx
move /Y   *.ddl    ..\Database\Tables\
move /Y   *.sql    ..\Database\Tables\

pause