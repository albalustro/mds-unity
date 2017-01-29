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
#endif

    public string login;
	public string pass;
    public LoginInfo loginInfo;

    public event Action OnConceptMapUpdatedByRemoteEvent;

    [SerializeField, ShowInInspector]
    private ConceptMap _conceptMap;
    public ConceptMap conceptMap { get { return _conceptMap; } }


    protected override void Awake()
    {
        base.Awake();
        if(UserProfile.Instance != this)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
            if(debugMode)
				if (conceptMap == null) 
                	SetConceptMapAtFirstAccess();
#endif
        }
    }

    //code: t000m000e000d000
    public void UpdateConcept(Scene challengeScene, ConceptTypes newConcept, DateTime startDate)
    {
        int w, e, c;

        if(challengeScene.IsChallenge() == false) return;

        w = challengeScene.GetWorldIndex() - 1;
        e = challengeScene.GetEpisodeIndex() - 1;
        c = challengeScene.GetChallengeIndex() - 1;

        var curChallenge = _conceptMap.worlds[w].episodes[e].challenges[c];
        curChallenge.concept = newConcept;
        curChallenge.startDate = startDate.ToString();

        // atualiza a liberacao do proximo episodio
        if(e < _conceptMap.worlds[w].episodes.Length - 1)
        {
            if(_conceptMap.worlds[w].episodes[e + 1].liberationStatus == EpisodeLiberationTypes.BLOCK_BY_CONCEPT)
                _conceptMap.worlds[w].episodes[e + 1].liberationStatus = CheckNextEpisodeLiberationStatus(_conceptMap.worlds[w].episodes[e]);           
        }

#if UNITY_EDITOR
        if(loginInfo == null)
            return;
#endif

        SendConceptMapToSyncer();
    }

    public EpisodeLiberationTypes CheckNextEpisodeLiberationStatus(ConceptEpisode cEpisode)
    {
        if(cEpisode.challenges.All(c => c.concept == ConceptTypes.CONCEPT_GREEN))
            return EpisodeLiberationTypes.ALLOW_BY_CONCEPT;
        return EpisodeLiberationTypes.BLOCK_BY_CONCEPT;
    }

    public void SetLoginInfo(string l, string p, LoginInfo i)
	{
		this.login = l;
		this.pass = p;
		this.loginInfo = i;
		SincronizeConceptMapOnLogin ();
	}

	private void SincronizeConceptMapOnLogin()
	{
		PersistenceManager.Instance.LoadConceptMap (this, ref _conceptMap);
		if (_conceptMap == null)
			SetConceptMapAtFirstAccess ();
		SaveUserProfile ();
		SendConceptMapToSyncer ();
	}

	private void SendConceptMapToSyncer()
	{
		if (loginInfo.status.code == ConnectionResponse.OK)
			ConceptSyncer.Instance.SendConceptMapToServer (loginInfo.token, _conceptMap, ReceiveConceptMapFromSyncer);
		else
            loginInfo.status.code = ConnectionResponse.CONNECTION_OFFLINE;
	}

	private void ReceiveConceptMapFromSyncer(ConceptMap cm)
	{
		if (cm == null)
            loginInfo.status.code = ConnectionResponse.CONNECTION_OFFLINE;
		else
		{
            _conceptMap = cm;
			SaveUserProfile ();

            if(OnConceptMapUpdatedByRemoteEvent != null)
                OnConceptMapUpdatedByRemoteEvent();
        }
	}
		
	private void SaveUserProfile()
	{
		PersistenceManager.Instance.SaveUserProfile (this);
	}

	private void SetConceptMapAtFirstAccess()
	{
		_conceptMap = new ConceptMap();
		_conceptMap.worlds = new ConceptWorld[4];
		for (int w = 0; w < 4; w++)
		{
			_conceptMap.worlds [w] = new ConceptWorld ();
			_conceptMap.worlds[w].episodes = new ConceptEpisode[8];
			for (int e = 0; e < 8; e++)
			{
				_conceptMap.worlds [w].episodes [e] = new ConceptEpisode ();
				_conceptMap.worlds [w].episodes [e].challenges = new ConceptChallenge[5];
				for (int c = 0; c < 5; c++)
				{
					_conceptMap.worlds [w].episodes [e].challenges [c] = new ConceptChallenge ();
					_conceptMap.worlds [w].episodes [e].challenges [c].concept = ConceptTypes.CONCEPT_NOT_PLAYED;
					_conceptMap.worlds [w].episodes [e].challenges [c].startDate = null;
					_conceptMap.worlds [w].episodes [e].challenges [c].startDate = null;
				}
			}
		}
		for (int i = 0; i < 4; i++)
		{
			_conceptMap.worlds [i].episodes [0].liberationStatus = EpisodeLiberationTypes.ALLOW_BY_FIRST_ACCESS;
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
             role = "Student",
             status = new StatusInfo() {  code= ConnectionResponse.CONNECTION_OFFLINE},
        };
    }
#endif
}
