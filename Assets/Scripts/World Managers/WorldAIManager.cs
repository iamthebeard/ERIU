using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance { get; private set;}

    [Header("Characters")]
    [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
    [SerializeField] List<AICharacterManager> spawnedCharacters;
    [SerializeField] List<AIBossManager> spawnedBosses;
    

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
        spawner.AttemptToSpawnCharacter();
        // if (spawnedCharacter != null)
        //     spawnedCharacters.Add(spawnedCharacter);
    }

    public void AddCharacterToSpawnedCharactersList(AICharacterManager aiCharacter)
    {
        if (spawnedCharacters.Contains(aiCharacter)) return;
        spawnedCharacters.Add(aiCharacter);

        AIBossManager bossCharacter = aiCharacter as AIBossManager;

        if (bossCharacter != null)
        {
            // This is a boss character
            if (spawnedBosses.Contains(bossCharacter)) return;
                spawnedBosses.Add(bossCharacter);
        }
    }

    public AIBossManager GetBossByID(string bossID)
    {
        return spawnedBosses.FirstOrDefault(boss => boss.bossID == bossID);
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
