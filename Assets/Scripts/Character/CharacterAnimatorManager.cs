using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

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

    public virtual void PlayTargetActionAnimation(
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false
    ) {
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        // Can be used to stop character from attempting new actions
        // For example, if you get damaged, and begin performing a damage animation,
        //  this flag will turn true if you are stunned.
        //  We can then check for this before attmepting new actions.
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;

        // Tell the server/host about this animation action.
        character.characterNetworkManager.NotifyOfActionAnimationServerRpc(
            NetworkManager.Singleton.LocalClientId,
            targetAnimation,
            applyRootMotion,
            character.isRolling,
            character.isBackstepping
        );
    }
}
