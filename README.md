# Devarc (v1.0.0)
Devarc is development templates for Unity project.

Devarc supports:
- PC, Mobile, WebGL clients.
- NodeJS, CSharp servers.
- MySQL database.

## Install ##
* Download & import packages into unity project.
  * https://github.com/devwinsoft/devarc/blob/main/install
  * https://github.com/MessagePack-CSharp/MessagePack-CSharp/releases
  * https://github.com/psygames/UnityWebSocket
    
* Download websocket-sharp.dll or source codes for CSharp server.
  * https://github.com/sta/websocket-sharp
    
## Getting Started ##
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
#### Step 2: Create shared tables with Excel. ####
#### Step 3: Create shared protocols with C#. ####
```csharp
namespace C2Auth
{
    public class RequestLogin
    {
        public string accountID;
        public string password;
    }
}

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
#### Step 4: Create batch files to build. ####


## License ##

Devarc is provided under [Apache License 2.0].


