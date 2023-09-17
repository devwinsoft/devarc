..\..\bin\TableBuilder.exe -cs .\Tables\SoundTable.xlsx
move /Y   *.cs    ..\UnityClient\Assets\Devarc\Sound\Generated\

..\..\bin\TableBuilder.exe -unity .\Tables\SoundTable.xlsx
move /Y   *.Table.cs    ..\UnityClient\Assets\Devarc\Sound\Generated\
move /Y   *.Editor.cs    ..\UnityClient\Assets\Devarc\Sound\Editor\

..\..\bin\TableBuilder.exe -json .\Tables\SoundTable.xlsx
move /Y   *@*.json  ..\UnityClient\Assets\Example\Resources\Tables\
move /Y   *.json    ..\UnityClient\Assets\Example\Bundles\Tables\


..\..\bin\TableBuilder.exe -cs .\Tables\GameTable.xlsx
copy /Y   *.cs    ..\CSharpServers\GameServer\Generated\Tables\
move /Y   *.cs    ..\UnityClient\Assets\Example\Scripts\Generated\Tables\

..\..\bin\TableBuilder.exe -unity .\Tables\GameTable.xlsx
move /Y   *.Table.cs    ..\UnityClient\Assets\Example\Scripts\Generated\Tables\
move /Y   *.Editor.cs    ..\UnityClient\Assets\Example\Scripts\Generated\Tables\Editor\

..\..\bin\TableBuilder.exe -json .\Tables\GameTable.xlsx
move /Y   *@*.json  ..\UnityClient\Assets\Example\Resources\Tables\
move /Y   *.json    ..\UnityClient\Assets\Example\Bundles\Tables\

..\..\bin\TableBuilder.exe -sql .\Tables\GameTable.xlsx
move /Y   *.ddl    ..\Database\Tables\
move /Y   *.sql    ..\Database\Tables\


..\..\bin\TableBuilder.exe -lstr .\Tables\StringTable.xlsx
move /Y   .\Tables\eng\*   ..\UnityClient\Assets\Example\Bundles\Strings\eng
move /Y   .\Tables\kor\*   ..\UnityClient\Assets\Example\Bundles\Strings\kor

pause