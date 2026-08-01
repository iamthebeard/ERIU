using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUndeadCombatManager : AICharacterCombatManager
{
    [SerializeField] CreatureHandDamageCollider rightHandDamageCollider;
    [SerializeField] CreatureHandDamageCollider leftHandDamageCollider;

    [Header("Damage")]
    [SerializeField] Damage damage = new Damage(50, 0, 0, 0, 0, 20);
    [SerializeField] float overhandAttackDamageModifier = 1.75f;
    [SerializeField] float overhandAttackPoiseDamageModifier = 2.5f;
    [SerializeField] float swipeAttackDamageModifier = 1.0f;
    [SerializeField] float swipeAttackPoiseDamageModifier = 1.0f;

    protected override void Start()
    {
        base.Start();

        SetOverhandAttackDamage(); // Default
        // Should technically call these from animations, if they had their own animations.
    }

    public void SetOverhandAttackDamage()
    {
        rightHandDamageCollider.damage = damage * overhandAttackDamageModifier;
        rightHandDamageCollider.damage.poise = damage.poise * overhandAttackPoiseDamageModifier;
        // leftHandDamageCollider.damage = damage * overhandAttackDamageModifier;
        // leftHandDamageCollider.damage.poise = damage.poise * overhandAttackPoiseDamageModifier;
    }

    public void SetSwipeAttackDamage()
    {
        rightHandDamageCollider.damage = damage * swipeAttackDamageModifier;
        rightHandDamageCollider.damage.poise = damage.poise * swipeAttackPoiseDamageModifier;
        // leftHandDamageCollider.damage = damage * swipeAttackDamageModifier;
        // leftHandDamageCollider.damage.poise = damage.poise * swipeAttackPoiseDamageModifier;
    }

    public void OpenDamageCollider()
    {
        rightHandDamageCollider.EnableDamageCollider();
        character.characterSoundFXManager.Whoosh();
        character.characterSoundFXManager.Grunt("attack");
        // leftHandDamageCollider.EnableDamageCollider();
    }

    public void CloseDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
        // leftHandDamageCollider.DisableDamageCollider();
    }
}
