..\..\bin\TableBuilder.exe -json .\Tables\GameDB.xlsx
move /Y   *.json    ..\..\src\Devarc.UnityClient\Assets\Example\Bundles\Tables\

..\..\bin\TableBuilder.exe -cs .\Tables\GameDB.xlsx
move /Y   *.cs      ..\..\src\Devarc.UnityClient\Assets\Example\Scripts\Tables\

pause