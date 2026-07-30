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
    public float lightAttack02Modifier;
    public float lightAttack02PoiseModifier;
    public float heavyAttack01Modifier;
    public float heavyAttack01PoiseModifier;
    public float heavyAttack02Modifier;
    public float heavyAttack02PoiseModifier;
    public float chargedHeavyAttack01Modifier;
    public float chargedHeavyAttack01PoiseModifier;
    public float chargedHeavyAttack02Modifier;
    public float chargedHeavyAttack02PoiseModifier;

    protected override void Awake()
    {
        base.Awake();

        damageCollider.enabled = false; // Melee weapons shouldn't be enabled on start

        if (characterCausingDamage == null)
        {
            characterCausingDamage = damageCollider.GetComponentInParent<CharacterManager>();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        // Don't call base as we'll be modifying quite a bit. base.OnTriggerEnter(other);

        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
        if (damageTarget == null) return;
        if (damageTarget == characterCausingDamage) return; // Don't damage ourselves


        // Is it necessary to get the collider from `other`? It's already a collider.
        // contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        contactPoint = other.ClosestPointOnBounds(transform.position);
        angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);
        // Experimenting with angleHitFrom
        // var lastFrameVelocity = damageCollider.attachedRigidbody.velocity
        // angleHitFrom = Vector3.Reflect(lastFrameVelocity.normalized, contactPoint - other.bounds.center);

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
        damageEffect.damage = damage;
        damageEffect.angleHitFrom = angleHitFrom;
        damageEffect.contactPoint = contactPoint;

        switch (characterCausingDamage.characterCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
                damageEffect.damage *= lightAttack01Modifier;
                damageEffect.damage.poise = damage.poise * lightAttack01PoiseModifier; // Set poise modifier separately
                break;
            case AttackType.LightAttack02:
                damageEffect.damage *= lightAttack02Modifier;
                damageEffect.damage.poise = damage.poise * lightAttack02PoiseModifier; // Set poise modifier separately
                break;
            case AttackType.HeavyAttack01:
                damageEffect.damage *= heavyAttack01Modifier;
                damageEffect.damage.poise = damage.poise * heavyAttack01PoiseModifier;
                break;
            case AttackType.HeavyAttack02:
                damageEffect.damage *= heavyAttack02Modifier;
                damageEffect.damage.poise = damage.poise * heavyAttack02PoiseModifier;
                break;
            case AttackType.ChargedHeavy01:
                damageEffect.damage *= chargedHeavyAttack01Modifier;
                damageEffect.damage.poise = damage.poise * chargedHeavyAttack01PoiseModifier;
                break;
            case AttackType.ChargedHeavy02:
                damageEffect.damage *= chargedHeavyAttack02Modifier;
                damageEffect.damage.poise = damage.poise * chargedHeavyAttack02PoiseModifier;
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
        Debug.Log( attackType + "weapon strike on character " + damageTarget.NetworkObjectId
            + " by character " + characterCausingDamage.NetworkObjectId + " for "
            + totalDamage + " and " + damageEffect.damage.poise + " poise damage.");
    }
}
