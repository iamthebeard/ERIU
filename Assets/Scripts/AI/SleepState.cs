using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Sleep")]
public class SleepState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        return base.Tick(aiCharacter);
    }
}
