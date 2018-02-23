using MDS.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMoveSairButton : MonoBehaviour {

    void Start()
    {
#if !PLAY_MOVE
        gameObject.SetActive(false);
#else
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsLogin())
        {
            gameObject.SetActive(false);
        }
#endif
    }
}
