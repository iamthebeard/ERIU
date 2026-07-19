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

    public void LoadRightWeapon()
    {
        if (player.playerInventoryManager.currentRightHandWeapon != null)
        {
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

    public void LoadLeftWeapon()
    {
        if (player.playerInventoryManager.currentLeftHandWeapon != null)
        {
            leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(leftHandWeaponModel, player.playerInventoryManager.currentLeftHandWeapon, true);
            leftHandWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
            leftHandWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
        }
    }
}
