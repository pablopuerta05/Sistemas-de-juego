using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    private void Awake()
    {
        inventory = GetComponent<InventoryManager>();
    }

    public void SpawnWeapon(GameObject weaponPrefab)
    {
        if (weaponIndex >= inventory.weaponSlots.Count)
        {
            Debug.LogError("Inventory weapon slots full");
            return;
        }

        GameObject spawnedWeapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);

        Weapon weapon = spawnedWeapon.GetComponent<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("Spawned object does not have a Weapon component!");
            return;
        }

        weapon.Initialise(weapon.data); // Asegúrate de que 'data' ya esté asignado en el prefab
        inventory.AddWeapon(weaponIndex, weapon);
        weaponIndex++;
    }

    public void SpawnPassiveItem(GameObject passiveItemPrefab)
    {
        if (passiveItemIndex >= inventory.passiveItemSlots.Count)
        {
            Debug.LogError("Inventory passive item slots full");
            return;
        }

        GameObject spawnedPassiveItem = Instantiate(passiveItemPrefab, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);

        Passive passive = spawnedPassiveItem.GetComponent<Passive>();
        if (passive == null)
        {
            Debug.LogError("Spawned object does not have a Passive component!");
            return;
        }

        passive.Initialise(passive.data); // Asegúrate de que 'data' ya esté asignado en el prefab
        inventory.AddPassiveItem(passiveItemIndex, passive);
        passiveItemIndex++;
    }
}
