using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance {get; private set;}
    [Header("NETWORK JOIN")]
    [SerializeField] bool startGameAsClient;

    [HideInInspector] public PlayerUIHUDManager playerUIHUDManager;
    [HideInInspector] public PopupManager popupManager;

    private void Awake()
    {
        // THERE CAN BE ONLY ONE INSTANCE OF THIS SCRIPT AT ONE TIME
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerUIHUDManager = GetComponentInChildren<PlayerUIHUDManager>();
        popupManager = GetComponentInChildren<PopupManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (startGameAsClient)
        {
            startGameAsClient = false;
            // Must first shut down the network as a host in order to start as a client
            NetworkManager.Singleton.Shutdown();
            // We then restart as a client
            NetworkManager.Singleton.StartClient();
        }
    }

}
