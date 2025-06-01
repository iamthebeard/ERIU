using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    CharacterManager character;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    // Process instant effects (taking damage, blocking, healing)
    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        // Take in an effect
        // Process it
        effect.ProcessEffect(character);
    }

    // Process over time effects (poison damage, status build up)

    // Process static effects (trinkets, buffs)
}
