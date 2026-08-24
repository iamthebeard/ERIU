using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{

}

public enum CharacterSlot
{
    CharacterSlot01,
    CharacterSlot02,
    CharacterSlot03,
    CharacterSlot04,
    CharacterSlot05,
    CharacterSlot06,
    CharacterSlot07,
    CharacterSlot08,
    CharacterSlot09,
    CharacterSlot10,
    NoSlot,
}

public enum CharacterGroup
{
    Friendly,
    Hostile
}

public enum WeaponModelSlotType
{
    RightHand,
    LeftHand,
    // Hips, back, etc.
}

public enum AttackType
{
    LightAttack01,
    LightAttack02,
    HeavyAttack01,
    HeavyAttack02,
    ChargedHeavy01,
    ChargedHeavy02,
    RunningAttack01,
    RollingAttack01,
    BackstepAttack01
}
