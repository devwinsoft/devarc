# Devarc (v1.0.0)
Devarc is cross-platform development templates.

#### Devarc supports ####
- Windows development evironment.
- PC, Mobile, WebGL clients.
- NodeJS, CSharp servers.
- MySQL database.

#### Devarc includes ####
- Protocol builder.
- Table builder.
- Unity: Asset manager.
- Unity: Simple effect manager.
- Unity: Simple animation manager.
- Unity: Simple sound manager.
- Unity: Simple data encryption.

#### Test Release ####
- [WebGL version](http://ec2-52-78-42-13.ap-northeast-2.compute.amazonaws.com/x/index.html)


## Install Client ##
#### Step 1: Create unity project. ####
#### Step 2: Download packages, and import into unity project. ####
  * https://github.com/devwinsoft/devarc/blob/main/install
  * https://github.com/MessagePack-CSharp/MessagePack-CSharp/releases
  * https://github.com/psygames/UnityWebSocket
    

## Install C# Server ##
#### Step 1: Create csharp project. ####
#### Step 2: Download projects, and import into C# project. ####
  * https://github.com/devwinsoft/devarc/tree/main/src/Devarc.Net.Common
  * https://github.com/devwinsoft/devarc/tree/main/src/Devarc.Net.Server
  * https://github.com/sta/websocket-sharp

## Install Node.JS Server ##
  * Now, working...
## Protocol Builder ##
#### Step 1: Create shared definitions with C#. ####
```csharp
public enum ErrorType
{
    SUCCESS          = 0,
    UNKNOWN          = 1,
    SERVER_ERROR     = 2,
    SESSION_EXPIRED  = 3,
}
```
#### Step 2: Create shared protocols with C#. ####
```csharp
// Protocol from:Client to:AuthServer
namespace C2Auth
{
    public class RequestLogin
    {
        public string accountID;
        public string password;
    }
}

// Protocol from:AuthServer to:Client
namespace Auth2C
{
    public class NotifyLogin
    {
        public ErrorType errorCode;
        public string sessionID;
        public int secret;
    }
}
```
#### Step 3: Create batch files. ####
```
IDL.exe -cs-def  {SchemaFolder}\Common.def
IDL.exe -js  {SchemaFolder}\AuthProtocol.idl  {SchemaFolder}\Common.def
TableBuilder.exe -cs {SchemaFolder}\SoundTable.xlsx
move /Y   *.cs    {UnityProjectFolder}\Assets\Scripts\Generated\
```

## Table Builder ##

#### Step 1: Create Excel Tables. ####

#### Step 2: Create batch files. ####
```
..\..\bin\TableBuilder.exe -cs .\Tables\GameTable.xlsx
move /Y   *.cs    ..\UnityClient\Assets\Example\Scripts\Generated\Tables\

..\..\bin\TableBuilder.exe -json .\Tables\GameTable.xlsx
move /Y   *.json    ..\UnityClient\Assets\Example\Bundles\Tables\

..\..\bin\TableBuilder.exe -sql .\Tables\GameTable.xlsx
move /Y   *.ddl    ..\Database\Tables\
move /Y   *.sql    ..\Database\Tables\
```

## Unity: Asset Management ##

#### Step 1: Create localizing table. ####
![img](screenshot/example_lstring.png)

#### Step 2: Edit addressable configuration. ####
![img](screenshot/example_addressable.png)

#### Step 3: Initialize DownloadManager. ####
```csharp
DownloadManager.Instance.AddToPatchList("effect");
DownloadManager.Instance.AddToPatchList("sound");

DownloadManager.Instance.OnPatch += (info) =>
{
    DownloadManager.Instance.BeginDownload();
};

DownloadManager.Instance.OnResult += () =>
{
    // Load bundles...
    StartCoroutine(loadAssets());
};
```

#### Step 4: Make loading scripts. ####
```csharp
IEnumerator loadAssets()
{
    var handle = AssetManager.Instance.LoadAssets_Bundle<TextAsset>("lstring", SystemLanguage.English);
    yield return handle;
    if (handle.IsValid())
    {
        Table.LString.LoadFromFile("LString");
    }

    yield return EffectManager.Instance.LoadBundle("effect");
    yield return SoundManager.Instance.LoadBundle("sound");
}
```

## Unity: Simple Effect Manager. ##

## Unity: Simple Animation Manager. ##

## Unity: Simple Sound Manager. ##
  
## Unity: Data Encryption ##

## License ##

Devarc is provided under [Apache License 2.0].


