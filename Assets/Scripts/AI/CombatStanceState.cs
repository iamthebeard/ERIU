using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Combat Stance")]
public class CombatStanceState : AIState
{
    // Sort through all possible attacks, select an attack based on distance, angle, and weight
    // If we a chosen attack available, switch to attack state
    // If we are too far from a target, return to pursue state
    // If a target is no longer present, return to idle state
    // Future: Perform other combat actions while waiting to attack (block, dodge, etc.)
    // Future: Perform in-combat motion actions, like strafe/circle patterns around target, while waiting to attack

    [Header("Attacks")]
    public List<AICharacterAttackAction> allAttacks;            // All attacks this ai character can perform ever
    private List<AICharacterAttackAction> potentialAttacks;     // List of attacks that is created during this state, all attacks possible in this situation (based on angle, distance, etc.)
    private AICharacterAttackAction chosenAttack;
    private AICharacterAttackAction previousAttack;

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false;
    [SerializeField] protected int chanceToPerformComb = 25;
    [SerializeField] protected bool hasRolledForComboChance = false;

    [Header("Engagement Distance")]
    [SerializeField] protected float maximumEngagementDistance = 5;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        // return base.Tick(aICharacter);

        if (aiCharacter.isPerformingAction) return this;
        if (!aiCharacter.navMeshAgent.enabled) aiCharacter.navMeshAgent.enabled = true;
        
        if (aiCharacter.characterCombatManager.currentLockOnTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idle);
        if (aiCharacter.characterCombatManager.targetDistance > maximumEngagementDistance)
            return SwitchState(aiCharacter, aiCharacter.pursueTarget);

        // If we want the AI character to face and turn towards its target when it's outside its FOV, include this
        if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
        {
            if(
                aiCharacter.aiCharacterCombatManager.detectionFieldOfView < aiCharacter.characterCombatManager.viewableAngle
                || aiCharacter.characterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.detectionFieldOfView
            )
            {
                aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }
        }

        // Rotate to face target
        
        if (chosenAttack == null)
            GetNewAttack(aiCharacter);
        else
        {
            // Check recovery timer
            // Pass chosen attack to attack state
            // Roll for combo chance
            // Switch state
        }
        
        // If we're not attacking, continue to walk towards target
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.characterCombatManager.currentLockOnTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aICharacter)
    {
        // Sort through all possible attacks
        //  Remove attacks that can't be used (based on angle and distance)
        // Place remaining attacks into a list
        // Pick an attack randomly based on weight
        // Select this attack and switch to attack state

        potentialAttacks = new List<AICharacterAttackAction>();
        int totalWeight = 0;

        foreach (var potentialAttack in allAttacks)
        {
            if (aICharacter.aiCharacterCombatManager.targetDistance < potentialAttack.minimumDistance) continue;
            if (aICharacter.aiCharacterCombatManager.targetDistance > potentialAttack.maximumDistance) continue;

            if (
                -potentialAttack.attackableAngle >= aICharacter.aiCharacterCombatManager.viewableAngle
                && aICharacter.aiCharacterCombatManager.viewableAngle <= potentialAttack.attackableAngle
            )
                continue;
            
            potentialAttacks.Add(potentialAttack);
            totalWeight += potentialAttack.attackWeight;
        }

        if (potentialAttacks.Count <= 0) return; // Probably should do something like change state

        int randomWeightValue = Random.Range(1, totalWeight + 1);
        int processedWeight = 0;
        for (int i = 0; i < potentialAttacks.Count; i++)
        {
            processedWeight += potentialAttacks[i].attackWeight;

            if (randomWeightValue <= processedWeight)
            {
                // This is the attack. Set both chosen and previous attacks to this attack.
                chosenAttack = potentialAttacks[i];
                previousAttack = chosenAttack;
                break;
            }
        }
    }

    protected virtual bool RollForOutcomeChance(int outcomeChance)
    {
        // bool outcomeWillBePerformed = false;
        // int randomPercentage = Random.Range(0, 100);

        // if (randomPercentage < outcomeChance)
        //     outcomeWillBePerformed = true;
        
        // return outcomeWillBePerformed;
        return Random.Range(0, 100) < outcomeChance;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasRolledForComboChance = false;
    }
}
