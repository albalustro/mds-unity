using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

public class DialogueSystem : Singleton<DialogueSystem>
{


    //Propriedades de um DialogueEntry
    public static string game;
    public static string world;
    public static string episode;
    public static string minigame;
    public static string slug;
    public static int id;
    public static int challenge = 1;
    public static bool isActive;
    //Componente que gerencia o canvas
    public Dialogue dialogue;
	private Canvas canvas;
    //Scriptable Object com todas as entradas de diálogo do Game/Mundo em questão
    public DialogueList SODialogue;
    //Caminhos para load do SO, voice over e emotion
    private string _soPath;
    private string _audioPath;
    private string _emotionPath;
    //Emotion atual
    //private string _currentEmotion;
    //Audio clip tocado ao clicar no botão de avançar conversa
    public AudioClip nextBtnClickFx;

    private AudioSource audioSource; //<<<<<<<<<< provisório até implementarmos o singleton do Audio Manager

    //Lista com os diálogos necessários apenas para o contexto atual
    public List<DialogueEntry> _currentDialogues;
    //Índice do diálogo sendo exibido
    private int _currentDialogueIndex;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>(); //<<<<<<<<<< provisório até implementarmos o singleton do Audio Manager
		canvas = GetComponentInParent<Canvas>();
		canvas.worldCamera = Camera.main;
        //Será substituído pelo componente de parse de scene
        game = SceneManager.GetActiveScene().name.Substring(1, 1);
        world = SceneManager.GetActiveScene().name.Substring(3, 1);
        episode = SceneManager.GetActiveScene().name.Substring(5, 1);

        //Verifica se a scene aberta é de episódio ou de challenge
        if (SceneManager.GetActiveScene().name.Length > 6)
        {
            //Scene de Challenge
            slug = "intro";
            challenge = Convert.ToInt32(SceneManager.GetActiveScene().name.Substring(7, 1));
            minigame = challenge.ToString();
        }
        else
        {
            //Scene de Episódio
            if (challenge == 0)
                challenge = 1;
            minigame = "";
            slug = "minigame" + challenge;
        }
        InitializeDialogueSystem();

    }

    void InitializeDialogueSystem()
    {
        //caminho padrão para as pastas de voice over e emotions

        _audioPath = "Audios/MDS " + game + "/World " + world + "/";

        _emotionPath = "Emotions/W" + world + "/guide_";

        _soPath = "SO/G" + game + "W" + world;

        SODialogue = Resources.Load(_soPath, typeof(DialogueList)) as DialogueList;


        //Gera a lista de diálogos pertinentes ao contexto atual
        _currentDialogues = new List<DialogueEntry>();
        _currentDialogues.Clear();
        _currentDialogues = GetDialoguesForCurrentContext(slug, minigame);
    }

    /// <summary>
    /// Abre a caixa de diálogo e chama o método ChangeDialog para definir emotion, texto e VO.
    /// Muda para verdadeido o valor da propriedade isActive que indica se o diálogo está ativo ou não.
    /// </summary>
    void OpenDialogueBox()
    {
        _currentDialogueIndex = 0;
        dialogue.gameObject.SetActive(true);
        isActive = true;
        ChangeDialog();
    }

    /// <summary>
    /// Fecha a caixa de diálogo.
    /// Muda para falso o valor da propriedade isActive que indica se o diálogo está ativo ou não.
    /// </summary>
    void CloseDialogueBox()
    {
        dialogue.gameObject.SetActive(false);
        isActive = false;
    }

    /// <summary>
    /// Informa ao challenge se o diálogo está abert ou não
    /// </summary>
    /// <returns>true se diálogo aberto, false se diálogo fechado</returns>
    public bool IsDialogueOpen()
    {
        return isActive;
    }

    /// <summary>
    /// Método invocado quando o jogador clica na seta para avançar o diálogo.
    /// Avança até a última mensagem da lista de diálogos do contexto e, ao terminar, fecha a caixa de diálogo
    /// </summary>
    public void NextDialogue()
    {
        audioSource.clip = nextBtnClickFx;
        audioSource.Play();
        _currentDialogueIndex++;
        if (_currentDialogueIndex >= _currentDialogues.Count)
            CloseDialogueBox();
        else
            ChangeDialog();
    }

    /// <summary>
    /// Exibe caixa de dialogo para slugs correspondentes
    /// </summary>
    /// <param name="_slugs">Slugs do dialogo a ser exibido</param>
    public void ShowDialogueMessage(Slug[] _slugs)
    {
        _currentDialogues.Clear();
        for (int i = 0; i < _slugs.Length; i++)
        {
            List<DialogueEntry> tempList = GetDialoguesForCurrentContext(_slugs[i].ToString(), minigame);
            _currentDialogues.AddRange(tempList);
        }
        if (_currentDialogues.Count <= 0)
        {
            Debug.LogError("Slug: " + slug + " não existe no json.\n Dado informado no Game: " + game + "\n World: " + world + "\n Episódio: " + episode + "\n Challenge: " + challenge);
            return;
        }
        OpenDialogueBox();
    }

    /// <summary>
    /// Altera o texto da caixa de diálogo e toca o som correspondente
    /// </summary>
    void ChangeDialog()
    {
        //if (_currentEmotion != _currentDialogues[_currentDialogueIndex].emotion)
        ChangeEmotion();
        dialogue.SetText(_currentDialogues[_currentDialogueIndex].text.ToUpper());
        AudioClip c = Resources.Load<AudioClip>(_audioPath + _currentDialogues[_currentDialogueIndex].sound);

        dialogue.PlayVoiceOver(c);

    }

    /// <summary>
    /// Altera o emotion (avatar do guia)
    /// </summary>
    void ChangeEmotion()
    {
        Sprite s = Resources.Load<Sprite>(_emotionPath + _currentDialogues[_currentDialogueIndex].emotion);
        dialogue.SetEmotion(s);
    }

    /// <summary>
    /// Cria uma lista de todos os diálogos do episódio atual
    /// </summary>
    /// <returns>Lista de DialogueEntry do episódio sendo jogado</returns>
    List<DialogueEntry> GetDialoguesForCurrentContext()
    {
        return SODialogue.dialogueList.Where(d => d.episode == episode).ToList();
    }

    /// <summary>
    /// Cria uma lista dos diálogo do contexto atual baseado em parâmetros
    /// </summary>
    /// <param name="slug">Tag slug no json de diálogos (victory, error ou hint)</param>
    /// <param name="minigame">Número do desafio (challenge). Null quando está na tela de episódio.</param>
    /// <returns>Lista de DialogueEntry pertinente ao contexto ativo</returns>
    List<DialogueEntry> GetDialoguesForCurrentContext(string _slug, string _minigame)
    {
        return SODialogue.dialogueList.Where(d => d.episode == episode && d.slug == _slug && d.minigame == _minigame).ToList();
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