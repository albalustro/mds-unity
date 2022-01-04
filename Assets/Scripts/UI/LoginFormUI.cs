using System;
using System.Collections;
using System.Collections.Generic;
using MDS.Core.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using MDS.Utilities;
using Newtonsoft.Json.Utilities;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class LoginFormUI : MonoBehaviour
{

    [SerializeField]
    private InputField _userField;
    [SerializeField]
    private InputField _passField;
    [SerializeField]
    private Toggle _rememberUser;
    [SerializeField]
    private Toggle _rememberPass;

    [SerializeField]
    private GameObject _panel;

    private bool _tryingLogin;

    private FeedbackUI _feedbackUI;
    private PersistenceManager _persistenceManager;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void InjectData();
#endif

    void Start()
    {

        _feedbackUI = FeedbackUI.Instance;
        _persistenceManager = PersistenceManager.Instance;

        _userField.shouldHideMobileInput = true;
        _passField.shouldHideMobileInput = true;
        TouchScreenKeyboard.hideInput = true;

        if (_persistenceManager.HasKey("rememberUser"))
        {
            _userField.text = _persistenceManager.GetString("rememberUser");
            _rememberUser.isOn = true;
        }
        if (_persistenceManager.HasKey("rememberPass"))
        {
            _passField.text = _persistenceManager.GetString("rememberPass");
            _rememberPass.isOn = true;
        }


#if UNITY_WEBGL && ! UNITY_EDITOR
        InjectData();
       // Application.ExternalCall("InjectData");
        //_panel.SetActive(false);
#endif

    }


#if UNITY_WEBGL
    public void ReceiveLogin(string loginData)
    {
        var data = loginData.Split(':');
        _userField.text = data[0];
        _passField.text = data[1];
        Login();
    }
#endif

#region Ações dos botões

    public async void Login()
    {
        if (_tryingLogin) return;


        if (_userField.text == "" || _passField.text == "")
            _feedbackUI.Show("Favor digitar usuário e senha.");
        else
        {
            if (_rememberUser.isOn)
                _persistenceManager.SetString("rememberUser", _userField.text);
            if (_rememberPass.isOn)
                _persistenceManager.SetString("rememberPass", _passField.text);

            _tryingLogin = true;
            _feedbackUI.SetText("Aguarde...").SetButtons(false, false, false, false).Show();

            var loginInfo = await NetworkManager.Instance.DoLogin(_userField.text, _passField.text);
            DoLoginCallback(loginInfo);
        }

    }

    #endregion

    private void DoLoginCallback(LoginInfo wsReturn)
    {
        Scene curScene = SceneManager.GetActiveScene();
        var dic = new Dictionary<string, object>();
        dic.Add("Game", "MDS" + curScene.GetGameIndex().ToString());
        dic.Add("Plat", Application.platform.ToString());
        dic.Add("User", _userField.text);

        _feedbackUI.Close();
        if (wsReturn == null) //Servidor nao respondeu, tentar efetuar o login offline
        {
            dic.Add("Conn", "offline");

            string pass = null;
            LoginInfo loginData = _persistenceManager.LoadLocalUserProfile(_userField.text, ref pass);
            if (loginData == null)
            {
                _feedbackUI.Show("Sem conexão com servidor.");
                _tryingLogin = false;
                dic.Add("login", "no - no local data");
            }
            else
            {
                if (_persistenceManager.GetMD5Hash(_passField.text) == pass)
                {
                    loginData.Status.code = ConnectionResponse.CONNECTION_OFFLINE;
                    loginData.Status.message = "Offline";
                    _feedbackUI.SetText("Sem conexão com servidor. Efetuando login em modo offline.")
                                .SetButtons(true, false, false, false)
                                .SetOKFeedback(() =>
                                {
                                    UserProfile.Instance.SetLoginInfo(_userField.text, _passField.text, loginData);
                                    SceneLoader.Instance.LoadRoomScene();
                                })
                                .Show();
                    dic.Add("login", "yes - offline");
                }
                else
                {
                    _feedbackUI.Show("Usuário ou senha inválidos.");
                    _tryingLogin = false;
                    dic.Add("login", "no - user/pass invalid");
                }
            }
        }
        else  //Servidor respondeu
        {
            dic.Add("Conn", "online");

            LoginInfo loginInfo = wsReturn;
            switch (loginInfo.Status.code)
            {
                //Login efetuado com sucesso
                case ConnectionResponse.OK:
                    //Enviando informações para o UserProfile
                    dic.Add("login", "yes - online");

                    UserProfile.Instance.SetLoginInfo(_userField.text, _passField.text, loginInfo);
                    SceneLoader.Instance.LoadRoomScene();
                    break;
                //Erro de usuário e/ou senha
                case ConnectionResponse.LOGIN_ERROR:
                    dic.Add("login", "no - user/pass invalid");

                    _feedbackUI.Show("Ocorreu um erro durante o login: " + loginInfo.Status.message);
                    _tryingLogin = false;
                    break;
            }
        }

        dic.Add("dt", DateTime.Now.ToString());

        Analytics.CustomEvent("GameLogin", dic);
        Analytics.FlushEvents();

    }


    #region Touchscreenkeyboard

    TouchScreenKeyboard _curTouchScreenKeyboard;
    public void OpenTKB_Login()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        LeanTween.moveY(_panel.GetComponent<RectTransform>(), 620, 0.3f);

        TouchScreenKeyboard.hideInput = true;
        _curTouchScreenKeyboard = TouchScreenKeyboard.Open(_userField.text, 
                                    TouchScreenKeyboardType.Default, 
                                    false, false, false, false);
        
        StopCoroutine(CloseTouchKB());
        StartCoroutine(CloseTouchKB());
#endif
    }

    public void OpenTKB_Pass()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        LeanTween.moveY(_panel.GetComponent<RectTransform>(), 620, 0.3f);

        TouchScreenKeyboard.hideInput = true;
        _curTouchScreenKeyboard = TouchScreenKeyboard.Open("",
                                    TouchScreenKeyboardType.Default,
                                    false, false, true, false);

        StopCoroutine(CloseTouchKB());
        StartCoroutine(CloseTouchKB());
#endif
    }

    public void CloseTouchScreenKeyboard()
    {
        _curTouchScreenKeyboard.active = false;
        StopCoroutine(CloseTouchKB());
        StartCoroutine(CloseTouchKB());
    }

    private IEnumerator CloseTouchKB()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        while (_curTouchScreenKeyboard != null && _curTouchScreenKeyboard.active)
            yield return null;

        _curTouchScreenKeyboard = null;

        if (_panel != null)
        {
            RectTransform r = _panel.GetComponent<RectTransform>();
            if (r!=null)
                LeanTween.moveY(r, 20, 0.3f);
        }
#else
        yield return null;
#endif

    }

	#endregion



	


}
