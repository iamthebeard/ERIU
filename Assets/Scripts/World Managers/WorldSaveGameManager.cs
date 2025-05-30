using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    }

    private void Update() {
        if (saveGame) SaveGame();
        if (loadGame) LoadGame();
        saveGame = false;
        loadGame = false;
    }

    private void DecideCharacterFileNameBasedOnCharacterSlot() {
        saveFileName = currentCharacterSlot.ToString() + ".sav";
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
        DecideCharacterFileNameBasedOnCharacterSlot();

        currentCharacterSaveData = new CharacterSaveData();
    }

    public void LoadGame() {
        DecideCharacterFileNameBasedOnCharacterSlot();

        saveFileDataWriter = new SaveFileDataWriter();
        // Generally works on multiple machine types
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        currentCharacterSaveData = saveFileDataWriter.LoadSaveFile();

        player.LoadGameDataFromCurrentCharacterData(ref currentCharacterSaveData);

        StartCoroutine(LoadWorldScene());
    }

    public void SaveGame() {
        DecideCharacterFileNameBasedOnCharacterSlot();

        saveFileDataWriter = new SaveFileDataWriter();
        // Generally works on multiple machine types
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        // Get player info from the game; put it into the characterSaveData
        player.SaveGameDataToCurrentCharacterData(ref currentCharacterSaveData);

        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterSaveData);
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
