using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    [Header("Current Weapon")]
    public WeaponItem currentWeaponBeingUsed;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
    {
        if (player.IsOwner)
        {
            // Perform the action on acting player
            weaponAction.AttemptToPerformAction(player, weaponPerformingAction);

            // Notify the server to perform the action on connected clients
            player.playerNetworkManager.NotifyServerOfWeaponActionServerRpc(NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID);
        }
    }

    public virtual void DrainStaminaBasedOnAttack()
    {
        if (!player.IsOwner) return;
        if (currentWeaponBeingUsed == null) return;

        float staminaDeducted = 0;

        switch (currentAttackType)
        {
            case AttackType.LightAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttack01StaminaCostModifier;
                break;
            case AttackType.HeavyAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttack01StaminaCostModifier;
                break;
            case AttackType.ChargedHeavy01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargedHeavyAttack01StaminaCostModifier;
                break;
            default:
                break;
        }

        player.playerNetworkManager.currentStamina.Value -= staminaDeducted;
    }

    public override void SetLockOnTarget(CharacterManager newLockOnTarget)
    {
        base.SetLockOnTarget(newLockOnTarget);

        if (player.IsOwner)
        {
            PlayerCamera.instance.SetCameraHeight();
        }
    }
}
