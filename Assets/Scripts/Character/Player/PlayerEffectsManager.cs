using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    [Header("Debug -- Delete Later")]
    [SerializeField] InstantCharacterEffect testInstantEffect;
    [SerializeField] bool processTestInstantEffect = false;

    private void Update() {
        if (processTestInstantEffect)
        {
            processTestInstantEffect = false;
            // Deep copy
            InstantCharacterEffect effect = Instantiate(testInstantEffect);
            ProcessInstantEffect(effect);
        }
    }
}
