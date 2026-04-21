using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ItemActionMenuUI : MonoBehaviour
{
    public GameObject panel;
    public Button useButton;
    public Button cancelButton;

    private int selectedIndex;
    private UnitController controller;

    void Start()
    {
        controller = FindFirstObjectByType<UnitController>();

        useButton.onClick.AddListener(OnUse);
        cancelButton.onClick.AddListener(OnCancel);
    }

    public void Show(int itemIndex, Vector3 position)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        selectedIndex = itemIndex;

        panel.transform.position = position;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    void OnUse()
    {
        controller.UseItemFromUI(selectedIndex);
        Hide();
    }

    void OnCancel()
    {
        Hide();
    }
}