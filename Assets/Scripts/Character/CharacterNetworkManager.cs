using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterNetworkManager : NetworkBehaviour
{
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

    // I added this in the "do it yourself" in episode 6
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
}
