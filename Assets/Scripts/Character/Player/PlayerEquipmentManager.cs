using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager player;

    public WeaponModelInstantiationSlot rightHandSlot;
    public WeaponModelInstantiationSlot leftHandSlot;

    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    [SerializeField] WeaponManager rightHandWeaponManager;
    [SerializeField] WeaponManager leftHandWeaponManager;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();

        // Get our weapon slots
        InitializeWeaponSlots();
    }

    protected override void Start()
    {
        base.Start();

        LoadWeaponsOnBothHands();
    }

    private void InitializeWeaponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.slotType == WeaponModelSlotType.RightHand)
            {
                rightHandSlot = weaponSlot;
            }
            else if (weaponSlot.slotType == WeaponModelSlotType.LeftHand)
            {
                leftHandSlot = weaponSlot;
            }
        }
    }

    public void LoadWeaponsOnBothHands()
    {
        LoadRightWeapon();
        LoadLeftWeapon();
    }

    // Right Weapon

    public void LoadRightWeapon()
    {
        if (player.playerInventoryManager.currentRightHandWeapon != null)
        {
            // Remove any already loaded weapon
            rightHandSlot.UnloadWeapon();

            // Instantiate a copy of the model.
            rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);

            // Assign the model to the right hand slot on our player model.
            // I send in the uninstantiated object. Why? Is it because those values don't get edited? 
            rightHandSlot.LoadWeapon(rightHandWeaponModel, player.playerInventoryManager.currentRightHandWeapon);

            // Assign the damage to the weapon collider.
            rightHandWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
            rightHandWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
        }
    }

    public void SwitchRightWeapon()
    {
        if(!player.IsOwner)
        {
            return;
        }

        // This is a hacked together animation, not a real weapon equip/switch animation
        player.playerAnimatorManager.PlayTargetActionAnimation("Action_A_4_1", false /*Not interacting*/, true, true, true);
        // If we have at least one other weapon, swap to next -- never to unarmed.
        // Otherwise, swap between unarmed and our single weapon

        player.playerInventoryManager.rightHandWeaponIndex = (player.playerInventoryManager.rightHandWeaponIndex + 1) % 3;


        // === Double-Switch Method ===
        // If we cycle to an unarmed weapon, try cycling again. 
        // If we have two weapons equipped, cycling past the unarmed will insure it is never equipped.
        // If we have only one weapon equipped, there will always be two unarmed weapons in a row, so it will still be equipped.
        // Example: Unarmed, Weapon, Unarmed.
        // On index 0, move to index 1, equip weapon.
        // On index 1, move to index 2, skip to index 0, equip unarmed.
        // On index 2, move to index 0, skip to index 1, equip weapon
        // Essentially always cycles between unarmed and armed.
        // Note: If no weapons are equipped, behaviour is a little weird as it will cycle between "different" unarmed slots
        if (
            player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID
            == WorldItemDatabase.Instance.unarmedWeapon.itemID
        )
        {
            player.playerInventoryManager.rightHandWeaponIndex = (player.playerInventoryManager.rightHandWeaponIndex + 1) % 3;
        }
        // Equip weapon in next slot
        player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID;

        // OLD METHOD (From https://www.youtube.com/watch?v=xrw_yOGp9Jo&list=PLD_vBJjpCwJvP9F9CeDRiLs08a3ldTpW5&index=21)
        // WeaponItem selectedWeapon = null;

        // // Disable two handing if we are two handing
        // // Check our weapon index (we have 3 slots)

        // // Increment within range 0-2
        // player.playerInventoryManager.rightHandWeaponIndex = player.playerInventoryManager.rightHandWeaponIndex + 1;
        // if (player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > 2)
        // {
        //     player.playerInventoryManager.rightHandWeaponIndex = 0;

        //     // Check if we are holding more than one weapon
        //     int weaponCount = 0;
        //     WeaponItem firstWeapon = null;
        //     int firstWeaponPosition = 0;

        //     for (int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
        //     {
        //         if (player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
        //         {
        //             weaponCount += 1;

        //             if (firstWeapon == null)
        //             {
        //                 firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];
        //                 firstWeaponPosition = i;
        //             }
        //         }
        //     }

        //     if (weaponCount <= 1)
        //     {
        //         // If we only have one weapon equipped, switching to a weapon means going to unarmed
        //         player.playerInventoryManager.rightHandWeaponIndex = -1;
        //         selectedWeapon = WorldItemDatabase.Instance.unarmedWeapon;
        //         player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
        //     }
        //     else
        //     {
        //         // Otherwise, switch to the next weapon
        //         player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
        //         player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
        //     }
        //     return;
        // }

        // foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInRightHandSlots)
        // {
        //     // Check to see if this is the unarmed weapon
        //     if (player.playerInventoryManager.weaponsInRightHandSlots[
        //             player.playerInventoryManager.rightHandWeaponIndex
        //         ].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID )
        //     {
        //         selectedWeapon =
        //             player.playerInventoryManager.weaponsInRightHandSlots[
        //                 player.playerInventoryManager.rightHandWeaponIndex
        //             ];
        //         // Assign network weapon ID so it switches for all connected clients
        //         player.playerNetworkManager.currentRightHandWeaponID.Value =
        //             player.playerInventoryManager.weaponsInRightHandSlots[
        //                 player.playerInventoryManager.rightHandWeaponIndex
        //             ].itemID;
        //         return;
        //     }
        // }
        // // selectedWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];
        // // player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;

        // if (selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
        // {
        //     SwitchRightWeapon(); // If this slot is empty, try next slot.
        // }
        // else
        // {
            
        // }
    }

    // Left Weapon

    public void LoadLeftWeapon()
    {
        if (player.playerInventoryManager.currentLeftHandWeapon != null)
        {
            leftHandSlot.UnloadWeapon();
            leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(leftHandWeaponModel, player.playerInventoryManager.currentLeftHandWeapon, true);
            leftHandWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
            leftHandWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
        }
    }

    public void SwitchLeftWeapon() // Uses Double-Switch Method
    {
        if(!player.IsOwner)
        {
            return;
        }

        // This is a hacked together animation, not a real weapon equip/switch animation
        player.playerAnimatorManager.PlayTargetActionAnimation("Action_A_4_1", false /*Not interacting*/, true, true, true);

        player.playerInventoryManager.leftHandWeaponIndex = (player.playerInventoryManager.leftHandWeaponIndex + 1) % 3;
        if (
            player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID
            == WorldItemDatabase.Instance.unarmedWeapon.itemID
        )
        {
            player.playerInventoryManager.leftHandWeaponIndex = (player.playerInventoryManager.leftHandWeaponIndex + 1) % 3;
        }
        // Equip weapon in next slot
        player.playerNetworkManager.currentLeftHandWeaponID.Value = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID;
    }
}
