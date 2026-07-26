using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTint : MonoBehaviour
{
    [SerializeField] public Material sharedMaterial;

    // Start is called before the first frame update
    void Start()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var ren in renderers)
        {
            ren.sharedMaterial = sharedMaterial;
        }
    }
}
