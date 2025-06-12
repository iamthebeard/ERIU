using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    // If the damage is caused by another character (by an attack, for example),
    //  we need to have access to the attacking character to calculate damage.
    public CharacterManager characterCausingDamage;

    [Header("Damage Amounts")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    private int finalDamageDealt = 0; // Combined damage after all calculations have been made.

    // Effect build-ups (to be added later)

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseBroken = false;
    // If character's poise is broken, play the stun animation instead of the damage animation.

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool overridDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound FX")]
    public bool willPlayDamageSFX = true;
    public AudioClip elementalDamageSoundFX; // Used on top of regular sfx when elemental/extra damage taken

    [Header("Direction Damage Taken From")]
    // Used to determine what damage animation to play, where to instantiate blood spatter, etc.
    public float angleHitFrom;
    public Vector3 contactPoint;

    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);

        if (character.isDead.Value) // If character is dead, do not process any additional damage effects 
            return;

        // Check for invulnerability (TODO)

        // Calculate damage
        CalculateDamage(character);
        // Check for status build-ups

        // Determine direction of damage
        // Play damage animaiton
        // Play damage SFX
        // Play damage VFX

        // If character is AI controlled...
        //  - Possibly switch targets
        //  - etc.
    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner) // Only update health and trigger animations on owner
            return;

        if (characterCausingDamage != null)
        {
            // Check for damage modifiers on the attacking character
        }

        // Check character for flat damage reduction 
        // Check character for armor absorptions

        // Add remaining damage of each type to determine final damage
        finalDamageDealt = Mathf.RoundToInt(
            physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage
        );
        if (finalDamageDealt <= 1) finalDamageDealt = 1; // Minimum of 1 damage

        // Apply final damage to character health
        character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

        // Calculate poise damage
        // Determine whether poise is broken


    }
} 