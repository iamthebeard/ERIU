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
    [Header("Character Slot")]
    public CharacterSlot characterSlot;
    [Header("Time Played")]
    public string timePlayed;
    [Header("Scene Index")]
    public int sceneIndex = 1;
    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;
    [Header("Stats")]
    public int vitality = 10;
    public int endurance = 10;

    [Header("Resources")]
    public int currentHealth;
    public float currentStamina;
}
