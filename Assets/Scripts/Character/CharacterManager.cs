using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;


    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool applyRootMotion = false;
    public bool isRolling = false;
    public bool isBackstepping = false;
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool canRotate = true;
    public bool canMove = true;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        animator = GetComponent<Animator>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
    }
    protected virtual void Update()
    {
        animator.SetBool("IsGrounded", isGrounded);

        // If this character is being controlled from our side,
        //  set the network position and rotation to our position and rotation.
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
            // I added this in the "do it yourself" in episode 5
            if (animator != null)
            {
                characterNetworkManager.animatorHorizontalParameter.Value = animator.GetFloat("Horizontal");
                characterNetworkManager.animatorVerticalParameter.Value = animator.GetFloat("Vertical");
            }
        }
        else
        {
            // If this character is being controlled elsewhere,
            //  set our position and rotation to the network values.
            transform.position =
                Vector3.SmoothDamp(
                    transform.position,
                    characterNetworkManager.networkPosition.Value,
                    ref characterNetworkManager.networkPositionVelocity,
                    characterNetworkManager.networkPositionSmoothTime
                );
            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    characterNetworkManager.networkRotation.Value,
                    characterNetworkManager.networkRotationSmoothTime
                );
            // I added this in the "do it yourself" in episode 5
            characterAnimatorManager.UpdateAnimatorMovement(
                characterNetworkManager.animatorHorizontalParameter.Value,
                characterNetworkManager.animatorVerticalParameter.Value,
                characterNetworkManager.isSprinting.Value
            );
        }
    }

    protected virtual void LateUpdate()
    {

    }

    public virtual IEnumerator ProcessDeathEvent(bool overrideDeathAnimation = false)
    {
        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;

            // Reset any flags that need to be reset
            // Nothing yet

            // If we are not grounded, play aerial death animation.

            if (!overrideDeathAnimation)
            {
                // Play regular death animation (or select randomly from the standard set)
                characterAnimatorManager.PlayTargetActionAnimation("Standing React Death Forward", true);
                // Would this work better?
                // animator.CrossFade("Standing React Death Forward", 0.5f);
            }
        }

        // Play death SFX (to all players, not just owner)

        yield return new WaitForSeconds(5);

        // Award player with runes and other after-death effecets

        // Disable character
    }

    public virtual void ReviveCharacter()
    {
        isDead.Value = false;
    }
}
