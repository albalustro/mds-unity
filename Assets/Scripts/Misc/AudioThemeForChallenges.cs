using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Core.SceneManagement;

public class AudioThemeForChallenges : MonoBehaviour {

	private AudioClip themeToPlay;

	IEnumerator Start () {
		EpisodeContext epContext = GameObject.FindObjectOfType<EpisodeContext> ();
		if (!epContext)
			yield break;
		yield return new WaitForSeconds (0.5f);
		AudioController.Instance.PlayTheme (themeToPlay);
	}
}
