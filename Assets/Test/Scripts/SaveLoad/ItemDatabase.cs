using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public List<Item> itemTemplates;
    private Dictionary<string, Item> lookup;

    private void Awake()
    {
        Instance = this;

        lookup = new Dictionary<string, Item>();
        foreach (var i in itemTemplates)
        {
            lookup[i.id] = i;
        }
    }

    public Item Create(string id)
    {
        if (!lookup.ContainsKey(id))
        {
            Debug.LogError("Item không tồn tại: " + id);
            return null;
        }

        return new Item
        {
            itemName = lookup[id].itemName,
            id = lookup[id].id,
            type = lookup[id].type,
            value = lookup[id].value
        };
    }
}