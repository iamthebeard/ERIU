using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotionManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Flags")]
    public bool isRolling = false;
    public bool isBackstepping = false;
    public bool isGrounded = true;
    public bool canRotate = true;
    public bool canMove = true;

    [Header("Ground & Jumping")]
    [SerializeField] protected float gravityForce = -9.86f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius = 0.3f;
    [SerializeField] protected Vector3 yVelocity; // The "force" with which our character is pulled up or down (jumping or falling)
    [SerializeField] protected float groundedYVelocity = -20; // The "force" with which our character is stuck to the ground whilst they are grounded
    [SerializeField] protected float fallStartYVelocity = -5; // The force at which our character begins to fall when they become ungrounded (rises as they fall longer)
    [SerializeField] protected bool fallingVelocityHasBeenSet = false;
    [SerializeField] protected float inAirTimer = 0;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        HandleGroundCheck();

        if (character.characterLocomotionManager.isGrounded)
        {
            // If we are not attempting to jump or move upward
            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else // If we are jumping or moving upward (in the air)
        {
            // If we are NOT jumping and we haveN'T set our falling velocity
            if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity;
            }

            inAirTimer += Time.deltaTime;
            character.animator.SetFloat("InAirTimer", inAirTimer);
            yVelocity.y += gravityForce * Time.deltaTime;
        }
        // Based on the above calculations, apply gravity.
        character.characterController.Move(yVelocity * Time.deltaTime);
    }

    protected void HandleGroundCheck()
    {
        character.characterLocomotionManager.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
    }

    protected void OnGizmosSelected()
    {
        Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
    }

    public void EnableCanRotate()
    {
        character.characterLocomotionManager.canRotate = true;
    }

    public void DisableCanRotate()
    {
        character.characterLocomotionManager.canRotate = false;
    }
}
