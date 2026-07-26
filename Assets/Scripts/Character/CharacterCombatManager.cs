using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterCombatManager : NetworkBehaviour
{
    CharacterManager character;

    [Header("Current Target")]
    public CharacterManager currentLockOnTarget;

    [Header("Current Weapon")]
    public AttackType currentAttackType;

    [Header("Lock On Transform")]
    [SerializeField] public Transform lockOnAnchor;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void SetLockOnTarget(CharacterManager newLockOnTarget)
    {
        if (character.IsOwner)
        {
            if (newLockOnTarget != null)
            {
                currentLockOnTarget = newLockOnTarget;
                // Tell the network we have a target and what it is
                character.characterNetworkManager.lockOnTargetID.Value = newLockOnTarget.NetworkObjectId;// > Do I need: .GetComponent<NetworkObject>().NetworkObjectId;
            }
            else
            {
                currentLockOnTarget = null;
            }
        }
    }
}
