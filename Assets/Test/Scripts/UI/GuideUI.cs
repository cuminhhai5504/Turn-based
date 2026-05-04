using UnityEngine;

using DG.Tweening;

public class GuideUI : MonoBehaviour
{
    public GameObject guidePanel;

    
    public void OpenGuide()
    {
        guidePanel.SetActive(true);
        guidePanel.transform.localScale = Vector3.zero;
        guidePanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }
    
}
