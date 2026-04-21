using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;
    public string[] levelScenes;
    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        SetupButton(level1Button, 1, unlockedLevel);
        SetupButton(level2Button, 2, unlockedLevel);
        SetupButton(level3Button, 3, unlockedLevel);
    }

    void SetupButton(Button btn, int levelIndex, int unlockedLevel)
    {
        if (levelIndex <= unlockedLevel)
        {
            btn.interactable = true;
            btn.onClick.AddListener(() => SelectLevel(levelIndex));
        }
        else
        {
            btn.interactable = false;

            // 👉 làm mờ
            var img = btn.GetComponent<Image>();
            img.color = new Color(1, 1, 1, 0.3f);
        }
    }

    void SelectLevel(int level)
    {
        PlayerPrefs.SetInt("SelectedLevel", level);

        SceneLoader.sceneToLoad = levelScenes[level - 1];

        SceneManager.LoadScene("LoadingScene");
    }
}