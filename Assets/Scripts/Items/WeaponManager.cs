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
    }
}
