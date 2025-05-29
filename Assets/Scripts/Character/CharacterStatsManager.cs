using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Stamina Regeneration")]
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenerationDelay = 1; // Time (seconds) to wait after a delay before regenerating stamina
    [SerializeField] float staminaRegenerationPerSecond = 10;
    [SerializeField] float staminaTickLength = 0.1f; // Seconds

    protected virtual void Awake() {
        character = GetComponent<CharacterManager>();
    }

    public int CalculateStaminaBasedOnEnduranceLevel(int endurance) {
        float stamina = 0;

        // Create an equation for how to calculate stamina based on endurance stat
        stamina = endurance * 10;

        return Mathf.RoundToInt(stamina);
    }

    public virtual void RegenerateStamina() {
        if(!character.IsOwner)
            return;

        if(character.characterNetworkManager.isSprinting.Value)
            return; // Don't regenerate while sprinting.
        
        if (character.isPerformingAction)
            return; // Don't regenerate during actions (dodge/attack/etc.)
            // May choose to make this more specific later.
        
        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenerationDelay) {
            if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value) {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= staminaTickLength) {
                    staminaTickTimer = 0;
                    
                    // Add a tick of stamina
                    character.characterNetworkManager.currentStamina.Value += staminaRegenerationPerSecond * staminaTickLength;
                }
            }
        }
    }

    public virtual void ResetStaminaRegenerationTimer(float oldStaminaAmount, float newStaminaAmount) {
        if (newStaminaAmount < oldStaminaAmount) // Only delay regeneration when *using* stamina, not regaining.
            staminaRegenerationTimer = 0;
    }
}
