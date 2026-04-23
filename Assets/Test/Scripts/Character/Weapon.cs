[System.Serializable]
public class Weapon
{
    public string weaponName;
    public string id;
    public int minRange; // ví dụ 1
    public int maxRange; // ví dụ 1 hoặc 2
    public int damage;
    public int maxDurability;
    public int currentDurability;

    public Weapon Clone()
    {
        return new Weapon
        {
            weaponName = this.weaponName,
            id = this.id,
            damage = this.damage,
            minRange = this.minRange,
            maxRange = this.maxRange,
            maxDurability = this.maxDurability,
            currentDurability = this.maxDurability
        };
    }
}