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
    protected virtual void Start() {}

    // Stats
    public void SetVitality(int oldValue, int newValue)
    {
        Debug.Log("Changing vitality from " + oldValue + " to " + newValue);
        character.characterNetworkManager.vitality.Value = newValue;
        character.characterNetworkManager.maxHealth.Value =
            CalculateMaxHealthBasedOnVitalityLevel(newValue);
        PlayerUIManager.instance.playerUIHUDManager.SetMaxHealthValue(character.characterNetworkManager.maxHealth.Value);
    }
    public void SetEndurance(int oldValue, int newValue)
    {
        Debug.Log("Changing endurance from " + oldValue + " to " + newValue);
        character.characterNetworkManager.endurance.Value = newValue;
        character.characterNetworkManager.maxStamina.Value =
            CalculateMaxStaminaBasedOnEnduranceLevel(newValue);
        PlayerUIManager.instance.playerUIHUDManager.SetMaxStaminaValue(character.characterNetworkManager.maxStamina.Value);
    }

    // Health
    public int CalculateMaxHealthBasedOnVitalityLevel(int vitality)
    {
        // Create an equation for how to calculate stamina based on endurance stat
        float health = vitality * 30;

        return Mathf.RoundToInt(health);
    }

    // Stamina
    
    public int CalculateMaxStaminaBasedOnEnduranceLevel(int endurance)
    {
        // Create an equation for how to calculate stamina based on endurance stat
        float stamina = endurance * 10;

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
