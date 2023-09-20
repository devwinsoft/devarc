using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : BaseScene
{
    public override IEnumerator OnEnterScene()
    {
        yield return null;
    }


    public override void OnLeaveScene()
    {
    }


    public void OnClick_GoBack()
    {
        SceneTransManager.Instance.LoadScene("ExampleScene");
    }
}
