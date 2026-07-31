using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{
    AICharacterManager aiCharacter;

    [Header("Detection")]
    [SerializeField] float detectionRadius = 15;
    [SerializeField] float detectionFieldOfView = 35;

    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
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
            if (!(-angleToTarget < angleToTarget && angleToTarget <= detectionFieldOfView)) continue; // Not in our FOV
            Debug.DrawLine(aiCharacter.transform.position, targetCharacter.characterCombatManager.lockOnAnchor.position);
            if (Physics.Linecast(
                aiCharacter.characterCombatManager.lockOnAnchor.position,
                targetCharacter.characterCombatManager.lockOnAnchor.position,
                WorldUtilityManager.Instance.GetEnvironmentLayers()
            ))
                continue; // Obstructed by environment



            // If a potential target is found
            aiCharacter.characterCombatManager.SetLockOnTarget(targetCharacter); // Set target and announce to network
        }
    }
}
