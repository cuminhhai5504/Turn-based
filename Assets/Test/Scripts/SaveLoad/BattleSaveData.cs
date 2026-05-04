using System.Collections.Generic;

[System.Serializable]
public class BattleSaveData
{
    public List<UnitSaveData> units;
    public List<string> usedEvents = new List<string>();
    public string sceneName;
    public int turnNumber;
    public string currentPhase; // "Player" / "Enemy"
}