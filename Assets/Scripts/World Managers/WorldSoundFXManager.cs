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

    [Header("Boss Tracks")]
    [SerializeField] AudioSource bossIntroPlayer = new AudioSource();
    [SerializeField] AudioSource bossLoopPlayer = new AudioSource();

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

    public void PlayBossTrack(AudioClip introTrack, AudioClip loopTrack)
    {
        bossIntroPlayer.clip = introTrack;
        bossIntroPlayer.loop = false;
        bossIntroPlayer.volume = 1;
        bossIntroPlayer.Play();

        bossLoopPlayer.clip = loopTrack;
        bossLoopPlayer.loop = true;
        bossLoopPlayer.volume = 1;
        bossLoopPlayer.PlayDelayed(bossIntroPlayer.clip.length);
    }

    public void StopBossMusic()
    {
        StartCoroutine(FadeOutBossMusicThenStop());
    }

    private IEnumerator FadeOutBossMusicThenStop()
    {
        while(bossLoopPlayer.volume > 0)
        {
            bossIntroPlayer.volume -= Time.deltaTime;
            bossLoopPlayer.volume -= Time.deltaTime;
            yield return null;
        }
        bossIntroPlayer.Stop();
        bossLoopPlayer.Stop();
    }

    public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
    {
        if (array == null || array.Length == 0) 
            return null;
        int index = Random.Range(0, array.Length);
        return array[index];
    }
}
