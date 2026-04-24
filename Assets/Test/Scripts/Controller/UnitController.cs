using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using DG.Tweening;
public class UnitController : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap highlightMove;
    public Tilemap highlightAttack;
    public Tilemap highlightWay;
    public TileBase highlightTile;
    public TileBase pathTile;

    private Unit selectedUnit;
    private GameState currentState = GameState.Idle;
    private HashSet<Vector2Int> validMoveTiles = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
    public ActionMenuUI actionMenu;
    public bool isMenuOpen = false;
    bool isBusy = false;
    bool hasMovedThisTurn;
    Vector2Int startGridPos;
    Vector3 startWorldPos;
    
    public ItemMenuUI itemMenuUI;
    public ItemActionMenuUI itemActionMenuUI;

    public WeaponMenuUI weaponMenuUI;

    enum GameState
    {
        Idle,
        Moving,
        Attacking,
        UsingItem,

    }
    bool HasUnitAt(Vector2Int pos)
    {
        foreach (var unit in TurnManager.Instance.playerUnits)
        {
            if (unit != null && unit.gridPosition == pos)
                return true;
        }

        foreach (var unit in TurnManager.Instance.enemyUnits)
        {
            if (unit != null && unit.gridPosition == pos)
                return true;
        }

        return false;
    }
    Unit GetUnitAt(Vector2Int pos)
    {
        foreach (var u in TurnManager.Instance.playerUnits)
        {
            if (u != null && u.gridPosition == pos)
                return u;
        }

        foreach (var u in TurnManager.Instance.enemyUnits)
        {
            if (u != null && u.gridPosition == pos)
                return u;
        }

        return null;
    }
    bool HasEnemyInRange(Unit unit)
    {
        foreach (var enemy in TurnManager.Instance.enemyUnits)
        {
            if (enemy == null) continue;

            foreach (var weapon in unit.weapons)
            {
                if (unit.CanAttackTarget(enemy, weapon))
                {
                    return true; // 🔥 chỉ cần 1 weapon đánh được là OK
                }
            }
        }

        return false;
    }
    public bool CanAttackCurrentUnit()
    {
        if (selectedUnit == null) return false;

        foreach (var enemy in TurnManager.Instance.enemyUnits)
        {
            if (enemy == null) continue;

            foreach (var weapon in selectedUnit.weapons)
            {
                if (selectedUnit.CanAttackTarget(enemy, weapon))
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    void Update()
    {
        if (GameManager.Instance.IsGameOver()) return;
        if (TurnManager.Instance.isUIBlocking)
            return;
        if (TurnManager.Instance.currentTurn != TurnManager.TurnState.PlayerTurn)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
        if (selectedUnit != null && currentState == GameState.Moving && !isMenuOpen)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3Int cell = groundTilemap.WorldToCell(mouseWorld);
            Vector2Int pos = (Vector2Int)cell;

            if (validMoveTiles.Contains(pos))
            {
                ShowPath(pos);
            }
            else
            {
                highlightWay.ClearAllTiles();
            }
        }
    }

    void HandleClick()
    {
        if (TurnManager.Instance.currentTurn != TurnManager.TurnState.PlayerTurn)
            return;
        if (isBusy) return;
        if (isMenuOpen && EventSystem.current.IsPointerOverGameObject())
            return;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector3Int cell = groundTilemap.WorldToCell(mouseWorld);
        Vector2Int gridPos = (Vector2Int)cell;

        // 🔥 lấy unit tại tile
        Unit unit = GetUnitAt(gridPos);

        if (currentState == GameState.Attacking)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            Unit target = GetUnitAt(gridPos);

            if (target != null && target.isEnemy != selectedUnit.isEnemy)
            {
                OpenWeaponMenu(target);
                return;
            }
            UndoMove();          // 🔥 FIX
            CloseAllMenus();
            return;
        }
        if (isMenuOpen)
        { 
            // 👉 Click ra ngoài → auto cancel
            UndoMove();
            CloseAllMenus();
            return;
        }
        if (unit != null && !unit.isEnemy)
        {
            if (selectedUnit != null && selectedUnit != unit)
            {
                CancelSelection();
            }

            if (selectedUnit == unit && currentState == GameState.Moving)
            {
                OpenActionMenu(unit);
                return;
            }

            SelectUnit(unit);
            return;
        }

        // 🔵 Di chuyển
        if (selectedUnit != null && currentState == GameState.Moving)
        {
            if (hasMovedThisTurn) return;
            Vector3Int cellPos = groundTilemap.WorldToCell(mouseWorld);
            TryMove(cellPos);
        }

    }

    void SelectUnit(Unit unit)
    {
        if (unit.hasActed) return;
        selectedUnit = unit;
        startGridPos = unit.gridPosition;
        startWorldPos = unit.transform.position;
        currentState = GameState.Moving;
        hasMovedThisTurn = false;
        highlightMove.ClearAllTiles();

        ShowMoveRange(unit);
    }

    void ShowMoveRange(Unit unit)
    {   
        validMoveTiles.Clear();
        cameFrom.Clear();
        Queue<(Vector2Int pos, int dist)> queue = new Queue<(Vector2Int, int)>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue((unit.gridPosition, 0));
        visited.Add(unit.gridPosition);

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();

            if (dist > unit.moveRange)
                continue;

            Vector3Int cell = (Vector3Int)current;

            if (groundTilemap.HasTile(cell))
            {
                if (current != unit.gridPosition)
                {
                    // ❌ KHÔNG cho đứng lên ô có unit
                    if (!HasUnitAt(current))
                    {
                        highlightMove.SetTile(cell, highlightTile);
                        validMoveTiles.Add(current);
                    }
                }
            }

            // 4 hướng
            Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                if (visited.Contains(next))
                    continue;

                Unit unitAtNext = GetUnitAt(next);

                if (unitAtNext != null)
                {
                    // ❌ nếu là enemy → chặn
                    if (unitAtNext.isEnemy != unit.isEnemy)
                        continue;

                    // ✔ nếu là đồng minh → cho đi xuyên (không continue)
                }

                // 🔥 vẫn lan BFS
                visited.Add(next);
                cameFrom[next] = current;
                queue.Enqueue((next, dist + 1));
            }
        }
    }

    void TryMove(Vector3Int cellPos)
    {
        if (hasMovedThisTurn) return;
        Vector2Int targetPos = (Vector2Int)cellPos;

        // 🔥 CHỈ cho đi vào ô BFS hợp lệ
        if (!validMoveTiles.Contains(targetPos))
        {
            CancelSelection();
            return;
        }

        Vector3 worldPos = groundTilemap.GetCellCenterWorld(cellPos);

        List<Vector2Int> path = BuildPath(targetPos);

        highlightMove.ClearAllTiles();
        if (hasMovedThisTurn) return;
        
        selectedUnit.MoveAlongPath(path, groundTilemap, () =>
        {
            highlightWay.ClearAllTiles();
            hasMovedThisTurn = true;
            currentState = GameState.Idle; // 🔥 KHÔNG quay lại Moving

            OpenActionMenu(selectedUnit); // 🔥 LUÔN mở menu
        });
    }
    void ShowAttackableEnemies(Unit unit)
    {
        highlightAttack.ClearAllTiles();

        foreach (var enemy in TurnManager.Instance.enemyUnits)
        {
            if (enemy == null) continue;

            foreach (var weapon in unit.weapons)
            {
                if (unit.CanAttackTarget(enemy, weapon))
                {
                    highlightAttack.SetTile((Vector3Int)enemy.gridPosition, highlightTile);
                    break; // 🔥 chỉ cần 1 weapon là đủ
                }
            }
        }
    }
    
    void CancelSelection()
    {
        highlightMove.ClearAllTiles();
        highlightAttack.ClearAllTiles();
        actionMenu.Hide();
        selectedUnit = null;
        currentState = GameState.Idle;
    }
    List<Vector2Int> BuildPath(Vector2Int target)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        Vector2Int current = target;

        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }
    void ShowPath(Vector2Int target)
    {
        highlightWay.ClearAllTiles();

        List<Vector2Int> path = BuildPath(target);

        foreach (var step in path)
        {
            highlightWay.SetTile((Vector3Int)step, pathTile);
        }
    }
    public void OnAttackSelected()
    {
        if (!HasEnemyInRange(selectedUnit))
            return;
        currentState = GameState.Attacking;
        isMenuOpen = true; // 🔥 QUAN TRỌNG
        ShowAttackableEnemies(selectedUnit);
        
    }

    public void OnWaitSelected()
    {
        // 🔥 check tile event trước
        ItemAdd[] eventItem = FindObjectsByType<ItemAdd>(FindObjectsSortMode.None);
        WeaponAdd[] eventWeapon = FindObjectsByType<WeaponAdd>(FindObjectsSortMode.None);

        foreach (var e in eventItem)
        {
            if (e.position == selectedUnit.gridPosition)
            {
                e.Trigger(selectedUnit);
            }
        }
        foreach (var e in eventWeapon)
        {
            if (e.position == selectedUnit.gridPosition)
            {
                e.Trigger(selectedUnit);
            }
        }
        selectedUnit.hasActed = true;
        selectedUnit.UpdateVisual();
        CancelSelection();

        TurnManager.Instance.CheckEndPlayerTurn();
    }

    public void OnCancelSelected()
    {
        // 👉 quay lại trạng thái chọn lại
        highlightMove.ClearAllTiles();
        highlightAttack.ClearAllTiles();

        currentState = GameState.Moving;
        ShowMoveRange(selectedUnit);
    }
    void OpenActionMenu(Unit unit)
    {
        highlightWay.ClearAllTiles();
        highlightMove.ClearAllTiles();

        currentState = GameState.Idle;
        isMenuOpen = true;

        actionMenu.Show(unit.transform.position);
    }
    public void UndoMove()
    {
        if (selectedUnit == null) return;

        // 🔥 trả về vị trí cũ
        selectedUnit.transform.position = startWorldPos;
        selectedUnit.gridPosition = startGridPos;

        // 🔥 reset trạng thái
        hasMovedThisTurn = false;
        highlightMove.ClearAllTiles();
        highlightWay.ClearAllTiles();
        highlightAttack.ClearAllTiles();

        currentState = GameState.Moving;

        // 🔥 hiện lại vùng di chuyển
        ShowMoveRange(selectedUnit);

    }
    public void OnItemSelected()
    {
        if (selectedUnit == null) return;

        currentState = GameState.UsingItem;
        isMenuOpen = true;
        ShowItemMenu(selectedUnit);
        itemMenuUI.Show(selectedUnit);
    }
    void ShowItemMenu(Unit unit)
    {
        for (int i = 0; i < unit.inventory.Count; i++)
        {
            Debug.Log(i + ": " + unit.inventory[i].itemName);
        }


    }
    
    public void UseItemFromUI(int index)
    {
        Item item = selectedUnit.inventory[index];

        item.Use(selectedUnit);

        selectedUnit.inventory.RemoveAt(index);

        selectedUnit.hasActed = true;
        selectedUnit.UpdateVisual();
        if (itemMenuUI != null)
            itemMenuUI.Hide();

        if (itemActionMenuUI != null)
            itemActionMenuUI.Hide();
        actionMenu.Hide();
        CancelSelection();
        
        isMenuOpen = false;
        TurnManager.Instance.CheckEndPlayerTurn();
    }
    void OpenWeaponMenu(Unit target)
    {
        isMenuOpen = true;
        weaponMenuUI.Show(selectedUnit, target, this);
    }
    public void AttackWithWeapon(Unit target, Weapon weapon)
    {
        isBusy = true;
        selectedUnit.attackDamage = weapon.damage;
        selectedUnit.lastUsedWeapon = weapon;
        selectedUnit.Attack(target, () =>
        {
            // 🔥 TRỪ ĐỘ BỀN
            weapon.currentDurability--;

            // 🔥 nếu vỡ
            if (weapon.currentDurability <= 0)
            {
                selectedUnit.weapons.Remove(weapon);

                PickupPopupManager.Instance.ShowPopup(
                    weapon.weaponName + " broke!",
                    selectedUnit.transform.position
                );
            }

            selectedUnit.hasActed = true;
            selectedUnit.UpdateVisual();

            CancelSelection();
            CloseAllMenus();

            isBusy = false;
            TurnManager.Instance.CheckEndPlayerTurn();
        });

    }
    void CloseAllMenus()
    {
        actionMenu.Hide();
        
        if (itemMenuUI != null)
            itemMenuUI.Hide();

        if (itemActionMenuUI != null)
            itemActionMenuUI.Hide();

        if (weaponMenuUI != null)
            weaponMenuUI.Hide();

        isMenuOpen = false;
    }
}