using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    public static WorldCharacterEffectsManager instance;

    [SerializeField] public TakeDamageEffect takeDamageEffect;
    [SerializeField] List<InstantCharacterEffect> instantEffects;
    [SerializeField] List<BaseCharacterEffect> otherEffects; // Temporary

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        GenerateEffectIDs();
    }

    private void GenerateEffectIDs()
    {
        for (int i = 0; i < instantEffects.Count + otherEffects.Count; i++)
        {
            if (i < instantEffects.Count)
                instantEffects[i].effectID = i;
            else if (i < instantEffects.Count + otherEffects.Count)
                otherEffects[i-instantEffects.Count].effectID = i;
        }
    }
}
