using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterCombatManager : NetworkBehaviour
{
    protected CharacterManager character;

    [Header("Current Target")]
    public CharacterManager currentLockOnTarget;
    public float viewableAngle; // This is really angleToTarget. Would rename, but this is what he calls i.
    public Vector3 targetDirection;
    public float targetDistance;

    [Header("Current Weapon")]
    public AttackType currentAttackType;

    [Header("Lock On Transform")]
    [SerializeField] public Transform lockOnAnchor;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimationPerformed;

    [Header("Attack Flags")]
    public bool canDoRollingAttack = false;
    public bool canDoBackstepAttack = false;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
        
    }

    public virtual void SetLockOnTarget(CharacterManager newLockOnTarget)
    {
        if (character.IsOwner)
        {
            if (newLockOnTarget != null)
            {
                Debug.Log("Setting " + character.NetworkObjectId + "'s LockOn target to " + newLockOnTarget.NetworkObjectId + ".");
                currentLockOnTarget = newLockOnTarget;
                // Tell the network we have a target and what it is
                character.characterNetworkManager.lockOnTargetID.Value = newLockOnTarget.NetworkObjectId;// > Do I need: .GetComponent<NetworkObject>().NetworkObjectId;
            }
            else
            {
                Debug.Log("Clearing " + character.NetworkObjectId + "'s LockOn target.");
                currentLockOnTarget = null;
            }
        }
    }

    public void IFramesStart()
    {
        if (character.IsOwner)
            character.characterNetworkManager.isInvulnerable.Value = true;
    }

    public void IFramesStop()
    {
        if (character.IsOwner)
            character.characterNetworkManager.isInvulnerable.Value = false;
    }

    public virtual void EnableCanDoCombo()
    {
        
    }

    public virtual void DisableCanDoCombo()
    {
        
    }

    public void EnableCanDoRollingAttack()
    {
        canDoRollingAttack = true;
    }

    public void DisableCanDoRollingAttack()
    {
        canDoRollingAttack = false;
    }

    public void EnableCanDoBackstepAttack()
    {
        canDoBackstepAttack = true;
    }

    public void DisableCanDoBackstepAttack()
    {
        canDoBackstepAttack = false;
    }
}
