using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioMixer mixer;
    public SFXLibrary sfx;

    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 🔥 KẾT NỐI MIXER
            musicSource.outputAudioMixerGroup = musicGroup;
            sfxSource.outputAudioMixerGroup = sfxGroup;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    // ================= MUSIC =================
    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
    
    // ================= SFX =================
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // ================= VOLUME =================
    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
    public void SyncSliders(Slider music, Slider sfx)
    {
        float musicVal = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVal = PlayerPrefs.GetFloat("SFXVolume", 1f);

        music.SetValueWithoutNotify(musicVal);
        sfx.SetValueWithoutNotify(sfxVal);
    }
}