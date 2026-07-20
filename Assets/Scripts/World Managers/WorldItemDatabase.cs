using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldItemDatabase : MonoBehaviour
{
    public static WorldItemDatabase Instance;

    [SerializeField] public WeaponItem unarmedWeapon;
    [SerializeField] public List<WeaponItem> weapons = new List<WeaponItem>();
    [SerializeField] public List<Item> items = new List<Item>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        items.Add(unarmedWeapon);

        foreach (var weapon in weapons)
        {
            items.Add(weapon); // Weapons are items
        }

        // Assign all unique items a unique item ID.
        for (int i=0; i < items.Count; i++)
        {
            items[i].itemID = i;
        }
    }

    public WeaponItem GetWeaponByID(int id)
    {
        return weapons.FirstOrDefault(weapon => weapon.itemID == id);
    }
}
