using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Pursue")]
public class PursueTargetState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        Debug.Log("Pursuing target");

        // Check if we are performing an action (if so, wait until action is complete)
        if (aiCharacter.isPerformingAction) return this;

        // Check if our target is null (if so, go back to idle state to find a new target)
        if (aiCharacter.characterCombatManager.currentLockOnTarget == null) return aiCharacter.idle;
        // > Check for out of line of sight?
        if (Physics.Linecast(
            aiCharacter.characterCombatManager.lockOnAnchor.position,
            aiCharacter.characterCombatManager.currentLockOnTarget.characterCombatManager.lockOnAnchor.position,
            WorldUtilityManager.Instance.GetEnvironmentLayers()
        ))
        {
            aiCharacter.characterCombatManager.SetLockOnTarget(null); // We no longer have line-of-sight to this target.
            aiCharacter.navMeshAgent.enabled = false;
            return SwitchState(aiCharacter, aiCharacter.idle);
        }

        // Make sure our navmesh agent is active (if not, enable it)
        if (aiCharacter.navMeshAgent.enabled == false)
            aiCharacter.navMeshAgent.enabled = true;

        // If we are within combat range (if so, switch to combat stance state)
        // if (aiCharacter.aiCharacterCombatManager.targetDistance <= aiCharacter.combatStance.maximumEngagementDistance) // Will cause them to stutter in and out of combat distance if you are moving when they catch up with you
        if (aiCharacter.aiCharacterCombatManager.targetDistance <= aiCharacter.navMeshAgent.stoppingDistance) // Now there are two things to set on different object. Should I just do (-0.5) or something? Or have an engagementStartDist and engagementEndDist?
            return SwitchState(aiCharacter, aiCharacter.combatStance);

        // If the target is unreachable, and we are far from home, return home

        // Pursue target
        
        // Option 1 (async, more performant, calculate path as we go). SG has had trouble with this not working on complicated terrain.
        // aiCharacter.navMeshAgent.SetDestination(aiCharacter.characterCombatManager.currentLockOnTarget.transform.position);

        // Option 2 (calculate whole path immediately, can be a problem if many long, complicated paths are complicated at once). SG hasn't seen performance issues.
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.characterCombatManager.currentLockOnTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        // Rotate towards target
        aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

        // Update distance, etc., to target
        aiCharacter.aiCharacterCombatManager.targetDirection = aiCharacter.aiCharacterCombatManager.currentLockOnTarget.transform.position - aiCharacter.transform.position;
        aiCharacter.aiCharacterCombatManager.viewableAngle = WorldUtilityManager.Instance.GetAngleOfTarget(aiCharacter.transform, aiCharacter.aiCharacterCombatManager.targetDirection);
        aiCharacter.aiCharacterCombatManager.targetDistance = Vector3.Distance(aiCharacter.transform.position, aiCharacter.aiCharacterCombatManager.currentLockOnTarget.transform.position);

        return this;
    }
}
