using UnityEngine;

public class EnemiesFactory : IItemFactory
{
    public GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Tried to create item, but prefab is null!");
            return null;
        }

        return Object.Instantiate(prefab, position, rotation);
    }
}
