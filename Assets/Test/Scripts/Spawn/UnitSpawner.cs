using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitSpawner : MonoBehaviour
{
    public Tilemap tilemap;
    public SpawnConfig spawnConfig;

    void Start()
    {
        // 🔥 Nếu đang Continue → KHÔNG spawn
        if (SaveLoadManager.Instance != null &&
            SaveLoadManager.Instance.isContinuing)
        {
            Debug.Log("Skip spawn (Continue)");
            return;
        }
        foreach (var data in spawnConfig.units)
        {
            SpawnUnit(data);
        }
    }

    void SpawnUnit(SpawnUnitData data)
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld((Vector3Int)data.gridPosition);

        GameObject unitObj = Instantiate(data.prefab, worldPos, Quaternion.identity);

        Unit unit = unitObj.GetComponent<Unit>();
        unit.Init(unit.data);
        unit.SetPosition(data.gridPosition);
        unit.isEnemy = data.isEnemy;
        
    }
    
}