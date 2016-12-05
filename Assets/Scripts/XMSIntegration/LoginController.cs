using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class LoginController : MDSBehaviour {

    #region Variáveis
	public static LoginController instance;
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
		//DEVO COLOCAR ISSO NO PERSISTENCE???
        if (PlayerPrefs.HasKey("rememberUser"))
            _userField.text = PlayerPrefs.GetString("rememberUser");
        if (PlayerPrefs.HasKey("rememberPass"))
        {
            _passField.text = PlayerPrefs.GetString("rememberPass");
            _rememberPass.isOn = true;
        }
    }
    #endregion

    #region Ações dos botões
    public void Login()
	{
        if (_userField.text == "" || _passField.text == "")
            OpenFeedbackPanel("Favor digitar usuário e senha.");
        else
        {
			//DEVO COLOCAR ISSO NO PERSISTENCE???
			if (_rememberUser.isOn)
				PlayerPrefs.SetString("rememberUser", _userField.text);
			if (_rememberPass.isOn)
				PlayerPrefs.SetString("rememberPass", _passField.text);
			ConnectionManager.Instance.DoLogin(_userField.text, _passField.text, ReturnResponseLoginValidate);
			OpenLoadingPanel();
        }
	}

    public void GuestLogin()
    {
        string guest = "{\"token\":\"Experimente\",\"api\":\"v1\",\"assets_url\":\"http://jogos.xmile.com.br/4/pt_br/\",\"assets_version\":\"1\",\"id\":99999,\"name\":\"Guest\",\"role\":\"Guest\"}";
		//PRECISO GUARDAR ESSA CHAVE???
		PlayerPrefs.SetString("GuestLoginInfo", guest);
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
			UserProfile userData = PersistenceManager.Instance.LoadLocalLoginInfo (_userField.text);
			if (userData == null)
				OpenFeedbackPanel("Falha ao realizar login.");
			else
			{
				if (PersistenceManager.Instance.GetMD5Hash(_passField.text) == userData.pass)
				{
					LoginInfo loginInfo = userData.loginInfo;
					loginInfo.status.code = ConnectionResponse.CONNECTION_OFFLINE;
					loginInfo.status.message = "Offline";
					UserProfile.Instance.SetLoginInfo (_userField.text, _passField.text, loginInfo);
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
