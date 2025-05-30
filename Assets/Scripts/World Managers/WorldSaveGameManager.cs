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
    [SerializeField] private PlayerManager player;
    public CharacterSlot currentCharacterSlot;
    public string saveFileName;
    public CharacterSaveData currentCharacterSaveData;

    [Header("Character Slots")]
    public CharacterSaveData characterSlot01;
    public CharacterSaveData characterSlot02;
    public CharacterSaveData characterSlot03;
    public CharacterSaveData characterSlot04;
    public CharacterSaveData characterSlot05;
    public CharacterSaveData characterSlot06;
    public CharacterSaveData characterSlot07;
    public CharacterSaveData characterSlot08;
    public CharacterSaveData characterSlot09;
    public CharacterSaveData characterSlot10;

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

    public ref CharacterSaveData CharacterSaveDataBySlot(CharacterSlot slot)
    {
        CharacterSaveData[] characterSlots = new CharacterSaveData[] {
            characterSlot01,
            characterSlot02,
            characterSlot03,
            characterSlot04,
            characterSlot05,
            characterSlot06,
            characterSlot07,
            characterSlot08,
            characterSlot09,
            characterSlot10,
        };
        CharacterSlot[] characterSlotNames = new CharacterSlot[] {
            CharacterSlot.CharacterSlot01,
            CharacterSlot.CharacterSlot02,
            CharacterSlot.CharacterSlot03,
            CharacterSlot.CharacterSlot04,
            CharacterSlot.CharacterSlot05,
            CharacterSlot.CharacterSlot06,
            CharacterSlot.CharacterSlot07,
            CharacterSlot.CharacterSlot08,
            CharacterSlot.CharacterSlot09,
            CharacterSlot.CharacterSlot10,
        };
        return ref characterSlots[Array.FindIndex(characterSlotNames, x => x == slot)];
    }

    public static string DecideCharacterFileNameBasedOnCharacterSlot(CharacterSlot characterSlot)
    {
        return characterSlot.ToString() + ".sav";
        // switch (currentCharacterSaveSlot) {
        //     case CharacterSaveSlot.CharacterSlot01:
        //         fileName = "characterSlot01"
        //         break;
        //     case CharacterSaveSlot.CharacterSlot02:
        //         break;
        //     case CharacterSaveSlot.CharacterSlot03:
        //         break;
        //     case CharacterSaveSlot.CharacterSlot04:
        //         break;
        //     case CharacterSaveSlot.CharacterSlot05:
        //         break;
        //     case CharacterSaveSlot.CharacterSlot06:
        //         break;
        //     case CharacterSaveSlot.CharacterSlot07:
        //         break;
        //     case CharacterSaveSlot.CharacterSlot08:
        //         break;
        //     case CharacterSaveSlot.CharacterSlot09:
        //         break;
        //     case CharacterSaveSlot.CharacterSlot10:
        //         break;
        // }
    }

    public void NewGame() {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlot(currentCharacterSlot);

        currentCharacterSaveData = new CharacterSaveData();
    }

    public void LoadGame() {
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

    public void SaveGame() {
        saveGame = false; // Don't keep trying to save.
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlot(currentCharacterSlot);

        saveFileDataWriter = new SaveFileDataWriter();
        // Generally works on multiple machine types
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        // Get player info from the game; put it into the characterSaveData
        player.SaveGameDataToCurrentCharacterData(ref currentCharacterSaveData);

        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterSaveData);
    }

    private void LoadAllCharacterSlots()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        Debug.Log("In LoadAllCharacterSlots");

        foreach (CharacterSlot slot in Enum.GetValues(typeof(CharacterSlot)))
        {
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlot(slot);
            CharacterSaveDataBySlot(slot) = saveFileDataWriter.LoadSaveFile();
            Debug.Log("Slot: " + slot.ToString() + "  Character: " + CharacterSaveDataBySlot(slot).characterName);
        }
    }

    public IEnumerator LoadWorldScene() // co-routine
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;
    }

    public int GetWorldSceneIndex() {
        return worldSceneIndex;
    }
}
