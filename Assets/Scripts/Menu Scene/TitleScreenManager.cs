using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;

    [Header("Menu Objects")]
    [SerializeField] GameObject titleScreenMainMenu;
    [SerializeField] GameObject titleScreenLoadGameMenu;

    [Header("Menu Buttons")]
    [SerializeField] Button mainMenuNewGameButton;
    [SerializeField] Button mainMenuLoadGameButton;
    [SerializeField] Button loadMenuReturnButton;

    [Header("Popups")]
    [SerializeField] GameObject noCharacterSlotsPopup;
    [SerializeField] Button noCharacterSlotsOkayButton;
    [SerializeField] GameObject deleteCharacterSlotPopup;
    [SerializeField] Button deleteCharacterOkayButton;
    [SerializeField] Button deleteCharacterCancelButton;

    [Header("Save Slots")]
    [SerializeField]
    public CharacterSlot currentSelectedSlot = CharacterSlot.NoSlot;


    public void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.instance.AttemptToCreateNewGame();
        // StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());
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

    public void DisplayNoFreeCharacterSlotsPopUp()
    {
        noCharacterSlotsPopup.SetActive(true);
        noCharacterSlotsOkayButton.Select();
    }

    public void CloseNoFreeCharacterSlotsPopUp()
    {
        noCharacterSlotsPopup.SetActive(false);
        mainMenuNewGameButton.Select();
    }

    // Character Slots

    public void SelectCharacterSlot(CharacterSlot slot)
    {
        currentSelectedSlot = slot;
    }

    public void SelectNoSlot()
    {
        currentSelectedSlot = CharacterSlot.NoSlot;
    }

    public void AttemptToDeleteCharacterSlot()
    {
        if (currentSelectedSlot != CharacterSlot.NoSlot)
        {
            deleteCharacterSlotPopup.SetActive(true);
            deleteCharacterOkayButton.Select();
        }
    }

    public void DeleteCharacterSlot()
    {
        WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);
        // Cycle the load game menu to refresh which slots are available
        titleScreenLoadGameMenu.SetActive(false);
        titleScreenLoadGameMenu.SetActive(true);
        CloseDeleteCharacterSlotPopup();
    }

    public void CloseDeleteCharacterSlotPopup()
    {
        deleteCharacterSlotPopup.SetActive(false);
        loadMenuReturnButton.Select();
    }
}
