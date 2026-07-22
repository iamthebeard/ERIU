using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] protected Collider damageCollider;

    [Header("Damage")]
    [SerializeField] public Damage damage = new Damage();

    protected Vector3 contactPoint;
    // Characters damaged in the current attack
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    protected virtual void Awake()
    {
        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // We could check to see if the colliding entity is a character:
        // if(other.gameObject.layer == LayerMask.NameToLayer("Character")) 
        // But we will instead make sure this collidor only interacts with the character layer.
        // Go to Edit --> Project Settings... --> Physics and use the interaction checkbox grid (at the bottom of the page)

        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
        if (damageTarget == null)
            return;

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

    protected virtual void DealDamageToTarget(CharacterManager damageTarget)
    {
        // Make sure we only damage the target once per attack
        if (charactersDamaged.Contains(damageTarget)) return;
        charactersDamaged.Add(damageTarget);

        // Build a copy of the TakeDamageEffect instant character effect and populate values
        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.damage = damage;

        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        charactersDamaged.Clear(); // Reset so we can damage the same characters again
    }
}
