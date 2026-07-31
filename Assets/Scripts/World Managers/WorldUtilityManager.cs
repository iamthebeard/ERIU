using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager Instance;

    [Header("Layers")]
    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask environmentLayers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public LayerMask GetCharacterLayers()
    {
        return characterLayers;
    }

    public LayerMask GetEnvironmentLayers()
    {
        return environmentLayers;
    }

    public bool IsHostileTo(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
    {
        return (attackingCharacter != targetCharacter); // Right now, just as long as they aren't on the same team
        // Eventually will have a switch for every combination
        // if (attackingCharacter == CharacterGroup.Friendly)
        // {
        //     switch (targetCharacter)
        //     {
        //         case CharacterGroup.Friendly:
        //             //
        //             break;
        //         case CharacterGroup.Hostile:
        //             //
        //             break;
        //         // etc.
        //     }
        // }
        // else if (attackingCharacter == CharacterGroup.Hostile)
        // {
            
        // }
    }

    public float GetAngleOfTarget(Transform characterTransform, Vector3 targetDirection)
    {
        targetDirection.y = 0;
        float viewableAngle = Vector3.Angle(characterTransform.forward, targetDirection);
        Vector3 cross = Vector3.Cross(characterTransform.forward, targetDirection);

        if (cross.y < 0)
            viewableAngle = -viewableAngle;

        return viewableAngle;
    }
}
