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

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        animator = GetComponent<Animator>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
    }
    protected virtual void Update()
    {
        // If this character is being controlled from our side,
        //  set the network position and rotation to our position and rotation.
        if (IsOwner) {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
            if(animator != null) {
                characterNetworkManager.animatorHorizontalParameter.Value = animator.GetFloat("Horizontal");
                characterNetworkManager.animatorVerticalParameter.Value = animator.GetFloat("Vertical");
            }
        } else {
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
            characterAnimatorManager.UpdateAnimatorMovement(
                characterNetworkManager.animatorHorizontalParameter.Value,
                characterNetworkManager.animatorVerticalParameter.Value
            );
        }
    }

    protected virtual void LateUpdate() {
        
    }
}
