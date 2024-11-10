..\..\bin\TableBuilder.exe -cs .\Tables\SoundTable.xlsx
move /Y   *.cs    ..\UnityClient\Assets\Devarc\Sound\Generated\

..\..\bin\TableBuilder.exe -unity .\Tables\SoundTable.xlsx
move /Y   *.Table.cs    ..\UnityClient\Assets\Devarc\Sound\Generated\
move /Y   *.Editor.cs    ..\UnityClient\Assets\Devarc\Sound\Editor\

..\..\bin\TableBuilder.exe -json .\Tables\SoundTable.xlsx
move /Y   *RESOURCE.json  ..\UnityClient\Assets\Resources\Tables\json\
move /Y   *BUNDLE.json      ..\UnityClient\Assets\Bundles\Tables\json\


..\..\bin\TableBuilder.exe -cs .\Tables\ItemTable.xlsx
..\..\bin\TableBuilder.exe -cs .\Tables\GameTable.xlsx
..\..\bin\TableBuilder.exe -cs .\Tables\SkillTable.xlsx
..\..\bin\TableBuilder.exe -cs .\Tables\UnitTable.xlsx
copy /Y   *.cs    ..\CSharpServers\GameServer\Generated\Tables\
move /Y   *.cs    ..\UnityClient\Assets\Scripts\Generated\Tables\

..\..\bin\TableBuilder.exe -unity .\Tables\ItemTable.xlsx
..\..\bin\TableBuilder.exe -unity .\Tables\GameTable.xlsx
..\..\bin\TableBuilder.exe -unity .\Tables\SkillTable.xlsx
..\..\bin\TableBuilder.exe -unity .\Tables\UnitTable.xlsx
move /Y   *.Table.cs    ..\UnityClient\Assets\Scripts\Generated\Tables\
move /Y   *.Editor.cs    ..\UnityClient\Assets\Scripts\Generated\Tables\Editor\

..\..\bin\TableBuilder.exe -json .\Tables\ItemTable.xlsx
..\..\bin\TableBuilder.exe -json .\Tables\GameTable.xlsx
..\..\bin\TableBuilder.exe -json .\Tables\SkillTable.xlsx
..\..\bin\TableBuilder.exe -json .\Tables\UnitTable.xlsx
move /Y   *.json    ..\UnityClient\Assets\Bundles\Tables\json\

..\..\bin\TableBuilder.exe -sql .\Tables\ItemTable.xlsx
..\..\bin\TableBuilder.exe -sql .\Tables\GameTable.xlsx
..\..\bin\TableBuilder.exe -sql .\Tables\SkillTable.xlsx
..\..\bin\TableBuilder.exe -sql .\Tables\UnitTable.xlsx
move /Y   *.ddl    ..\Database\Tables\
move /Y   *.sql    ..\Database\Tables\


..\..\bin\TableBuilder.exe -lstr .\Tables\StringTable.xlsx
move /Y   .\Tables\Korean\*@*     ..\UnityClient\Assets\Resources\LStrings\json\Korean\
move /Y   .\Tables\English\*@*    ..\UnityClient\Assets\Resources\LStrings\json\English\
move /Y   .\Tables\Japanese\*@*   ..\UnityClient\Assets\Resources\LStrings\json\Japanese\
move /Y   .\Tables\Chinese\*@*    ..\UnityClient\Assets\Resources\LStrings\json\Chinese\
move /Y   .\Tables\Korean\*       ..\UnityClient\Assets\Bundles\LStrings\json\Korean\
move /Y   .\Tables\English\*      ..\UnityClient\Assets\Bundles\LStrings\json\English\
move /Y   .\Tables\Japanese\*     ..\UnityClient\Assets\Bundles\LStrings\json\Japanese\
move /Y   .\Tables\Chinese\*      ..\UnityClient\Assets\Bundles\LStrings\json\Chinese\

pause