using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance { get; private set;}

    [Header("Characters")]
    [SerializeField] GameObject[] aiCharacters;
    [SerializeField] List<GameObject> spawnedCharacters;

    [Header("DEBUG")]
    [SerializeField] bool despawnAICharacters = false;
    [SerializeField] bool respawnAICharacters = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer /* || NetworkManager.Singleton.IsHost ?*/)
        {
            // Spawn all AI in scene
            StartCoroutine(WaitForSceneToLoadThenSpawnCharacters());
        }
    }

    private void Update()
    {
        if (despawnAICharacters)
        {
            despawnAICharacters = false;
            DespawnAllCharacters();
        }
        if (respawnAICharacters)
        {
            respawnAICharacters = false;
            SpawnAllCharacters();
        }
    }

    private IEnumerator WaitForSceneToLoadThenSpawnCharacters()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
            yield return null;
        
        SpawnAllCharacters();
    }

    private void SpawnAllCharacters()
    {
        foreach (var character in aiCharacters)
        {
            GameObject instantiatedCharacter = Instantiate(character);
            instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
            spawnedCharacters.Add(instantiatedCharacter);
        }
    }

    private void DespawnAllCharacters()
    {
        foreach (var character in spawnedCharacters)
        {
            character.GetComponent<NetworkObject>().Despawn();
            spawnedCharacters.Remove(character);
        }
    }

    private void DisableAllCharacters()
    {
        // TODO to disable all characters & sync disabled status on network
        // (Beneficial for memory load to spawn everything at beginning and enable/disable on demand)
        // (Splitting characters into areas, etc.)
    }
}
