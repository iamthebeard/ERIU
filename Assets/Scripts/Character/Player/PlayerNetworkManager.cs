using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerNetworkManager : CharacterNetworkManager
{
    [Header("Player Name")]
    public NetworkVariable<FixedString64Bytes> characterName =
        new NetworkVariable<FixedString64Bytes>(
            "Character",
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
    public NetworkVariable<FixedString64Bytes> timePlayed =
        new NetworkVariable<FixedString64Bytes>(
            "00:00:00",
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
}
