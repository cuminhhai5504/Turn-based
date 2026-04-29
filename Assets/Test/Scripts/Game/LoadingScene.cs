using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class LoadingScene : MonoBehaviour
{
    public Slider slider;
    public Image fadePanel;          // màn đen
    

    void Start()
    {
        StartCoroutine(LoadGame());      
        // Fade in
        fadePanel.DOFade(0, 1f);
    }
    void OnEnable()
    {
        DOTween.KillAll(); // 🔥 reset toàn bộ tween cũ
    }
    IEnumerator LoadGame()
    {
        
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneLoader.sceneToLoad);
        op.allowSceneActivation = false;
        float fakeProgress = 0f;
        while (fakeProgress < 1f)
        {
            // 👉 tiến dần giống EXP bar
            float target = Mathf.Clamp01(op.progress / 0.9f);

            fakeProgress = Mathf.MoveTowards(fakeProgress, target, Time.deltaTime*0.4f);

            slider.value = fakeProgress;
            

            yield return null;
        }

        // 👉 giữ lại 1 chút cho đẹp
        yield return new WaitForSeconds(0.5f);

        fadePanel.DOFade(1, 0.5f).OnComplete(() =>
        {
            op.allowSceneActivation = true;
        });
    }
}