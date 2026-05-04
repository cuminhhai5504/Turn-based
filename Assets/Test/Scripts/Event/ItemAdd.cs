using UnityEngine;
using static UnityEditor.Progress;

public class ItemAdd : MonoBehaviour
{
    public Vector2Int position;

    public Item rewardItem;

    public string id;
    public bool isUsed = false;

    public void Trigger(Unit unit)
    {
        if (isUsed) return;

        if (rewardItem != null)
        {
            unit.inventory.Add(rewardItem);
            Debug.Log("Nhận item: " + rewardItem.itemName);
            PickupPopupManager.Instance.ShowPopup("+ " + rewardItem.itemName, unit.transform.position);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfx.pickup);
        }

        
        isUsed = true;
        SaveLoadManager.Instance.MarkEventUsed(id);
        // optional: ẩn object
        gameObject.SetActive(false);
    }
}