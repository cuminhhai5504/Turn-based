using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    private string savePath;

    private void Awake()
    {
        Instance = this;
        savePath = Application.persistentDataPath + "/save.json";
    }

    public void SaveGame()
    {
        BattleSaveData data = new BattleSaveData();
        data.units = new List<UnitSaveData>();

        foreach (var unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            data.units.Add(unit.GetSaveData());
        }

        // 🔥 nếu có TurnManager thì thêm
        data.turnNumber = TurnManager.Instance.turnCount;
        data.currentPhase = TurnManager.Instance.currentTurn.ToString();

        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString("SAVE_DATA", json);
        PlayerPrefs.Save();

        Debug.Log("Saved with PlayerPrefs!");
    }
    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SAVE_DATA"))
        {
            Debug.LogError("Không có save!");
            return;
        }

        string json = PlayerPrefs.GetString("SAVE_DATA");
        BattleSaveData data = JsonUtility.FromJson<BattleSaveData>(json);

        StartCoroutine(LoadRoutine(data));
    }
    IEnumerator LoadRoutine(BattleSaveData data)
    {
        // 🔥 XÓA UNIT CŨ
        foreach (var u in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            Destroy(u.gameObject);
        }

        yield return null;

        // 🔥 SPAWN LẠI
        foreach (var uData in data.units)
        {
            GameObject prefab = UnitDatabase.Instance.GetPrefab(uData.unitID);

            GameObject obj = Instantiate(prefab);
            Unit unit = obj.GetComponent<Unit>();

            unit.LoadFromData(uData);
            TurnManager.Instance.AddUnit(unit);
        }

        // 🔥 RESTORE TURN
        TurnManager.Instance.turnCount = data.turnNumber;

        if (System.Enum.TryParse(data.currentPhase, out TurnManager.TurnState state))
        {
            TurnManager.Instance.SetState(state);
        }

        Debug.Log("Load xong!");
    }
}