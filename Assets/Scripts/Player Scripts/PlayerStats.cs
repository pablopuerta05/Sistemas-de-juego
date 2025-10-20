using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private CharacterData characterData;

    // current stats
    private float currentHealth;
    private float currentRecovery;
    private float currentMoveSpeed;
    private float currentMight;
    private float currentProjectileSpeed;
    private float currentMagnet;
    private float luck;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return currentHealth; }
        set { UpdateStat(ref currentHealth, value, UIManager.Instance.currentHealthDisplay, "Health"); }
    }

    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set { UpdateStat(ref currentRecovery, value, UIManager.Instance.currentRecoveryDisplay, "Recovery"); }
    }

    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set { UpdateStat(ref currentMoveSpeed, value, UIManager.Instance.currentMoveSpeedDisplay, "Move Speed"); }
    }

    public float CurrentMight
    {
        get { return currentMight; }
        set { UpdateStat(ref currentMight, value, UIManager.Instance.currentMightDisplay, "Might"); }
    }

    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set { UpdateStat(ref currentProjectileSpeed, value, UIManager.Instance.currentProjectileSpeedDisplay, "Projectile Speed"); }
    }

    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set { UpdateStat(ref currentMagnet, value, UIManager.Instance.currentMagnetDisplay, "Magnet"); }
    }

    public float Luck { get { return luck; } }

    #endregion

    // class for defining a level range and the corresponding experience cap increase for that range
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    PlayerInventory playerInventory;
    public int weaponIndex;
    public int passiveItemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TextMeshProUGUI levelText;

    private void Awake()
    {
        characterData = CharacterSelector.GetData();

        if (characterData == null)
        {
            Debug.LogError("Character data not found!");
            return;
        }

        CharacterSelector.instance.DestroySingleton();

        if (characterData == null)
        {
            Debug.LogWarning("Character data not found, loading default character...");
            //characterData = Resources.Load<CharacterData>("DefaultCharacterData"); // toma un scriptable object default
        }

        playerInventory = GetComponent<PlayerInventory>();

        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory component not found!");
        }

        // assign the variables
        CurrentHealth = characterData.stats.maxHealth;
        CurrentRecovery = characterData.stats.recovery;
        CurrentMoveSpeed = characterData.stats.moveSpeed;
        CurrentMight = characterData.stats.might;
        CurrentProjectileSpeed = characterData.stats.projectileSpeed;
        CurrentMagnet = characterData.stats.magnet;

        // spawn the starting weapon
        //playerInventory.SpawnWeapon(characterData.StartingWeapon); //bugged cannot convert from Weapon class to GO class!!
    }

    private void Start()
    {
        InitializeStatUI();
        UIManager.Instance.AssignChosenCharacterUI(characterData);
    }

    private void InitializeStatUI()
    {
        UIManager ui = UIManager.Instance;
        ui.currentHealthDisplay.text = $"Health: {currentHealth:F1}";
        ui.currentRecoveryDisplay.text = $"Recovery: {currentRecovery:F1}";
        ui.currentMoveSpeedDisplay.text = $"Move Speed: {currentMoveSpeed:F1}";
        ui.currentMightDisplay.text = $"Might: {currentMight:F1}";
        ui.currentProjectileSpeedDisplay.text = $"Projectile Speed: {currentProjectileSpeed:F1}";
        ui.currentMagnetDisplay.text = $"Magnet: {currentMagnet:F1}";
    }

    private void UpdateStat(ref float stat, float newValue, TextMeshProUGUI display, string statName)
    {
        if (stat != newValue)
        {
            stat = newValue;
            if (UIManager.Instance != null && display != null)
            {
                display.text = $"{statName}: {stat:F1}"; // con un decimal, queda más prolijo
            }
        }
    }
}
