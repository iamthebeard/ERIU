using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{

    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage; // When calculating damage, this is used to check for attacker's damage modifiers, effects, etc.

    
}
