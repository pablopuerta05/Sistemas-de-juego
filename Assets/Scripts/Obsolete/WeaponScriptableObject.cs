using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Replaced for Weapon Data")]
[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    public GameObject Prefab { get => prefab; set => prefab = value; }

    // base stats for weapons
    [SerializeField] private float damage;
    public float Damage { get => damage; set => damage = value; }

    [SerializeField] private float speed;
    public float Speed { get => speed; set => speed = value; }

    [SerializeField] private float cooldownDuration;
    public float CooldownDuration { get => cooldownDuration; set => cooldownDuration = value; }

    [SerializeField] private int pierce;
    public int Pierce { get => pierce; set => pierce = value; }

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
