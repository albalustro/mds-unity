using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using MDS.ScriptableObjects;

public class LoginController : MDSBehaviour {

    #region Variáveis
	public static LoginController instance;
<<<<<<< HEAD
	private int currentGameIndex;
=======

    [SerializeField]
    private ConnectionConfig _connectionConfiguration;

>>>>>>> master
    [SerializeField] private InputField _userField;
	[SerializeField] private InputField _passField;
    [SerializeField] private Toggle _rememberUser;
    [SerializeField] private Toggle _rememberPass;
    [SerializeField] private GameObject _feedBackPanel;
    [SerializeField] private Text _fbText;
    [SerializeField] private Button _fbButton;
    #endregion

    #region Métodos Unity
	protected override void Awake()
	{
		base.Awake();	
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	}

    void Start () {

        ConnectionManager.Instance.Initialize(_connectionConfiguration);
        ConceptSyncer.Instance.Initialize(_connectionConfiguration);

        if (PersistenceManager.Instance.HasKey("rememberUser"))
            _userField.text = PersistenceManager.Instance.GetString("rememberUser");
        if (PersistenceManager.Instance.HasKey("rememberPass"))
        {
            _passField.text = PersistenceManager.Instance.GetString("rememberPass");
            _rememberPass.isOn = true;
        }

		//currentGameIndex = PersistenceManager.Instance.GetGameIndex ();
    }
    #endregion

    #region Ações dos botões
    public void Login()
	{
        if (_userField.text == "" || _passField.text == "")
            OpenFeedbackPanel("Favor digitar usuário e senha.");
        else
        {
			if (_rememberUser.isOn)
                PersistenceManager.Instance.SetString("rememberUser", _userField.text);
			if (_rememberPass.isOn)
                PersistenceManager.Instance.SetString("rememberPass", _passField.text);
			ConnectionManager.Instance.DoLogin(_userField.text, _passField.text, ReturnResponseLoginValidate);
			OpenLoadingPanel();
        }
	}

    public void GuestLogin()
    {
        string guest = "{\"token\":\"Experimente\",\"api\":\"v1\",\"assets_url\":\"http://jogos.xmile.com.br/4/pt_br/\",\"assets_version\":\"1\",\"id\":99999,\"name\":\"Guest\",\"role\":\"Guest\"}";
        PersistenceManager.Instance.SetString("GuestLoginInfo", guest);
		UserProfile.Instance.loginInfo = JsonConvert.DeserializeObject<LoginInfo>(guest);
		////////////FadeToWhite////////////////
		//Carregando próxima Scene
        SceneManager.LoadScene("Splash", LoadSceneMode.Single);
    }

    #endregion

	public void ReturnResponseLoginValidate(LoginInfo wsReturn)
	{
		CloseFeedbackPanel ();
		if (wsReturn == null) //Servidor nao respondeu, tentar efetuar o login offline
		{
            string pass = null;
			LoginInfo loginData = PersistenceManager.Instance.LoadLocalUserProfile (_userField.text, ref pass);
			if (loginData == null)
				OpenFeedbackPanel("Falha ao realizar login.");
			else
			{
				if (PersistenceManager.Instance.GetMD5Hash(_passField.text) == pass)
				{
					loginData.status.code = ConnectionResponse.CONNECTION_OFFLINE;
                    loginData.status.message = "Offline";
					UserProfile.Instance.SetLoginInfo (_userField.text, _passField.text, loginData);
				}
				else
					OpenFeedbackPanel("Usuário ou senha inválidos.");
			}
		} 
		else  //Servidor respondeu
		{
			LoginInfo loginInfo = wsReturn;
			switch (loginInfo.status.code)
			{
			//Login efetuado com sucesso
			case ConnectionResponse.OK:
                //Enviando informações para o UserProfile
				UserProfile.Instance.SetLoginInfo(_userField.text, _passField.text, loginInfo);
				break;
			//Erro de usuário e/ou senha
			case ConnectionResponse.LOGIN_ERROR:
				OpenFeedbackPanel(loginInfo.status.message);
				break;
			}
		}

		//string roomToLoad = "G" + currentGameIndex + "Room";
		//SceneManager.LoadScene (roomToLoad, LoadSceneMode.Single);
	}

    #region Paineis de Feedback

    /// <summary>
    /// Exibe painel com texto informativo
    /// </summary>
    /// <param name="text">Texto a ser exibido no painel</param>
    public void OpenFeedbackPanel(string text)
    {
        _fbText.text = text;
        _fbButton.gameObject.SetActive(true);
        _feedBackPanel.SetActive(true);
    }

    /// <summary>
    /// Fecha painel informativo
    /// </summary>
    public void CloseFeedbackPanel()
    {
        _fbText.text = "";
        _feedBackPanel.SetActive(false);
    }

    /// <summary>
    /// Exibe painel de Loading...
    /// </summary>
	public void OpenLoadingPanel()
    {
        _fbText.text = "Carregando...";
        _fbButton.gameObject.SetActive(false);
        _feedBackPanel.SetActive(true);
    }

    #endregion

}
