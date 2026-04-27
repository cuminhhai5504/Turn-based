using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour
{
    public static WeaponDatabase Instance;

    public List<Weapon> weaponTemplates;

    private Dictionary<string, Weapon> lookup;

    private void Awake()
    {
        Instance = this;
        lookup = new Dictionary<string, Weapon>();

        foreach (var w in weaponTemplates)
        {
            lookup[w.id] = w;
        }
    }

    public Weapon Create(string id)
    {
        if (!lookup.ContainsKey(id))
        {
            Debug.LogError("Weapon không tồn tại: " + id);
            return null;
        }

        return lookup[id].Clone();
    }
}