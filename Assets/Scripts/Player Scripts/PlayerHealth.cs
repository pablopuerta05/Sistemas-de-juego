using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats playerStats;
    private PlayerExperience playerExperience;
    private InventoryManager inventory;
    public ParticleSystem damageEffect;

    // I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration = 0.5f;
    private float invincibilityTimer;
    private bool isInvincible;

    private void Awake()
    {
        // Recupera referencias
        playerStats = GetComponent<PlayerStats>();
        playerExperience = GetComponent<PlayerExperience>();
        inventory = GetComponent<InventoryManager>();

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found on Player!");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        UpdateHealthBar();
    }

    private void Update()
    {
        // Control de invencibilidad
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

    public void TakeDamage(float dmg)
    {
        if (isInvincible) return;

        playerStats.CurrentHealth -= dmg;

        if (damageEffect)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }

        invincibilityTimer = invincibilityDuration;
        isInvincible = true;

        if (playerStats.CurrentHealth <= 0)
        {
            playerStats.CurrentHealth = 0;
            Kill();
        }

        UpdateHealthBar();
    }

    public void RestoreHealth(float amount)
    {
        float maxHealth = playerStats.CharacterData.stats.maxHealth;
        playerStats.CurrentHealth = Mathf.Min(playerStats.CurrentHealth + amount, maxHealth);
        UpdateHealthBar();
    }

    private void Recover()
    {
        float maxHealth = playerStats.CharacterData.stats.maxHealth;
        if (playerStats.CurrentHealth < maxHealth)
        {
            playerStats.CurrentHealth += playerStats.CurrentRecovery * Time.deltaTime;
            playerStats.CurrentHealth = Mathf.Min(playerStats.CurrentHealth, maxHealth);
            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        if (UIManager.Instance == null || UIManager.Instance.healthBar == null)
            return;

        float maxHealth = playerStats.CharacterData.stats.maxHealth;
        UIManager.Instance.healthBar.fillAmount = playerStats.CurrentHealth / maxHealth;
    }

    private void Kill()
    {
        if (!GameManager.Instance.isGameOver)
        {
            if (playerExperience != null)
                UIManager.Instance.AssignLevelReachedUI(playerExperience.level);

            if (inventory != null)
                UIManager.Instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponUISlots, inventory.passiveItemUISlots);

            GameManager.Instance.SetGameState(GameManager.GameState.GameOver);
        }
    }
}
