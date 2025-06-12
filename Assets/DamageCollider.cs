using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    protected Vector3 contactPoint;
    // Characters damaged in the current attack
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();


    public void OnTriggerEnter(Collider other)
    {
        // We could check to see if the colliding entity is a character:
        // if(other.gameObject.layer == LayerMask.NameToLayer("Character")) 
        // But we will instead make sure this collidor only interacts with the character layer.
        // Go to Edit --> Project Settings... --> Physics and use the interaction checkbox grid (at the bottom of the page)

        CharacterManager damageTarget = other.GetComponent<CharacterManager>();
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
        charactersDamaged.Add(damageTarget);

        // Build a copy of the TakeDamageEffect instant character effect and populate values
        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.holyDamage = holyDamage;

        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }
}
