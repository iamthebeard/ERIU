using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHUDManager : MonoBehaviour
{
    [Header("Stat Bars")]
    [SerializeField] UI_StatBar healthBar;
    [SerializeField] UI_StatBar staminaBar;

    [Header("Boss Health Bars")]
    public Transform bossHealthBarParent;
    public GameObject bossHealthBarPrefab;

    [Header("Quick Slots")]
    [SerializeField] Image rightWeaponQuickSlotIcon;
    [SerializeField] Image leftWeaponQuickSlotIcon;
    [SerializeField] Image spellQuickSlotIcon;
    [SerializeField] Image itemQuickSlotIcon;

    public void ResetHUD()
    {
        healthBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(true);
    }

    public void SetNewStaminaValue(float oldValue, float newValue)
    {
        staminaBar.SetStat(newValue);
    }

    public void SetMaxStaminaValue(int maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }
    
    public void SetNewHealthValue(int oldValue, int newValue)
    {
        healthBar.SetStat(newValue);
    }

    public void SetMaxHealthValue(int maxHealtha) {
        healthBar.SetMaxStat(maxHealtha);
    }

    public void SetRightWeaponQuickSlotIcon(int weaponID)
    {
        // Method 1: DIRECTLY reference the right weapon in the hand of the player
        // Pros: Straightforward
        // Cons: Will cause an error if called before loading weapons. So order matters!
        // Example: Loading a saved game, if you reference weapon icons in UI before loading weapons.
        // Fine if you remember your order of operations

        // Method 2: REQUIRE an item ID of the weapon as an argument, fetch the weapon from our databse and use it to get the weapon item's icon
        // Pros: Current weapon ID is a saved value, we can load it independently from what weapons are loaded ?? > What does this mean?
        // Cons: Indirect
        // Great if you don't want to remember order of operations
        WeaponItem weapon = WorldItemDatabase.Instance.GetWeaponByID(weaponID);
        if (weapon == null || weapon.itemIcon == null)
        {
            Debug.Log("Unable to load weapon " + weaponID + (weapon == null ? "." : "'s icon. (" + weapon.itemIcon.ToString() + ") Weapon: " + weapon.itemName + "."));
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }

        // Check if we meet the item's requirements (so we can show the indicator X)
        // Check whether the item is inactive (like the offhand when two-handing) or unusable (like a spell without a catalyst) so we can make it transparent

        Debug.Log("Loading weapon " + weaponID + (weapon == null ? "." : "'s icon. (" + weapon.itemIcon.ToString() + ") Weapon: " + weapon.itemName + "."));
        rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
        rightWeaponQuickSlotIcon.enabled = true;   
    }

    public void SetLeftWeaponQuickSlotIcon(int weaponID)
    {
        WeaponItem weapon = WorldItemDatabase.Instance.GetWeaponByID(weaponID);
        if (weapon == null || weapon.itemIcon == null)
        {
            Debug.Log("Unable to load weapon " + weaponID + (weapon == null ? "." : "'s icon. (" + weapon.itemIcon.ToString() + ") Weapon: " + weapon.itemName + "."));
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }

        // Check if we meet the item's requirements (so we can show the indicator X)
        // Check whether the item is inactive (like the offhand when two-handing) or unusable (like a spell without a catalyst) so we can make it transparent

        Debug.Log("Loading weapon " + weaponID + (weapon == null ? "." : "'s icon. (" + weapon.itemIcon.ToString() + ") Weapon: " + weapon.itemName + "."));
        leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
        leftWeaponQuickSlotIcon.enabled = true;
    }
}
