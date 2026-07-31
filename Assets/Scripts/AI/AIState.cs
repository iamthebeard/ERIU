using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Base")]
public class AIState : ScriptableObject
{
    public virtual AIState Tick(AICharacterManager aICharacter)
    {
        Debug.Log("AI Tick for " + aICharacter.name + " (" + aICharacter.NetworkObjectId + ")");
        // Do logic to find the player
        // If we have found the player
        //  Do logic to determine the next state.
        //  i.e. If we have a target, change to pursue state.
        //  If we are within attack range, change to attack state.
        return this;
    }

    protected virtual AIState SwitchState(AICharacterManager aiCharacter, AIState newState)
    {
        ResetStateFlags(aiCharacter);
        return newState;
    }

    protected virtual void ResetStateFlags(AICharacterManager aiCharacter)
    {
        
    }
}
