using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    float health;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return health; }
        // If we try and set the current health, the UI interface on the pause screen will also be updated

        set
        {
            // check if the value has changed
            if (health != value)
            {
                health = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentHealthDisplay.text = string.Format("Health: {0} / {1}", health, actualStats.maxHealth);
                }
                // add any aditional logic here that needs to be executed when the value changes
            }
        }
    }

    public float MaxHealth
    {
        get { return actualStats.maxHealth; }
        // If we try and set the max health, the UI interface on the pause screen will also be updated

        set
        {
            // check if the value has changed
            if (actualStats.maxHealth != value)
            {
                actualStats.maxHealth = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentHealthDisplay.text = string.Format("Health: {0} / {1}", health, actualStats.maxHealth);
                }
                // Update the real time value of the stat
                // Add any additional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentRecovery
    {
        get { return Recovery; }
        set { CurrentRecovery = value; }
    }

    public float Recovery
    {
        get { return actualStats.recovery; }
        set
        {
            // check if the value has changed
            if (actualStats.recovery != value)
            {
                actualStats.recovery = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentRecoveryDisplay.text = "Recovery: " + actualStats.recovery;
                }
                // add any aditional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set { MoveSpeed = value; }
    }

    public float MoveSpeed
    {
        get { return actualStats.speed; }
        set
        {
            // check if the value has changed
            if (actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentMoveSpeedDisplay.text = "Move Speed: " + actualStats.moveSpeed;
                }
                // add any aditional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentMight
    {
        get { return Might; }
        set { Might = value; }
    }

    public float Might
    {
        get { return actualStats.might; }
        set
        {
            // check if the value has changed
            if (actualStats.might != value)
            {
                actualStats.might = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentMightDisplay.text = "Might: " + actualStats.might;
                }
                // add any aditional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return Speed; }
        set { Speed = value; }
    }

    public float Speed
    {
        get { return actualStats.speed; }
        set
        {
            // check if the value has changed
            if (actualStats.speed != value)
            {
                actualStats.speed = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + actualStats.speed;
                }
                // add any aditional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentMagnet
    {
        get { return Magnet; }
        set { Magnet = value; }
    }

    public float Magnet
    {
        get { return actualStats.magnet; }
        set
        {
            // check if the value has changed
            if (actualStats.magnet != value)
            {
                actualStats.magnet = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentMagnetDisplay.text = "Magnet: " + actualStats.magnet;
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

    PlayerInventory inventory;
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

        inventory = GetComponent<PlayerInventory>();

        // assign the variables
        baseStats = actualStats = characterData.stats;
        health = actualStats.maxHealth;
    }

    private void Start()
    {
        // Spawn the starting weapon
        inventory.Add(characterData.StartingWeapon);

        // initialize the experience cap as the first experience cap increase
        experienceCap = levelRanges[0].experienceCapIncrease;

        // set the current stats display
        GameManager.Instance.currentHealthDisplay.text = "Health: " + CurrentHealth;
        GameManager.Instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery;
        GameManager.Instance.currentMoveSpeedDisplay.text = "Move Speed: " + CurrentMoveSpeed;
        GameManager.Instance.currentMightDisplay.text = "Might: " + CurrentMight;
        GameManager.Instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + CurrentProjectileSpeed;
        GameManager.Instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet;

        GameManager.Instance.AssignChosenCharacterUI(characterData);

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

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
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
                Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);
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
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public void Kill()
    {
        if (!GameManager.Instance.isGameOver)
        {
            GameManager.Instance.AssignLevelReachedUI(level);
            GameManager.Instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        // only heal the player if their current health is less than their maximum health
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;

            // make sure the player's health doesn't exceed their maximum health
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
    }

    private void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;

            // healing limit to avoid exceeding max health
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
    }

    [System.Obsolete]
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
        //inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); // add the weapon to it's inventory slot

        weaponIndex++;
    }

    [System.Obsolete]
    public void SpawnPassiveItem(GameObject passiveItem)
    {
        // checking if the slots are full, and returning if it is
        if (passiveItemIndex >= inventory.passiveSlots.Count - 1)
        {
            Debug.LogError("Inventory slots full");
            return;
        }

        // spawn the starting weapon
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); // set the weapon to be child of the player
        //inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>()); // add the weapon to it's inventory slot

        passiveItemIndex++;
    }
}
