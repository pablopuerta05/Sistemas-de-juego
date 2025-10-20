using UnityEngine;

[CreateAssetMenu(fileName = "Ring Event Data", menuName = "2D Top-Down Rogue-like/Event Data/Ring")]
public class RingEventData : EventData
{
    [Header("mob data")]
    public ParticleSystem spawnEffectPrefab;
    public Vector2 scale = new Vector2(1, 1);
    [Min(0)] public float spawnRadius = 10f;
    [Min(0)] public float lifespan = 15f;

    private IItemFactory enemiesFactory;

    public override bool Activate(PlayerStats player = null, bool alwaysFires = false)
    {
        // preguntar por esta linea
        enemiesFactory = new EnemiesFactory();

        // only activate this if the player is present
        if (player)
        {
            GameObject[] spawns = GetSpawns();
            float angleOffset = 2 * Mathf.PI / Mathf.Max(1, spawns.Length);
            float currentAngle = 0;

            foreach (GameObject g in spawns)
            {
                // calculate the spawn position
                Vector3 spawnPosition = player.transform.position + new Vector3(spawnRadius * Mathf.Cos(currentAngle) * scale.x, spawnRadius * Mathf.Sin(currentAngle) * scale.y);

                // if a particle effect is assigned, play it on the position
                if (spawnEffectPrefab)
                {
                    Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);
                }

                // then spawn the enemy
                GameObject s = enemiesFactory.Create(g, spawnPosition, Quaternion.identity);

                // if there is a lifespan on the mob, set them to be destroyed
                if (lifespan > 0)
                {
                    Destroy(s, lifespan);
                }

                currentAngle += angleOffset;
            }
        }

        return false;
    }
}
