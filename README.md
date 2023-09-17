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
- Unity: Asset management.
- Unity: Simple effect management.
- Unity: Simple animation management.
- Unity: Simple sound management.
- Unity: Simple data encryption.

#### Test Release ####
- [WebGL version](http://ec2-52-78-42-13.ap-northeast-2.compute.amazonaws.com/b/index.html)


## Install Client ##
* Download & import packages into unity project.
  * https://github.com/devwinsoft/devarc/blob/main/install
  * https://github.com/MessagePack-CSharp/MessagePack-CSharp/releases
  * https://github.com/psygames/UnityWebSocket
    

## Install Server ##
* Download websocket-sharp.dll or source codes for CSharp server.
  * https://github.com/sta/websocket-sharp
    
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
#### Step 3: Create batch files to build. ####
```
IDL.exe -cs-def  {SchemaFolder}\Common.def
IDL.exe -js  {SchemaFolder}\AuthProtocol.idl  {SchemaFolder}\Common.def
TableBuilder.exe -cs {SchemaFolder}\SoundTable.xlsx
move /Y   *.cs    {UnityProjectFolder}\Assets\Scripts\Generated\
```

## Table Builder ##

#### Step 1: Create Excel Tables. ####

#### Step 2: Create batch files to build. ####
```
..\..\bin\TableBuilder.exe -cs .\Tables\GameTable.xlsx
move /Y   *.cs    ..\UnityClient\Assets\Example\Scripts\Generated\Tables\

..\..\bin\TableBuilder.exe -json .\Tables\GameTable.xlsx
move /Y   *.json    ..\UnityClient\Assets\Example\Bundles\Tables\

..\..\bin\TableBuilder.exe -sql .\Tables\GameTable.xlsx
move /Y   *.ddl    ..\Database\Tables\
move /Y   *.sql    ..\Database\Tables\
```

## Unity Asset Management ##

#### Step 1: Create localizing table. ####
<img src="screenshot/example_lstring.png" width="70%" height="70%"/>

#### Step 2: Edit addressable configuration. ####
![img](screenshot/example_addressable.png)

#### Step 3: Make loading scripts. ####
```
    IEnumerator loadAssets()
    {
        var handle = AssetManager.Instance.LoadAssets_Bundle<TextAsset>("lstring", SystemLanguage.English);
        yield return handle;
        if (handle.IsValid())
        {
            Table.LString.LoadFromFile("LString");
        }
    }
```

## Unity Effect Management. ##

## Unity Animation Management. ##

## Unity Sound Management. ##
  
## Unity Data Encryption ##

## License ##

Devarc is provided under [Apache License 2.0].


