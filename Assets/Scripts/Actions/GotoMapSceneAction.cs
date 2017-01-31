using System.Collections;
using System.Collections.Generic;
using MDS.Actions;
using MDS.Core.SceneManagement;
using UnityEngine;

public class GotoMapSceneAction : BaseAction {

    public override IEnumerator Execute()
    {
        if(byPass) yield break;
        yield return base.Execute();


        SceneLoader.Instance.LoadMapScene();

    }

}
