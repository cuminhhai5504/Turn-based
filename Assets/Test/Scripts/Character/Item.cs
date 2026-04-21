using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;

    public enum ItemType
    {
        Heal,
        Buff
    }

    public ItemType type;
    public string id;
    public int value;

    public void Use(Unit user)
    {
        switch (type)
        {
            case ItemType.Heal:
                user.currentHP += value;
                user.currentHP = Mathf.Min(user.currentHP, user.data.maxHP);
                Debug.Log(user.name + " hồi " + value + " HP");
                break;

            case ItemType.Buff:
                user.moveRange += value;
                Debug.Log(user.name + " tăng " + value + " nhanh nhẹn");
                break;
        }
    }
}