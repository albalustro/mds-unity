[System.Serializable]
public class UserProfile : Singleton<UserProfile>
{
	public string login;
	public string pass;
    public LoginInfo loginInfo;
	public ConceptMap conceptMap;

	//code: t000m000e000d000

	public void SetLoginInfo(string l, string p, LoginInfo i)
	{
		this.login = l;
		this.pass = p;
		this.loginInfo = i;
		SincronizeConceptMapOnLogin ();
	}

	private void SincronizeConceptMapOnLogin()
	{
		PersistenceManager.Instance.LoadConceptMap (this, ref conceptMap);
		if (conceptMap == null)
			SetConceptMapAtFirstAccess ();
		SaveUserProfile ();
		SendConceptMapToSyncer (conceptMap);
	}

	private void SendConceptMapToSyncer(ConceptMap conceptToSend)
	{
		if (loginInfo.status.code == ConnectionResponse.OK)
			ConceptSyncer.Instance.SendConceptMapToServer (loginInfo.token, conceptToSend, ReceiveConceptMapFromSyncer);
		else
            loginInfo.status.code = ConnectionResponse.CONNECTION_OFFLINE;
	}

	private void ReceiveConceptMapFromSyncer(ConceptMap cm)
	{
		if (cm == null)
            loginInfo.status.code = ConnectionResponse.CONNECTION_OFFLINE;
		else
		{
            conceptMap = cm;
			SaveUserProfile ();
		}
	}
		
	private void SaveUserProfile()
	{
		PersistenceManager.Instance.SaveUserProfile (this);
	}

	private void SetConceptMapAtFirstAccess()
	{
		conceptMap = new ConceptMap();
		conceptMap.worlds = new ConceptWorld[4];
		for (int w = 0; w < 4; w++)
		{
			conceptMap.worlds [w] = new ConceptWorld ();
			conceptMap.worlds[w].episodes = new ConceptEpisode[8];
			for (int e = 0; e < 8; e++)
			{
				conceptMap.worlds [w].episodes [e] = new ConceptEpisode ();
				conceptMap.worlds [w].episodes [e].challenges = new ConceptChallenge[5];
				for (int c = 0; c < 5; c++)
				{
					conceptMap.worlds [w].episodes [e].challenges [c] = new ConceptChallenge ();
					conceptMap.worlds [w].episodes [e].challenges [c].concept = ConceptTypes.CONCEPT_NOT_PLAYED;
					conceptMap.worlds [w].episodes [e].challenges [c].startDate = null;
					conceptMap.worlds [w].episodes [e].challenges [c].startDate = null;
				}
			}
		}
		for (int i = 0; i < 4; i++)
		{
			conceptMap.worlds [i].episodes [0].liberationStatus = EpisodeLiberationTypes.ALLOW_BY_FIRST_ACCESS;
		}
	}
}
