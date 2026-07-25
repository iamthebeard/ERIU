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
    [SerializeField] public Damage damage = new Damage();

    private int finalDamageDealt = 0; // Combined damage after all calculations have been made.

    // Effect build-ups (to be added later)

    [Header("Poise")]
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
        // Play damage animation
        PlayDirectionalBasedDamageAnimation(character);
        // Play damage SFX
        PlayDamageSFX(character);
        // Play damage VFX
        PlayDamageVFX(character);

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
        finalDamageDealt = Mathf.RoundToInt(damage.TotalDamage);
        if (finalDamageDealt <= 1) finalDamageDealt = 1; // Minimum of 1 damage

        // Apply final damage to character health
        character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

        // Calculate poise damage
        // Determine whether poise is broken
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        // If we have fire damage, play fire particles
        // Etc.

        character.characterEffectsManager.PlayBloodSpatterVFX(contactPoint, angleHitFrom);
    }

    private void PlayDamageSFX(CharacterManager character)
    {
        // If fire damage, play burn SFX
        // Etc.

        // TODO: Determine sound by damage type

        AudioClip damageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.bladeHitSFX);

        character.characterSoundFXManager.PlaySoundFX(damageSFX);
    }

    private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
    {
        if (!character.IsOwner) return;
        if (character.isDead.Value) return;

        // Calculate if poise is broken
        poiseBroken = true;

        if (angleHitFrom >= 145 && angleHitFrom <= 180)
        {
            // Play front animation
            damageAnimation = character.characterAnimatorManager.hitForwardMedium01;
        }
        else if (angleHitFrom <= -145 && angleHitFrom >= -180)
        {
            // Still play front animation
            damageAnimation = character.characterAnimatorManager.hitForwardMedium01;
        }
        else if (angleHitFrom >= -45 && angleHitFrom <= 45)
        {
            // Play back animation
            damageAnimation = character.characterAnimatorManager.hitBackwardMedium01;
        }
        else if (angleHitFrom >= -144 && angleHitFrom <= -45)
        {
            // Play Left animation
            damageAnimation = character.characterAnimatorManager.hitLeftMedium01;
        }
        else if (angleHitFrom >= 45 && angleHitFrom <= 144)
        {
            // Play right animation
            damageAnimation = character.characterAnimatorManager.hitRightMedium01;
        }

        if (poiseBroken)
        {
            character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
        }
    }
} 