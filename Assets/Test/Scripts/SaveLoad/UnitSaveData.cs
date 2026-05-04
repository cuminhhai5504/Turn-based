using System.Collections.Generic;

[System.Serializable]
public class UnitSaveData
{
    public string unitID;
    public int x;
    public int y;

    public int hp;
    public int maxHp;

    public int level;
    public int exp;
    public int expToNext;

    public bool hasActed;
    public bool isEnemy;
    public bool isUsed;

    public List<WeaponSaveData> weapons;
    public List<ItemSaveData> items;

}