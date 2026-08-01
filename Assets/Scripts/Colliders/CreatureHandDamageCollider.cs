using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHandDamageCollider : MeleeWeaponDamageCollider
{

    public AICharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();

        damageCollider = GetComponent<Collider>();
        aiCharacter = GetComponentInParent<AICharacterManager>();
    }

    protected override void DealDamageToTarget(CharacterManager damageTarget)
    {
        // base.DealDamageToTarget(damageTarget);
        // Make sure we only damage the target once per attack
        if (charactersDamaged.Contains(damageTarget)) return;
        charactersDamaged.Add(damageTarget);

        // Build a copy of the TakeDamageEffect instant character effect and populate values
        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.damage = damage;
        damageEffect.angleHitFrom = angleHitFrom;
        damageEffect.contactPoint = contactPoint;

        // switch (characterCausingDamage.characterCombatManager.currentAttackType)
        // {
        //     case AttackType.LightAttack01:
        //         damageEffect.damage *= lightAttack01Modifier;
        //         damageEffect.damage.poise = damage.poise * lightAttack01PoiseModifier; // Set poise modifier separately
        //         break;
        //     case AttackType.LightAttack02:
        //         damageEffect.damage *= lightAttack02Modifier;
        //         damageEffect.damage.poise = damage.poise * lightAttack02PoiseModifier; // Set poise modifier separately
        //         break;
        //     case AttackType.HeavyAttack01:
        //         damageEffect.damage *= heavyAttack01Modifier;
        //         damageEffect.damage.poise = damage.poise * heavyAttack01PoiseModifier;
        //         break;
        //     case AttackType.HeavyAttack02:
        //         damageEffect.damage *= heavyAttack02Modifier;
        //         damageEffect.damage.poise = damage.poise * heavyAttack02PoiseModifier;
        //         break;
        //     case AttackType.ChargedHeavy01:
        //         damageEffect.damage *= chargedHeavyAttack01Modifier;
        //         damageEffect.damage.poise = damage.poise * chargedHeavyAttack01PoiseModifier;
        //         break;
        //     case AttackType.ChargedHeavy02:
        //         damageEffect.damage *= chargedHeavyAttack02Modifier;
        //         damageEffect.damage.poise = damage.poise * chargedHeavyAttack02PoiseModifier;
        //         break;
        //     default:
        //         break;
        // }

        // Will be a network effect now! damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);

        // We have to decide which is the source of truth, the attacking or receiving character.
        // if (characterCausingDamage.IsOwner) // Only send one damage request no matter how many clients there are
        if (damageTarget.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyOfCharacterDamageServerRpc(
                damageTarget.NetworkObjectId,
                characterCausingDamage.NetworkObjectId,
                damageEffect.damage,
                damageEffect.angleHitFrom,
                damageEffect.contactPoint.x,
                damageEffect.contactPoint.y,
                damageEffect.contactPoint.z
            );
        }
        float totalDamage = damageEffect.damage.TotalDamage;

        string attackType = "";
        switch (characterCausingDamage.characterCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
            case AttackType.LightAttack02:
                attackType = "Light ";
                break;
            case AttackType.HeavyAttack01:
            case AttackType.HeavyAttack02:
                attackType = "Heavy ";
                break;
            case AttackType.ChargedHeavy01:
            case AttackType.ChargedHeavy02:
                attackType = "Charged heavy ";
                break;
        }
        Debug.Log( attackType + "character hand strike on character " + damageTarget.NetworkObjectId
            + " by character " + characterCausingDamage.NetworkObjectId + " for "
            + totalDamage + " and " + damageEffect.damage.poise + " poise damage.");
    }
}
