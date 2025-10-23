using UnityEngine;

/// <summary>
/// A class that takes a PassiveData and is used to increment a player's stat when received.
/// </summary>

public class Passive : Item
{
    public PassiveData data;
    [SerializeField] private CharacterData.Stats currentBoosts;

    [System.Serializable]
    public struct Modifier
    {
        public string name, description;
        public CharacterData.Stats boosts;
    }

    // for dinamically created passives, call iniutialise to set everything up
    public virtual void Initialise(PassiveData data)
    {
        base.Initialise(data);
        this.data = data;
        currentBoosts = data.baseStats.boosts;
    }

    public virtual CharacterData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    // levels up the weapon by 1, and calculates the corresponding stats
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        // prevent level up if we are already at max level
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("cannot level up {0} to level {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel));
            return false;
        }

        // otherwise, add stats of the next level to our weapon.
        currentBoosts += data.GetLevelData(++currentLevel).boosts;
        return true;    
    }
}
