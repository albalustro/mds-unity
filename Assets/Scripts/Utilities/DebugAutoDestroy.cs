using System.Collections;
using System.Collections.Generic;
using MDS.Core.SceneManagement;
using UnityEngine;

public class DebugAutoDestroy : MonoBehaviour {
#if UNITY_EDITOR
    private void Awake()
    {
        if(SceneLoader.Instance.destroyDebugObjectsOnSceneLoad)
            DestroyImmediate(gameObject);
    }
#endif
}
