using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    //public EnemyScriptableObject EnemyData;

    Transform player;

    public static int count; // track the number of enemies on the screen

    // Current stats
    public float currentMoveSpeed;
    public float currentHealth;
    public float currentDamage;

    [Header("damage feedback")]
    public Color damageColor = new Color(1, 0, 0, 1); // what the color of the damage flash should be
    public float damageFlashDuration = 0.2f; // how long the flash should last
    public float deathFadeTime = 0.6f; // how much time it takes for the enemy to fade
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    EnemyMovement movement;

    private void Awake()
    {
        count++;

        // assing the variables
        //currentMoveSpeed = EnemyData.MoveSpeed;
        //currentHealth = EnemyData.MaxHealth;
        //currentDamage = EnemyData.Damage;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        movement = GetComponent<EnemyMovement>();
    }

    //private void Update()
    //{
    //    if (Vector2.Distance(transform.position, player.position) >= despawnDistance)
    //    {
    //        ReturnEnemy();

    //    }
    //}

    // This function always needs at least 2 values, the amount of damage dealt <dmg>, as well as where the damage is coming from, which is passed as
    // <sourcePosition> is necessary because it is used to calculate the direction of the knockback
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());

        // create the text popup when the enemy takes damage
        if (dmg > 0)
        {
            //GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
        }

        // apply knockback if it is not 0
        if (knockbackForce > 0)
        {
            // gets the direction of the knockback
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    // this is a coroutine function that makes the enemy flash when taking damage
    IEnumerator DamageFlash()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = originalColor;
    }

    public void Kill()
    {
        // enable drops if the enemy is killed, since drops are disabled by default
        DropRateManager drops = GetComponent<DropRateManager>();
        if (drops)
        {
            drops.active = true;
        }

        StartCoroutine(KillFade());
    }

    IEnumerator KillFade()
    {
        // waits for a single frame
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = spriteRenderer.color.a;

        // this is a loop that fires every frame
        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            // set the color for this frame
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // reference the script from the collided collider and deal damage using TakeDamage()
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage); // make sure to use current damage instead of weaponData.Damage
        }
    }

    private void OnDestroy()
    {
        count--;
    }

    //    void ReturnEnemy()
    //    {
    //        EnemySpawner es = FindObjectOfType<EnemySpawner>();
    //        transform.position = player.position + es.transform.position;
    //    }
}
