using System.Collections.Generic;
using UnityEngine;

public class UnitDatabase : MonoBehaviour
{
    public static UnitDatabase Instance;

    public List<UnitPrefabEntry> units;

    private Dictionary<string, GameObject> lookup;

    void Awake()
    {
        Instance = this;
        lookup = new Dictionary<string, GameObject>();

        foreach (var u in units)
        {
            lookup[u.id] = u.prefab;
        }
    }

    public GameObject GetPrefab(string id)
    {
        if (lookup.ContainsKey(id))
            return lookup[id];

        Debug.LogError("Không tìm thấy unit: " + id);
        return null;
    }
}

[System.Serializable]
public class UnitPrefabEntry
{
    public string id;
    public GameObject prefab;
}