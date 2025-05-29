using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterNetworkManager : NetworkBehaviour
{
    [HideInInspector] CharacterManager character;

    [Header("Position")]
    public NetworkVariable<Vector3> networkPosition =
        new NetworkVariable<Vector3>(
            Vector3.zero,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
    public Vector3 networkPositionVelocity;
    public float networkPositionSmoothTime = 0.1f;

    public NetworkVariable<Quaternion> networkRotation =
        new NetworkVariable<Quaternion>(
            Quaternion.identity,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
    public float networkRotationSmoothTime = 0.1f;

    // I added this in the "do it yourself" in episode 5
    [Header("Animator")]
    public NetworkVariable<float> animatorHorizontalParameter =
        new NetworkVariable<float>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
    public NetworkVariable<float> animatorVerticalParameter =
        new NetworkVariable<float>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
    
    [Header("Flags")]
    public NetworkVariable<bool> isSprinting =
        new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
    
    protected virtual void Awake() {
        character = GetComponent<CharacterManager>();
    }

    // A server RPC is a function called from a client to a server/host
    [ServerRpc]
    public void NotifyOfActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion, bool isRolling, bool isBackstepping) {
        if (IsServer) {
            PlayActionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion, isRolling, isBackstepping);
        }
    }

    // A client RPC is a function called from the server/host to all clients present
    [ClientRpc]
    public void PlayActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion, bool isRolling, bool isBackstepping) {
        // Make sure not to run the function on the character who sent it.
        if (clientID != NetworkManager.Singleton.LocalClientId) {
            PerformActionAnimationFromServer(animationID, applyRootMotion, isRolling, isBackstepping);
        }
    }

    // Question: Why can't I just call PlayTargetActionAnimation?
    private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion, bool isRolling, bool isBackstepping) {
        character.applyRootMotion = applyRootMotion;
        character.isRolling = isRolling;
        character.isBackstepping = isBackstepping;
        character.animator.CrossFade(animationID, 0.2f);
    }
}
