using UnityEngine;

public interface IItemFactory
{
    GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation);
}
