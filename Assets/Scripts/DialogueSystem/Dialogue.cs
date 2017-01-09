using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MDSBehaviour {

    //fala do personagem (guia)
    public Text text;
    //avatar do personagem (guia)
    public Image emotion;
    //botão para avaçar diálogo
    public Button nextBtn;
    //Fonte de som que reproduz o voice over
    public AudioSource voice;

    protected override void Awake()
    {
        base.Awake();
        text = GameObject.Find("DialogueText").GetComponent<Text>();
        nextBtn = GameObject.Find("NextBtn").GetComponent<Button>();
        emotion = GameObject.Find("Emotion").GetComponent<Image>();
        voice = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Método responsável por atualizar o texto da caixa de diálogo do intermediador
    /// </summary>
    /// <param name="s">String com o texto a ser exibido na caixa de diálogo</param>
    public void SetText(string s)
    {
        text.text = s;
    }

    /// <summary>
    /// Método responsável por tocar o som do Voice Over correspondente ao texto de diálogo exibido
    /// </summary>
    /// <param name="clip">Clip de som a ser tocado</param>
    public void PlayVoiceOver(AudioClip clip)
    {
        voice.clip = clip;
        voice.Play();
    }

    /// <summary>
    /// Método responsável por alterar o Emotion (avatar do guia/intermediador)
    /// </summary>
    /// <param name="sprite">Sprite do emotion a ser exibido</param>
    public void SetEmotion(Sprite sprite)
    {
        emotion.sprite = sprite;
    }
}
