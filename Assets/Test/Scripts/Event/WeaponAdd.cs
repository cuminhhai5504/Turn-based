using UnityEngine;

public class WeaponAdd : MonoBehaviour
{
    public Vector2Int position;

    
    public Weapon rewardWeapon;

    bool isUsed = false;

    public void Trigger(Unit unit)
    {
        if (isUsed) return;

        if (rewardWeapon != null)
        {
            unit.weapons.Add(rewardWeapon);
            Debug.Log("Nhận weapon: " + rewardWeapon.weaponName);
            PickupPopupManager.Instance.ShowPopup("+ " + rewardWeapon.weaponName, unit.transform.position);
        }


        isUsed = true;

        // optional: ẩn object
        gameObject.SetActive(false);
    }
}