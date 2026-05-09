using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject audioPanel;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    private bool isOpen = false;
    private void Start()
    {
        audioPanel.SetActive(false);
        // Sync UI với AudioManager hiện tại
        musicSlider.SetValueWithoutNotify(
            AudioManager.Instance.MusicVolume
        );

        sfxSlider.SetValueWithoutNotify(
            AudioManager.Instance.SFXVolume
        );

        // Đăng ký sự kiện
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }
    public void ToggleAudioPanel()
    {
        isOpen = !isOpen;
        if( isOpen )
        {
            audioPanel.SetActive(true);
            audioPanel.transform.localScale = Vector3.zero;
            audioPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            
            audioPanel.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(()=>
            {
                audioPanel.SetActive(false);
            });
        }
    }

    private void OnMusicChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnDestroy()
    {
        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
    }
}