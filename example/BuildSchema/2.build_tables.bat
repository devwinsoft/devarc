..\..\bin\TableBuilder.exe -cs .\Tables\SoundTable.xlsx
move /Y   *.cs    ..\UnityClient\Assets\Devarc\Sound\Generated\

..\..\bin\TableBuilder.exe -unity .\Tables\SoundTable.xlsx
move /Y   *.Table.cs    ..\UnityClient\Assets\Devarc\Sound\Generated\
move /Y   *.Editor.cs    ..\UnityClient\Assets\Devarc\Sound\Editor\

..\..\bin\TableBuilder.exe -json .\Tables\SoundTable.xlsx
move /Y   *RESOURCE.json  ..\UnityClient\Assets\Example\Resources\Tables\
move /Y   *BUNDLE.json      ..\UnityClient\Assets\Example\Bundles\Tables\


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
move /Y   .\Tables\Korean\*@*     ..\UnityClient\Assets\Example\Resources\LStrings\Korean
move /Y   .\Tables\Korean\*       ..\UnityClient\Assets\Example\Bundles\LStrings\Korean
move /Y   .\Tables\English\*@*    ..\UnityClient\Assets\Example\Resources\LStrings\English
move /Y   .\Tables\English\*      ..\UnityClient\Assets\Example\Bundles\LStrings\English
move /Y   .\Tables\Japanese\*@*   ..\UnityClient\Assets\Example\Resources\LStrings\Japanese
move /Y   .\Tables\Japanese\*     ..\UnityClient\Assets\Example\Bundles\LStrings\Japanese
move /Y   .\Tables\Chinese\*@*    ..\UnityClient\Assets\Example\Resources\LStrings\Chinese
move /Y   .\Tables\Chinese\*      ..\UnityClient\Assets\Example\Bundles\LStrings\Chinese

pause