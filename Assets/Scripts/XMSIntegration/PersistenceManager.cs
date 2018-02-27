using UnityEngine;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using MDS.Utilities;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PersistenceManager : Singleton<PersistenceManager> {


    private string _gameIndexToComposePlayerPrefsKey
    {
        get
        {
            Scene current = SceneManager.GetActiveScene(); // + garbage collection - requests
            if (current.name.Equals("splash")==false)
                return  "_" + current.GetGameIndex().ToString();
            return "__";
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if(PersistenceManager.Instance != this)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);

    }

    public LoginInfo LoadLocalUserProfile(string user, ref string pass)
	{
        string key = user + _gameIndexToComposePlayerPrefsKey;
       // Log("Key: " + key);
        if(!HasKey(key))
        {
            return null;
        }
        AuxClass aux = JsonConvert.DeserializeObject<AuxClass>(PlayerPrefs.GetString(key));
        LoginInfo li = aux.loginInfo;
        pass = aux.pass;
        return li;
	}

	public void LoadConceptMap(UserProfile profile, ref ConceptMap cm)
	{
        string key = profile.login + _gameIndexToComposePlayerPrefsKey;
        if (!PlayerPrefs.HasKey(key))
            cm = null;
        else
        {
            AuxClass aux = JsonConvert.DeserializeObject<AuxClass>
                (PlayerPrefs.GetString(key));
            cm = aux.conceptMap;
        }
	}

	public void SaveUserProfile(UserProfile profile)
	{
        AuxClass aux = new AuxClass();
        aux.login  = profile.login;
        aux.pass  = GetMD5Hash(profile.pass);
        aux.loginInfo = profile.loginInfo;
        aux.conceptMap = profile.conceptMap;
        string key = profile.login + _gameIndexToComposePlayerPrefsKey;
        PlayerPrefs.SetString (key, JsonConvert.SerializeObject (aux));
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    #region PLAYMOVE
    //PlayMove Stuff
#if MDS1
    private const string playmoveKey = "PlayMoveRegistredNamesMDS1";
#else
#if MDS2
    private const string playmoveKey = "PlayMoveRegistredNamesMDS2";
#else
#if MDS3
    private const string playmoveKey = "PlayMoveRegistredNamesMDS3";
#endif
#endif
#endif

    public List<PlayMoveUserData> GetPlayMoveData
    {
        get
        {
            List<PlayMoveUserData> list = JsonConvert.DeserializeObject<List<PlayMoveUserData>>(PlayerPrefs.GetString(playmoveKey));
            if (list == null)
            {
                Debug.Log("Created Empty Profile");
                ResetPlayMoveData();
                list = JsonConvert.DeserializeObject<List<PlayMoveUserData>>(PlayerPrefs.GetString(playmoveKey));
            }
            
            return list;
        }
    }
    public List<string> AllPlayersName
    {
        get
        {
            List<PlayMoveUserData> playmoveData = GetPlayMoveData;
            List<string> names = new List<string>();
            foreach (var item in playmoveData)
            {
                names.Add(item.name);
            }
            return names;
        }
    }

    public void RefreshPlayerData(UserProfile userToRefresh)
    {
        bool done = false;
        string nome = userToRefresh.login;
        List<PlayMoveUserData> playmoveData = GetPlayMoveData;
        foreach (var item in playmoveData)
        {
            if (item.name == nome)
            {
                item.conceptMap = userToRefresh.conceptMap;
                item.loginInfo = userToRefresh.loginInfo;
                item.score = userToRefresh.score;
                done = true;
            }
        }

        if (done)
        {
            PlayerPrefs.SetString(playmoveKey, JsonConvert.SerializeObject(playmoveData));
            PlayerPrefs.Save();
        }
        else
        {
            SaveNewPlayMovePlayer(userToRefresh);
        }
    }

    public LoginInfo GetPlayMoveLoginInfo(string login)
    {
        List<PlayMoveUserData> playmoveData = GetPlayMoveData;
        foreach (var item in playmoveData)
        {
            if(item.name == login)
            {
                return item.loginInfo;
            }
        }
        return null;
    }

    /// <summary>
    /// Tries to save a new user. Returns 'true' if new is name, after adding it to the save
    /// </summary>
    public bool SaveNewPlayMovePlayer(UserProfile newUser)
    {
        List<PlayMoveUserData> data = GetPlayMoveData;
        if (data != null) {
            foreach (var item in data)
            {
                if (item.name == newUser.name)
                {
                    return false;
                }
            }
        }
        else
        {
            ResetPlayMoveData();
        }
        PlayMoveUserData pmud = new PlayMoveUserData
        {
            name = newUser.login,
            loginInfo = newUser.loginInfo,
            conceptMap = newUser.conceptMap,
            score = newUser.score
        };
        data.Add(pmud);
        PlayerPrefs.SetString(playmoveKey, JsonConvert.SerializeObject(data));
        PlayerPrefs.Save();
        if(data.Count > 1)
        {
            RemovePlayMovePlayer("");
        }
        return true;
    }

    public void RemovePlayMovePlayer(string nameToRemove)
    {
        Debug.Log("Removing: " + nameToRemove);
        List<PlayMoveUserData> data = GetPlayMoveData;
        if (data.Count > 1) {
            foreach (var item in data)
            {
                if (item.name == nameToRemove)
                {
                    data.Remove(item);
                    break;
                }
            }
            //Debug.Log(data);
            PlayerPrefs.SetString(playmoveKey, JsonConvert.SerializeObject(data));
            PlayerPrefs.Save();
 
        }
        else
        {
            if(nameToRemove != "")
            {
                ResetPlayMoveData();
            }
        }
    }

    public void ResetPlayMoveData()
    {
        Debug.Log("Reseting Play Move Data");
        List<PlayMoveUserData> data = new List<PlayMoveUserData>();
        PlayMoveUserData pmud = new PlayMoveUserData
        {
            name = "",
            loginInfo = null,
            conceptMap = null,
            score = 0
        };
        data.Add(pmud);
        PlayerPrefs.SetString(playmoveKey, JsonConvert.SerializeObject(data));
        PlayerPrefs.Save();
    }
#endregion

#region Segurança
    /// <summary>
    /// Método para gerar o MD5 de uma string
    /// </summary>
    /// <param name="text">Texto a ser gerado a Hash MD5</param>
    /// <returns>MD5 Hash do texto informado</returns>
    public string GetMD5Hash(string text)
	{
        // TROCA NECESSARIA POR CONTA DO ANDROID com stripping code.. :)
        //MD5 md5Hash = MD5.Create();
        var md5Hash = new MD5CryptoServiceProvider();

        // Converter a String para array de bytes
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(text));
		// Cria-se um StringBuilder para recompôr a string.
		StringBuilder sBuilder = new StringBuilder();
		// Loop para formatar cada byte como uma String em hexadecimal
		for (int i = 0; i < data.Length; i++)
			sBuilder.Append(data[i].ToString("x2"));
		return sBuilder.ToString();
	}
#endregion

    public class AuxClass
    {
        public AuxClass()
        {

        }
        public string login;
        public string pass;
        public LoginInfo loginInfo;
        public ConceptMap conceptMap;
    }
}

[System.Serializable]
public class PlayMoveUserData
{
    public string name;
    public LoginInfo loginInfo;
    public ConceptMap conceptMap;
    public int score;
}
