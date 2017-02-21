using System;
using UnityEngine;
using UnityEngine.Audio;
using MDS.Utilities;
using UnityEngine.SceneManagement;
using FullInspector;
using MDS.Core.SceneManagement;

[RequireComponent(typeof(AudioListener))]
public class AudioController : Singleton<AudioController>
{
    [SerializeField]
    private AudioClip[] themeLogin;
    [SerializeField]
    private AudioClip[][] themeWorlds;

    private AudioSource _SoundFXSource = new AudioSource();
	private AudioSource _VoiceOverSource = new AudioSource();
	private AudioSource _ThemeSource = new AudioSource();
	private float _ThemeVolume = 0.70f;
	private float _VoiceOverVolume = 1.0f;
	private float _SoundFXVolume = 1.0f;
	private bool _ThemeStatus;
	private bool _VoiceOverStatus;
	private bool _SoundFXStatus;
	[InspectorTooltip("Valores entre -80 e -10, sendo -80 => sem volume"), SerializeField, InspectorRange(-80, -10)]
	private float _themeVolWhileVoiceOverIsPlaying;

	public AudioMixer audioMixer;

	protected override void Awake()
	{
		base.Awake();

		if(AudioController.Instance != this)
			Destroy(gameObject);
		else
			DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
		if (audioMixer == null) {
			audioMixer =  Resources.Load("AudioMixer", typeof(AudioMixer)) as AudioMixer;
			_themeVolWhileVoiceOverIsPlaying = -20;
		}

#endif

		_SoundFXSource = gameObject.AddComponent<AudioSource>();
		_VoiceOverSource = gameObject.AddComponent<AudioSource>();
		_ThemeSource = gameObject.AddComponent<AudioSource>();

		_SoundFXSource.loop = false;
		_SoundFXSource.volume = _SoundFXVolume;
		_SoundFXSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups ("Master")[3];

		_VoiceOverSource.loop = false;
		_VoiceOverSource.volume = _VoiceOverVolume;
		_VoiceOverSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups ("Master")[2];

		_ThemeSource.loop = true;
		_ThemeSource.volume = _ThemeVolume;
		_ThemeSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups ("Master")[1];


        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
	}

    private void SceneManager_sceneLoaded(Scene curScene, LoadSceneMode arg1)
    {
        StopVoiceOver();
        if(curScene.name.Equals("splash")) return;
        if (curScene.IsLogin())
        {
            PlayTheme(themeLogin.GetRandom());
        }
        else if (curScene.IsChallenge())
        {
            // se nao tem um contexto, é porque veio pela chave na cena do mapa. 
            // dessa forma o audio do episodio nao inicia, entao temos que
            // inicia-lo na mão
            if (EpisodeContext.HasEpisodeContext()==false)
            {
                int w, e;
                w = curScene.GetWorldIndex() - 1;
                e = curScene.GetEpisodeIndex() - 1;
                PlayTheme(themeWorlds[w][e]);
            }
        }
    }

    void Start()
	{
		_SoundFXStatus = true;
		_ThemeStatus = true;
		_VoiceOverStatus = true;
	}
		
	public void PlaySoundFX(AudioClip clip)
	{
		if (!clip)
			return;

		_SoundFXSource.Stop ();
		_SoundFXSource.clip = clip;
		_SoundFXSource.Play();
	}

	public void PlayVoiceOver(AudioClip clip)
	{
		if (!clip)
			return;

		_VoiceOverSource.clip = clip;
		//reduz o volume do theme
		audioMixer.SetFloat("ThemeVol", _themeVolWhileVoiceOverIsPlaying);
		_VoiceOverSource.Play();
	}

	public void PlayTheme(AudioClip clip)
	{
		if (!clip)
			return;

		_ThemeSource.clip = clip;
		_ThemeSource.Play();
	}

	public bool ThemeOn
	{
		get
		{
			return _ThemeStatus;
		}

		set
		{
			_ThemeStatus = value;
			_ThemeSource.mute = !_ThemeStatus;
		}
	}

	public bool SoundFXOn
	{
		get
		{
			return _SoundFXStatus;
		}

		set
		{
			_SoundFXStatus = value;
			_SoundFXSource.mute = !_SoundFXStatus;
		}
	}

	public void Mute()
	{
		_SoundFXSource.Stop();
		_SoundFXSource.clip = null;

		_ThemeSource.Stop ();
	}

	public void UnMute()
	{
		ThemeOn = true;
		SoundFXOn = true;
		_ThemeSource.Play ();
	}

    internal void StopVoiceOver()
    {
        _VoiceOverSource.Stop();
        _VoiceOverSource.clip = null;
		//restaura o volume do theme
		audioMixer.SetFloat("ThemeVol", 0);
    }

}

