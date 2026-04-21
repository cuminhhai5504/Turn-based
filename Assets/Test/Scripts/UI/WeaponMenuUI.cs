using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class WeaponMenuUI : MonoBehaviour
{
    public GameObject panel;
    public List<WeaponSlot> slots;

    private Unit currentUnit;
    private Unit currentTarget;
    private UnitController controller;

    public void Show(Unit unit, Unit target, UnitController ctrl)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        currentUnit = unit;
        currentTarget = target;
        controller = ctrl;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < unit.weapons.Count)
            {
                Weapon weapon = unit.weapons[i];

                if (unit.CanAttackTarget(target, weapon))
                {
                    slots[i].SetWeapon(weapon.weaponName);

                    int index = i;
                    slots[i].button.onClick.RemoveAllListeners();
                    slots[i].button.onClick.AddListener(() =>
                    {
                        controller.AttackWithWeapon(currentTarget, weapon);
                        Hide();
                    });
                }
                else
                {
                    slots[i].SetEmpty();
                }
            }
            else
            {
                slots[i].SetEmpty();
            }
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}