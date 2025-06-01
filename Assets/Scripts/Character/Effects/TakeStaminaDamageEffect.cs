using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
public class TakeStaminaDamageEffect : InstantCharacterEffect
{
    public float staminaDamage;

    public override void ProcessEffect(CharacterManager character)
    {
        // Debug.Log("In ProcessEffect", character);
        CalculateStaminaDamage(character);
    }

    private void CalculateStaminaDamage(CharacterManager character)
    {
        // Why do this?
        // Compare the base stamina damage against other player effects/modifiers
        //  Ex: check for blocking buff

        // Subtract stamina

        // Play FX
        if (character.IsOwner)
        {
            character.characterNetworkManager.currentStamina.Value -= staminaDamage;
        }
    }
}
