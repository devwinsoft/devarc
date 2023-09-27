using UnityEngine;
using Devarc;

public class AuthNetwork : WebClient
{
    public void RequestLogin(C2Auth.RequestLogin request, RequestCallback<Auth2C.NotifyLogin> callback)
    {
        Post<C2Auth.RequestLogin, Auth2C.NotifyLogin>(request, callback);
    }

    public void RequestLogout(C2Auth.RequestLogout request, RequestCallback<Auth2C.NotifyLogout> callback)
    {
        Post<C2Auth.RequestLogout, Auth2C.NotifyLogout>(request, callback);
    }
}