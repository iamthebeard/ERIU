using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Default Sound FX")]
    [SerializeField] public AudioClip[] whooshes;
    [SerializeField] public AudioClip[] damageGrunts;
    [SerializeField] public AudioClip[] attackGrunts;
    [SerializeField] public AudioClip[] footstepsGeneric;

    protected virtual void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundFX(AudioClip soundFX, float volume = 1.0f, bool randomizePitch = true, float pitchRandom = 0.1f)
    {
        if (soundFX == null) return;

        
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

    public virtual void Grunt(string type = "damage")
    {
        if (type == "attack")
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunts));
        else
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));
    }

    public virtual void Footstep(string type = "default")
    {
        switch (type)
        {
            default:
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(footstepsGeneric));
                break;
        }
    }
}
