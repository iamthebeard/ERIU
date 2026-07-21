using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Test Action")]
public class WeaponItemAction : ScriptableObject
{
    public int actionID;

    public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        // What does every weapon action have in common?
        // 1. Which weapon the acting player is using
        // playerPerformingAction.playerCombabatManager.currentWeaponBeingUsed = weaponPerformingAction; // Do in NetworkManager instead
        if (playerPerformingAction.IsOwner)
        {
            playerPerformingAction.playerNetworkManager.currentWeaponBeingUsedID.Value = weaponPerformingAction.itemID;
        }

        UnityEngine.Debug.Log("The action has fired.");
    }
}
