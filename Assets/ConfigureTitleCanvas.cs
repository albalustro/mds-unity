using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Utilities;
using UnityEngine.SceneManagement;

public class ConfigureTitleCanvas : MonoBehaviour {

	void Start () {
		Scene scene = SceneManager.GetActiveScene ();
		if (!scene.IsEpisode ())
			this.gameObject.SetActive (false);
	}
}
