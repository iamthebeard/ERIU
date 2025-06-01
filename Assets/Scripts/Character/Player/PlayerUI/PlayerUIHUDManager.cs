using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIHUDManager : MonoBehaviour
{
    [SerializeField] UI_StatBar healthBar;
    [SerializeField] UI_StatBar staminaBar;

    public void ResetHUD()
    {
        healthBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(true);
    }

    public void SetNewStaminaValue(float oldValue, float newValue)
    {
        staminaBar.SetStat(newValue);
    }

    public void SetMaxStaminaValue(int maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }
    
    public void SetNewHealthValue(float oldValue, float newValue)
    {
        healthBar.SetStat(newValue);
    }

    public void SetMaxHealthValue(int maxHealtha) {
        healthBar.SetMaxStat(maxHealtha);
    }
}
