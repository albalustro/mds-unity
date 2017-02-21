using UnityEngine;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using MDS.Utilities;
using UnityEngine.SceneManagement;

public class PersistenceManager : Singleton<PersistenceManager> {

    private string _gameIndexToComposePlayerPrefsKey
    {
        get
        {
            if (SceneManager.GetActiveScene().name.Equals("splash")==false)
                return  "_" + SceneManager.GetActiveScene().GetGameIndex().ToString();
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
        Log("Key: " + key);
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

	#region Segurança
	/// <summary>
	/// Método para gerar o MD5 de uma string
	/// </summary>
	/// <param name="text">Texto a ser gerado a Hash MD5</param>
	/// <returns>MD5 Hash do texto informado</returns>
	public string GetMD5Hash(string text)
	{
		MD5 md5Hash = MD5.Create();
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
