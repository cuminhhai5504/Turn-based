using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    public Image fadePanel;
    void Start()
    {
        fadePanel.DOFade(0, 1f);
        
    }
    public void OnPlay()
    {
        PlayerPrefs.DeleteAll();
        SaveLoadManager.Instance.cachedData = null;
        // 🔥 Fade ra đen trước
        fadePanel.DOFade(1, 0.5f).OnComplete(() =>
        {
            SceneLoader.sceneToLoad = "LevelSelect";
            // 🔥 Sau khi fade xong mới chuyển scene
            SceneManager.LoadScene("LoadingScene");
        });
    }
    public void OnContinue()
    {
        fadePanel.DOFade(1, 0.5f).OnComplete(() =>
        {
            SaveLoadManager.Instance.ContinueGame();
        });
        
    }
    public void OnQuit()
    {
        Application.Quit();
    }
}