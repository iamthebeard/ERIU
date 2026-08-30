using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIManager : MonoBehaviour
{
    public UI_CharacterBar characterBar;
    public bool hasFloatingHPBar = true;

    public void OnHPChanged(int oldValue, int newValue)
    {
        characterBar.oldHealthValue = oldValue;
        characterBar.SetStat(newValue);
    }
}
