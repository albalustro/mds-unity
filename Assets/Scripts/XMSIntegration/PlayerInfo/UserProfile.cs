using System;
using System.Linq;
using FullInspector;
using MDS.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class UserProfile : Singleton<UserProfile>
{

#if UNITY_EDITOR
	public bool debugMode = false;
	public bool forceFirstAccessConceptMap = false;
#endif

	public string login;
	public string pass;
	public LoginInfo loginInfo;

	public event Action OnConceptMapUpdatedByRemoteEvent;

	[SerializeField, ShowInInspector]
	private ConceptMap _conceptMap;
	public ConceptMap conceptMap { get { return _conceptMap; } }

	private const string STUDENT_ROLE = "Estudante";

	public bool IsStudent
	{
		get
		{
			return loginInfo.Role.Equals(STUDENT_ROLE);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (UserProfile.Instance != this)
			Destroy(gameObject);
		else
		{
			DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
			if (debugMode)
				if (conceptMap == null)
					SetConceptMapAtFirstAccess();
#endif
		}
	}

	public void UpdateConcept(Scene challengeScene, ConceptTypes newConcept, DateTime startDate)
	{
		int w, e, c;

		if (!challengeScene.IsChallenge())
			return;

		w = challengeScene.GetWorldIndex() - 1;
		e = challengeScene.GetEpisodeIndex() - 1;
		c = challengeScene.GetChallengeIndex() - 1;

		var conceptData = _conceptMap.GetConceptData(w, e, c);
		if (conceptData.Concept.HasValue && conceptData.Concept.Value > (int)newConcept)
		{
			return;
		}

		conceptData.Concept = (int)newConcept;
		conceptData.StartDate = startDate;
		conceptData.EndDate = DateTime.UtcNow;

		// atualiza a liberacao do proximo episodio
		_conceptMap.TryUpdateNextEpisodeLiberationStatus(challengeScene);

#if UNITY_EDITOR
		if (loginInfo == null)
			return;
#endif

		SaveUserProfile();
		SendConceptMapToSyncer();
	}

	public void SetLoginInfo(string l, string p, LoginInfo i)
	{
		this.login = l;
		this.pass = p;
		this.loginInfo = i;
		SincronizeConceptMapOnLogin();
	}

	private void SincronizeConceptMapOnLogin()
	{
		PersistenceManager.Instance.LoadConceptMap(this, ref _conceptMap);
#if UNITY_EDITOR
		if (_conceptMap == null || forceFirstAccessConceptMap)
#else
        if(_conceptMap == null)
#endif
		{
			SetConceptMapAtFirstAccess();
		}
		SaveUserProfile();
		SendConceptMapToSyncer();
	}

	private async void SendConceptMapToSyncer()
	{
		if (loginInfo.Status.code != ConnectionResponse.OK)
		{
			return;
		}

		ConceptMap cm = await NetworkManager.Instance.DoSincronize(_conceptMap);

		if (cm == null) // estava on line no login (caso contrario nem teria enviado nada..) e voltou com algum erro
		{
			loginInfo.Status.code = ConnectionResponse.CONNECTION_OFFLINE;
		}
		else
		{
			_conceptMap = cm;
			SaveUserProfile();

			OnConceptMapUpdatedByRemoteEvent?.Invoke();
		}
	}

	private void SaveUserProfile()
	{
		PersistenceManager.Instance.SaveUserProfile(this);
	}

	private void SetConceptMapAtFirstAccess()
	{

		_conceptMap = new ConceptMap();
		_conceptMap.Concepts = new ConceptData[4 * 8 * 5];
		int index = 0;
		for (int w = 0 ; w < 4 ; w++)
		{
			for (int e = 0 ; e < 8 ; e++)
			{
				for (int c = 0 ; c < 5 ; c++)
				{
					_conceptMap.Concepts[index++] = new ConceptData()
					{
						WorldIndex = w,
						EpisodeIndex = e,
						ChallengeIndex = c,
						Concept = (int)ConceptTypes.CONCEPT_NOT_PLAYED,
						EndDate = null,
						StartDate = null,
						LiberationType = e == 0 ? EpisodeLiberationTypes.ALLOW_BY_FIRST_ACCESS : EpisodeLiberationTypes.BLOCK_BY_CONCEPT
					};
				}
			}
		}
	}

#if UNITY_EDITOR
	[InspectorButton]
	public void CreateNewUserProfileForDebug()
	{
		Instance.SetConceptMapAtFirstAccess();
		this.login = "Teste";
		this.pass = "";
		this.loginInfo = new LoginInfo()
		{
			Role = "Estudante",
			Status = new StatusInfo() { code = ConnectionResponse.CONNECTION_OFFLINE },
		};
	}
#endif
}
