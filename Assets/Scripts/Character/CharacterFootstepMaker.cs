using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootstepMaker : MonoBehaviour
{
    [HideInInspector] CharacterManager character;
    AudioSource audioSource;
    GameObject steppedOnObject;

    private bool hasTouchedGround = false;
    private bool hasPlayedFootstepSFX = false;

    private float touchingGroundRadius = 0.05f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        character = GetComponentInParent<CharacterManager>();
    }

    private void FixedUpdate()
    {
        CheckForFootsteps();

        if (hasTouchedGround && !hasPlayedFootstepSFX)
        {
            hasPlayedFootstepSFX = true;

            // TODO: Determine sound based on the type of ground stepped on

            PlayFootstepSFX();
        }
    }

    private void CheckForFootsteps()
    {
        if (character == null) return;
        if (!character.characterNetworkManager.isMoving.Value) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, character.transform.TransformDirection(Vector3.down), out hit, touchingGroundRadius, WorldUtilityManager.Instance.GetEnvironmentLayers()))
        {
            hasTouchedGround = true;

            if (!hasPlayedFootstepSFX)
            {
                steppedOnObject = hit.transform.gameObject;
            }
        }
        else
        {
            hasTouchedGround = false;
            hasPlayedFootstepSFX = false;
            steppedOnObject = null;
        }
    }

    private void PlayFootstepSFX()
    {
        character.characterSoundFXManager.Footstep();
    }
}
