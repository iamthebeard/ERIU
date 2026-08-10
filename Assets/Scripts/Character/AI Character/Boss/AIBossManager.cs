using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AIBossManager : AICharacterManager
{
    // Give a unique ID
    public string bossID = "boss01";
    // When this AI is spawned, check our save file (dictionary)
    // If the save file does not contain a boss monster with this ID, we add it.
    // If it is present, check if the boss has been awakend or defeated
    [SerializeField] bool hasBeenDefeated;
    // If the boss has been defeated, disable this game object
    // If the boss has been awakened, 
    [SerializeField] bool hasBeenAwakened;

    [Header("DEBUG")]
    [SerializeField] bool awakenBoss = false;
    [SerializeField] bool defeatBoss = false;
    [SerializeField] bool resetBoss = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        if (!WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.ContainsKey(bossID))
        {
            // Add it to the list if it's not already there.
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Add(bossID, false);
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Add(bossID, false);
        }
        else
        {
            hasBeenDefeated = WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened[bossID];

            if (hasBeenDefeated)
            {
                aiCharacterNetworkManager.isActive.Value = false;
            }
        }


    }

    protected override void Update()
    {
        base.Update();

        DebugMenu();
    }

    private void DebugMenu()
    {
        if (awakenBoss)
        {
            awakenBoss = false;
            hasBeenAwakened = true;
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Remove(bossID);
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Add(bossID, true);
            WorldSaveGameManager.instance.SaveGame();
        }
        if (defeatBoss)
        {
            defeatBoss = false;
            hasBeenDefeated = true;
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Remove(bossID); // Any reason can't just do [bossID] = true?
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Add(bossID, true);
            gameObject.SetActive(false);
            WorldSaveGameManager.instance.SaveGame();
        }
        if (resetBoss)
        {
            resetBoss = false;
            hasBeenAwakened = false;
            hasBeenDefeated = false;
            gameObject.SetActive(true);
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened[bossID] = false;
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated[bossID] = false;
            WorldSaveGameManager.instance.SaveGame();
        }
    }

    override public IEnumerator ProcessDeathEvent(bool overrideDeathAnimation = false)
    {
        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;

            // Reset any flags that need to be reset
            // Nothing yet

            // If we are not grounded, play aerial death animation.

            if (!overrideDeathAnimation)
            {
                // Play regular death animation (or select randomly from the standard set)
                characterAnimatorManager.PlayTargetActionAnimation("Standing React Death Forward", true);
                // Would this work better?
                // animator.CrossFade("Standing React Death Forward", 0.5f);
            }

            // Save boss status
            hasBeenDefeated = true;
            if (!WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.ContainsKey(bossID))
            {
                // Add it to the list if it's not already there.
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Add(bossID, true);
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Add(bossID, true);
            }
            else
            {
                // Otherwise, load the data that already exists on this boss
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Add(bossID, true);
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Add(bossID, true);
            }
            WorldSaveGameManager.instance.SaveGame();
        }

        // Play death SFX (to all players, not just owner)

        yield return new WaitForSeconds(5);

        // Award player with runes and other after-death effecets

        // Disable character
    }
}
