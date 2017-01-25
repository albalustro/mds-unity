using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MDS.Utilities;
using MDS.Core.SceneManagement;

public class MapSceneButtonController : MDSBehaviour {

	public SpriteRenderer episodeIndexRenderer;
	public Sprite episodeIndexNormal;
	public Sprite episodeIndexCompleted;
	public Sprite episodeIndexLocked;
	public int episodeIndex;
	public GameObject keyPanel;


	void Start () {
		SetEpisodeIndexSprite ();
	}
	
	void SetEpisodeIndexSprite()
	{
		int w, e;
		Scene scene = SceneManager.GetActiveScene ();
		w = scene.GetWorldIndex() - 1;
		e = episodeIndex - 1;
		print ("world: " + w + "   Episode: " + e);
		EpisodeLiberationTypes liberationStatus = UserProfile.Instance.conceptMap.worlds [w].episodes [e].liberationStatus;

		if (UserProfile.Instance.conceptMap.worlds[w].episodes[e].CheckEpisodeComplete())
			episodeIndexRenderer.sprite = episodeIndexCompleted;
		else if(liberationStatus == EpisodeLiberationTypes.ALLOW_BY_CONCEPT )
			episodeIndexRenderer.sprite = episodeIndexNormal;
		else
			episodeIndexRenderer.sprite = episodeIndexLocked;
	}

	public void LoadEpisode()
	{
		SceneLoader.Instance.LoadEpisodeScene (episodeIndex);
	}


	//Chave abrir caixa de dialogo com sistema de chave		OK
	//Fazer metodo para saber se um episodio está completo	OK
	//Fazer metodo para saber se um challenge esá liberado	OK
}
