using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField] private float multiplier;
    public float Multiplier { get =>  multiplier; set => multiplier = value; }

    [SerializeField] private int level;
    public int Level { get => level; set => level = value; }

    [SerializeField] private GameObject nextLevelPrefab; // the prefab of the next level i.e. what the object becomes when it levels up
    // not to be confused with the prefab to be spawned at the next level
    public GameObject NextLevelPrefab { get => nextLevelPrefab; set => nextLevelPrefab = value; }

    [SerializeField] private Sprite icon;
    public Sprite Icon { get => icon; set => icon = value; }

    [SerializeField] new string name;
    public string Name { get => name; set => name = value; }

    [SerializeField] string description; // what is the description of this weapon? (if this weapon is an upgrade, place the description of the upgrades)
    public string Description { get => description; set => description = value; }
}
