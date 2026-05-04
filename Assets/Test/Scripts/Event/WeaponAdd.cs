using UnityEngine;

public class WeaponAdd : MonoBehaviour
{
    public Vector2Int position;

    
    public Weapon rewardWeapon;

    public string id;
    public bool isUsed = false;

    public void Trigger(Unit unit)
    {
        if (isUsed) return;

        if (rewardWeapon != null)
        {
            unit.weapons.Add(rewardWeapon);
            Debug.Log("Nhận weapon: " + rewardWeapon.weaponName);
            PickupPopupManager.Instance.ShowPopup("+ " + rewardWeapon.weaponName, unit.transform.position);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfx.pickup);
        }


        isUsed = true;
        SaveLoadManager.Instance.MarkEventUsed(id);
        // optional: ẩn object
        gameObject.SetActive(false);
    }
}