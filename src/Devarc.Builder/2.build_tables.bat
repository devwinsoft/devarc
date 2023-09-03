..\..\bin\TableBuilder.exe -cs .\Tables\SoundTable.xlsx
move /Y   *.cs    ..\Devarc.UnityClient\Assets\Devarc\devarc.client.sound\Runtime\Generated\

..\..\bin\TableBuilder.exe -unity .\Tables\SoundTable.xlsx -path Example/Bundles/Tables/
move /Y   *.Editor.cs    ..\Devarc.UnityClient\Assets\Devarc\devarc.client.sound\Editor\
move /Y   *.Table.cs     ..\Devarc.UnityClient\Assets\Devarc\devarc.client.sound\Runtime\Generated\

pause