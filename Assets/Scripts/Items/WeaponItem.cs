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
    [SerializeField] public Damage damage = new Damage();

    [Header("Weapon modifiers")]
    //  Light attack, heavy attack, critical damage, etc.
    public float lightAttack01Modifier = 1;
    public float lightAttack01PoiseModifier = 1;
    public float heavyAttack01Modifier = 1.5f;
    public float heavyAttack01PoiseModifier = 1.5f;
    public float chargedHeavyAttack01Modifier = 2;
    public float chargedHeavyAttack01PoiseModifier = 2;
    // Poise bonus while attacking
    // Poise modifiers, etc.

    // Guard absorptions

    [Header("Stamina Costs")]
    public int baseStaminaCost = 20;
    public float lightAttack01StaminaCostModifier = 1;
    // Modifiers: light attack, heavy, running, etc.

    // Item based actions (RB, RT, LB, LT)
    [Header("Actions")]
    public WeaponItemAction rb_Action_OneHanded; // One hand right bumper/button action
    public WeaponItemAction rt_Action_OneHanded;
    public WeaponItemAction rtCharged_Action_OneHanded;

    // Ash of war

    // SFX

    [Header("Transformation to Wield")]
    public float xPosition;
    public float yPosition;
    public float zPosition;
    public float xRotation;
    public float yRotation;
    public float zRotation;
    public float xScale;
    public float yScale;
    public float zScale;
}
