using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] GameObject objectPrefab;
    [SerializeField] GameObject instantiatedGameObject;

    void Awake()
    {
    }

    void Start()
    {
        WorldObjectManager.instance.SpawnObject(this);

        // We don't want this object active in the scene. It's just a dummy to indicate where a character will spawn.
        gameObject.SetActive(false);
    }

    public GameObject AttemptToSpawnObject()
    {
        if (objectPrefab == null) return null;

        instantiatedGameObject = Instantiate(objectPrefab);

        // We use this model to set the position and rotation of the spawned character in the scene.
        instantiatedGameObject.transform.position = transform.position;
        instantiatedGameObject.transform.rotation = transform.rotation;

        instantiatedGameObject.GetComponent<NetworkObject>().Spawn();

        return instantiatedGameObject;
    }
}

