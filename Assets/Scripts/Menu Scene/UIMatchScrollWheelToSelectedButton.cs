using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMatchScrollWheelToSelectedButton : MonoBehaviour
{
    [HideInInspector] GameObject currentSelected;
    [HideInInspector] GameObject previousSelected;
    [HideInInspector] RectTransform currentSelectedTransform;
    [SerializeField] RectTransform contentPanel; // Assign in inspector
    [SerializeField] ScrollRect scrollView; // Assign in inspector

    private void Update()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null && currentSelected != previousSelected)
        {
            previousSelected = currentSelected;
            currentSelectedTransform = currentSelected.GetComponent<RectTransform>();
            SnapTo(currentSelectedTransform);
        }
    }

    private void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 newPosition =
            (Vector2)scrollView.transform.InverseTransformPoint(contentPanel.position)
            - (Vector2)scrollView.transform.InverseTransformPoint(target.position);

        newPosition.x = 0; // Only snap along the y axis (up/down)

        contentPanel.anchoredPosition = newPosition;
    }
}
