using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// Since we want to reference this data for every safe file,
//  this script is not a monobehaviour and is instead serializable
public class CharacterSaveData
{
    [Header("Character Name")]
    public string characterName;
    [Header("Time Played")]
    public string timePlayed;
    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;
}
