using UnityEngine;
using Devarc;

public class AuthNetwork : WebClient
{
    public void RequestLogin(C2Auth.RequestLogin request, RequestCallback<Auth2C.NotifyLogin> responseCallback, ErrorCallback errorCallback = null)
    {
        Post<C2Auth.RequestLogin, Auth2C.NotifyLogin>(request, responseCallback, errorCallback);
    }
}