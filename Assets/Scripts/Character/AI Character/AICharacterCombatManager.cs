using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{
    // AICharacterManager aiCharacter;

    [Header("Detection")]
    [SerializeField] public float detectionRadius = 15;
    [SerializeField] public float detectionFieldOfView = 35;

    protected override void Awake()
    {
        base.Awake();

        // aiCharacter = GetComponent<AICharacterManager>();
    }

    public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
    {
        if (currentLockOnTarget != null) return; // Don't find a target while we already have one

        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.Instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();
            if (targetCharacter == null) continue;
            if (targetCharacter == aiCharacter) continue;
            if (targetCharacter.isDead.Value) continue;
            // Check if the character is on my team
            if (!WorldUtilityManager.Instance.IsHostileTo(aiCharacter.characterGroup, targetCharacter.characterGroup)) continue;

            // Check if the character is in view
            Vector3 directionToTarget = targetCharacter.transform.position - aiCharacter.transform.position;
            float angleToTarget = Vector3.Angle(directionToTarget, aiCharacter.transform.forward);
            if (!(/*-angleToTarget < angleToTarget && Don't need this because it's not a signed angle */angleToTarget <= detectionFieldOfView))
                continue; // Not in our FOV

            Debug.DrawLine(aiCharacter.transform.position, targetCharacter.characterCombatManager.lockOnAnchor.position);
            if (Physics.Linecast(
                aiCharacter.characterCombatManager.lockOnAnchor.position,
                targetCharacter.characterCombatManager.lockOnAnchor.position,
                WorldUtilityManager.Instance.GetEnvironmentLayers()
            ))
                continue; // Obstructed by environment



            // If a potential target is found
            aiCharacter.characterCombatManager.SetLockOnTarget(targetCharacter); // Set target and announce to network

            
            // Option 1
            //  Play a "pivot/turn" animation
            //  Do not apply root motion (in code)
            //  Rotate the character using code to face its target
            // * Can be done with a single animation (doesn't look as good, but fine for prototyping)

            // Option 2
            //  Calculate the angle of the target in respect to the character
            //  Use the calculated angle to determine what "pivot/turn" animation should be played
            //  Play the animation with root motion
            if (aiCharacter.aiCharacterCombatManager.currentLockOnTarget != null)
            {
                aiCharacter.aiCharacterCombatManager.targetDirection = aiCharacter.aiCharacterCombatManager.currentLockOnTarget.transform.position - transform.position;
                aiCharacter.aiCharacterCombatManager.viewableAngle = WorldUtilityManager.Instance.GetAngleOfTarget(transform, aiCharacter.aiCharacterCombatManager.targetDirection);
            }
            PivotTowardsTarget(aiCharacter);
        }
    }

    public void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        // Play a pivot animation, depending on the viewable angle of the current target
        if (aiCharacter.isPerformingAction) return;

        // Need actual turning animations
        // if (viewableAngle > 20 && viewableAngle <= 60)
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_45", true);
        // else if (viewableAngle > 60 && viewableAngle <= 110)
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
        // else if (viewableAngle < -20 && viewableAngle >= -60)
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_45", true);
        // else if (viewableAngle < -60 && viewableAngle <= -110)
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);


        // A hack since I don't have turning animations
        // if (viewableAngle > 0)
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("OneHand_Up_Run_F_R90_A (one step)", true/*DO NOT apply root motion (option 1)*/);
        // else
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("OneHand_Up_Run_F_L90_A (one step)", true /*DO NOT apply root motion (option 1)*/);
        // Quaternion toTarget = Quaternion.LookRotation(aiCharacter.aiCharacterCombatManager.targetDirection);
        // aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, toTarget, Time.deltaTime);

        

        
    }
}
