using System.Collections;
using System.Collections.Generic;
using MDS.Core.SceneManagement;
using UnityEngine;

public class DebugAutoDestroy : MonoBehaviour {

    private void Awake()
    {
        if(SceneLoader.Instance.destroyDebugObjectsOnSceneLoad)
            DestroyImmediate(gameObject);
    }

}
