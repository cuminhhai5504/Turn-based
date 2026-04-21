using UnityEngine;
using UnityEngine.UI;

public class UnitHPUI : MonoBehaviour
{
    public Unit unit;
    public Image hpFill;

    void Update()
    {
        if (unit == null) return;
        float percent = (float)unit.currentHP / unit.data.maxHP;
        hpFill.fillAmount = percent;
    }
}