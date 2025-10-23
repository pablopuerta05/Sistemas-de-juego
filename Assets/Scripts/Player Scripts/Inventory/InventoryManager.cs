using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<Weapon> weaponSlots = new List<Weapon>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(6);

    public List<Passive> passiveItemSlots = new List<Passive>(6);
    public int[] passiveItemLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(6);

    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject initialWeapon;
        public WeaponData WeaponData;
    }

    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem;
        public PassiveData PassiveItemData;
    }

    [System.Serializable]
    public class UpgradeUI
    {
        public TextMeshProUGUI upgradeNameDisplay;
        public TextMeshProUGUI upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>();
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>();
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    private PlayerStats player;
    private PlayerInventory playerInventory;

    private void Start()
    {
        player = GetComponent<PlayerStats>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnLevelUpApplied += RemoveAndApplyUpgrades;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnLevelUpApplied -= RemoveAndApplyUpgrades;
    }

    public void AddWeapon(int slotIndex, Weapon weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.currentLevel;
        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].sprite = weapon.data.icon;
        ExitLevelUpStateIfNeeded();
    }

    public void AddPassiveItem(int slotIndex, Passive passiveItem)
    {
        passiveItemSlots[slotIndex] = passiveItem;
        passiveItemLevels[slotIndex] = passiveItem.currentLevel;
        passiveItemUISlots[slotIndex].enabled = true;
        passiveItemUISlots[slotIndex].sprite = passiveItem.data.icon;
        ExitLevelUpStateIfNeeded();
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex];

            if (!weapon.DoLevelUp())
            {
                Debug.LogWarning("This weapon is already at max level.");
                return;
            }

            weaponLevels[slotIndex] = weapon.currentLevel;
            weaponUpgradeOptions[upgradeIndex].WeaponData = weapon.data;
            ExitLevelUpStateIfNeeded();
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveItemSlots.Count > slotIndex)
        {
            Passive passive = passiveItemSlots[slotIndex];

            if (!passive.DoLevelUp())
            {
                Debug.LogWarning("This passive item is already at max level.");
                return;
            }

            passiveItemLevels[slotIndex] = passive.currentLevel;
            passiveItemUpgradeOptions[upgradeIndex].PassiveItemData = passive.data;
            ExitLevelUpStateIfNeeded();
        }
    }

    private void ApplyUpgradeOptions()
    {
        List<WeaponUpgrade> availableWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItemUpgrades = new List<PassiveItemUpgrade>(passiveItemUpgradeOptions);

        foreach (var upgradeOption in upgradeUIOptions)
        {
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0) return;

            int upgradeType = (availableWeaponUpgrades.Count == 0) ? 2 :
                              (availablePassiveItemUpgrades.Count == 0) ? 1 :
                              Random.Range(1, 3);

            if (upgradeType == 1)
            {
                WeaponUpgrade chosen = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(chosen);

                if (chosen != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool isNew = true;

                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        if (weaponSlots[i] != null && weaponSlots[i].data == chosen.WeaponData)
                        {
                            isNew = false;

                            if (!weaponSlots[i].CanLevelUp())
                            {
                                DisableUpgradeUI(upgradeOption);
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, chosen.weaponUpgradeIndex));
                            upgradeOption.upgradeDescriptionDisplay.text = "Sube a nivel " + (weaponSlots[i].currentLevel + 1);
                            upgradeOption.upgradeNameDisplay.text = chosen.WeaponData.name;
                            break;
                        }
                    }

                    if (isNew)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => playerInventory.SpawnWeapon(chosen.initialWeapon));
                        upgradeOption.upgradeDescriptionDisplay.text = chosen.WeaponData.baseStats.description;
                        upgradeOption.upgradeNameDisplay.text = chosen.WeaponData.name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosen.WeaponData.icon;
                }
            }
            else if (upgradeType == 2)
            {
                PassiveItemUpgrade chosen = availablePassiveItemUpgrades[Random.Range(0, availablePassiveItemUpgrades.Count)];
                availablePassiveItemUpgrades.Remove(chosen);

                if (chosen != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool isNew = true;

                    for (int i = 0; i < passiveItemSlots.Count; i++)
                    {
                        if (passiveItemSlots[i] != null && passiveItemSlots[i].data == chosen.PassiveItemData)
                        {
                            isNew = false;

                            if (!passiveItemSlots[i].CanLevelUp())
                            {
                                DisableUpgradeUI(upgradeOption);
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, chosen.passiveItemUpgradeIndex));
                            upgradeOption.upgradeDescriptionDisplay.text = "Sube a nivel " + (passiveItemSlots[i].currentLevel + 1);
                            upgradeOption.upgradeNameDisplay.text = chosen.PassiveItemData.name;
                            break;
                        }
                    }

                    if (isNew)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => playerInventory.SpawnPassiveItem(chosen.initialPassiveItem));
                        upgradeOption.upgradeDescriptionDisplay.text = chosen.PassiveItemData.baseStats.description;
                        upgradeOption.upgradeNameDisplay.text = chosen.PassiveItemData.baseStats.name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosen.PassiveItemData.icon;
                }
            }
        }
    }

    private void RemoveUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
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

    private void ExitLevelUpStateIfNeeded()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState.gameState == GameManager.GameState.LevelUp)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.Gameplay);
        }
    }
}
