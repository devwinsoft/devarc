using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ILoginManager
{
    void SignIn(System.Action<bool> callback);
    void SignOut();
}

