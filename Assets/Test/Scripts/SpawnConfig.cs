using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Spawn Config")]
public class SpawnConfig : ScriptableObject
{
    public List<SpawnUnitData> units;
}