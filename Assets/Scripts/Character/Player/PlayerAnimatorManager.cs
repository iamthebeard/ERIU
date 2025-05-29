using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    private void OnAnimatorMove() {
        if (character.applyRootMotion) {
            Vector3 velocity = character.animator.deltaPosition;
            character.characterController.Move(velocity);
            character.transform.rotation *= character.animator.deltaRotation;
        }
    }
}
