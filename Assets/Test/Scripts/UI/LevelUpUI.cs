using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static Unit;
using UnityEngine.EventSystems;

public class LevelUpUI : MonoBehaviour
{
    public static LevelUpUI Instance;
    public GameObject panel;
    public Text nameText;
    public Text levelOld, levelNew;
    public Text hpOld, hpNew;
    public Text maxhpOld, maxhpNew;
    public Text moveOld, moveNew;
    public Text expOld, expNew;
    public Text expToNextOld, expToNexNew;
    public bool IsShowing()
    {
        return panel.activeSelf;
    }
    void Update()
    {
        if (panel.activeSelf && Input.GetMouseButtonDown(0))
        {
            // Nếu click KHÔNG phải UI
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Hide();
            }
        }
    }
    void Awake()
    {
        Instance = this;
    }
    public void Hide()
    {
        panel.SetActive(false);

        TurnManager.Instance.isUIBlocking = false;
    }
    public void ShowLevelUp(UnitStatsSnapshot oldStats, UnitStatsSnapshot newStats)
    {
        
        panel.SetActive(true);
        //name
        nameText.text = newStats.unitName;
        //Level
        levelOld.text = oldStats.level.ToString();
        levelNew.text = newStats.level.ToString();
        // HP
        hpOld.text = oldStats.currentHP.ToString();
        hpNew.text = newStats.currentHP.ToString();
        // maxHP
        maxhpOld.text = oldStats.maxHP.ToString();
        maxhpNew.text = newStats.maxHP.ToString();
        // MoveR
        moveOld.text = oldStats.moveRange.ToString();
        moveNew.text = newStats.moveRange.ToString();
        //EXP
        expOld.text = oldStats.currentExp.ToString();
        expNew.text = newStats.currentExp.ToString();
        //EXPtoNext
        expToNextOld.text = oldStats.expToNext.ToString();
        expToNexNew.text = newStats.expToNext.ToString(); 
    }
}