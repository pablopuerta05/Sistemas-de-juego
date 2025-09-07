using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else
            {
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }
            Debug.Log(string.Format("Assigned {0} to player", item.name));
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool isEmpty()
        {
            return item == null;
        }
    }

    public List<slot> weaponSlots = new List<slot>(6);
    public List<slot> passiveSlots = new List<slot>(6);

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image UpgradeIcon;
        public Button UpgradeButton;
    }

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>(); // list of upgrade options for weapons
    public List<PassiveData> availablePassives = new List<PassiveData>(); // list of upgrade options for passive items
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>(); // list of UI for upgrade options present in the scene

    PlayerStats player;

    private void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    //  Checks if the inventory has an item of a certain type
    public bool Has(ItemData type)
    {
        return Get(type);
    }

    public Item Get(ItemData type)
    {
        if (type is WeaponData)
        {
            return Get(type as WeaponData);
        }
        else if (type is PassiveData)
        {
            return Get(type as PassiveData);
        }
        return null;
    }

    // Find a passive of a certain type in the inventory
    public Passive Get(PassiveData type)
    {
        foreach (slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p.data == type)
            {
                return p;
            }
        }
        return null;
    }

    // Find a weapon of a certain type in the inventory
    public Weapon Get(WeaponData type)
    {
        foreach (slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w.data == type)
            {
                return w;
            }
        }
        return null;
    }

    // Removes a weapon of a particular type, as specified by <data>
    public bool Remove(WeaponData data, bool removeUpgradeAvailability = false)
    {
        // Remove this weapon from the upgrade pool
        if (removeUpgradeAvailability)
        {
            availableWeapons.Remove(data);
        }

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if (w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }

        return false;
    }

    // Removes a passive of a particular type, as specified by <data>
    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        // Remove this weapon from the upgrade pool
        if (removeUpgradeAvailability)
        {
            availablePassives.Remove(data);
        }

        for (int i = 0; i < passiveSlots.Count; i++)
        {
            Passive p = passiveSlots[i].item as Passive;
            if (p.data == data)
            {
                passiveSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }

        return false;
    }

    // If an ItemData is passed, determine what type it is and call the respective overload
    // We also have an optional boolean to remove this item from the upgrade list
    public bool Remove(ItemData data, bool removeUpgradeAvailability = false)
    {
        if (data is PassiveData)
        {
            return Remove(data as PassiveData, removeUpgradeAvailability);
        }
        else if (data is WeaponData)
        {
            return Remove(data as WeaponData, removeUpgradeAvailability);
        }
        else
        {
            return false;
        }
    }

    // Finds an empty slot and adds a weapon of a certain type, returns the slot number that the item was put in
    public int Add(WeaponData data)
    {
        int slotNum = -1;

        // Try to find an empty slot
        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].isEmpty())
            {
                slotNum = i;
                break;
            }
        }

        // If there is no empty slot, exit
        if (slotNum < 0)
        {
            return slotNum;
        }

        // Otherwise create the weapon it the slot
        // Get the type of the weapon we want to spawn
        Type weaponType = Type.GetType(data.behaviour);

        if (weaponType != null)
        {
            // Spawn the weapon GameObject
            GameObject go = new GameObject(data.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.Initialise(data);
            spawnedWeapon.transform.SetParent(transform); // Set the weapon to be a child of the player
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.OnEquip();

            // Assign the weapon to the slot
            weaponSlots[slotNum].Assign(spawnedWeapon);

            // Close the level up UI if it is on
            if (GameManager.Instance != null && GameManager.Instance.choosingUpgrade)
            {
                GameManager.Instance.EndLevelUp();
            }
            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format("Invalid weapon type specified for {0}", data.name));
        }
        return -1;
    }

    // Finds an empty slot and adds a passive of a certain type, returns the slot number that the item was put in
    public int Add(PassiveData data)
    {
        int slotNum = -1;

        // Try to find an empty slot
        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].isEmpty())
            {
                slotNum = i;
                break;
            }
        }

        // If there is no empty slot, exit
        if (slotNum < 0)
        {
            return slotNum;
        }

        // Otherwise create the passive it the slot
        // Get the type of the passive we want to spawn
        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform); // Set the passive to be a child of the player
        p.transform.localPosition = Vector2.zero;

        // Assign the passive to the slot
        passiveSlots[slotNum].Assign(p);

        // Close the level up UI if it is on
        if (GameManager.Instance != null && GameManager.Instance.choosingUpgrade)
        {
            GameManager.Instance.EndLevelUp();
        }
        player.RecalculateStats();

        return slotNum;
    }

    // If we don't know what item is being added, this function will determine that
    public int Add(ItemData data)
    {
        if (data is WeaponData)
        {
            return Add(data as WeaponData);
        }
        else if (data is PassiveData)
        {
            return Add(data as PassiveData);
        }
        return -1;
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex].item as Weapon;
            if (!weapon.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}", weapon.name));
                return;
            }
        }

        if (GameManager.Instance != null && GameManager.Instance.choosingUpgrade)
        {
            GameManager.Instance.EndLevelUp();
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveSlots.Count > slotIndex)
        {
            Passive p = passiveSlots[slotIndex].item as Passive;
            if (!p.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}", p.name));
                return;
            }
        }

        if (GameManager.Instance != null && GameManager.Instance.choosingUpgrade)
        {
            GameManager.Instance.EndLevelUp();
        }
        player.RecalculateStats();
    }

    // Determines what upgrade options should appear
    private void ApplyUpgradeOptions()
    {
        // Make a duplicate of the available weapon / passive upgrade lists so we can iterate through them in the function
        List<WeaponData> availableWeaponUpgrades = new List<WeaponData>(availableWeapons);
        List<PassiveData> availablePassiveItemUpgrades = new List<PassiveData>(availablePassives);

        // Iterate through each slot in the upgrade UI
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            // If there are no more available upgrades, then we abort.
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                return;
            }

            // Determine whether this upgrade should be for passive or active weapons
            int upgradeType;

            if (availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                // Random generates a number between 1 and 2
                upgradeType = UnityEngine.Random.Range(1, 3);
            }

            // Generates an active weapon upgrade
            if (upgradeType == 1)
            {
                // Pick a weapon upgrade, then remove it so that we don't get it twice.
                WeaponData chosenWeaponUpgrade = availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);

                // Ensure that the selected weapon data is valid.
                if (chosenWeaponUpgrade != null)
                {
                    // Turns on the UI slot.
                    EnableUpgradeUI(upgradeOption);

                    // Loops throgh all our existing weapons. If we find a match, we will hook an event listener to the button that will level up the weapon
                    // when this upgrade option is clicked.
                    bool isLevelUp = false;

                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        Weapon w = weaponSlots[i].item as Weapon;

                        if (w != null && w.data == chosenWeaponUpgrade)
                        {
                            // If the weapon is already at the max level, don't allow the upgrade
                            if (chosenWeaponUpgrade.maxLevel <= w.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = false;
                                break;
                            }

                            // Set the Event Listener, item and level description to be that of the next level
                            upgradeOption.UpgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i)); // Apply button functionality
                            Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.UpgradeIcon.sprite = chosenWeaponUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    // If the code gets here, it means that we will be adding a new weapon, instead of upgrading an existing weapon.
                    if (!isLevelUp)
                    {
                        upgradeOption.UpgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade)); // Apply button functionality
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description; // Apply initial description
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name; // Apply initial name
                        upgradeOption.UpgradeIcon.sprite = chosenWeaponUpgrade.icon;
                    }
                }
            }
            else if (upgradeType == 2)
            {
                // NOTE: We have to recode this system, as right now it disables an upgrade slot if we hit a weapon that has already reached max level.
                PassiveData chosenPassiveUpgrade = availablePassiveItemUpgrades[UnityEngine.Random.Range(0, availablePassiveItemUpgrades.Count)];
                availablePassiveItemUpgrades.Remove(chosenPassiveUpgrade);

                if (chosenPassiveUpgrade != null)
                {
                    // Turns on the UI slot
                    EnableUpgradeUI(upgradeOption);

                    // Loops through all our existing passive. If we find a match, we will hook an event listener to the button that will level up the
                    // weapon when this upgrade option is clicked.
                    bool isLevelUp = false;
                    for (int i = 0; i < passiveSlots.Count; i++)
                    {
                        Passive p = passiveSlots[i].item as Passive;
                        if (p != null && p.data == chosenPassiveUpgrade)
                        {
                            // If the passive is already at the max level, do not allow upgrade.
                            if (chosenPassiveUpgrade.maxLevel <= p.currentLevel)
                            {
                                //DisableUpgradeUI(upgradeOption);
                                isLevelUp = false;
                                break;
                            }

                            upgradeOption.UpgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i)); // Apply button functionality
                            Passive.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.UpgradeIcon.sprite = chosenPassiveUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    if (!isLevelUp) // Spawn a new passive item
                    {
                        upgradeOption.UpgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade)); // Apply button functionality
                        Passive.Modifier nextLevel = chosenPassiveUpgrade.baseStats;
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description; // Apply initial description
                        upgradeOption.upgradeNameDisplay.text = nextLevel.name; // Apply initial name
                        upgradeOption.UpgradeIcon.sprite = chosenPassiveUpgrade.icon;
                    }
                }
            }
        }
    }

    private void RemoveUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.UpgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption); // Call the DisableUpgradeUI method here to disable all UI options before applying upgrades to them
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    private void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    private void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}
