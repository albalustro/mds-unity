using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class DialogueSystem : MDSBehaviour {

    //Componente que gerencia o canvas
    public Dialogue dialogue;
    //Array com todos os registros de diálogo (vindo do json)
    //public DialogueEntry[] _dialogues;
    private DialogueList SODialogue;
    //Instância para o arquivo de gerenciamento global do Dialogue System
    private DSGlobal global;
    //Caminhos para load do SO, voice over e emotion
    private string _soPath;
    private string _audioPath;
    private string _emotionPath;
    //Emotion atual
    private string _currentEmotion;
    //Audio clip tocado ao clicar no botão de avançar conversa
    public AudioClip nextBtnClickFx;

    private float delay;

    private AudioSource audioSource; //<<<<<<<<<< provisório até implementarmos o singleton do Audio Manager

    public List<DialogueEntry> _currentDialogues;
    private int _currentDialogueIndex;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>(); //<<<<<<<<<< provisório até implementarmos o singleton do Audio Manager
        delay = 0;
        //Será substituído pelo componente de parse de scene
        DSGlobal.game = SceneManager.GetActiveScene().name.Substring(1, 1);
        DSGlobal.world = SceneManager.GetActiveScene().name.Substring(3, 1);
        DSGlobal.episode = SceneManager.GetActiveScene().name.Substring(5, 1);
        if (SceneManager.GetActiveScene().name.Length > 6)
        {
            DSGlobal.slug = "intro";
            DSGlobal.challenge = Convert.ToInt32(SceneManager.GetActiveScene().name.Substring(7, 1));
            DSGlobal.minigame = DSGlobal.challenge.ToString();
        }
        else
        {
            if (DSGlobal.challenge == 0)
            {
                delay = 1;
                DSGlobal.challenge = 1;
            }
            DSGlobal.minigame = "";
            DSGlobal.slug = "minigame" + DSGlobal.challenge;
        }
        print(DSGlobal.slug);
        InitializeDialogueSystem();

        //atribuindo uma ação ao botão 'Next'
        //dialogue.nextBtn.GetComponent<Button>().onClick.AddListener(delegate { NextDialog(); });
    }

    void InitializeDialogueSystem()
    {
        //caminho padrão para as pastas de voice over e emotions
        _audioPath = "Assets/MDS " + DSGlobal.game + "/Dialogue/World " + DSGlobal.world + "/";
        _emotionPath = "Assets/Dialogue System/Emotions/W" + DSGlobal.world + "/guide_";
        _soPath = "Assets/Dialogue System/SO/G" + DSGlobal.game + "W" + DSGlobal.world + ".asset";

        //DSGlobal.id = 0;      //Ainda não sei como utilizar

        //carrega o json e preenche o array de diálogos (talvez saia se formos utilizar o scriptable object
        //LoadDataFromJson load = new LoadDataFromJson();
        //_dialogues = load.LoadFromJson("Assets/Dialogue System/Json/G" + DSGlobal.game + "W" + DSGlobal.world + ".json");
        SODialogue = AssetDatabase.LoadAssetAtPath(_soPath, typeof(DialogueList)) as DialogueList;

        //Gera a lista de diálogos pertinentes ao contexto atual
        _currentDialogues = new List<DialogueEntry>();
        _currentDialogues.Clear();
        _currentDialogues = GetDialoguesForCurrentContext(DSGlobal.slug, DSGlobal.minigame);
        StartCoroutine("Teste");
    }

    IEnumerator Teste()
    {
        yield return new WaitForSeconds(1);
        OpenDialogueBox();
    }

    public void OpenDialogueBox()
    {
        _currentDialogueIndex = 0;
        dialogue.gameObject.SetActive(true);
        DSGlobal.isActive = true;
        ChangeDialog();
    }

    public void CloseDialogueBox()
    {
        dialogue.gameObject.SetActive(false);
        DSGlobal.isActive = false;
    }

    public void NetxDialogue()
    {
        audioSource.clip = nextBtnClickFx;
        audioSource.Play();
        _currentDialogueIndex++;
        if (_currentDialogueIndex >= _currentDialogues.Count)
            CloseDialogueBox();
        else
            ChangeDialog();
    }

    public void GetVictoryEntry()
    {
        DSGlobal.challenge++;
        DSGlobal.slug = "victory";
        _currentDialogues = GetDialoguesForCurrentContext(DSGlobal.slug, DSGlobal.minigame);
        if (_currentDialogues.Count <= 0)
        {
            _currentDialogues = SODialogue.dialogueList.Where(d => d.episode == DSGlobal.episode && d.slug.Remove(d.slug.Length - 1) == "victory" && d.minigame == DSGlobal.minigame).ToList();
        }
        OpenDialogueBox();
    }

    public void GetErrorEntry()
    {
        DSGlobal.slug = "error";
        //_currentDialogues = GetDialoguesForCurrentContext();
        if (_currentDialogues.Count <= 0)
        {
            //_currentDialogues = _dialogues.Where(d => d.episode == DSGlobal.episode && d.slug.Substring(0,5) == "error" && d.minigame == DSGlobal.minigame).ToList();
        }
        OpenDialogueBox();
    }

    public void GetHintEntry()
    {
        //GetDialoguesForCurrentContext();
    }

    /// <summary>
    /// Altera o texto da caixa de diálogo e toca o som correspondente
    /// </summary>
    void ChangeDialog()
    {
        if (_currentEmotion != _currentDialogues[_currentDialogueIndex].emotion)
            ChangeEmotion();
        dialogue.SetText(_currentDialogues[_currentDialogueIndex].text.ToUpper());
        AudioClip c = AssetDatabase.LoadAssetAtPath(_audioPath + _currentDialogues[_currentDialogueIndex].sound + ".mp3", typeof(AudioClip)) as AudioClip;
        dialogue.PlayVoiceOver(c);
    }

    /// <summary>
    /// Altera o emotion (avatar do guia)
    /// </summary>
    void ChangeEmotion()
    {
        Sprite s = AssetDatabase.LoadAssetAtPath(_emotionPath + _currentDialogues[_currentDialogueIndex].emotion + ".png", typeof(Sprite)) as Sprite;
        dialogue.SetEmotion(s);
    }

    /// <summary>
    /// Método utilizado para criar uma lista dos diálogos do contexto atual
    /// </summary>
    /// <returns>Lista de DialogueEntry pertinente ao contexto ativo</returns>
    public List<DialogueEntry> GetDialoguesForCurrentContext()
    {
        return SODialogue.dialogueList.Where(d => d.episode == DSGlobal.episode).ToList();
    }

    public List<DialogueEntry> GetDialoguesForCurrentContext(string slug, string minigame)
    {
        return SODialogue.dialogueList.Where(d => d.episode == DSGlobal.episode && d.slug == slug && d.minigame == minigame).ToList();
    }

    /// <summary>
    /// Retorna o indice do array de diálogos conforme contexto atual
    /// </summary>
    /// <returns>Número inteiro, índice do array de diálogos</returns>
    //private int GetCurrentDialogue(bool c)
    //{
    //    return _dialogues.Select((e, i) => new { Word = e, Index = i }).First(x => x.Word.episode == DSGlobal.episode).Index;
    //    if (c)
    //        return Array.FindIndex<DialogueEntry>(_dialogues, d => d.episode == DSGlobal.episode && d.slug == DSGlobal.slug && d.minigame == DSGlobal.minigame && DSGlobal.challenge == true);
    //    else
    //        return Array.FindIndex<DialogueEntry>(_dialogues, d => d.episode == DSGlobal.episode && d.slug == DSGlobal.slug && d.minigame == "");
    //}
}
