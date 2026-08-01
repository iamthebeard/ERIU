using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Attack")]
public class AttackState : AIState
{
    [HideInInspector] public AICharacterAttackAction currentAttack;
    [HideInInspector] public bool willPerformCombo = false;

    [Header("State Flags")]
    protected bool hasPerformedAttack = false;
    protected bool hasPerformedCombo = false;

    [Header("Behavior")]
    [SerializeField] protected bool pivotAfterAttack = false;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        Debug.Log("attacking");
        // return base.Tick(aiCharacter);

        if (aiCharacter.aiCharacterCombatManager.currentLockOnTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idle);
        if (aiCharacter.aiCharacterCombatManager.currentLockOnTarget.isDead.Value)
            return SwitchState(aiCharacter, aiCharacter.idle);
        // if (aiCharacter.characterCombatManager.targetDistance > currentAttack.maximumDistance)
        //     return SwitchState(aiCharacter, aiCharacter.pursueTarget);
        
        // Rotate toward target while attacking (tracking)
        aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhileAttacking(aiCharacter);

        // Set movement values to 0
        aiCharacter.characterAnimatorManager.UpdateAnimatorMovement(0, 0, false);

        // Perform a combo
        if (willPerformCombo && !hasPerformedCombo)
        {
            if (currentAttack.comboAction != null)
            {
                // TODO if can combo
                hasPerformedCombo = true;
                currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
            }
        }

        if (aiCharacter.isPerformingAction)
            return this;

        if (!hasPerformedAttack)
        {
            if (aiCharacter.aiCharacterCombatManager.actionRecoveryTimer > 0)
                return this;
            
           PerformAttack(aiCharacter);

           // Return to top, so we can potentially perform a combo
           return this;
        }

        if (pivotAfterAttack)
        {
            aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        }

        return SwitchState(aiCharacter, aiCharacter.combatStance);
    }

    protected void PerformAttack(AICharacterManager aiCharacter)
    {
        Debug.Log("performing attack " + currentAttack.name);
        hasPerformedAttack = true;
        currentAttack.AttemptToPerformAction(aiCharacter);
        aiCharacter.aiCharacterCombatManager.actionRecoveryTimer = currentAttack.recoveryTime;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasPerformedAttack = false;
        hasPerformedCombo = false;
    }
}
