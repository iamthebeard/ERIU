using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelInstantiationSlot : MonoBehaviour
{
    public WeaponModelSlotType slotType; // Left hand, right hand, hip, back, etc.
    public GameObject currentWeaponModel;

    public void UnloadWeapon()
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
    }

    public void LoadWeapon(GameObject weaponModelToLoad, WeaponItem weapon, bool reverse = false)
    {
        currentWeaponModel = weaponModelToLoad;
        weaponModelToLoad.transform.parent = transform;
        // weaponModelToLoad.transform.localPosition = Vector3.zero;
        // weaponModelToLoad.transform.localRotation = Quaternion.identity;
        // weaponModelToLoad.transform.localScale = Vector3.one;

        // Transform modelTransform = weaponModelToLoad.transform;
        // weaponModelToLoad.transform.position = modelTransform.localPosition;
        // weaponModelToLoad.transform.rotation = modelTransform.rotation;
        // weaponModelToLoad.transform = modelTransform.localScale;

        // weaponModelToLoad.transform.localScale = 100 * Vector3.one;

        int reflection = reverse ? -1 : 1;  // For left-hand weapons, some values need to be reflected.
                                            // So far, that seems to be the xPosition and the zRotation. 
                                            // Need to dig further into what this should really be.
        weaponModelToLoad.transform.localPosition = new Vector3(reflection * weapon.xPosition, weapon.yPosition, weapon.zPosition);
        weaponModelToLoad.transform.localRotation = Quaternion.Euler(weapon.xRotation, weapon.yRotation, reflection * weapon.zRotation);
        weaponModelToLoad.transform.localScale = new Vector3(weapon.xScale, weapon.yScale, weapon.zScale);
    }
}
