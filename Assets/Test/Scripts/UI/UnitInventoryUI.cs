using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnitInventoryUI : MonoBehaviour
{
    public GameObject panel;
    public Transform content;
    public GameObject textPrefab;
    public CanvasGroup canvasGroup;
    public void ShowItems(Unit unit)
    {
        panel.SetActive(true);
        Clear();
        
        foreach (var item in unit.inventory)
        {
            GameObject obj = Instantiate(textPrefab, content);
            obj.GetComponent<Text>().text = item.itemName + " (" + item.value + ")";
        }
        PlayShowAnim();
    }

    public void ShowWeapons(Unit unit)
    {
        panel.SetActive(true);
        Clear();

        foreach (var weapon in unit.weapons)
        {
            GameObject obj = Instantiate(textPrefab, content);
            obj.GetComponent<Text>().text = weapon.weaponName + " (" + weapon.damage + ")" + " [" + weapon.currentDurability + "/" + weapon.maxDurability + "]";
        }
        PlayShowAnim();
    }
    void PlayShowAnim()
    {
        panel.transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0;

        panel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        canvasGroup.DOFade(1f, 0.3f);
    }

    public void Clear()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void Hide()
    {
        panel.transform.DOScale(0f, 0.25f)
        .SetEase(Ease.InBack);

        canvasGroup.DOFade(0f, 0.2f)
            .OnComplete(() =>
            {
                panel.SetActive(false);
            });
    }
}