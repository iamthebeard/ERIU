using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{

    [Header("Attacking Character")]
    [SerializeField] public CharacterManager characterCausingDamage; // When calculating damage, this is used to check for attacker's damage modifiers, effects, etc.

    [Header("Damage")]

    [Header("Weapon Attack Modifiers")]
    public float lightAttack01Modifier;
    public float lightAttack01PoiseModifier;

    protected override void Awake()
    {
        base.Awake();

        damageCollider.enabled = false; // Melee weapons shouldn't be enabled on start

        if (characterCausingDamage == null)
        {
<<<<<<< Updated upstream
            characterCausingDamage = GetComponentInParent<CharacterManager>();
=======
            characterCausingDamage = damageCollider.GetComponentInParent<CharacterManager>();
>>>>>>> Stashed changes
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        // Don't call base as we'll be modifying quite a bit. base.OnTriggerEnter(other);

        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
        if (damageTarget == null) return;
<<<<<<< Updated upstream
        if (damageTarget != characterCausingDamage) return; // Don't damage ourselves
=======
        if (damageTarget == characterCausingDamage) return; // Don't damage ourselves
>>>>>>> Stashed changes


        // Is it necessary to get the collider from `other`? It's already a collider.
        // contactPoint = other.gameObject.GetComponent<Collidor>().ClosestPointOnBounds(transform.position);
        contactPoint = other.ClosestPointOnBounds(transform.position);

        // Check if we can damage this target
        //  For coop, summons, mobs within a group, etc.

        // Check if target is blocking

        // Check if target is invulnerable

        // Deal damage
        DealDamageToTarget(damageTarget);
    }

    protected override void DealDamageToTarget(CharacterManager damageTarget)
    {
        // base.DealDamageToTarget(damageTarget);
        // Make sure we only damage the target once per attack
        if (charactersDamaged.Contains(damageTarget)) return;
        charactersDamaged.Add(damageTarget);

        // Build a copy of the TakeDamageEffect instant character effect and populate values
        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.poiseDamage = poiseDamage;

        switch (characterCausingDamage.characterCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
                ApplyAttackDamageModifiers(lightAttack01Modifier, damageEffect);
                damageEffect.poiseDamage *= lightAttack01PoiseModifier;
                break;
            default:
                break;
        }

        // Will be a network effect now! damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);

        if (characterCausingDamage.IsOwner) // Only send one damage request no matter how many clients there are
        {
            damageTarget.characterNetworkManager.NotifyOfCharacterDamageServerRpc(
                damageTarget.NetworkObjectId,
                characterCausingDamage.NetworkObjectId,
                damageEffect.physicalDamage,
                damageEffect.fireDamage,
                damageEffect.magicDamage,
                damageEffect.fireDamage,
                damageEffect.lightningDamage,
                damageEffect.holyDamage,
                damageEffect.poiseDamage,
                damageEffect.contactPoint.x,
                damageEffect.contactPoint.y,
                damageEffect.contactPoint.z
            );
        }
        float totalDamage = damageEffect.physicalDamage
            + damageEffect.fireDamage
            + damageEffect.magicDamage
            + damageEffect.fireDamage
            + damageEffect.lightningDamage
            + damageEffect.holyDamage;

        Debug.Log("Weapon strike on character " + damageTarget.NetworkObjectId + " by character " + characterCausingDamage.NetworkObjectId + " for "
            + totalDamage + " and " + damageEffect.poiseDamage + " poise damage.");
    }

    private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
    {
        damage.physicalDamage *= modifier;
        damage.magicDamage *= modifier;
        damage.fireDamage *= modifier;
        damage.lightningDamage *= modifier;
        damage.holyDamage *= modifier;
    }


}
