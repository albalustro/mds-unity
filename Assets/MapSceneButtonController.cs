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

        int gameIndex = scene.GetGameIndex();

        int worldIndex = scene.GetWorldIndex ();
		//SceneLoader.Instance.
		//Fazer a chamada da cena usando o SceneLoader
		SceneManager.LoadScene ("G" + gameIndex + "W" + worldIndex + "E" + episodeIndex, LoadSceneMode.Single);
	}


	//Chave abrir caixa de dialogo com sistema de chave
	//Fazer metodo para saber se um episodio está completo
	//Fazer metodo para saber se um challenge esá completo
}
