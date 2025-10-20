using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats playerStats;
    private PlayerExperience playerExperience;
    private CharacterData characterData;
    private InventoryManager inventory;
    public ParticleSystem damageEffect;

    // I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    private float invincibilityTimer;
    private bool isInvincible;

    private void Awake()
    {
        characterData = CharacterSelector.GetData();

        if (characterData == null)
        {
            Debug.LogError("Character data not found!");
            return;
        }

        CharacterSelector.instance.DestroySingleton();

        playerStats = GetComponent<PlayerStats>();
        inventory = GetComponent<InventoryManager>();
        playerExperience = GetComponent<PlayerExperience>();
    }

    private void Start()
    {
        UpdateHealthBar();
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            playerStats.CurrentHealth -= dmg;

            if (damageEffect)
            {
                Instantiate(damageEffect, transform.position, Quaternion.identity);
            }

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if (playerStats.CurrentHealth <= 0)
            {
                Kill();
            }

            UpdateHealthBar();
        }
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

    public void RestoreHealth(float amount)
    {
        // only heal the player if their current health is less than their maximum health
        if (playerStats.CurrentHealth < characterData.stats.maxHealth)
        {
            playerStats.CurrentHealth += amount;

            // make sure the player's health doesn't exceed their maximum health
            if (playerStats.CurrentHealth > characterData.stats.maxHealth)
            {
                playerStats.CurrentHealth = characterData.stats.maxHealth;
            }
        }
    }

    private void Recover()
    {
        if (playerStats.CurrentHealth < characterData.stats.maxHealth)
        {
            playerStats.CurrentHealth += playerStats.CurrentRecovery * Time.deltaTime;

            // healing limit to avoid exceeding max health
            if (playerStats.CurrentHealth > characterData.stats.maxHealth)
            {
                playerStats.CurrentHealth = characterData.stats.maxHealth;
            }
        }
    }

    private void UpdateHealthBar()
    {
        // update the health bar
        UIManager.Instance.healthBar.fillAmount = playerStats.CurrentHealth / characterData.stats.maxHealth;
    }

    private void Kill()
    {
        if (!GameManager.Instance.isGameOver)
        {
            UIManager.Instance.AssignLevelReachedUI(playerExperience.level);
            UIManager.Instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponUISlots, inventory.passiveItemUISlots);
            GameManager.Instance.SetGameState(GameManager.GameState.GameOver);
        }
    }
}
