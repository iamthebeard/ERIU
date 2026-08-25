using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{

    [SerializeField] string lightAttackAnimation = "main_hand_light_attack";
    [SerializeField] string lightAttack02Animation = "main_hand_light_attack_02";
    [SerializeField] string runningAttackAnimation = "main_hand_running_attack"; // Same animations because I don't have any of these
    [SerializeField] string rollingAttackAnimation = "main_hand_running_attack"; // Same animations because I don't have any of these
    [SerializeField] string backstepAttackAnimation = "main_hand_running_attack"; // Same animations because I don't have any of these

    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (!playerPerformingAction.IsOwner) return;
        if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0) return;
        if (!playerPerformingAction.playerLocomotionManager.isGrounded) return;

        if (playerPerformingAction.characterNetworkManager.isSprinting.Value)
        {
            // If sprinting, do a run attack
            PerformRunningAttack(playerPerformingAction, weaponPerformingAction);
        }
        else if (playerPerformingAction.playerCombatManager.canDoRollingAttack)
        {
            // If we attack in the canDoRollingAttack window, do a rolling attack
            PerformRollingAttack(playerPerformingAction, weaponPerformingAction);
        }
        else if (playerPerformingAction.playerCombatManager.canDoBackstepAttack)
        {
            // If we attack in the canDoRollingAttack window, do a rolling attack
            PerformBackstepAttack(playerPerformingAction, weaponPerformingAction);
        }
        else
        {
            PerformLightAttack(playerPerformingAction, weaponPerformingAction);
        }
    }

    private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        // If we are attacking and have reached the combo window, perform the next combo attack
        if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            // Perform the next attack, based on the previous attack
            if (playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == lightAttackAnimation)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02, lightAttack02Animation, true);
            }
            else
            {
                // Start the loop over again
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, lightAttackAnimation, true);
            }
        }
        else if (!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, lightAttackAnimation, true);
        }
    }

    private void PerformRunningAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if (!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.RunningAttack01, runningAttackAnimation, true, false, false, false);
        }
    }

    private void PerformRollingAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.playerCombatManager.DisableCanDoRollingAttack();
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.RollingAttack01, rollingAttackAnimation, true, false, false, false);
    }

    private void PerformBackstepAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.playerCombatManager.DisableCanDoBackstepAttack();
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.BackstepAttack01, backstepAttackAnimation, true, false, false, false);
    }
}
