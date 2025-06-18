using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PopupManager : MonoBehaviour
{
    [Header("YOU DIED popup")]
    [SerializeField] GameObject youDiedPopupGameObject;
    [SerializeField] TextMeshProUGUI youDiedPopupBackgroundText;
    [SerializeField] TextMeshProUGUI youDiedPopupText;
    [SerializeField] CanvasGroup youDiedPopupCanvasGroup;  // Allows us to set the alpha fade over time
    [SerializeField] private float textStretchSpeed = 0.05f;
    [SerializeField] private float textStretchTimeInSeconds = 8;
    [SerializeField] private float textStretchAmount = 30;//8.32f;
    [SerializeField] private float fadeInTimeInSeconds = 5;
    [SerializeField] private float popupWaitTimeInSeconds = 2;
    [SerializeField] private float fadeOutTimeInSeconds = 5;

    public void SendYouDiedPopup()
    {
        // Activate post-processing effects
        Debug.Log("in SendYouDiedPopup");
        // Activate popup
        youDiedPopupGameObject.SetActive(true);
        // Stretch out the text
        StartCoroutine(StretchPopupTextOverTime(youDiedPopupBackgroundText, textStretchTimeInSeconds, textStretchAmount, textStretchSpeed));
        // Fade in popup over time
        youDiedPopupCanvasGroup.alpha = 0.0f;  // Reset to invisible
        StartCoroutine(FadePopupOverTime(youDiedPopupCanvasGroup, fadeInTimeInSeconds, 0));
        // Wait
        // Fade out popup over time
        // youDiedPopupCanvasGroup.alpha = 1.0f;  // Make sure it is visible
        StartCoroutine(FadePopupOverTime(youDiedPopupCanvasGroup, fadeOutTimeInSeconds, fadeInTimeInSeconds + popupWaitTimeInSeconds));
        // // Deactivate object
        // youDiedPopupGameObject.SetActive(false);
    }

    private IEnumerator StretchPopupTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount, float stretchSpeed)
    {
        if (duration > 0)
        {
            text.characterSpacing = 0; // Resets our character spacing

            float startValue = text.characterSpacing;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float delta = stretchSpeed * timer / duration;
                text.characterSpacing = Mathf.Lerp(startValue, stretchAmount, delta);
                yield return null;
            }
        }

        yield return null;
    }

    private IEnumerator FadePopupOverTime(CanvasGroup canvas, float duration, float delayBeforeFading)
    {
        while (delayBeforeFading > 0)
        {
            delayBeforeFading -= Time.deltaTime;
            yield return null;
        }

        if (duration > 0)
        {
            // If we are currently faded out, fade in. And visa versa.
            float endAlpha = Mathf.Round(Mathf.Clamp01(1 - canvas.alpha));
            // canvas.alpha = 0; // Resets our fade

            float initial = Mathf.Round(canvas.alpha);
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float delta = timer / duration;
                canvas.alpha = Mathf.Lerp(initial, endAlpha, delta);
                yield return null;
            }

            canvas.alpha = endAlpha;  // Just make sure it fades all the way
        }

        yield return null;
    }

}
