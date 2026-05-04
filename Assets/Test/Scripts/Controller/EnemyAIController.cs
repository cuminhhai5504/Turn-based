using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class EnemyAIController : MonoBehaviour
{
    public Tilemap groundTilemap;
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

    public void StartEnemyTurn()
    {
        Debug.Log("=== ENEMY TURN START ===");
        StartCoroutine(EnemyTurnCoroutine());
    }

    IEnumerator EnemyTurnCoroutine()
    {
        List<Unit> enemies = TurnManager.Instance.enemyUnits;
        List<Unit> players = TurnManager.Instance.playerUnits;
        yield return new WaitForSeconds(1f);

        foreach (var enemy in enemies)
        {
            
            if (enemy == null) continue;
            yield return new WaitWhile(() => TurnManager.Instance.isUIBlocking);
            yield return new WaitForSeconds(1f);

            Unit target = FindNearestPlayer(enemy, players);

            if (target == null || !target.IsAlive()) continue;
            yield return new WaitWhile(() => TurnManager.Instance.isUIBlocking);

            // 👉 kiểm tra nếu đã trong tầm đánh
            Weapon weapon = GetBestWeapon(enemy, target);

            if (weapon != null)
            {
                yield return new WaitWhile(() => TurnManager.Instance.isUIBlocking);
                bool done = false;

                enemy.attackDamage = weapon.damage;
                enemy.lastUsedWeapon = weapon;
                enemy.Attack(target, () =>
                {
                    done = true;
                    enemy.hasActed = true;
                    enemy.UpdateVisual();
                });

                yield return new WaitUntil(() => done);
                yield return new WaitWhile(() => TurnManager.Instance.isUIBlocking);
                continue;
            }

            // 👉 tìm đường đơn giản
            List<Vector2Int> path = FindPathBFS(enemy, enemy.gridPosition, target.gridPosition, enemy.moveRange);
            

            if (path.Count > 0)
            {
                Debug.Log("Đi tới: " + path[0]); // 🔥 xem nó định đi đâu
            }
            else
            {
                Debug.Log("❌ Không tìm được path!");
            }
            if (path.Count > 0)
            {
                yield return new WaitWhile(() => TurnManager.Instance.isUIBlocking);
                bool done = false;

                enemy.MoveAlongPath(path, groundTilemap, () => done = true);

                yield return new WaitUntil(() => done);
                yield return new WaitWhile(() => TurnManager.Instance.isUIBlocking);
            }
            target = FindNearestPlayer(enemy, players);

            if (target == null || !target.IsAlive()) continue;
            yield return new WaitWhile(() => TurnManager.Instance.isUIBlocking);
            // 👉 kiểm tra lại sau khi move
            weapon = GetBestWeapon(enemy, target);

            if (weapon != null)
            {
                yield return new WaitWhile(() => TurnManager.Instance.isUIBlocking);
                bool done = false;

                enemy.attackDamage = weapon.damage;
                enemy.lastUsedWeapon = weapon;
                enemy.Attack(target, () =>
                {
                    done = true;
                    enemy.hasActed = true;
                    enemy.UpdateVisual();
                });

                yield return new WaitUntil(() => done);
                yield return new WaitWhile(() => TurnManager.Instance.isUIBlocking);
            }
            else
            {
                enemy.hasActed = true;
                enemy.UpdateVisual();
            }


        }
        // 🔥 CHỜ tất cả event (EXP + Promotion) hoàn toàn xong
        yield return new WaitWhile(() => TurnManager.Instance.isEventRunning);

        // 🔥 an toàn thêm 1 frame
        yield return null;

        // 🔥 Kết thúc lượt enemy
        TurnManager.Instance.EndEnemyTurn();
    }

    // ================= HELPER =================

    Unit FindNearestPlayer(Unit enemy, List<Unit> players)
    {
        Unit nearest = null;
        int minDist = int.MaxValue;

        foreach (var player in players)
        {
            if (player == null || !player.IsAlive()) continue;

            int dist = Distance(enemy.gridPosition, player.gridPosition);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = player;
            }
            
        }


        return nearest;
    }

    int Distance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    
    List<Vector2Int> FindPathBFS(Unit enemy, Vector2Int start, Vector2Int target, int moveRange)
    {

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();

        queue.Enqueue(start);
        distance[start] = 0;

        Vector2Int[] directions = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                if (distance.ContainsKey(next))
                    continue;

                if (!groundTilemap.HasTile((Vector3Int)next))
                    continue;

                if (next != start && HasUnitAt(next))
                    continue;

                int newDist = distance[current] + 1;

                if (newDist > moveRange)
                    continue;

                distance[next] = newDist;

                cameFrom[next] = current;
                queue.Enqueue(next);
            }
        }

        // 🔥 chọn ô gần target nhất (KHÔNG lấy start)
        Vector2Int best = start;
        bool foundAttackPosition = false;
        int bestDist = int.MaxValue;

        foreach (var pos in distance.Keys)
        {
            if (pos == start) continue;

            int distToTarget = Distance(pos, target);

            // 🔥 kiểm tra có weapon nào đánh được từ đây không
            foreach (var weapon in enemy.weapons)
            {
                if (distToTarget >= weapon.minRange && distToTarget <= weapon.maxRange)
                {
                    // ✅ Ưu tiên vị trí có thể attack
                    if (!foundAttackPosition || distToTarget < bestDist)
                    {
                        foundAttackPosition = true;
                        bestDist = distToTarget;
                        best = pos;
                    }
                }
            }
        }

        // ❌ nếu không có vị trí nào attack được → fallback (đuổi lại gần)
        if (!foundAttackPosition)
        {
            int minDist = Distance(start, target);

            foreach (var pos in distance.Keys)
            {
                if (pos == start) continue;

                int distToTarget = Distance(pos, target);

                if (distToTarget < minDist)
                {
                    minDist = distToTarget;
                    best = pos;
                }
            }
        }

        // 🔥 fallback nếu vẫn đứng im
        if (best == start)
        {
            foreach (var dir in directions)
            {
                Vector2Int next = start + dir;

                if (groundTilemap.HasTile((Vector3Int)next) && !HasUnitAt(next))
                {
                    return new List<Vector2Int> { next }; // đi 1 bước bất kỳ
                }
            }

            return new List<Vector2Int>(); // không đi được thật
        }

        // 🔥 build path
        List<Vector2Int> path = new List<Vector2Int>();

        Vector2Int currentPos = best;

        while (cameFrom.ContainsKey(currentPos))
        {
            path.Add(currentPos);
            currentPos = cameFrom[currentPos];
        }

        path.Reverse();
        return path;
    }
    Weapon GetBestWeapon(Unit attacker, Unit target)
    {
        Weapon bestWeapon = null;
        int bestDamage = -1;

        foreach (var weapon in attacker.weapons)
        {
            if (attacker.CanAttackTarget(target, weapon))
            {
                if (weapon.damage > bestDamage )
                {
                    bestDamage = weapon.damage;
                    bestWeapon = weapon;
                }
            }
        }

        return bestWeapon;
    }
}