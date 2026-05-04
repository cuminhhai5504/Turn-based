using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    
    public BattleSaveData cachedData;

    void Awake()
    {
        // 🔥 FIX: Singleton + sống xuyên scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        BattleSaveData data = new BattleSaveData();
        data.units = new List<UnitSaveData>();

        foreach (var unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            data.units.Add(unit.GetSaveData());
        }

        data.sceneName = SceneManager.GetActiveScene().name;
        data.turnNumber = TurnManager.Instance.turnCount;
        data.currentPhase = TurnManager.Instance.currentTurn.ToString();
        // 🔥 FIX: tránh null
        if (cachedData != null && cachedData.usedEvents != null)
            data.usedEvents = cachedData.usedEvents;
        else
            data.usedEvents = new List<string>();

        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString("SAVE_DATA", json);
        PlayerPrefs.Save();

        Debug.Log("Saved with PlayerPrefs!");
    }
    
    public IEnumerator LoadRoutine(BattleSaveData data)
    {
        Debug.Log("LOAD START");
        TurnManager.Instance.isLoading = true; // 🔥 bật loading
        // 🔥 XÓA unit cũ
        foreach (var u in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            Destroy(u.gameObject);
        }

        yield return null;

        // 🔥 SPAWN lại unit
        foreach (var uData in data.units)
        {
            GameObject prefab = UnitDatabase.Instance.GetPrefab(uData.unitID);

            if (prefab == null)
            {
                Debug.LogError("Missing prefab: " + uData.unitID);
                continue;
            }

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

        // 🔥 UNLOCK game
        TurnManager.Instance.isEventRunning = false;
        TurnManager.Instance.isUIBlocking = false;

        Debug.Log("LOAD DONE");

    }
    public void ContinueGame()
    {
        if (!PlayerPrefs.HasKey("SAVE_DATA"))
        {
            Debug.Log("No save!");
            return;
        }

        string json = PlayerPrefs.GetString("SAVE_DATA");
        cachedData = JsonUtility.FromJson<BattleSaveData>(json);
        
        // 🔥 đưa scene cần load cho LoadingScene
        SceneLoader.sceneToLoad = cachedData.sceneName;

        SceneManager.LoadScene("LoadingScene");
    }
    public void MarkEventUsed(string id)
    {
        // 🔥 FIX 1: đảm bảo cachedData tồn tại
        if (cachedData == null)
        {
            cachedData = new BattleSaveData();
            cachedData.usedEvents = new List<string>();
        }

        // 🔥 FIX 2: đảm bảo list tồn tại
        if (cachedData.usedEvents == null)
        {
            cachedData.usedEvents = new List<string>();
        }

        if (!cachedData.usedEvents.Contains(id))
        {
            cachedData.usedEvents.Add(id);
        }
    }
    public void RestoreMapEvents()
    {
        if (cachedData == null) return;

        var used = cachedData.usedEvents;

        foreach (var item in FindObjectsByType<ItemAdd>(FindObjectsSortMode.None))
        {
            if (used.Contains(item.id))
            {
                item.gameObject.SetActive(false);
            }
        }

        foreach (var weapon in FindObjectsByType<WeaponAdd>(FindObjectsSortMode.None))
        {
            if (used.Contains(weapon.id))
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }
    void OnApplicationQuit()
    {
        TryAutoSave();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            TryAutoSave();
        }
    }
    void TryAutoSave()
    {
        if (TurnManager.Instance == null) return;

        if (TurnManager.Instance.isLoading) return;

        if (TurnManager.Instance.playerUnits.Count == 0) return;

        if (TurnManager.Instance.isEventRunning) return;
        // 🔥 FIX QUAN TRỌNG
        if (TurnManager.Instance.currentTurn == TurnManager.TurnState.EnemyTurn)
            return;

        Debug.Log("AUTO SAVE...");
        SaveGame();
    }
}