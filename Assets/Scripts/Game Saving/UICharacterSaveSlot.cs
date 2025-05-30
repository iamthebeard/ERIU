using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICharacterSaveSlot : MonoBehaviour
{
    SaveFileDataWriter saveFileDataWriter;

    [Header("Game Slot")]
    public CharacterSlot slot;

    [Header("Character Info")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI timePlayed;

    private void OnEnable()
    {
        LoadSaveSlot();
    }

    private void LoadSaveSlot()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = WorldSaveGameManager.DecideCharacterFileNameBasedOnCharacterSlot(slot);
        if (saveFileDataWriter.CheckIfFileExists())
        {
            CharacterSaveData characterSaveData = WorldSaveGameManager.instance.CharacterSaveDataBySlot(slot);
            characterName.text = characterSaveData.characterName;
            timePlayed.text = characterSaveData.timePlayed;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
