..\..\bin\TableBuilder.exe -json .\Tables\GameDB.xlsx
move /Y   *.json    ..\UnityClient\Assets\Example\Bundles\Tables\

..\..\bin\TableBuilder.exe -cs .\Tables\GameDB.xlsx
move /Y   *.cs      ..\UnityClient\Assets\Example\Scripts\Tables\

pause