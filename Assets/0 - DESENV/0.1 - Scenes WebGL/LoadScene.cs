using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MDS.Core.SceneManagement;

public class LoadScene : MonoBehaviour {


	public void Load(int index)
	{
        SceneLoader.Instance.LoadChallenge(index);
    }
}
