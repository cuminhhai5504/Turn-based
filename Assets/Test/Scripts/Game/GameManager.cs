using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameState
{
    Playing,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public GameState currentState = GameState.Playing;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentState = GameState.Playing;
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentState = GameState.Playing;
        SaveLoadManager.Instance.RestoreMapEvents();
    }
    public bool IsGameOver()
    {
        return currentState != GameState.Playing;
    }
    public void CheckGameResult()
    {
        
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        foreach (var unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if (unit.isEnemy && unit.IsAlive())
                allEnemiesDead = false;

            if (!unit.isEnemy && unit.IsAlive())
                allPlayersDead = false;
        }

        if (allEnemiesDead)
        {
            WinGame();
        }
        else if (allPlayersDead)
        {
            LoseGame();
        }
    }
    public void SaveProgressAfterBattle()
    {
        PlayerProgressData data = new PlayerProgressData();
        data.playerUnits = new List<UnitSaveData>();

        foreach (var unit in TurnManager.Instance.playerUnits)
        {
            if (unit != null && unit.IsAlive())
            {
                data.playerUnits.Add(unit.GetSaveData());
            }
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PROGRESS_DATA", json);
        PlayerPrefs.Save();

        Debug.Log("Progress Saved!");
    }
    void WinGame()
    {
        TurnManager.Instance.isUIBlocking = true;
        currentState = GameState.Win;
        // 🔥 Lấy level hiện tại
        int currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);

        // 🔥 Level đã mở
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // 🔥 Nếu thắng level cao nhất đã mở → mở tiếp
        if (currentLevel >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            PlayerPrefs.Save();
        }
        SaveProgressAfterBattle();
        Debug.Log("Victory!");
        UIManager.Instance.ShowResult("Victory!");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfx.win);
    }

    void LoseGame()
    {
        TurnManager.Instance.isUIBlocking = true;
        currentState = GameState.Lose;
        Debug.Log("Defeat!");
        UIManager.Instance.ShowResult("Defeat!");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfx.lose);
    }
    
}