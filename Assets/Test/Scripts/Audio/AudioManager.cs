using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Mixer")]
    public AudioMixer mixer;

    [Header("Library")]
    public SFXLibrary sfx;

    [Header("Mixer Groups")]
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    // =========================
    // PROPERTIES
    // =========================

    public float MusicVolume => musicVolume;
    public float SFXVolume => sfxVolume;

    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        // Connect mixer groups
        musicSource.outputAudioMixerGroup = musicGroup;
        sfxSource.outputAudioMixerGroup = sfxGroup;

        LoadVolume();
        ApplyVolume();
    }

    // =========================
    // LOAD / SAVE
    // =========================

    private void LoadVolume()
    {
        musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat(MUSIC_KEY, musicVolume);
        PlayerPrefs.SetFloat(SFX_KEY, sfxVolume);

        PlayerPrefs.Save();
    }

    // =========================
    // APPLY
    // =========================

    private void ApplyVolume()
    {
        mixer.SetFloat(
            "MusicVolume",
            Mathf.Log10(Mathf.Clamp(musicVolume, 0.0001f, 1f)) * 20
        );

        mixer.SetFloat(
            "SFXVolume",
            Mathf.Log10(Mathf.Clamp(sfxVolume, 0.0001f, 1f)) * 20
        );
    }

    // =========================
    // VOLUME
    // =========================

    public void SetMusicVolume(float value)
    {
        musicVolume = value;

        ApplyVolume();
        SaveVolume();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;

        ApplyVolume();
        SaveVolume();
    }

    // =========================
    // MUSIC
    // =========================

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // =========================
    // SFX
    // =========================

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }
}