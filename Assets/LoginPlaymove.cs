using MDS.Core.SceneManagement;
using MDS.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPlaymove : MonoBehaviour
{
    private bool _tryingLogin;
    private FeedbackUI _feedbackUI;
    private PersistenceManager _persistenceManager;
    [SerializeField] private Toggle _registerScore;

    void Start () {
        _feedbackUI = FeedbackUI.Instance;
        _persistenceManager = PersistenceManager.Instance;

#if !PLAY_MOVE
        gameObject.SetActive(false);
#endif
    }

    public void Login()
    {
        if (_tryingLogin) return;

        _tryingLogin = true;
        _feedbackUI.SetText("Aguarde...").SetButtons(false, false, false, false).Show();

        if (_registerScore.isOn)
        {
            _persistenceManager.SetInt("RegisterScore", 1);
            //Abrir tela para registro do jogador
            //Essa partida irá armazenar pontuação para o ranking
        }
        //Entra no jogo com todas as fases liberadas e não contabilizará pontos
        else
        {
            _persistenceManager.SetInt("RegisterScore", 0);
            LoginInfo infoGuest = new LoginInfo();
            infoGuest = null;
            DoLoginCallback(infoGuest);
        }
    }

    private void DoLoginCallback(LoginInfo wsReturn)
    {
        Scene curScene = SceneManager.GetActiveScene();
        var dic = new Dictionary<string, object>();
        dic.Add("Game", "MDS" + curScene.GetGameIndex().ToString());
        dic.Add("Plat", "Playmove");
        dic.Add("User", "PMUser");

        _feedbackUI.Close();

        //Logando como Guest (sem registro de usuário e sem contagem de pontos para o placar
        if (wsReturn == null) 
        {
            dic.Add("Conn", "offline");
            LoginInfo loginData = new LoginInfo();
            loginData.status = new StatusInfo();
            loginData.status.code = ConnectionResponse.CONNECTION_OFFLINE;
            loginData.role = "NotRegisteredPlayMoveUser";
            loginData.status.message = "Playmove sem registro";
            //_feedbackUI.SetText("Efetuando login sem registro")
            //            .SetButtons(true, false, false, false)
            //            .SetOKFeedback(() =>
            //            {
            //                UserProfile.Instance.SetPlayMoveLoginInfo("Guest", "", loginData);
            //                SceneLoader.Instance.LoadRoomScene();
            //            })
            //            .Show();
#if PLAY_MOVE
            UserProfile.Instance.SetPlayMoveLoginInfo("Guest", "", loginData);
#endif
            SceneLoader.Instance.LoadRoomScene();
            dic.Add("login", "yes - offline");
        }
        else
        {
            //dic.Add("Conn", "online");

            //LoginInfo loginInfo = wsReturn;
            //switch (loginInfo.status.code)
            //{
            //    //Login efetuado com sucesso
            //    case ConnectionResponse.OK:
            //        //Enviando informações para o UserProfile
            //        dic.Add("login", "yes - online");

            //        UserProfile.Instance.SetLoginInfo(_userField.text, _passField.text, loginInfo);
            //        SceneLoader.Instance.LoadRoomScene();
            //        break;
            //    //Erro de usuário e/ou senha
            //    case ConnectionResponse.LOGIN_ERROR:
            //        dic.Add("login", "no - user/pass invalid");

            //        _feedbackUI.Show("Ocorreu um erro durante o login: " + loginInfo.status.message);
            //        _tryingLogin = false;
            //        break;
            //}
        }

        dic.Add("dt", DateTime.Now.ToString());

        Analytics.CustomEvent("GameLogin", dic);
        Analytics.FlushEvents();

    }
}
