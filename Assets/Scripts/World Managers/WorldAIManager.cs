using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance { get; private set;}

    [Header("Characters")]
    [SerializeField] List<GameObject> spawnedCharacters;
    [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;

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
            foreach (var spawner in aiCharacterSpawners)
                SpawnCharacter(spawner);
        }
    }

    public void SpawnCharacter(AICharacterSpawner spawner)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        aiCharacterSpawners.Add(spawner);
        GameObject spawnedCharacter = spawner.AttemptToSpawnCharacter();
        if (spawnedCharacter != null)
            spawnedCharacters.Add(spawnedCharacter);
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
