using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Character Data")]
    [SerializeField] private CharacterData characterData;
    public CharacterData CharacterData => characterData;

    public CharacterData.Stats baseStats;
    [SerializeField] private CharacterData.Stats actualStats;

    #region Current Stats Properties

    // current stats
    public float CurrentHealth { get; set; }
    public float CurrentRecovery { get; set; }
    public float CurrentMoveSpeed { get; set; }
    public float CurrentMight { get; set; }
    public float CurrentProjectileSpeed { get; set; }
    public float CurrentMagnet { get; set; }
    public float Luck { get; set; }

    #endregion

    // class for defining a level range and the corresponding experience cap increase for that range
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    [Header("References")]
    private PlayerInventory playerInventory;
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

        playerInventory = GetComponent<PlayerInventory>();

        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory component not found!");
        }

        // Asigna los valores base
        baseStats = actualStats = characterData.stats;

        // spawn the starting weapon
        playerInventory.SpawnWeapon(characterData.StartingWeapon);
    }

    private void Start()
    {
        // assign the variables
        CurrentHealth = characterData.stats.maxHealth;
        CurrentRecovery = characterData.stats.recovery;
        CurrentMoveSpeed = characterData.stats.moveSpeed;
        CurrentMight = characterData.stats.might;
        CurrentProjectileSpeed = characterData.stats.projectileSpeed;
        CurrentMagnet = characterData.stats.magnet;
        Luck = 1f; // si lo agregas luego a CharacterData, reemplázalo

        InitializeStatUI();
        UIManager.Instance.AssignChosenCharacterUI(characterData);
        UIManager.Instance.InitializeRuntimeUI(characterData, CurrentHealth);
    }

    private void InitializeStatUI()
    {
        if (UIManager.Instance == null) return;

        UIManager ui = UIManager.Instance;
        ui.currentHealthDisplay.text = $"Health: {CurrentHealth:F1}";
        ui.currentRecoveryDisplay.text = $"Recovery: {CurrentRecovery:F1}";
        ui.currentMoveSpeedDisplay.text = $"Move Speed: {CurrentMoveSpeed:F1}";
        ui.currentMightDisplay.text = $"Might: {CurrentMight:F1}";
        ui.currentProjectileSpeedDisplay.text = $"Projectile Speed: {CurrentProjectileSpeed:F1}";
        ui.currentMagnetDisplay.text = $"Magnet: {CurrentMagnet:F1}";
    }
}
