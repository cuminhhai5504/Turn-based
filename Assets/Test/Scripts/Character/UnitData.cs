using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/UnitData")]
public class UnitData : ScriptableObject
{
    
    public string unitName;
    public string id;
    public int maxHP;
    public int currentHP;
    public int moveRange;
    public int attackRange;
    public int level;
    public int currentEXP;
    public int expToNextLevel;

    public List<Item> startItems;
    public List<Weapon> startWeapons;
    
}