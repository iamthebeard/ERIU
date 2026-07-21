using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;
    public WeaponItem currentWeaponBeingUsed;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
    {
        // Perform the action on acting player
        weaponAction.AttemptToPerformAction(player, weaponPerformingAction);

        // Notify the server to perform the action on connected clients
    }
}
