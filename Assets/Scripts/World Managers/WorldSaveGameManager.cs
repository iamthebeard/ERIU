using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Collections;
using System;
using System.Linq;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance; // Singleton (accessible from anywhere; only one in the whole project)

    [Header("World Scene Index")]
    [SerializeField] int worldSceneIndex = 1;

    [Header("SAVE/LOAD")]
    [SerializeField] bool saveGame = false;
    [SerializeField] bool loadGame = false;

    [Header("Current Character Data")]
    public PlayerManager player;
    public CharacterSlot currentCharacterSlot;
    public string saveFileName;
    public CharacterSaveData currentCharacterSaveData;

    [Header("Character Slots")]
    // public CharacterSaveData[] characterSlots = new CharacterSaveData[10];
    // [SerializeField]
    public CharacterSaveData[] characterSlots = new CharacterSaveData[0];
    [HideInInspector]
    public Dictionary<CharacterSlot, CharacterSaveData> characterSlotsDict = new Dictionary<CharacterSlot, CharacterSaveData>();
    // public CharacterSaveData characterSlot01;
    // public CharacterSaveData characterSlot02;
    // public CharacterSaveData characterSlot03;
    // public CharacterSaveData characterSlot04;
    // public CharacterSaveData characterSlot05;
    // public CharacterSaveData characterSlot06;
    // public CharacterSaveData characterSlot07;
    // public CharacterSaveData characterSlot08;
    // public CharacterSaveData characterSlot09;
    // public CharacterSaveData characterSlot10;

    [Header("Save Data Writer")]
    private SaveFileDataWriter saveFileDataWriter;

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
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject); // Stays with us through scene transitions
        LoadAllCharacterSlots();
    }

    private void Update() {
        if (saveGame) SaveGame();
        if (loadGame) LoadGame();
    }

    // public ref CharacterSaveData CharacterSaveDataBySlot(CharacterSlot slot)
    // {
    //     CharacterSaveData[] characterSlots = new CharacterSaveData[] {
    //         characterSlot01,
    //         characterSlot02,
    //         characterSlot03,
    //         characterSlot04,
    //         characterSlot05,
    //         characterSlot06,
    //         characterSlot07,
    //         characterSlot08,
    //         characterSlot09,
    //         characterSlot10,
    //     };
    //     CharacterSlot[] characterSlotNames = new CharacterSlot[] {
    //         CharacterSlot.CharacterSlot01,
    //         CharacterSlot.CharacterSlot02,
    //         CharacterSlot.CharacterSlot03,
    //         CharacterSlot.CharacterSlot04,
    //         CharacterSlot.CharacterSlot05,
    //         CharacterSlot.CharacterSlot06,
    //         CharacterSlot.CharacterSlot07,
    //         CharacterSlot.CharacterSlot08,
    //         CharacterSlot.CharacterSlot09,
    //         CharacterSlot.CharacterSlot10,
    //     };
    //     return ref characterSlots[Array.FindIndex(characterSlotNames, x => x == slot)];
    // }

    public static string DecideCharacterFileNameBasedOnCharacterSlot(CharacterSlot characterSlot)
    {
        return characterSlot.ToString() + ".sav";
    }

    // public IEnumerable ForEachCharacterSlot()
    // {
    //     CharacterSaveData[] characterSlots = new CharacterSaveData[] {
    //         characterSlot01,
    //         characterSlot02,
    //         characterSlot03,
    //         characterSlot04,
    //         characterSlot05,
    //         characterSlot06,
    //         characterSlot07,
    //         characterSlot08,
    //         characterSlot09,
    //         characterSlot10,
    //     };
    //     CharacterSlot[] characterSlotNames = new CharacterSlot[] {
    //         CharacterSlot.CharacterSlot01,
    //         CharacterSlot.CharacterSlot02,
    //         CharacterSlot.CharacterSlot03,
    //         CharacterSlot.CharacterSlot04,
    //         CharacterSlot.CharacterSlot05,
    //         CharacterSlot.CharacterSlot06,
    //         CharacterSlot.CharacterSlot07,
    //         CharacterSlot.CharacterSlot08,
    //         CharacterSlot.CharacterSlot09,
    //         CharacterSlot.CharacterSlot10,
    //     };
    //     for (int i = 0; i < 10; i++) {
    //         yield return new Tuple<CharacterSlot, CharacterSaveData>(characterSlotNames[i], characterSlots[i]);
    //     }
    // }

    public void AttemptToCreateNewGame()
    {
        // Check to see if we have character slots open
        saveFileDataWriter = new SaveFileDataWriter();
        // foreach (Tuple<CharacterSlot, CharacterSaveData> tuple in ForEachCharacterSlot()) // Could probably just use the enum, as I don't actually need the data right now.
        foreach (CharacterSlot characterSlot in Enum.GetValues(typeof(CharacterSlot)))
        {
            // CharacterSlot slot = tuple.Item1;
            CharacterSlot slot = characterSlot;
            if (slot == CharacterSlot.NoSlot) continue;
            // CharacterSaveData data = tuple.Item2;
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlot(slot);
            if (!saveFileDataWriter.CheckIfFileExists())
            {
                // This slot is available. Create game in this slot
                currentCharacterSlot = slot;
                // Start off with blank character data
                currentCharacterSaveData = new CharacterSaveData();

                // Start game
                NewGame();
                return;
            }
        }

        // If there are no free slots, notify the player.
        TitleScreenManager.instance.DisplayNoFreeCharacterSlotsPopUp();
    }

    private void NewGame()
    {
        SaveGame(); // Save the newly created character
        StartCoroutine(LoadWorldScene());
    }

    public void LoadGame()
    {
        loadGame = false; // Don't keep trying to load.
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlot(currentCharacterSlot);

        saveFileDataWriter = new SaveFileDataWriter();
        // Generally works on multiple machine types
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        currentCharacterSaveData = saveFileDataWriter.LoadSaveFile();

        player.LoadGameDataFromCurrentCharacterData(ref currentCharacterSaveData);

        StartCoroutine(LoadWorldScene());
    }

    public void SaveGame()
    {
        saveGame = false; // Don't keep trying to save.
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlot(currentCharacterSlot);

        saveFileDataWriter = new SaveFileDataWriter();
        // Generally works on multiple machine types
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        // Get player info from the game; put it into the characterSaveData
        player.SaveGameDataToCurrentCharacterData(ref currentCharacterSaveData);

        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterSaveData);
        characterSlotsDict[currentCharacterSlot] = currentCharacterSaveData;
        RefreshCharacterSlotsInInspector();
    }

    public void DeleteGame(CharacterSlot slotToDelete)
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlot(slotToDelete);
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.DeleteSaveFile();
        characterSlotsDict.Remove(slotToDelete);
        RefreshCharacterSlotsInInspector();
    }

    private void LoadAllCharacterSlots()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        foreach (CharacterSlot slot in Enum.GetValues(typeof(CharacterSlot)))
        {
            if (slot == CharacterSlot.NoSlot) continue;
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlot(slot);
            if (!saveFileDataWriter.CheckIfFileExists()) break;
            // CharacterSaveDataBySlot(slot) = saveFileDataWriter.LoadSaveFile();
            characterSlotsDict[slot] = saveFileDataWriter.LoadSaveFile();
            // Debug.Log("Slot: " + slot.ToString() + "  Character: " + characterSlotsDict[slot].characterName + characterSlotsDict[slot].xPosition);
        }
        RefreshCharacterSlotsInInspector();
    }

    private void RefreshCharacterSlotsInInspector()
    {
        characterSlots = characterSlotsDict.Values.ToArray();
    }

    public IEnumerator LoadWorldScene() // co-routine
    {
        // If you will use a universal game scene
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        // If you have different scenes for different areas/levels (must have a default for new games)
        // AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterSaveData.sceneIndex);

        yield return null;
    }

    public int GetWorldSceneIndex() {
        return worldSceneIndex;
    }
}
