﻿namespace C2Game
{
    public class RequestLogin
    {
        public string sessionID;
        public int secret;
    }
}

namespace Game2C
{
    public class NotifyLogin
    {
        public ErrorType errorCode;
    }
}

