using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
public class HeavyAttackWeaponItemAction : WeaponItemAction
{

    [SerializeField] string heavyAttackAnimation = "OneHand_Up_Attack_B_1";

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
        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if (playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, heavyAttackAnimation, true);
        }
        else if (playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, heavyAttackAnimation, true);
        }
    }
}
