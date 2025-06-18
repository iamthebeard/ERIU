using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponItem : Item
{
    // Animator controller override (to change attack animations based on weapon equipped)

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Requirements")]
    public int strReq = 0;
    public int dexReq = 0;
    public int intReq = 0;
    public int faiReq = 0;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    public int mageDamage = 0;
    public int fireDamage = 0;
    public int holyDamage = 0;
    public int lightningDamage = 0;

    // Weapon modifiers:
    //  Light attack, heavy attack, critical damage, etc.

    [Header("Poise DAmage")]
    public float basePoiseDamage = 10;
    // Poise bonus while attacking
    // Poise modifiers, etc.

    // Guard absorptions

    [Header("Stamina Costs")]
    public int baseStaminaCost = 20;
    // Modifiers: light attack, heavy, running, etc.

    // Item based actions (RB, RT, LB, LT)

    // Ash of war

    // SFX
}
