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
    [SerializeField] public NetworkVariable<bool> hasBeenDefeated = 
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // If the boss has been defeated, disable this game object
    // If the boss has been awakened, 
    [SerializeField] public NetworkVariable<bool> hasBeenAwakened = 
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] List<FogWallInteractable> associatedFogWalls;
    [SerializeField] string inactiveAnimation;
    [SerializeField] string awakenAnimation;

    [Header("DEBUG")]
    [SerializeField] bool awakenBoss = false;
    [SerializeField] bool defeatBoss = false;
    [SerializeField] bool resetBoss = false;
    
    protected override void Update()
    {
        base.Update();

        DebugMenu();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            currentState = sleep; // Set initial state to sleep instead of idle.
        }

        if (IsServer)
        {
            if (!WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.ContainsKey(bossID))
            {
                // Add it to the list if it's not already there.
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Add(bossID, false);
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Add(bossID, false);
            }
            else
            {
                hasBeenDefeated.Value = WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated[bossID];
                hasBeenAwakened.Value = WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened[bossID];
            }

            associatedFogWalls = new List<FogWallInteractable>();

            // Locate fog wall
            StartCoroutine(GetFogWallsFromWorldObjectManager());

            if (hasBeenDefeated.Value)
            {
                aiCharacterNetworkManager.isActive.Value = false;
                // If the boss has been defeated, disable fog walls
                for (int i = 0; i < associatedFogWalls.Count; i++)
                {
                    associatedFogWalls[i].isActive.Value = false;
                }
            }

            if (hasBeenAwakened.Value && !hasBeenDefeated.Value)
            {
                // If the boss has been awakened, enable the fog walls
                for (int i = 0; i < associatedFogWalls.Count; i++)
                {
                    associatedFogWalls[i].isActive.Value = true;
                }
            }
        }

        if (!hasBeenAwakened.Value)
        {
            // Set stock animation to inactive
            characterAnimatorManager.PlayTargetActionAnimation(inactiveAnimation, false);
        }



    }

    private void DebugMenu()
    {
        if (awakenBoss)
        {
            awakenBoss = false;
            WakeBoss();
            WorldSaveGameManager.instance.SaveGame();
        }
        if (defeatBoss)
        {
            defeatBoss = false;
            hasBeenDefeated.Value = true;
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Remove(bossID); // Any reason can't just do [bossID] = true?
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Add(bossID, true);
            gameObject.SetActive(false);
            WorldSaveGameManager.instance.SaveGame();
        }
        if (resetBoss)
        {
            resetBoss = false;
            hasBeenAwakened.Value = false;
            hasBeenDefeated.Value = false;
            gameObject.SetActive(true);
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Remove(bossID);
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Add(bossID, false);
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Remove(bossID);
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Add(bossID, false);
            WorldSaveGameManager.instance.SaveGame();
        }
    }

    private IEnumerator GetFogWallsFromWorldObjectManager()
    {
        while (WorldObjectManager.instance.fogWalls.Count == 0)
            yield return new WaitForEndOfFrame();
        
        // Can either share the same ID for the boss and the fog wall, or simply place a fogwall ID variable here to look for it using that.
        foreach (var fogWall in WorldObjectManager.instance.fogWalls)
        {
            if (fogWall.fogWallBossID == bossID)
            {
                associatedFogWalls.Add(fogWall);
            }
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
            hasBeenDefeated.Value = true;
            if (!WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.ContainsKey(bossID))
            {
                // Add it to the list if it's not already there.
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Add(bossID, true);
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Add(bossID, true);
            }
            else
            {
                WakeBoss();
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Add(bossID, true);
            }
            WorldSaveGameManager.instance.SaveGame();
        }

        // Play death SFX (to all players, not just owner)

        yield return new WaitForSeconds(5);

        // Award player with runes and other after-death effecets

        // Disable character
    }

    public void WakeBoss()
    {
        if (!hasBeenAwakened.Value)
        {
            // On first time calling, play waking animation
            characterAnimatorManager.PlayTargetActionAnimation(awakenAnimation, true);
        }

        if (IsOwner)
        {
            hasBeenAwakened.Value = true;
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Remove(bossID);
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesAwakened.Add(bossID, true);

            currentState = idle;

            foreach (var fogWall in associatedFogWalls)
            {
                fogWall.isActive.Value = true;
            }
        }
    }

    public void DefeatBoss()
    {
        if (IsOwner)
        {
            hasBeenDefeated.Value = true;
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Remove(bossID);
            WorldSaveGameManager.instance.currentCharacterSaveData.bossesDefeated.Add(bossID, true);
            
            foreach (var fogWall in associatedFogWalls)
            {
                fogWall.isActive.Value = false;
            }
        }
    }
}
