using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    [Header("Current Weapon")]
    public WeaponItem currentWeaponBeingUsed;

    [Header("Flags")]
    public bool canComboWithMainHandWeapon = false;
    // public bool canComboWithOffHandWeapon = false;

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
            case AttackType.LightAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttack02StaminaCostModifier;
                break;
            case AttackType.HeavyAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttack01StaminaCostModifier;
                break;
            case AttackType.HeavyAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttack02StaminaCostModifier;
                break;
            case AttackType.ChargedHeavy01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargedHeavyAttack01StaminaCostModifier;
                break; 
            case AttackType.ChargedHeavy02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargedHeavyAttack02StaminaCostModifier;
                break;
            case AttackType.RunningAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.runningAttack01StaminaCostModifier;
                break;
            case AttackType.RollingAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.rollingAttack01StaminaCostModifier;
                break;
            case AttackType.BackstepAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.backstepAttack01StaminaCostModifier;
                break; 
            default:
                break;
        }
        Debug.Log("Used " + staminaDeducted + " stamina.");

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

    public override void EnableCanDoCombo()
    {
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            player.playerCombatManager.canComboWithMainHandWeapon = true;
        }
        // else if -- enable offhand weapon
    }

    public override void DisableCanDoCombo()
    {
        player.playerCombatManager.canComboWithMainHandWeapon = false;
        // player.playerCombatManager.canComboWithOffHandWeapon = false;
    }
}
