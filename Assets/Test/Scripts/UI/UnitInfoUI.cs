using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
public class UnitInfoUI : MonoBehaviour
{
    public static UnitInfoUI Instance;

    public GameObject panel;

    public Text nameText;
    public Text levelText;
    public Text hpText;
    public Text maxHPText;
    public Text moveRangeText;
    public Text expText;
    public Text expToNextText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }
    void Update()
    {
        
    }
    public void Show(Unit unit)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        nameText.text = unit.data.unitName;
        levelText.text = unit.level.ToString();
        hpText.text = unit.currentHP.ToString();
        maxHPText.text = unit.maxHP.ToString();
        moveRangeText.text = unit.moveRange.ToString();
        expText.text = unit.currentEXP.ToString();
        expToNextText.text = unit.expToNextLevel.ToString();  
    }

    
}