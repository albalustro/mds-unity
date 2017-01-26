using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MDS.Utilities;
using MDS.Core.SceneManagement;

public class MapSceneButtonController : MDSBehaviour {

	public  SpriteRenderer episodeIndexRenderer;
	public  Sprite episodeIndexNormal;
	public  Sprite episodeIndexCompleted;
	public  Sprite episodeIndexLocked;
	public  int episodeIndex;
	public  GameObject keyBase;
	public GameObject keyColliderGO;
	public  GameObject locked;		//cadeado
	public  GameObject letter;		//letter

	private Collider2D _btnCollider;
	private Animator _anim;

	void Start () {
		_anim = GetComponent<Animator> ();
		_btnCollider = GetComponent<Collider2D> ();
		SetupButtonsByConpects ();
	}
	
	void SetupButtonsByConpects()
	{
		int w, e;

		Scene scene = SceneManager.GetActiveScene ();
		w = scene.GetWorldIndex() - 1;
		e = episodeIndex - 1;
		EpisodeLiberationTypes liberationStatus = UserProfile.Instance.conceptMap.worlds [w].episodes [e].liberationStatus;

		//ALLOW FOR TEACHER = TUDO LIBERADO
		if (liberationStatus == EpisodeLiberationTypes.ALLOW_FOR_TEACHER)
		{
			_anim.SetInteger ("Status", 1);
			episodeIndexRenderer.sprite = episodeIndexNormal;
		} 
		else
		{
			//EPISÓDIO COMPLETO (ROXO)
			if (UserProfile.Instance.conceptMap.worlds [w].episodes [e].CheckEpisodeComplete ())
			{
				_anim.SetInteger ("Status", 2);
				episodeIndexRenderer.sprite = episodeIndexCompleted;
			} 
		//EPISÓDIO LIBERADO (VERDE)
			else if (liberationStatus == EpisodeLiberationTypes.ALLOW_BY_CONCEPT || liberationStatus == EpisodeLiberationTypes.ALLOW_BY_FIRST_ACCESS)
			{
				_anim.SetInteger ("Status", 1);
				episodeIndexRenderer.sprite = episodeIndexNormal;
				if (!UserProfile.Instance.conceptMap.worlds [w].episodes [e].CheckDirectAccessToChallenge (episodeIndex))
					keyColliderGO.SetActive (false);

			} 
		//EPISÓDIO BLOQUEADO (LARANJA/CINZA)
			else
			{
				keyBase.SetActive (false);
				_btnCollider.enabled = false;
				locked.SetActive (true);
				_anim.SetInteger ("Status", 0);
				episodeIndexRenderer.sprite = episodeIndexLocked;
				if (liberationStatus == EpisodeLiberationTypes.ALLOW_BY_TEACHER)
				{
					letter.SetActive (true);
					keyBase.SetActive (true);
					_btnCollider.enabled = true;
					if (!UserProfile.Instance.conceptMap.worlds [w].episodes [e].CheckDirectAccessToChallenge (episodeIndex))
						keyColliderGO.SetActive (false);
				}
			}
		}
	}
		


	public void LoadEpisode()
	{
		SceneLoader.Instance.LoadEpisodeScene (episodeIndex);
	}


	//Chave abrir caixa de dialogo com sistema de chave		OK
	//Fazer metodo para saber se um episodio está completo	OK
	//Fazer metodo para saber se um challenge esá liberado	OK
}
