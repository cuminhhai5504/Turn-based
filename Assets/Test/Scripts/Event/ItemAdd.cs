using UnityEngine;
using static UnityEditor.Progress;

public class ItemAdd : MonoBehaviour
{
    public Vector2Int position;

    public Item rewardItem;
    

    bool isUsed = false;

    public void Trigger(Unit unit)
    {
        if (isUsed) return;

        if (rewardItem != null)
        {
            unit.inventory.Add(rewardItem);
            Debug.Log("Nhận item: " + rewardItem.itemName);
            PickupPopupManager.Instance.ShowPopup("+ " + rewardItem.itemName, unit.transform.position);
        }

        
        isUsed = true;

        // optional: ẩn object
        gameObject.SetActive(false);
    }
}