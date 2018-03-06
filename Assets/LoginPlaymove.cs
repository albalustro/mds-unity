using MDS.Core.SceneManagement;
using MDS.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Playmove;

public class LoginPlaymove : MonoBehaviour
{
    private int pageIndex = 1;
    private bool _tryingLogin;
    private FeedbackUI _feedbackUI;
    private PersistenceManager _persistenceManager;
    private List<string> allNames = new List<string>();
    private List<string> inSceneNames = new List<string>();
    [SerializeField] private Toggle _registerScore;
    [SerializeField] private GameObject loginCanvas;
    [SerializeField] private GameObject registerCanvas;
    [SerializeField] private Text[] buttonTextList;
    [SerializeField] private InputField nameField;
    public GameObject keyboard;
    public GraphicRaycaster[] canvasCaster;
    [SerializeField] private PlayTableKeyboard.KeyboardTextEvent confirmEvent = new PlayTableKeyboard.KeyboardTextEvent();
    [SerializeField] private PlayTableKeyboard.KeyboardEvent cancelEvent = new PlayTableKeyboard.KeyboardEvent();

    void Start () {
        _feedbackUI = FeedbackUI.Instance;
        _persistenceManager = PersistenceManager.Instance;

#if !PLAY_MOVE
        gameObject.SetActive(false);
#endif
    }

    public void SwipePageIndex(bool nextPage)
    {
        if (nextPage)
        {
            pageIndex++;
        }
        else
        {
            pageIndex--;
        }
        allNames = _persistenceManager.AllPlayersName;

        if (allNames.Count > 0)
        {
            pageIndex = Mathf.Clamp(pageIndex, 1, Mathf.CeilToInt(allNames.Count / 10) + 1);
        }
        SwitchRegisterCanvas(true);
    }
    public void SwitchRegisterCanvas(bool showRegister)
    {
        if (showRegister)
        {
            foreach (var item in buttonTextList)
            {
                item.text = "---";
            }
            allNames = _persistenceManager.AllPlayersName;
            var aux = allNames.Page(pageIndex, 10);
            inSceneNames.Clear();
            int i = 0;
            string name;
            foreach (var item in aux)
            {
                name = item.ToString();
                if (name != "")
                {
                    inSceneNames.Add(name);
                    buttonTextList[i].text = name;
                    i++;
                }
            }
        }
        loginCanvas.SetActive(!showRegister);
        registerCanvas.SetActive(showRegister);
        
    }

    public void Login()
    {
        if (_tryingLogin) return;

        if (_registerScore.isOn)
        {
            //_persistenceManager.SetInt("RegisterScore", 1);
            SwitchRegisterCanvas(true);
            
            //Abrir tela para registro do jogador
            //Essa partida irá armazenar pontuação para o ranking
        }
        //Entra no jogo com todas as fases liberadas e não contabilizará pontos
        else
        {
            _tryingLogin = true;
            _feedbackUI.SetText("Aguarde...").SetButtons(false, false, false, false).Show();

            _persistenceManager.SetInt("RegisterScore", 0);
            LoginInfo infoGuest = new LoginInfo();
            infoGuest = null;
            DoLoginCallback(infoGuest);
        }
    }

    public void LoginWithNewName()
    {
        if (PlayTableKeyboard.Instance.BannedName())
        {
            _feedbackUI.SetText("Nome impróprio, tente outro nome!").SetButtons(true, false, false, false).SetOKFeedback(() =>
             {
                 _feedbackUI.Close();
                 _feedbackUI.CloseHandler();
                 PlayTableKeyboard.Instance.ClearText();
             }).Show();
        }
        else
        {
            string newName = PlayTableKeyboard.Instance.Text;
            if (newName != "")
            {
                allNames = _persistenceManager.AllPlayersName;
                foreach (var item in allNames)
                {
                    if (item == newName)
                    {
                        _feedbackUI.SetText("O nome '" + newName + "' já existe, escolha outro, por favor!").SetButtons(false, true, false, false).Show();
                        return;
                    }
                }

                _tryingLogin = true;
                _feedbackUI.SetText("Aguarde...").SetButtons(false, false, false, false).Show();

                _persistenceManager.SetInt("RegisterScore", 1);
                LoginInfo infoGuest = new LoginInfo()
                {
                    name = newName
                };

                DoLoginCallback(infoGuest);

            }
            else
            {
                _feedbackUI.SetText("Digite um nome valido, por favor!").SetButtons(false, true, false, false).Show();
                return;
            }

            ShowKeyboard(false);
        }
    }
    public void ShowKeyboard(bool value)
    {
        keyboard.SetActive(value);
        foreach (var item in canvasCaster)
        {
            item.enabled = !value;
        }
        if (value)
        {
            PlayTableKeyboard.Instance.onConfirm = confirmEvent;
            PlayTableKeyboard.Instance.onCancel = cancelEvent;
        }
        else
        {
            PlayTableKeyboard.Instance.ClearText();
        }

    }

    public void LoginWithRegistredName(int index)
    {
        string nome = buttonTextList[index].text;
        if (nome != "---")
        {
            _tryingLogin = true;
            _feedbackUI.SetText("Aguarde...").SetButtons(false, false, false, false).Show();
            _persistenceManager.SetInt("RegisterScore", 1);
            LoginInfo info = _persistenceManager.GetPlayMoveLoginInfo(nome);
            info.name = nome;
            DoLoginCallback(info);
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
            Debug.Log("Playmove Login");
            UserProfile.Instance.SetPlayMoveLoginInfo("Guest", "", loginData);
#endif
            SceneLoader.Instance.LoadRoomScene();
        }
        else
        {
            wsReturn.status = new StatusInfo();
            wsReturn.status.code = ConnectionResponse.CONNECTION_OFFLINE;
            wsReturn.role = "RegisteredPlayMoveUser";
            wsReturn.status.message = "Playmove com registro";

            UserProfile.Instance.SetPlayMoveLoginInfo(wsReturn.name, "", wsReturn);
            SceneLoader.Instance.LoadRoomScene();
        }
        dic.Add("Conn", "offline");
        dic.Add("login", "yes - offline");
        dic.Add("dt", DateTime.Now.ToString());
        Analytics.CustomEvent("GameLogin", dic);
        Analytics.FlushEvents();

    }
}
