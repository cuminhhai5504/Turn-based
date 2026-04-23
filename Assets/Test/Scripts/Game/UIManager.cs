using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public GameObject settingPanel;
    public GameObject confirmPanel;
    public GameObject unitinfoPanel;
    public GameObject itemPanel;
    public GameObject itemActionPanel;
    public GameObject actionMenuPanel;
    public GameObject listPanel;
    public Image fadePanel;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        

        resultPanel.SetActive(false);
    }

    public void ShowResult(string result)
    {
        resultPanel.SetActive(true);
        resultText.text = result;
    }
    public void RestartGame()
    {
        StartCoroutine(RestartCoroutine());
    }

    IEnumerator RestartCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void CloseAllUI()
    {
        ClosePanel(settingPanel);
        ClosePanel(confirmPanel);
        ClosePanel(unitinfoPanel);
        ClosePanel(itemPanel);
        ClosePanel(itemActionPanel);
        ClosePanel(actionMenuPanel);
        ClosePanel(listPanel);
    }
    void ClosePanel(GameObject panel)
    {
        if (!panel.activeSelf) return;

        panel.transform.DOScale(0f, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                panel.SetActive(false);
            });
    }
    public bool IsAnyUIOpen()
    {
        return settingPanel.activeSelf
            || confirmPanel.activeSelf
            || unitinfoPanel.activeSelf
            || itemPanel.activeSelf
            || itemActionPanel.activeSelf
            || actionMenuPanel.activeSelf
            || listPanel.activeSelf;
            
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // nếu có UI đang mở
            if (UIManager.Instance.IsAnyUIOpen())
            {
                // nếu click KHÔNG phải UI
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    UIManager.Instance.CloseAllUI();
                }
            }
        }
    }
    public void BackToSelectLevel()
    {
        // 🔥 Fade ra đen trước
        fadePanel.DOFade(1, 0.5f).OnComplete(() =>
        {
            SceneLoader.sceneToLoad = "LevelSelect";
            // 🔥 Sau khi fade xong mới chuyển scene
            SceneManager.LoadScene("LoadingScene");
        });
    }
}