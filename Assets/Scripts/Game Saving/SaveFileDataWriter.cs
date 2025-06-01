using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveFileDataWriter
{
    public string saveDataDirectoryPath = Application.persistentDataPath;
    public string saveFileName = "";

    // Before creating a new save file, we must check if the file already exists
    //  (See if the "character slot" is already filled.)
    public bool CheckIfFileExists() {
        return File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName));
    }

    public void DeleteSaveFile() {
        File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
    }

    public void CreateNewCharacterSaveFile(CharacterSaveData characterSaveData) {
        // Make a path to save the file
        string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);
        string dataToStore = "";
        try {
            // Create the directory the file will be written to, if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("Creating save file at path: " + savePath);

            // Serialize the C# Game Data object to JSON format
            dataToStore = JsonUtility.ToJson(characterSaveData, true);

            // Write the JSON to a file
            using (FileStream stream = new FileStream(savePath, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(dataToStore);
                }
            }
        } catch (Exception) {
            Debug.LogError("Error while trying to save character data. Game not saved.");
            // Debug.LogError("characterSaveData: ", characterSaveData);
            // Debug.LogError("dataToStore: ", dataToStore);
            // Debug.LogError("exception: ", ex);
            
        }
    }

    public CharacterSaveData LoadSaveFile() {
        CharacterSaveData characterSaveData = null;
        string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

        if(File.Exists(loadPath)) {
            try {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(loadPath, FileMode.Open)) {
                    using (StreamReader reader = new StreamReader(stream)) {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                // Debug.Log("Loading save game " + loadPath + " with json: " + dataToLoad);

                // Deserialize the data
                characterSaveData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            } catch (Exception) {
                Debug.LogError("Unable to load file");
            }
        }

        return characterSaveData;
    }
}
