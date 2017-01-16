using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MDS.Utilities;

public class MapSceneButtonController : MDSBehaviour {

	public SpriteRenderer episodeIndexRenderer;
	public Sprite episodeIndexNormal;
	public Sprite episodeIndexCompleted;
	public Sprite episodeIndexLocked;
	public int episodeIndex;

	void Start () {
		SetEpisodeIndexSprite (episodeIndexLocked);
	}
	
	void SetEpisodeIndexSprite(Sprite sprite)
	{
		episodeIndexRenderer.sprite = sprite;
	}

	public void LoadEpisodeScene()
	{
		Scene scene = SceneManager.GetActiveScene ();
		int worldIndex = Extensions.GetWorldIndex (scene);

		#if MDS1
		int gameIndex = 1;
		#endif

		#if MDS2
		int gameIndex = 2;
		#endif

		#if MDS3
		int gameIndex = 3;
		#endif

		SceneManager.LoadScene ("G" + gameIndex + "W" + worldIndex + "E" + episodeIndex, LoadSceneMode.Single);
	}
}
