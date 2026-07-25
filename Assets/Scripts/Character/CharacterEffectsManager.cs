using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("VFX")]
    [SerializeField] GameObject bloodSpatterVFX;

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

    // Process visual effects
    public void PlayBloodSpatterVFX(Vector3 contactPoint, float angleHitFrom /*I'm trying to add this*/)
    {
        Debug.Log("Playing blood spatter");
        // If we manually have placed a blood spatter VFX for this model, play that instead.
        if (bloodSpatterVFX != null)
        {
            GameObject bloodSpatter = Instantiate(bloodSpatterVFX, contactPoint, Quaternion.identity);
        }
        else // Use default VFX
        {
            GameObject bloodSpatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSpatterVFX, contactPoint, Quaternion.Euler(0, angleHitFrom, 0));
        }

    }
}
