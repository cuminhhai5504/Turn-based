using UnityEngine;

public class PromotionUI : MonoBehaviour
{
    public static PromotionUI Instance;

    public GameObject panel;

    public GameObject warriorPrefab;
    public GameObject archerPrefab;
    public GameObject lancerPrefab;

    private Unit currentUnit;
    
    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(Unit unit)
    {
        
        TurnManager.Instance.isUIBlocking = true;
        currentUnit = unit;
        panel.SetActive(true);
        
    }

    public void ChooseWarrior()
    {
        currentUnit.Promote(warriorPrefab);
        Close();
    }

    public void ChooseArcher()
    {
        currentUnit.Promote(archerPrefab);
        Close();
    }

    public void ChooseLancer()
    {
        currentUnit.Promote(lancerPrefab);
        Close();
    }

    void Close()
    {
        
        panel.SetActive(false);

        TurnManager.Instance.isUIBlocking = false;
        TurnManager.Instance.CheckEndPlayerTurn();
        GameManager.Instance.CheckGameResult();
    }
}