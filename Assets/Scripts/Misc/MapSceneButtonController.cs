using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MDS.Utilities;
using MDS.Core.SceneManagement;
using UnityEngine.UI;

public class MapSceneButtonController : MDSBehaviour
{

	public SpriteRenderer episodeIndexRenderer;
	public Sprite episodeIndexNormal;
	public Sprite episodeIndexCompleted;
	public Sprite episodeIndexLocked;
	public int episodeIndex;
	public GameObject keyBase;
	public GameObject keyColliderGO;
	public GameObject locked;       //cadeado
	public GameObject letter;       //letter
	public GameObject crystalsHolder;

	private Collider2D _btnCollider;
	private Animator _anim;

	private bool _loadingScene = false;

	void Start()
	{
		_anim = GetComponent<Animator>();
		_btnCollider = GetComponent<Collider2D>();
		SetupButtonsByConpects();
	}

	void SetupButtonsByConpects()
	{
		int w, e;

		Scene scene = SceneManager.GetActiveScene();
		w = scene.GetWorldIndex() - 1;
		e = episodeIndex - 1;

		EpisodeLiberationTypes liberationStatus = EpisodeLiberationTypes.ALLOW_FOR_TEACHER;
		//bool hasDirectAccessToChallenges = UserProfile.Instance.conceptMap.CheckDirectAccessToChallenge(scene);
		bool hasDirectAccessToChallenges = false;

		if (UserProfile.Instance.IsStudent)
		{
			liberationStatus = UserProfile.Instance.conceptMap.EpisodeLiberationStatusByWorldAndEpisode(w, e);
		}

		//ALLOW FOR TEACHER = TUDO LIBERADO
		if (liberationStatus == EpisodeLiberationTypes.ALLOW_FOR_TEACHER)
		{
			_anim.SetInteger("Status", 1);
			episodeIndexRenderer.sprite = episodeIndexNormal;
			hasDirectAccessToChallenges = true;
		}
		else
		{
			//EPISÓDIO COMPLETO (ROXO)
			if (UserProfile.Instance.conceptMap.CheckEpisodeComplete(w, e))
			{
				_anim.SetInteger("Status", 2);
				episodeIndexRenderer.sprite = episodeIndexCompleted;
			}
			//EPISÓDIO LIBERADO (VERDE)
			else if (liberationStatus == EpisodeLiberationTypes.ALLOW_BY_CONCEPT || liberationStatus == EpisodeLiberationTypes.ALLOW_BY_FIRST_ACCESS)
			{
				_anim.SetInteger("Status", 1);
				episodeIndexRenderer.sprite = episodeIndexNormal;
			}
			//EPISÓDIO BLOQUEADO (LARANJA/CINZA)
			else
			{
				keyBase.SetActive(false);
				_btnCollider.enabled = false;
				locked.SetActive(true);
				_anim.SetInteger("Status", 0);
				episodeIndexRenderer.sprite = episodeIndexLocked;

				//EPISÓDIO LIBERADO PELO PROFESSOR
				if (liberationStatus == EpisodeLiberationTypes.ALLOW_BY_TEACHER)
				{
					letter.SetActive(true);
					keyBase.SetActive(true);
				}
			}
		}

		keyColliderGO.SetActive(hasDirectAccessToChallenges);
		StartCoroutine(SetupCrystals(w, e));
	}

	private IEnumerator SetupCrystals(int world, int episode)
	{
		yield return new WaitForSeconds(0.5f);
		for (int i = 0 ; i < 5 ; i++)
		{
			if (!UserProfile.Instance.IsStudent || UserProfile.Instance.conceptMap.CheckChallengeComplete(world, episode, i))
			{
				crystalsHolder.transform.GetChild(i).gameObject.SetActive(true);
			}
		}
	}


	public void LoadEpisode()
	{
		if (_loadingScene)
			return;
		_loadingScene = true;
		SceneLoader.Instance.LoadEpisodeScene(episodeIndex);
	}


	public void ChangeButtonConceptTemporarilyToNormal()
	{
		_anim.SetInteger("Status", 1);
		episodeIndexRenderer.sprite = episodeIndexNormal;
		_btnCollider.enabled = true;
	}
}
