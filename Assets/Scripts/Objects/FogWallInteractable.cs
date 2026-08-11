using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FogWallInteractable : NetworkBehaviour
{
    [SerializeField] GameObject[] fogWallGameObjects;

    // [Header("Active")]
    public string fogWallBossID;
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        OnIsActiveChanged(false, isActive.Value);
        isActive.OnValueChanged += OnIsActiveChanged;

        WorldObjectManager.instance.AddFogWallToList(this);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        isActive.OnValueChanged -= OnIsActiveChanged;
    }

    private void OnIsActiveChanged(bool oldStatus, bool newStatus)
    {
        foreach (var fogObj in fogWallGameObjects)
        {
            fogObj.SetActive(newStatus);
        }
    }
}
