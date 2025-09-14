using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private CharacterScriptableObject characterData;

    // current stats
    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    float currentMagnet;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            // check if the value has changed
            if (currentHealth != value)
            {
                currentHealth = value;
                if (GameManager.Instance != null)
                {
                    UIManager.Instance.currentHealthDisplay.text = "Health: " + currentHealth;
                }
                // add any aditional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            // check if the value has changed
            if (currentRecovery != value)
            {
                currentRecovery = value;
                if (GameManager.Instance != null)
                {
                    UIManager.Instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
                }
                // add any aditional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            // check if the value has changed
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                if (GameManager.Instance != null)
                {
                    UIManager.Instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
                }
                // add any aditional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentMight
    {
        get { return currentMight; }
        set
        {
            // check if the value has changed
            if (currentMight != value)
            {
                currentMight = value;
                if (GameManager.Instance != null)
                {
                    UIManager.Instance.currentMightDisplay.text = "Might: " + currentMight;
                }
                // add any aditional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            // check if the value has changed
            if (currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
                if (GameManager.Instance != null)
                {
                    UIManager.Instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
                }
                // add any aditional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            // check if the value has changed
            if (currentMagnet != value)
            {
                currentMagnet = value;
                if (GameManager.Instance != null)
                {
                    UIManager.Instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;
                }
                // add any aditional logic here that needs to be executed when the value changes
            }
        }
    }
    #endregion

    public ParticleSystem damageEffect;

    // experience and level of the player
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    // class for defining a level range and the corresponding experience cap increase for that range
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    // I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    private float invincibilityTimer;
    private bool isInvincible;

    public List<LevelRange> levelRanges;

    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TextMeshProUGUI levelText;

    private void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();

        // assign the variables
        CurrentHealth = characterData.MaxHealth;
        CurrentRecovery = characterData.Recovery;
        CurrentMoveSpeed = characterData.MoveSpeed;
        CurrentMight = characterData.Might;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentMagnet = characterData.Magnet;

        // spawn the starting weapon
        SpawnWeapon(characterData.StartingWeapon);
    }

    private void Start()
    {
        // initialize the experience cap as the first experience cap increase
        experienceCap = levelRanges[0].experienceCapIncrease;

        // set the current stats display
        UIManager.Instance.currentHealthDisplay.text = "Health: " + currentHealth;
        UIManager.Instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
        UIManager.Instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
        UIManager.Instance.currentMightDisplay.text = "Might: " + currentMight;
        UIManager.Instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
        UIManager.Instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;

        //UIManager.Instance.AssignChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    private void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }

        Recover();
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();

        UpdateExpBar();
    }

    private void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;

            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }

            experienceCap += experienceCapIncrease;

            UpdateLevelText();

            GameManager.Instance.StartLevelUp();
        }
    }

    private void UpdateExpBar()
    {
        // update exp bal fill amount
        expBar.fillAmount = (float) experience / experienceCap;
    }

    private void UpdateLevelText()
    {
        // update level text
        levelText.text = "LV. " + level.ToString();
    }

    public void TakeDamage(float dmg)
    {
        // if the player is not invincible, apply damage and start invincibility
        if (!isInvincible)
        {
            CurrentHealth -= dmg;

            // if there is a damage effect assigned, play it
            if (damageEffect)
            {
                Instantiate(damageEffect, transform.position, Quaternion.identity);
            }

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if (CurrentHealth <= 0)
            {
                Kill();
            }

            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        // update the health bar
        healthBar.fillAmount = currentHealth / characterData.MaxHealth;
    }

    public void Kill()
    {
        if (!GameManager.Instance.isGameOver)
        {
            //UIManager.Instance.AssignLevelReachedUI(level);
            //UIManager.Instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponUISlots, inventory.passiveItemUISlots);
            //GameManager.Instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        // only heal the player if their current health is less than their maximum health
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += amount;

            // make sure the player's health doesn't exceed their maximum health
            if (CurrentHealth > characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
        }
    }

    private void Recover()
    {
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;

            // healing limit to avoid exceeding max health
            if (CurrentHealth > characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
        }
    }

    public void SpawnWeapon(GameObject weapon)
    {
        // checking if the slots are full, and returning if it is
        if (weaponIndex >= inventory.weaponSlots.Count - 1)
        {
            Debug.LogError("Inventory slots full");
            return;
        }

        // spawn the starting weapon
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform); // set the weapon to be child of the player
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); // add the weapon to it's inventory slot

        weaponIndex++;
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        // checking if the slots are full, and returning if it is
        if (passiveItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogError("Inventory slots full");
            return;
        }

        // spawn the starting weapon
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); // set the weapon to be child of the player
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>()); // add the weapon to it's inventory slot

        passiveItemIndex++;
    }
}
