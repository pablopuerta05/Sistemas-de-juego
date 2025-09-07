using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Replacement for the PassiveItemScriptableObject class. The idea is we want to store all passive item level data in one single object, instead of having multiple objects
/// to store a single passive item, which is what we would have had to do if we continued using PassiveItemScriptableObject.
/// </summary>

[CreateAssetMenu(fileName = "Passive Data", menuName = "2D Top-Down Rogue-like/Passive Data")]
public class PassiveData : ItemData
{
    public Passive.Modifier baseStats;
    public Passive.Modifier[] growth;

    public Passive.Modifier GetLevelData(int level)
    {
        // pick the stats from the next level
        if (level - 2 < growth.Length)
        {
            return growth[level - 2];
        }

        // return an empty value and a warning
        Debug.LogWarning(string.Format("Passive doesn't have its level up stats configured for level {0}.", level));
        return new Passive.Modifier();
    }
}
