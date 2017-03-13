using System.Collections;
using System.Collections.Generic;
using MDS.Core.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoginFormUI : MonoBehaviour {

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

    void Start () {

        _feedbackUI = FeedbackUI.Instance;
        _persistenceManager = PersistenceManager.Instance;

        _userField.shouldHideMobileInput = true;
        _passField.shouldHideMobileInput = true;
        TouchScreenKeyboard.hideInput = true;

        if(_persistenceManager.HasKey("rememberUser"))
        {
            _userField.text = _persistenceManager.GetString("rememberUser");
            _rememberUser.isOn = true;
        }
        if(_persistenceManager.HasKey("rememberPass"))
        {
            _passField.text = _persistenceManager.GetString("rememberPass");
            _rememberPass.isOn = true;
        }

        
    }

    #region Ações dos botões

    public void Login()
    {
        if(_tryingLogin) return;

        if(_userField.text == "" || _passField.text == "")
            _feedbackUI.Show("Favor digitar usuário e senha.");
        else
        {
            if(_rememberUser.isOn)
                _persistenceManager.SetString("rememberUser", _userField.text);
            if(_rememberPass.isOn)
                _persistenceManager.SetString("rememberPass", _passField.text);

            _tryingLogin = true;
            _feedbackUI.SetText("Aguarde...").SetButtons(false, false, false, false).Show();

            ConnectionManager.Instance.DoLogin(_userField.text, _passField.text, DoLoginCallback);

        }
    }

    #endregion

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
        
        while(_curTouchScreenKeyboard.active)
            yield return null;

        _curTouchScreenKeyboard = null;

        LeanTween.moveY(_panel.GetComponent<RectTransform>(), 20, 0.3f);
#else
        yield return null;
#endif

    }





    private void DoLoginCallback(LoginInfo wsReturn)
    {

        _feedbackUI.Close();
        if(wsReturn == null) //Servidor nao respondeu, tentar efetuar o login offline
        {
            string pass = null;
            LoginInfo loginData = _persistenceManager.LoadLocalUserProfile(_userField.text, ref pass);
            if(loginData == null)
            {
                _feedbackUI.Show("Sem conexão com servidor.");
                _tryingLogin = false;
            }
            else
            {
                if(_persistenceManager.GetMD5Hash(_passField.text) == pass)
                {
                    loginData.status.code = ConnectionResponse.CONNECTION_OFFLINE;
                    loginData.status.message = "Offline";
                    _feedbackUI.SetText("Sem conexão com servidor. Efetuando login em modo offline.")
                                .SetButtons(true, false, false, false)
                                .SetOKFeedback(() =>
                                {
                                    UserProfile.Instance.SetLoginInfo(_userField.text, _passField.text, loginData);
                                    SceneLoader.Instance.LoadRoomScene();
                                })
                                .Show();
                    
                }
                else
                {
                    _feedbackUI.Show("Usuário ou senha inválidos.");
                    _tryingLogin = false;
                }
            }
        }
        else  //Servidor respondeu
        {
            LoginInfo loginInfo = wsReturn;
            switch(loginInfo.status.code)
            {
                //Login efetuado com sucesso
                case ConnectionResponse.OK:
                    //Enviando informações para o UserProfile
                    UserProfile.Instance.SetLoginInfo(_userField.text, _passField.text, loginInfo);
                    SceneLoader.Instance.LoadRoomScene();
                    break;
                //Erro de usuário e/ou senha
                case ConnectionResponse.LOGIN_ERROR:
                    _feedbackUI.Show("Ocorreu um erro durante o login: " + loginInfo.status.message);
                    _tryingLogin = false;
                    break;
            }
        }
    }

   
}
