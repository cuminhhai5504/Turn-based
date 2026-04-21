using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsUI : MonoBehaviour
{
    public GameObject settingPanel;
    public GameObject confirmPanel;
    public Image fadePanel;

    // 🔘 mở menu
    public void OpenSettings()
    {
        settingPanel.SetActive(true);
        // scale nhỏ → to
        settingPanel.transform.localScale = Vector3.zero;
        settingPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        
    }

    // ▶ Continue
    public void OnContinue()
    {
        settingPanel.SetActive(false);
    }
    

    // mở confirm
    public void OnMainMenu()
    {
        confirmPanel.SetActive(true);
        confirmPanel.transform.localScale = Vector3.zero;
        confirmPanel.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
    }

    // ✅ Yes → về main menu
    public void OnConfirmYes()
    {
        fadePanel.DOFade(1f, 0.5f).OnComplete(() =>
        {
            SceneManager.LoadScene("MainMenu");
        });
    }

    // ❌ No → đóng confirm
    public void OnConfirmNo()
    {
        confirmPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack)
        .OnComplete(() =>
        {
            confirmPanel.SetActive(false);
        });
    }
}