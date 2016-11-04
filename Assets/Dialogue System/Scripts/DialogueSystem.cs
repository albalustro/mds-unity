using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

public class DialogueSystem : MDSBehaviour {

    public static DialogueSystem instance = null;

    //Componente que gerencia o canvas
    public Dialogue dialogue;
    //Scriptable Object com todas as entradas de diálogo do Game/Mundo em questão
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

    private AudioSource audioSource; //<<<<<<<<<< provisório até implementarmos o singleton do Audio Manager

    //Lista com os diálogos necessários apenas para o contexto atual
    public List<DialogueEntry> _currentDialogues;
    //Índice do diálogo sendo exibido
    private int _currentDialogueIndex;

    protected override void Awake()
    {
        base.Awake();

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>(); //<<<<<<<<<< provisório até implementarmos o singleton do Audio Manager
        
        //Será substituído pelo componente de parse de scene
        DSGlobal.game = SceneManager.GetActiveScene().name.Substring(1, 1);
        DSGlobal.world = SceneManager.GetActiveScene().name.Substring(3, 1);
        DSGlobal.episode = SceneManager.GetActiveScene().name.Substring(5, 1);

        //Verifica se a scene aberta é de episódio ou de challenge
        if (SceneManager.GetActiveScene().name.Length > 6)
        {
            //Scene de Challenge
            DSGlobal.slug = "intro";
            DSGlobal.challenge = Convert.ToInt32(SceneManager.GetActiveScene().name.Substring(7, 1));
            DSGlobal.minigame = DSGlobal.challenge.ToString();
        }
        else
        {
            //Scene de Episódio
            if (DSGlobal.challenge == 0)
                DSGlobal.challenge = 1;
            DSGlobal.minigame = "";
            DSGlobal.slug = "minigame" + DSGlobal.challenge;
        }
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
        DSGlobal.auxCount = 0;

        //Carrega o scriptable object correspondente ao Game e Mundo escolhido pelo jogador (puxando pelo nome da Scene)
        SODialogue = AssetDatabase.LoadAssetAtPath(_soPath, typeof(DialogueList)) as DialogueList;

        //Gera a lista de diálogos pertinentes ao contexto atual
        _currentDialogues = new List<DialogueEntry>();
        _currentDialogues.Clear();
        _currentDialogues = GetDialoguesForCurrentContext(DSGlobal.slug, DSGlobal.minigame);
        OpenDialogueBox();
    }

    void OpenDialogueBox()
    {
        _currentDialogueIndex = 0;
        dialogue.gameObject.SetActive(true);
        DSGlobal.isActive = true;
        ChangeDialog();
    }

    void CloseDialogueBox()
    {
        dialogue.gameObject.SetActive(false);
        DSGlobal.isActive = false; //esse cara nao faz mais sentido
		//disparar um evento informando que o dialogo esta ativo ou nao
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
    /// Exibe mensagem de 'victory' do diálogo atual quando o jogador acerta um desafio
    /// </summary>
    public void ShowVictoryDialogueMessage(int victoryAmount = 0)
    {
        if (victoryAmount == 0)
        {
            DSGlobal.slug = "victory";
            DSGlobal.challenge++;
            //DSGlobal.challengeFinished = true;
        }
        else
        {
            //desafios múltiplos
        }
        _currentDialogues = GetDialoguesForCurrentContext(DSGlobal.slug, DSGlobal.minigame);
        OpenDialogueBox();
    }

    /// <summary>
    /// Exibe mensagem de 'error' do diálogo atual quando o jogador erra um desafio 
    /// </summary>
    public void ShowErrorDialogueMessage(int errorAmount = 0) //o contador vai estar no challenge
    {
        if (errorAmount == 0)
        {
            DSGlobal.slug = "error";
            _currentDialogues = GetDialoguesForCurrentContext(DSGlobal.slug, DSGlobal.minigame);
            //Resolve automaticamente para o jogador e libera o botão de validar (nesse caso de error simples, não tem hint). Após validado, mostra mensagem de vitória e finaliza o desafio.
			//nao é responsabilidade do dialogue
        }
        else
        {
            DSGlobal.auxCount++;
            DSGlobal.slug = "error" + DSGlobal.auxCount;
            _currentDialogues = GetDialoguesForCurrentContext(DSGlobal.slug, DSGlobal.minigame);
            if (DSGlobal.auxCount >= errorAmount)
            {
                DSGlobal.auxCount = errorAmount;
				_currentDialogues = GetDialoguesForCurrentContext(DSGlobal.slug, DSGlobal.minigame);
				//_currentDialogues = _currentDialogues.Concat (_tempDialogueList);
				//Resolve automaticamente para o jogador e libera o botão de validar => nao é responsabilidade do dialogue
            }
        }
		ShowHintDialogueMessage ();
    }

    /// <summary>
    /// Exibe mensagem de 'hint' do diálogo atual quando 
    /// </summary>
    public void ShowHintDialogueMessage()
    {
		List<DialogueEntry> _tempDialogueList = GetDialoguesForCurrentContext("hint", DSGlobal.minigame);
		if (_tempDialogueList.Count <= 0)
		{
			_tempDialogueList = GetDialoguesForCurrentContext("hint" + DSGlobal.auxCount, DSGlobal.minigame);
		}
		_currentDialogues.Add (_tempDialogueList[0]);
		OpenDialogueBox();
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
    /// Cria uma lista de todos os diálogos do episódio atual
    /// </summary>
    /// <returns>Lista de DialogueEntry do episódio sendo jogado</returns>
    public List<DialogueEntry> GetDialoguesForCurrentContext()
    {
        return SODialogue.dialogueList.Where(d => d.episode == DSGlobal.episode).ToList();
    }

    /// <summary>
    /// Cria uma lista dos diálogo do contexto atual baseado em parâmetros
    /// </summary>
    /// <param name="slug">Tag slug no json de diálogos (victory, error ou hint)</param>
    /// <param name="minigame">Número do desafio (challenge). Null quando está na tela de episódio.</param>
    /// <returns>Lista de DialogueEntry pertinente ao contexto ativo</returns>
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
