using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    [Header("Current Target")]
    public CharacterManager currentTarget;

    [Header("Current Weapon")]
    public AttackType currentAttackType;

    [Header("Lock On Transform")]
    [SerializeField] public Transform lockOnAnchor;

    protected virtual void Awake()
    {
        
    }
}
