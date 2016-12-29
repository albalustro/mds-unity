using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioListener))]
public class AudioController : Singleton<AudioController>
{
	private AudioSource _SoundFXSource = new AudioSource();
	private AudioSource _VoiceOverSource = new AudioSource();
	private AudioSource _ThemeSource = new AudioSource();
	private float _ThemeVolume = 0.4f;
	private float _VoiceOverVolume = 0.4f;
	private float _SoundFXVolume = 1.0f;
	private bool _ThemeStatus;
	private bool _VoiceOverStatus;
	private bool _SoundFXStatus;

	public AudioMixerGroup themeMixer;
	public AudioMixerGroup soundFXMixer;
	public AudioMixerGroup voiceOverMixer;

	void Awake()
	{
		_SoundFXSource = gameObject.AddComponent<AudioSource>();
		_VoiceOverSource = gameObject.AddComponent<AudioSource>();
		_ThemeSource = gameObject.AddComponent<AudioSource>();

		_SoundFXSource.loop = false;
		_SoundFXSource.volume = _SoundFXVolume;
		_SoundFXSource.outputAudioMixerGroup = soundFXMixer;

		_VoiceOverSource.loop = false;
		_VoiceOverSource.volume = _VoiceOverVolume;
		_VoiceOverSource.outputAudioMixerGroup = voiceOverMixer;

		_ThemeSource.loop = true;
		_ThemeSource.volume = _ThemeVolume;
		_ThemeSource.outputAudioMixerGroup = themeMixer;
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

		_SoundFXSource.clip = clip;
		_SoundFXSource.Play();
	}

	public void PlayVoiceOver(AudioClip clip)
	{
		if (!clip)
			return;

		_VoiceOverSource.clip = clip;
		_VoiceOverSource.Play();
	}

	public void PlayTheme(AudioClip clip)
	{
		if (!clip)
			return;

		_ThemeSource.clip = clip;
		_ThemeSource.Play();
		_ThemeSource.volume = _ThemeVolume;
	}

	public bool ThemeIsOn
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

	public bool VoceOverIsOn
	{
		get
		{
			return _VoiceOverStatus;
		}

		set
		{
			_VoiceOverStatus = value;
			_VoiceOverSource.mute = !_VoiceOverStatus;
		}
	}

	public bool SoundFXIsOn
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

	public void StopAllSounds()
	{
		_SoundFXSource = gameObject.AddComponent<AudioSource>();
		_VoiceOverSource = gameObject.AddComponent<AudioSource>();
		_ThemeSource = gameObject.AddComponent<AudioSource>();

		_SoundFXSource.Stop();
		_SoundFXSource.clip = null;

		_ThemeSource.Stop ();
		_ThemeSource.clip = null;

		_VoiceOverSource.Stop ();
		_VoiceOverSource.clip = null;
	}
}

