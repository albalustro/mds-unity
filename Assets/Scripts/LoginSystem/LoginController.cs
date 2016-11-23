using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LoginController : MonoBehaviour {

	private string _url;

    [Space(5)]
    [Header("Dados Usuário")]
    public LoginInfo loginInfo;

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

    [Space(5)]
    [Header("DEBUG")]
    [SerializeField] private GameObject _DEBUGPanel;

    void Start () {
		_url = "https://stage-xms.xmile.com.br/api/gamelogin";
        if (PlayerPrefs.HasKey("rememberUser"))
            _userField.text = PlayerPrefs.GetString("rememberUser");
        if (PlayerPrefs.HasKey("rememberPass"))
            _passField.text = PlayerPrefs.GetString("rememberPass");
    }

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

    private void OpenFeedbackPanel(string text)
    {
        _fbText.text = text;
        _fbButton.gameObject.SetActive(true);
        _feedBackPanel.SetActive(true);
    }

    public void CloseFeedbackPanel()
    {
        _fbText.text = "";
        _feedBackPanel.SetActive(false);
    }

    private void OpenLoadingPanel()
    {
        _fbText.text = "Carregando...";
        _fbButton.gameObject.SetActive(false);
        _feedBackPanel.SetActive(true);
    }

    IEnumerator ValidateLogin(WWW www)
	{
        OpenLoadingPanel();
		yield return www;
		if (www.error == null)
		{
			string wsReturn = www.text.Trim ();
			JsonUtility.FromJsonOverwrite (wsReturn, loginInfo);
            CloseFeedbackPanel();
            switch (loginInfo.status.code)
            {
                case 0:
                    CloseFeedbackPanel();
                    _DEBUGPanel.SetActive(true);
                    break;
                case 1:
                    OpenFeedbackPanel(loginInfo.status.message);
                    break;
            }
		} 
		else
		{
			Debug.LogError (www.error);
		}
	}
}
