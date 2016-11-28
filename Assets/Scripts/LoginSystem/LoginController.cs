using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;

public class LoginController : MDSBehaviour {

    #region Variáveis

    private string _url;

    [Space(5)]
    [Header("Campos")]
    [SerializeField] private InputField _userField;
	[SerializeField] private InputField _passField;
    [SerializeField] private Toggle _rememberUser;
    [SerializeField] private Toggle _rememberPass;

    [Space(5)]
    [Header("Feedback")]
    [SerializeField] private GameObject _feedBackPanel;
    [SerializeField] private Text _fbText;
    [SerializeField] private Button _fbButton;

    public UserProfile m_UserProfile;

    #endregion

    #region Métodos Unity

    protected override void Awake()
    {
        base.Awake();
        m_UserProfile = UserProfile.Instance;
    }

    void Start () {
		_url = "https://stage-xms.xmile.com.br/api/gamelogin2";
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

    /// <summary>
    /// Método chamado na operação de clique do botão Jogar.
    /// Essa função executa a validação do login.
    /// </summary>
    public void Login()
	{
        if (_userField.text == "" || _passField.text == "")
            OpenFeedbackPanel("Favor digitar usuário e senha.");
        else
        {
            WWWForm loginForm = new WWWForm();
            loginForm.AddField("login", _userField.text);
            loginForm.AddField("password", _passField.text);
            loginForm.AddField("game", "4");
            loginForm.AddField("season_id", "1");

            if (_rememberUser.isOn)
                PlayerPrefs.SetString("rememberUser", _userField.text);
            if (_rememberPass.isOn)
                PlayerPrefs.SetString("rememberPass", _passField.text);

            WWW www = new WWW(_url, loginForm);
            StartCoroutine(ValidateLogin(www));
        }
	}

    public void GuestLogin()
    {
        string guest = "{\"token\":\"Experimente\",\"api\":\"v1\",\"assets_url\":\"http://jogos.xmile.com.br/4/pt_br/\",\"assets_version\":\"1\",\"id\":99999,\"name\":\"Guest\",\"role\":\"Guest\"}";
        print(guest);
        PlayerPrefs.SetString("GuestLoginInfo", guest);
        ////////////FadeToWhite////////////////
        //Carregando próxima Scene
        m_UserProfile.loginInfo = JsonUtility.FromJson<LoginInfo>(guest);
        SceneManager.LoadScene("Splash", LoadSceneMode.Single);
    }

    #endregion

    #region Paineis de Feedback

    /// <summary>
    /// Exibe painel com texto informativo
    /// </summary>
    /// <param name="text">Texto a ser exibido no painel</param>
    private void OpenFeedbackPanel(string text)
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
    private void OpenLoadingPanel()
    {
        _fbText.text = "Carregando...";
        _fbButton.gameObject.SetActive(false);
        _feedBackPanel.SetActive(true);
    }

    #endregion

    #region Validações (Online e Offline)

    /// <summary>
    /// Validação da ação de login
    /// </summary>
    /// <param name="www">Objeto WWW contendo informações de URL e Form com dados de usuário e senha</param>
    /// <returns></returns>
    IEnumerator ValidateLogin(WWW www)
	{
        OpenLoadingPanel();
        //Tentando logar online
        yield return www;
		if (www.error == null)
		{
            //servidor respondeu
			string wsReturn = www.text.Trim ();
            m_UserProfile.loginInfo = JsonUtility.FromJson<LoginInfo>(wsReturn);
            CloseFeedbackPanel();
            switch (m_UserProfile.loginInfo.status.code)
            {
                //Login efetuado com sucesso
                case 0:
                    CloseFeedbackPanel();
                    //Criptografando a senha do usuário
                    string MD5Password = GetMD5Hash(_passField.text);
                    //Acrescentando a senha às informações do usuário e convertendo as infos para objeto
                    wsReturn = MD5Password + "|" + wsReturn;
                    //Criando chave no registro
                    PlayerPrefs.SetString(_userField.text, wsReturn);
                    ////////////FadeToWhite////////////////
                    //Carregando próxima Scene
                    SceneManager.LoadScene("Splash", LoadSceneMode.Single);
                    break;
                //Erro de usuário e/ou senha
                case 1:
                    OpenFeedbackPanel(m_UserProfile.loginInfo.status.message);
                    break;
            }
        } 
		else
		{
            CloseFeedbackPanel();
            //Servidor não respondeu
            Debug.LogError (www.error);
            //Verificando se possui dados offline
            if (PlayerPrefs.HasKey(_userField.text))
            {
                string[] userData = PlayerPrefs.GetString(_userField.text).Split('|');
                if (GetMD5Hash(_passField.text) == userData[0])
                {
                    //Realizando login offline
                    Debug.Log("Realizando login offline...");
                    m_UserProfile.loginInfo = JsonUtility.FromJson<LoginInfo>(userData[1]);
                    print(userData[1]);
                    CloseFeedbackPanel();
                    ////////////FadeToWhite////////////////
                    //Carregando próxima Scene
                    SceneManager.LoadScene("Splash", LoadSceneMode.Single);
                }
                else
                {
                    OpenFeedbackPanel("Usuário ou senha inválidos.");
                }
            }
            else
            {
                //Não conseguiu logar online e não possui informações de login offline anterior
                OpenFeedbackPanel("Falha ao realizar login.");
            }
		}
	}

    #endregion

    #region Segurança

    /// <summary>
    /// Método para gerar o MD5 de uma string
    /// </summary>
    /// <param name="text">Texto a ser gerado a Hash MD5</param>
    /// <returns>MD5 Hash do texto informado</returns>
    private string GetMD5Hash(string text)
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
}
