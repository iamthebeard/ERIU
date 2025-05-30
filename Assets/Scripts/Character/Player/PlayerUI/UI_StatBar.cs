using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    private Slider slider;
    // Variable to scale bar size depending on stat (Higher stat -> longer the bar)
    // Secondary bar behind main bar for polish effect (yellow bar that shows how much stamina an action used)

    protected virtual void Awake() {
        slider = GetComponent<Slider>();
    }

    public virtual void SetStat(float newValue) {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue) {
        slider.maxValue = maxValue;
        SetStat(maxValue);
    }
}
