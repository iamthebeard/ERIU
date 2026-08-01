using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{

    [SerializeField] string lightAttackAnimation = "main_hand_light_attack";
    [SerializeField] string lightAttack02Animation = "main_hand_light_attack_02";

    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (!playerPerformingAction.IsOwner)
        {
            return;
        }

        // Check for stops
        if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
        {
            return;
        }
        if (!playerPerformingAction.playerLocomotionManager.isGrounded)
        {
            return;
        }
        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
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
}
