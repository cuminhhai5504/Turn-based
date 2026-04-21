using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ActionMenuUI : MonoBehaviour
{
    public GameObject panel;
    public Button attackBtn;
    public Button waitBtn;
    public Button cancelBtn;
    public Button itemsBtn;
    private UnitController controller;

    void Start()
    {
        controller = FindFirstObjectByType<UnitController>();

        attackBtn.onClick.AddListener(OnAttack);
        waitBtn.onClick.AddListener(OnWait);
        cancelBtn.onClick.AddListener(OnCancel);
        itemsBtn.onClick.AddListener(OnItemSelected);
    }

    public void Show(Vector3 worldPos)
    {
        panel.SetActive(true);
        panel.transform.DOKill();
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        attackBtn.gameObject.SetActive(controller.CanAttackCurrentUnit());
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        panel.transform.position = screenPos;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    void OnAttack()
    {
        controller.isMenuOpen = false;
        controller.OnAttackSelected();
        Hide();
    }

    void OnWait()
    {
        controller.isMenuOpen = false;
        controller.OnWaitSelected();
        Hide();
    }

    void OnCancel()
    {
        controller.isMenuOpen = false;
        controller.UndoMove();
        Hide();
    }
    public void OnItemSelected()
    {

        controller.isMenuOpen = false; // 🔥 QUAN TRỌNG (giống attack/wait)
        controller.OnItemSelected();
        Hide();
    }
}