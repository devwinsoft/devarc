public enum STAT_TYPE
{
    None,
}

public class VECTOR3
{
    public float x;
    public float y;
    public float z;
}


public enum ErrorType
{
    SUCCESS          = 0,
    UNKNOWN          = 1,
    DATABASE_ERROR   = 2,
    PROTOCOL_ERROR   = 3,
    SERVER_ERROR     = 4,
    SESSION_EXPIRED  = 5,
    SESSION_REMAIN   = 6,
    INVALID_PASSWORD = 7,
    INVALID_SECRET   = 8,
}

public class CommonResult
{
    public ErrorType errorCode;
}


public class CustomSigninResult
{
    public ErrorType errorCode;
    public string secret;
}

public class GoogleCodeResult
{
    public ErrorType errorCode;
    public string code;
}

public class GoogleSigninResult
{
    public ErrorType errorCode;
    public string account_id;
    public string access_token;
    public string refresh_token;
    public int expires_in;
    public string secret;
}

public class GoogleRefreshResult
{
    public ErrorType errorCode;
    public string access_token;
    public string refresh_token;
    public int expires_in;
}
