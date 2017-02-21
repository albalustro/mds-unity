using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using MDS.ScriptableObjects;
using MDS.Core.SceneManagement;
using MDS.Utilities;

public class LoginController : MDSBehaviour {

    #region Variáveis
	public static LoginController instance;
    
    [SerializeField] private InputField _userField;
	[SerializeField] private InputField _passField;
    [SerializeField] private Toggle _rememberUser;
    [SerializeField] private Toggle _rememberPass;
    
	[SerializeField] private AudioClip[] ambientSound;

    private bool _tryingLogin = false;
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

       
		AudioController.Instance.PlayTheme (ambientSound.GetRandom());
    }
    #endregion

    #region Ações dos botões
    public void Login()
	{
        if(_tryingLogin) return;
        
        if (_userField.text == "" || _passField.text == "")
            OpenFeedbackPanel("Favor digitar usuário e senha.");
        else
        {
			if (_rememberUser.isOn)
                PersistenceManager.Instance.SetString("rememberUser", _userField.text);
			if (_rememberPass.isOn)
                PersistenceManager.Instance.SetString("rememberPass", _passField.text);

            _tryingLogin = true;
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
        //SceneManager.LoadScene("Splash", LoadSceneMode.Single);
    }
    #endregion

	public void ReturnResponseLoginValidate(LoginInfo wsReturn)
	{

        FeedbackUI.Instance.Close();
		if (wsReturn == null) //Servidor nao respondeu, tentar efetuar o login offline
		{
            string pass = null;
			LoginInfo loginData = PersistenceManager.Instance.LoadLocalUserProfile (_userField.text, ref pass);
            if(loginData == null)
            {
                OpenFeedbackPanel("Falha ao realizar login.");
                _tryingLogin = false;
            }
            else
            {
                if(PersistenceManager.Instance.GetMD5Hash(_passField.text) == pass)
                {
                    loginData.status.code = ConnectionResponse.CONNECTION_OFFLINE;
                    loginData.status.message = "Offline";
                    OpenFeedbackPanel("Login offline");
                    UserProfile.Instance.SetLoginInfo(_userField.text, _passField.text, loginData);
                    SceneLoader.Instance.LoadRoomScene();
                }
                else
                {
                    OpenFeedbackPanel("Usuário ou senha inválidos.");
                    _tryingLogin = false;
                }
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
				SceneLoader.Instance.LoadRoomScene ();
				break;
			//Erro de usuário e/ou senha
			case ConnectionResponse.LOGIN_ERROR:
				OpenFeedbackPanel(loginInfo.status.message);
                    _tryingLogin = false;
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
        FeedbackUI.Instance.SetText(text).Show();
    }


    /// <summary>
    /// Exibe painel de Loading...
    /// </summary>
	public void OpenLoadingPanel()
    {
        FeedbackUI.Instance.SetText("Aguarde...").SetButtons(false, false, false, false).Show();
    }
    #endregion

}
