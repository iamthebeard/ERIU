using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WorldObjectManager : MonoBehaviour
{
    public static WorldObjectManager instance;

    [Header("Objects")]
    [SerializeField] List<GameObject> spawnedObjects;
    [SerializeField] List<ObjectSpawner> objectSpawners;

    [Header("Fog Walls")]
    public List<FogWallInteractable> fogWalls;

    // 1. Create an object script that will hold the logic for fog walls
    // 2. Spawn in fogwalls as network obects during start of game (must have a spawner object)
    // 3. Create a general object spawner script and prefab
    // 4. When the fog walls are spawned, add them to the world fog wall list
    // 5. Grab the correct fog wall from the list on the boss manager when the boss is being initialized
    // 6. Set fog wall behavior by boss status (present if awakend but not defeated)

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

    public void SpawnObject(ObjectSpawner spawner)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        objectSpawners.Add(spawner);
        GameObject spawnedObject = spawner.AttemptToSpawnObject();
        if (spawnedObject != null)
            spawnedObjects.Add(spawnedObject);
    }

    public void AddFogWallToList(FogWallInteractable fogWall)
    {
        if (!fogWalls.Contains(fogWall))
            fogWalls.Add(fogWall);
    }

    public void RemoveFogWallFromList(FogWallInteractable fogWall)
    {
        if (fogWalls.Contains(fogWall))
            fogWalls.Remove(fogWall);
    }
}
