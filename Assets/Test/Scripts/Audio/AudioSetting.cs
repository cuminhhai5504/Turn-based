using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    void OnEnable()
    {
        AudioManager.Instance.SyncSliders(musicSlider, sfxSlider);
    }
}
