using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public TurnUI turnUI;
    public EndTurnUI endTurnUI;
    public int turnCount = 1;
    
    public bool isUIBlocking = false;
    public bool isEventRunning = false;
    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
        Busy
    }
    public void SetState(TurnState newState)
    {
        currentTurn = newState;
    }
    public TurnState currentTurn;

    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();

    public EnemyAIController enemyAI;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentTurn = TurnState.Busy;
    }

    void Start()
    {
        Invoke(nameof(InitUnits), 0.1f);
    }
    void InitUnits()
    {
        FindAllUnits();
        StartPlayerTurn();
    }


    // 🔍 Tự động tìm toàn bộ unit trong scene
    void FindAllUnits()
    {
        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);

        playerUnits.Clear();
        enemyUnits.Clear();

        foreach (var unit in allUnits)
        {
            if (unit.isEnemy)
                enemyUnits.Add(unit);
            else
                playerUnits.Add(unit);
        }
    }

    // ================= PLAYER TURN =================

    public void StartPlayerTurn()
    {
        currentTurn = TurnState.Busy; // 🔥 khóa input

        turnUI.ShowTurn(turnCount);

        foreach (var unit in playerUnits)
        {
            if (unit != null)
                unit.ResetTurn();
        }

        StartCoroutine(StartTurnDelay());
    }
    IEnumerator StartTurnDelay()
    {
        yield return new WaitForSeconds(1f); // thời gian UI "Lượt X"

        currentTurn = TurnState.PlayerTurn; // 🔓 mở input

        Debug.Log("PLAYER TURN");
    }


    // 👉 gọi sau mỗi action của player
    public void CheckEndPlayerTurn()
    {
        if (currentTurn != TurnState.PlayerTurn)
            return;
        if (GameManager.Instance.IsGameOver()) return;
        foreach (var unit in playerUnits)
        {
            if (unit != null && !unit.hasActed)
                return; // vẫn còn unit chưa dùng
        }
        EndPlayerTurn();

    } 

    public void EndPlayerTurn()
    {
        if (GameManager.Instance.IsGameOver()) return;
        currentTurn = TurnState.EnemyTurn;

        Debug.Log("ENEMY TURN");

        StartEnemyTurn();
    }

    // ================= ENEMY TURN =================

    void StartEnemyTurn()
    {
        if (GameManager.Instance.IsGameOver()) return;
        currentTurn = TurnState.EnemyTurn;
        endTurnUI.ShowEndTurn(turnCount);
        foreach (var unit in enemyUnits)
        {
            if (unit != null)
                unit.ResetTurn();
        }

        enemyAI.StartEnemyTurn();
    }

    // 👉 gọi khi AI xong
    public void EndEnemyTurn()
    {
        if (currentTurn != TurnState.EnemyTurn)
            return;
        if (GameManager.Instance.IsGameOver()) return;
        turnCount++;
        StartPlayerTurn();
        
    }
    public void AddUnit(Unit unit)
    {
        if (unit.isEnemy)
            enemyUnits.Add(unit);
        else
            playerUnits.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        playerUnits.Remove(unit);
        enemyUnits.Remove(unit);
    }
    
}