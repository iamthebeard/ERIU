using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] public MeleeWeaponDamageCollider meleeDamageCollider;

    private void Awake()
    {
        meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager characterWeildingWeapon, WeaponItem weapon)
    {
        meleeDamageCollider.characterCausingDamage = characterWeildingWeapon;
        meleeDamageCollider.damage = weapon.damage;

        meleeDamageCollider.lightAttack01Modifier = weapon.lightAttack01Modifier;
        meleeDamageCollider.lightAttack01PoiseModifier = weapon.lightAttack01PoiseModifier;

        meleeDamageCollider.lightAttack02Modifier = weapon.lightAttack02Modifier;
        meleeDamageCollider.lightAttack02PoiseModifier = weapon.lightAttack02PoiseModifier;

        meleeDamageCollider.heavyAttack01Modifier = weapon.heavyAttack01Modifier;
        meleeDamageCollider.heavyAttack01PoiseModifier = weapon.heavyAttack01PoiseModifier;

        meleeDamageCollider.heavyAttack02Modifier = weapon.heavyAttack02Modifier;
        meleeDamageCollider.heavyAttack02PoiseModifier = weapon.heavyAttack02PoiseModifier;

        meleeDamageCollider.chargedHeavyAttack01Modifier = weapon.chargedHeavyAttack01Modifier;
        meleeDamageCollider.chargedHeavyAttack01PoiseModifier = weapon.chargedHeavyAttack01PoiseModifier;

        meleeDamageCollider.chargedHeavyAttack02Modifier = weapon.chargedHeavyAttack02Modifier;
        meleeDamageCollider.chargedHeavyAttack02PoiseModifier = weapon.chargedHeavyAttack02PoiseModifier;
        
    }
}
