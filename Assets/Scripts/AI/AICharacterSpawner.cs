using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AICharacterSpawner : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] GameObject characterPrefab;
    [SerializeField] GameObject instantiatedGameObject;

    void Awake()
    {
    }

    void Start()
    {
        WorldAIManager.instance.SpawnCharacter(this);

        // We don't want this object active in the scene. It's just a dummy to indicate where a character will spawn.
        gameObject.SetActive(false);
    }

    public void AttemptToSpawnCharacter()
    {
        instantiatedGameObject = Instantiate(characterPrefab);

        // We use this model to set the position and rotation of the spawned character in the scene.
        instantiatedGameObject.transform.position = transform.position;
        instantiatedGameObject.transform.rotation = transform.rotation;

        instantiatedGameObject.GetComponent<NetworkObject>().Spawn();

        WorldAIManager.instance.AddCharacterToSpawnedCharactersList(instantiatedGameObject.GetComponent<AICharacterManager>());
    }
}
