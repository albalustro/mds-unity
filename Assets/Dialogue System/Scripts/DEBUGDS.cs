using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DEBUGDS : MonoBehaviour {

	public void DEBUG_SET_GAME(int num)
    {
        DSGlobal.game = num.ToString();
        SceneManager.LoadScene("World");
    }

    public void DEBUG_SET_WORLD(int num)
    {
        DSGlobal.world = num.ToString();
        SceneManager.LoadScene("Episode");
    }

    public void DEBUG_SET_EPISODE(int num)
    {
        SceneManager.LoadScene("G" + DSGlobal.game + "W" + DSGlobal.world + "E" + num);
    }

    public void DEBUG_SET_CHALLENGE(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void DEBUG_BACK(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
