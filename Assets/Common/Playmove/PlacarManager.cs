using MDS.Utilities;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Playmove;

public class PlacarManager : MonoBehaviour {
    PersistenceManager persintence;
    List<PlayMoveUserData> userData, orderedData, showingNames;
    int pageIndex = 1;
    int? selectedIndex;
    [SerializeField] private Text[] names, points, indexes;
    private Outline[] highlights;
    [SerializeField] private LoginPlaymove login;
    [SerializeField] private Button[] keyButtons;
    [SerializeField] private PlayTableKeyboard.KeyboardTextEvent confirmEvent = new PlayTableKeyboard.KeyboardTextEvent();
    [SerializeField] private PlayTableKeyboard.KeyboardEvent cancelEvent = new PlayTableKeyboard.KeyboardEvent();

    public void ShowKeyboard(bool value)
    {
         
        login.keyboard.SetActive(value);
        foreach (var item in login.canvasCaster)
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


    private void Awake()
    {
        persintence = PersistenceManager.Instance;
        highlights = new Outline[names.Length];
        int i = 0;
        foreach (var item in names)
        {
            highlights[i] = item.GetComponent<Outline>();
            i++;
        }
    }

    private void OnEnable()
    {
        ShowNamesAndPoints();
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
        userData = persintence.GetPlayMoveData;
        int cnt = userData.Count;

        if (cnt > 0)
        {
            pageIndex = Mathf.Clamp(pageIndex, 1, Mathf.CeilToInt(cnt / 10)+1);
        }
        ShowNamesAndPoints();
    }

    public void ArrageData()
    {
        userData = persintence.GetPlayMoveData;

        orderedData = userData.OrderByDescending(x => x.score).ToList();
    }
    public void ResetPlacar()
    {
        foreach (var item in names)
        {
            item.text = "---";
        }
        foreach (var item in indexes)
        {
            item.text = "---";
        }
        foreach (var item in points)
        {
            item.text = "---";
        }
        foreach (var item in keyButtons)
        {
            item.gameObject.SetActive(false);
        }
        ResetHighLights();
        selectedIndex = null;
    }
    public void ShowNamesAndPoints()
    {
        ArrageData();
        ResetPlacar();

        showingNames = orderedData.Page(pageIndex, 10).ToList();


        int i = 0;
        foreach (var item in showingNames)
        {
            if (item.name == "") continue;
            names[i].text = item.name;
            points[i].text = item.score.ToString("0");
            indexes[i].text = (i + 1 +(10*(pageIndex-1))).ToString("0");
            keyButtons[i].gameObject.SetActive(true);
            i++;
        }
    }

    void ResetHighLights()
    {
        foreach (var item in highlights)
        {
            item.enabled = false;
        }
    }
    public void SelectIndex(int index)
    {
        if (index < showingNames.Count)
        {
            selectedIndex = index;
            ResetHighLights();
            highlights[index].enabled = true;
        }
    }

    public void TrashSelected()
    {
        if (selectedIndex != null)
        {
            string nome = showingNames[(int)selectedIndex].name;
            FeedbackUI.Instance.SetText("Digite APAGAR para deletar o usuário: '" + nome + "'!")
                .SetButtons(false, false, true, true)
                .SetSimFeedback(() =>
                {
                    ShowKeyboard(true);
                })
                .SetNaoFeedback(() =>
                {
                    FeedbackUI.Instance.Close();
                }).Show();
        }
    }
    public void DeletePlayer()
    {
        if (PlayTableKeyboard.Instance.Text.ToUpper().Equals("APAGAR"))
        {
            if (selectedIndex != null)
            {
                Debug.Log(selectedIndex);
                string nome = showingNames[(int)selectedIndex].name;
                persintence.RemovePlayMovePlayer(nome);
                ShowNamesAndPoints();
                FeedbackUI.Instance.SetText("Nome Removido!")
                    .SetButtons(true, false, false, false)
                    .SetOKFeedback(() =>
                    {
                        FeedbackUI.Instance.Close();
                        FeedbackUI.Instance.CloseHandler();
                    }).Show();
            }
        }
        else
        {
            FeedbackUI.Instance.SetText("Palavra Incorreta!")
                .SetButtons(true, false, false, false)
                .SetOKFeedback(() => {
                    FeedbackUI.Instance.Close();
                    FeedbackUI.Instance.CloseHandler();
                }).Show();
        }
        ShowKeyboard(false);
    }
    public void OpenConceptMapFromIndex(int index)
    {
        FindObjectOfType<PlayMoveChallengeMapUI>().Open(showingNames[index].conceptMap);
    }
}
