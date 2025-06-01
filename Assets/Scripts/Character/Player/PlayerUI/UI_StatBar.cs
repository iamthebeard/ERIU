using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    private Slider slider;
    private RectTransform rectTransform;
    // Secondary bar behind main bar for polish effect (yellow bar that shows how much stamina an action used)

    [Header("Bar Options")]
    // Variables to scale bar size depending on stat (Higher stat -> longer the bar)
    [SerializeField] protected bool scaleBarLengthWithStats = true;
    [SerializeField] protected float widthScaleMultiplier = 1;

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void SetStat(float newValue) {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue)
    {
        slider.maxValue = maxValue;
        SetStat(maxValue);

        if (scaleBarLengthWithStats)
        {
            // Scale the transform of this object
            rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
        }
        // Reset the HUD
        PlayerUIManager.instance.playerUIHUDManager.ResetHUD();
    }
}
