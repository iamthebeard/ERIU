using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterAnimatorManager : MonoBehaviour
{
    public CharacterManager character;

    // int horizontal;
    // int vertical;

    [Header("Damage Animations")]
    [SerializeField] public string hitForwardMedium01 = "Hit_F_1";  // "hit_Forward_Medium_01";
    [SerializeField] public string hitForwardMedium02 = "Hit_F_2";
    [SerializeField] public string hitForwardMedium03 = "";
    [SerializeField] public string hitBackwardMedium01 = "Hit_F_1";  // "hit_Backward_Medium_01";
    [SerializeField] public string hitBackwardMedium02 = "";
    [SerializeField] public string hitBackwardMedium03 = "";
    [SerializeField] public string hitLeftMedium01 = "Hit_F_1";  // "hit_Left_Medium_01";
    [SerializeField] public string hitLeftMedium02 = "";
    [SerializeField] public string hitLeftMedium03 = "";
    [SerializeField] public string hitRightMedium01 = "Hit_F_1";  // "hit_Right_Medium_01";
    [SerializeField] public string hitRightMedium02 = "";
    [SerializeField] public string hitRightMedium03 = "";

    [SerializeField] public List<string> onHitAnimations = new List<string>();
    public string lastNonRepeatableAnimationPlayed;

    protected virtual void Awake() {
        character = GetComponent<CharacterManager>();

        // Could pass these hash values instead of strings as the first argument to "SetFloat" below.
        // horizontal = Animator.StringToHash("Horizontal");
        // vertical = Animator.StringToHash("Vertical");
    }

    protected virtual void Start()
    {
        // Add all animations to their various animation sets.
        onHitAnimations.Add(hitForwardMedium01);
        onHitAnimations.Add(hitForwardMedium02);
    }

    public string GetRandomAnimationFromList(List<string> animationList, bool dontRepeat = false)
    {
        // Don't worry about nulls for now.
        // animationList.RemoveAll(x => x == null);

        int randomIndex = Random.Range(0, animationList.Count);
    
        // Check if we've played a recent damage animation in this list
        if (dontRepeat && animationList.Contains(lastNonRepeatableAnimationPlayed))
        {
            int lastAnimationIndex = animationList.IndexOf(lastNonRepeatableAnimationPlayed);
            if (randomIndex == lastAnimationIndex)
            {
                // If the randomly selected item is later on the list than the non-repeatable one,
                //  skip the non-repeateable one, looping to the front if necessary.
                randomIndex = (randomIndex + 1) % animationList.Count;
            }
        }

        return animationList[randomIndex];
    }

    public void UpdateAnimatorMovement(float horizontalMovement, float verticalMovement, bool isSprinting){
        float horizontal = horizontalMovement;
        float vertical = verticalMovement;
        
        if (isSprinting) 
            vertical = 2;

        // Option 1
        character.animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        character.animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);

        // Option 2: Snapped (not used because our movement is already snapped, and our animation looks fine blended)
        // float snappedHorizontal = Mathf.Round(Mathf.Clamp(horizontalMovement, -1, 1) * 2) / 2;
        // float snappedVertical = Mathf.Round(Mathf.Clamp(verticalMovement, -1, 1) * 2) / 2;
    }

    public virtual void PlayTargetActionAnimation(
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false
    ) {
        Debug.Log("Playing " + targetAnimation + " on " + character.NetworkObjectId + ".");
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

    public virtual void PlayTargetAttackActionAnimation(
        AttackType attackType,
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false
    ) {
        Debug.Log("Playing " + targetAnimation + " on " + character.NetworkObjectId + ".");

        // New to attacks
        // Keep track of last attack performed (for combos)
        character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;
        // Keep track of current attack type (light, heavy, etc. for parries, bounces, etc.)
        character.characterCombatManager.currentAttackType = attackType;
        // Update animation set to current weapon's animations
        // Decide if our attack can be parried
        // Tell the network our "isAttacking" flag (for counter damage, etc.)

        // Same as PlayTargetActionAnimation
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;

        // Tell the server/host about this animation action.
        character.characterNetworkManager.NotifyOfAttackActionAnimationServerRpc(
            NetworkManager.Singleton.LocalClientId,
            targetAnimation,
            applyRootMotion,
            character.isRolling,
            character.isBackstepping
        );
    }

    public virtual void EnableCanDoCombo()
    {
        
    }

    public virtual void DisableCanDoCombo()
    {
        
    }
}
