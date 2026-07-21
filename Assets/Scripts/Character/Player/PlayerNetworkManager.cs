using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class PlayerNetworkManager : CharacterNetworkManager
{
    PlayerManager player;

    [Header("Player")]
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

    [Header("Equipment")]
    public NetworkVariable<int> currentRightHandWeaponID = 
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
    public NetworkVariable<int> currentLeftHandWeaponID = 
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
    public NetworkVariable<int> currentWeaponBeingUsedID = 
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
    public NetworkVariable<bool> isUsingRightHand = 
        new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
    public NetworkVariable<bool> isUsingLeftHand = 
        new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public void SetCharacterActionHand(bool rightHandedAction)
    {
        if(rightHandedAction)
        {
            isUsingRightHand.Value = true;
            isUsingLeftHand.Value = false;
        }
        else
        {
            isUsingRightHand.Value = false;
            isUsingLeftHand.Value = true;
        }
    }
    
    public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentRightHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadRightWeapon();
    }

    public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadLeftWeapon();
    }

    public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
        player.playerCombatManager.currentWeaponBeingUsed = newWeapon;
    }

    // Item based actions
    [ServerRpc]
    public void NotifyServerOfWeaponActionServerRpc(ulong clientID, int actionID, int weaponID)
    {
        if (IsServer)
        {
            NotifyServerOfWeaponActionClientRpc(clientID, actionID, weaponID);
        }
    }

    [ClientRpc]
    public void NotifyServerOfWeaponActionClientRpc(ulong clientID, int actionID, int weaponID)
    {
        // Do not play the action again for the client who called it
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformWeaponAction(actionID, weaponID);
        }
    }

    private void PerformWeaponAction(int actionID, int weaponID)
    {
        WeaponItemAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(actionID);

        if (weaponAction == null)
        {
            UnityEngine.Debug.LogError("Action " + actionID + "is null; cannot be performed.");
            return;
        }

        weaponAction.AttemptToPerformAction(player, WorldItemDatabase.Instance.GetWeaponByID(weaponID));
    }
}
