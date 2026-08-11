using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    public static WorldSoundFXManager instance;

    [Header("Action Sounds")]
    public AudioClip[] rollSFX;

    [Header("Weapon Sounds")]
    public AudioClip[] bladeHitSFX;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
    {
        if (array == null || array.Length == 0) 
            return null;
        int index = Random.Range(0, array.Length);
        return array[index];
    }
}
