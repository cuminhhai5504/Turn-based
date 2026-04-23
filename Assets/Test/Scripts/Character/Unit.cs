using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    
    public Vector2Int gridPosition;
    public bool isEnemy;
    public string unitName;
    public int moveRange;

    public int maxHP;
    public int currentHP;
    public int attackDamage;
    public int attackRange;
    public int level;
    public int currentEXP;
    public int expToNextLevel = 10;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public List<Item> inventory = new List<Item>();
    public List<Weapon> weapons = new List<Weapon>();
    public bool hasActed = false;
    public Weapon lastUsedWeapon;
    public UnitData data;
    Transform visualTransform;
    UnitEXPUI expUI;

    public void Init(UnitData data)
    {
        unitName = data.unitName;

        maxHP = data.maxHP;
        currentHP = data.maxHP;
        moveRange = data.moveRange;
        attackRange = data.attackRange;
        level = data.level;
        currentEXP = data.currentEXP;
        expToNextLevel = data.expToNextLevel;

        // COPY item (quan trọng: không dùng reference trực tiếp)
        inventory = new List<Item>(data.startItems);
        weapons = new List<Weapon>();

        foreach (var w in data.startWeapons)
        {
            weapons.Add(w.Clone());
        }
    }
    public bool IsAlive()
    {
        return currentHP > 0;
    }
    public bool CanAttackTarget(Unit target, Weapon weapon)
    {
        int dist = Mathf.Abs(target.gridPosition.x - gridPosition.x)
                 + Mathf.Abs(target.gridPosition.y - gridPosition.y);

        return dist >= weapon.minRange && dist <= weapon.maxRange;
    }

    void Awake()
    {
        visualTransform = transform.Find("Visual");
        animator = visualTransform.GetComponent<Animator>();
        spriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
        
    }

    void Start()
    {
        maxHP = data.maxHP;
        currentHP = maxHP;
        moveRange = data.moveRange;
        attackRange = data.attackRange;

        inventory = new List<Item>(data.startItems);
        weapons = new List<Weapon>();

        foreach (var w in data.startWeapons)
        {
            Weapon clone = new Weapon();

            clone.weaponName = w.weaponName;
            clone.damage = w.damage;
            clone.minRange = w.minRange;
            clone.maxRange = w.maxRange;

            clone.maxDurability = w.maxDurability;
            clone.currentDurability = w.maxDurability; // 🔥 reset sạch

            weapons.Add(clone);
        }
        expUI = GetComponentInChildren<UnitEXPUI>();
    }

    // =========================
    // 📍 POSITION
    // =========================
    public void SetPosition(Vector2Int pos)
    {
        gridPosition = pos;
    }
    public void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        Color c = spriteRenderer.color;

        if (hasActed)
            c.a = 0.5f; // 🔥 mờ
        else
            c.a = 1f;   // bình thường

        spriteRenderer.color = c;
    }
    void FaceTarget(Vector3 targetPos)
    {
        if (targetPos.x > transform.position.x)
            visualTransform.localScale = new Vector3(1, 1, 1);
        else
            visualTransform.localScale = new Vector3(-1, 1, 1);
    }

    // =========================
    // 🚶 MOVE THEO PATH (QUAN TRỌNG)
    // =========================
    public void MoveAlongPath(List<Vector2Int> path, Tilemap tilemap, System.Action onComplete)
    {
        StartCoroutine(MovePathCoroutine(path, tilemap, onComplete));
    }

    IEnumerator MovePathCoroutine(List<Vector2Int> path, Tilemap tilemap, System.Action onComplete)
    {
        animator.SetBool("isMoving", true);

        float speed = 5f;

        foreach (var step in path)
        {
            Vector3 target = tilemap.GetCellCenterWorld((Vector3Int)step);

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                // 👉 quay mặt theo hướng di chuyển
                if (target.x > transform.position.x)
                    visualTransform.localScale = new Vector3(1, 1, 1);
                else
                    visualTransform.localScale = new Vector3(-1, 1, 1);

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            transform.position = target;

            // 👉 cập nhật grid theo từng bước
            gridPosition = step;
        }

        animator.SetBool("isMoving", false);
        onComplete?.Invoke();
    }

    // =========================
    // ⚔️ ATTACK
    // =========================
    public void Attack(Unit target, System.Action onComplete)
    {
        Debug.Log("ATTACK CALLED: " + name + " -> " + target.name);
        StartCoroutine(AttackCoroutine(target, onComplete));
    }

    IEnumerator AttackCoroutine(Unit target, System.Action onComplete)
    {
        FaceTarget(target.transform.position);
        // 🔥 Đánh chính
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.3f);

        target.TakeDamage(attackDamage, this);
        
        // 🔥 CHỜ 1 chút cho mượt
        yield return new WaitForSeconds(0.7f);

        // =========================
        // 🔥 COUNTER ATTACK
        // =========================

        // Nếu target chết → không phản
        if (target == null || target.currentHP <= 0)
        {
            onComplete?.Invoke();
            yield break; 
        }

        // Tính khoảng cách
        Weapon counterWeapon = target.GetCounterWeapon(this);

        if (counterWeapon != null)
        {
            target.FaceTarget(transform.position);
            target.animator.SetTrigger("Attack");

            yield return new WaitForSeconds(0.3f);

            TakeDamage(counterWeapon.damage, target);

            // 🔥 TRỪ ĐỘ BỀN KHI PHẢN ĐÒN
            counterWeapon.currentDurability--;

            // 🔥 nếu vỡ
            if (counterWeapon.currentDurability <= 0)
            {
                target.weapons.Remove(counterWeapon);

                PickupPopupManager.Instance.ShowPopup(
                    counterWeapon.weaponName + " broke!",
                    target.transform.position
                );
            }
        }
        onComplete?.Invoke();
    }
    
    Weapon GetCounterWeapon(Unit attacker)
    {
        // 👉 1. ưu tiên weapon đã dùng
        if (lastUsedWeapon != null)
        {
            if (CanAttackTarget(attacker, lastUsedWeapon))
                return lastUsedWeapon;
        }

        // 👉 2. fallback weapon đầu
        if (weapons.Count > 0)
        {
            if (CanAttackTarget(attacker, weapons[0]))
                return weapons[0];
        }

        return null;
    }
    // =========================
    // ❤️ DAMAGE
    // =========================
    public void TakeDamage(int damage, Unit attacker = null)
    {
        currentHP -= damage;
        PlayHitFlash();
        DamageTextManager.Instance.ShowDamage(damage, transform.position);
        if (currentHP <= 0)
        {
            Die(attacker);
        }
        Debug.Log(name + " took damage: " + damage);
    }
    // =========================
    // ❤️ EXP
    // =========================
    public void GainEXP(int amount)
    {
        if (isEnemy) return;
        StartCoroutine(GainEXPCoroutine(amount));
    }
    IEnumerator GainEXPCoroutine(int amount)
    {
        int expGained = amount;

        while (expGained > 0)
        {
            int expNeeded = expToNextLevel - currentEXP;

            // 👉 đủ để level up
            if (expGained >= expNeeded)
            {
                // 🔥 Phase 1: chạy tới full
                if (expUI != null)
                {
                    yield return expUI.AnimateEXP(currentEXP, expToNextLevel, expToNextLevel);
                }

                expGained -= expNeeded;

                // 🔥 LEVEL UP
                currentEXP = 0;
                LevelUp();

                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                // 🔥 Phase 2: chạy phần dư
                if (expUI != null)
                {
                    yield return expUI.AnimateEXP(currentEXP, currentEXP + expGained, expToNextLevel);
                }

                currentEXP += expGained;
                expGained = 0;
            }

        }
        if (expUI != null)
        {
            StartCoroutine(expUI.HideAfter());
        }
    }
    void LevelUp()
    {
        if (isEnemy) return;
        var oldStats = GetStats();
        level++;
        if (level == 2 && data.unitName == "Recruit")
        {
            
            TurnManager.Instance.isUIBlocking = true;
            PromotionUI.Instance.Show(this);

            return;
        }
        maxHP += 2;
        expToNextLevel += 5;
        currentHP = maxHP;
        
        var newStats = GetStats();
        LevelUpUI ui = FindFirstObjectByType<LevelUpUI>();
        if (ui != null)
        {
            
            ui.ShowLevelUp(oldStats, newStats);
        }
        
        Debug.Log(name + " LEVEL UP! → Lv " + level);
        TurnManager.Instance.CheckEndPlayerTurn();
        GameManager.Instance.CheckGameResult();
    }
    public class UnitStatsSnapshot
    {
        public string unitName;
        public int level;
        public int currentHP;
        public int maxHP;
        public int moveRange;
        public int currentExp;
        public int expToNext;
    }
    UnitStatsSnapshot GetStats()
    {
        return new UnitStatsSnapshot
        {
            unitName = data.unitName,
            level = level,
            currentHP = currentHP,
            maxHP = maxHP,
            moveRange = moveRange,
            currentExp = currentEXP,
            expToNext = expToNextLevel
        };
    }
    public void Promote(GameObject newPrefab)
    {
        Vector2Int pos = gridPosition;
        bool enemy = isEnemy;
        bool acted = hasActed;
        // Lấy world position từ tilemap
        Vector3 worldPos = FindFirstObjectByType<UnityEngine.Tilemaps.Tilemap>()
            .GetCellCenterWorld((Vector3Int)pos);

        // Spawn unit mới
        GameObject newUnitObj = Instantiate(newPrefab, worldPos, Quaternion.identity);

        Unit newUnit = newUnitObj.GetComponent<Unit>();

        // Gán lại thông tin cơ bản
        newUnit.SetPosition(pos);
        newUnit.isEnemy = enemy;
        TurnManager.Instance.RemoveUnit(this);
        TurnManager.Instance.AddUnit(newUnit);
        newUnit.hasActed = true;
        newUnit.UpdateVisual();
        // ❗ Xóa unit cũ
        Destroy(gameObject);
    }
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) // chuột phải
        {
            UnitInfoUI.Instance.Show(this);
            UnitEXPUI expUI = GetComponentInChildren<UnitEXPUI>(); // 🔥 lấy lại
            
            if (expUI != null)
            {
                expUI.ShowInstant(currentEXP, expToNextLevel);

                // 🔥 tự ẩn sau 2s
                StartCoroutine(expUI.HideAfter());
            }
        }
    }
    void PlayHitFlash()
    {
        if (spriteRenderer == null) return;

        // 🔥 flash trắng
        spriteRenderer.color = Color.white;

        spriteRenderer.DOColor(new Color(1, 1, 1, 0.3f), 0.05f)
            .SetLoops(3, LoopType.Yoyo);

        // 🔥 giật nhẹ (knock)
        transform.DOShakePosition(0.2f, 0.2f, 10, 90, false, true);

        // 🔥 scale punch (cảm giác impact)
        transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1);

        // 🔥 reset màu
        DOVirtual.DelayedCall(0.2f, () =>
        {
            UpdateVisual();
        });
    }


    void Die(Unit killer = null)
    {
        if (killer != null)
        {
            killer.GainEXP(15); 
        }
        StartCoroutine(DieCoroutine());
    }
    IEnumerator DieCoroutine()
    {
        // 🔥 cho hit effect chạy xong
        yield return new WaitForSeconds(0.25f);

        transform.DOKill();
        spriteRenderer.DOKill();

        Destroy(gameObject);
    }
    public void ResetTurn()
    {
        hasActed = false;
        UpdateVisual();
    }
}