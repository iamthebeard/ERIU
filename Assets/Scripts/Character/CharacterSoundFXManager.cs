using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Default Sound FX")]
    public AudioClip[] whooshes;

    protected virtual void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundFX(AudioClip soundFX, float volume = 1.0f, bool randomizePitch = true, float pitchRandom = 0.1f)
    {
        audioSource.PlayOneShot(soundFX, volume);
        audioSource.pitch = 1; // Reset pitch

        if (randomizePitch)
        {
            audioSource.pitch += Random.Range(-pitchRandom, pitchRandom); // Randomly adjust pitch by +/- 10%
        }
    }

    public void PlayRollSoundFX() {
        // audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
        PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.rollSFX));
    }

    public void Whoosh(WeaponItem weapon = null)
    {
        AudioClip[] whooshesToPlay;

        if (weapon != null && weapon.whooshes != null && weapon.whooshes.Length > 0)
            whooshesToPlay = weapon.whooshes;
        else
            whooshesToPlay = whooshes;
        
        PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(whooshesToPlay));

    }
}
