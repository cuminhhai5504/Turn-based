using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class ProgressUnitSpawner : MonoBehaviour
{
    public Tilemap tilemap;
    public List<DeploymentPoint> deploymentPoints;

    void Start()
    {
        LoadProgressUnits();
    }

    void LoadProgressUnits()
    {
        
        if (!PlayerPrefs.HasKey("PROGRESS_DATA"))
        {
            Debug.Log("No progress data!");
            return;
        }

        string json = PlayerPrefs.GetString("PROGRESS_DATA");
        PlayerProgressData data = JsonUtility.FromJson<PlayerProgressData>(json);

        for (int i = 0; i < data.playerUnits.Count; i++)
        {
            if (i >= deploymentPoints.Count)
                break;

            var uData = data.playerUnits[i];

            GameObject prefab = UnitDatabase.Instance.GetPrefab(uData.unitID);

            Vector2Int pos = deploymentPoints[i].gridPosition;
            Vector3 worldPos = tilemap.GetCellCenterWorld((Vector3Int)pos);

            GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity);
            Unit unit = obj.GetComponent<Unit>();

            unit.LoadFromData(uData, false);

            unit.SetPosition(pos);
            unit.isEnemy = false;
            unit.transform.position = tilemap.GetCellCenterWorld((Vector3Int)pos);
            TurnManager.Instance.AddUnit(unit);
        }
    }
}