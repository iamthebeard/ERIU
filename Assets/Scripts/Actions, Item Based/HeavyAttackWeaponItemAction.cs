using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
public class HeavyAttackWeaponItemAction : WeaponItemAction
{

    [SerializeField] string heavyAttackAnimation = "main_hand_heavy_attack";
    [SerializeField] string heavyAttack02Animation = "main_hand_heavy_attack_02";

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
        if (!playerPerformingAction.isGrounded)
        {
            return;
        }
        PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        Debug.Log("Performing heavy attack");
        // If we are attacking and have reached the combo window, perform the next combo attack
        if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            // Perform the next attack, based on the previous attack
            if (playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == heavyAttackAnimation)
            {
                Debug.Log("Comboing. Expecting " + heavyAttack02Animation);
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack02, heavyAttack02Animation, true);
            }
            else
            {
                // Start the loop over again
                Debug.Log("Comboing, starting over. Expecting " + heavyAttack02Animation);
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavyAttackAnimation, true);
            }
        }
        else if (!playerPerformingAction.isPerformingAction)
        {
            Debug.Log("Not comboing. Expecting " + heavyAttack02Animation);
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavyAttackAnimation, true);
        }
    }
}
