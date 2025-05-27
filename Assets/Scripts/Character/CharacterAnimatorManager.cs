using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    public CharacterManager character;

    protected virtual void Awake() {

    }

    public void UpdateAnimatorMovement(float horizontalMovement, float verticalMovement){
        // Option 1
        character.animator.SetFloat("Horizontal", horizontalMovement, 0.1f, Time.deltaTime);
        character.animator.SetFloat("Vertical", verticalMovement, 0.1f, Time.deltaTime);

        // Option 2: Snapped (not used because our movement is already snapped, and our animation looks fine blended)
        float snappedHorizontal = Mathf.Round(Mathf.Clamp(horizontalMovement, -1, 1) * 2) / 2;
        float snappedVertical = Mathf.Round(Mathf.Clamp(verticalMovement, -1, 1) * 2) / 2;
    }
}
