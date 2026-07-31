using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Idle")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        // Base behavior is to always stay in the same state.
        // return base.Tick(aICharacter);

        if (aiCharacter.characterCombatManager.currentLockOnTarget != null)
        {
            // if (Physics.Linecast(
            //     aiCharacter.characterCombatManager.lockOnAnchor.position,
            //     aiCharacter.characterCombatManager.currentLockOnTarget.characterCombatManager.lockOnAnchor.position,
            //     WorldUtilityManager.Instance.GetEnvironmentLayers()
            // ))
            // {
            //     aiCharacter.characterCombatManager.SetLockOnTarget(null); // We no longer have line-of-sight to this target.
            //     return this;
            // }
            // We have a target, so return the pursue target state
            Debug.Log("Pursuing " + aiCharacter.characterCombatManager.currentLockOnTarget.name + "(" + aiCharacter.characterCombatManager.currentLockOnTarget.NetworkObjectId + ")");
            return SwitchState(aiCharacter, aiCharacter.pursueTarget);
        }
        else
        {
            // Continue to search for a target
            Debug.Log("No target");
            aiCharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);
            return this;
        }
    }
}
