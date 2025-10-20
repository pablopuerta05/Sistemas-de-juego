using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable] // serialize the class
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }

    public bool active = false;
    public List<Drops> drops;

    private IItemFactory gemFactory;

    private void Awake()
    {
        // Si no se asigna desde afuera, usar una por defecto
        gemFactory = new ExperienceGemsFactory();
    }

    private void OnDestroy()
    {
        if (!active || !gameObject.scene.isLoaded) // stops the spawning error from appearing when stopping play mode
        {
            return;
        }

        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>();

        foreach (Drops rate in drops)
        {
            if (randomNumber <= rate.dropRate)
            {
                possibleDrops.Add(rate);
            }
        }

        // check if there are possible drops
        if (possibleDrops.Count > 0)
        {
            Drops drops = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];
            gemFactory.Create(drops.itemPrefab, transform.position, Quaternion.identity); // get an object from the factory
        }
    }
}
