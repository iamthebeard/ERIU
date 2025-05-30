using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] GameObject titleScreenMainMenu;
    [SerializeField] GameObject titleScreenLoadGameMenu;

    [Header("Buttons")]
    [SerializeField] Button loadMenuReturnButton;
    [SerializeField] Button mainMenuLoadGameButton;

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.instance.NewGame();
        StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());
    }

    public void OpenLoadGameMenu()
    {
        // Switch to load game menu
        titleScreenMainMenu.SetActive(false);
        titleScreenLoadGameMenu.SetActive(true);
        loadMenuReturnButton.Select();
    }

    public void CloseLoadGameMenu()
    {
        // Switch to main menu
        titleScreenLoadGameMenu.SetActive(false);
        titleScreenMainMenu.SetActive(true);
        mainMenuLoadGameButton.Select();
    }

}
