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
    [SerializeField] public NetworkVariable<bool> fightInProgress = 
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] List<FogWallInteractable> associatedFogWalls;
    [SerializeField] string inactiveAnimation;
    [SerializeField] string awakenAnimation;
    [SerializeField] string phaseChangeAnimation;
    public string defeatedMessage = "ENEMY DEFEATED";

    [SerializeField] CombatStanceState phase2CombatStance;
    public float phaseChangeHealthThreshold = 0.5f;

    [Header("Music")]
    [SerializeField] AudioClip bossIntroClip;
    [SerializeField] AudioClip bossLoopClip;

    [Header("DEBUG")]
    [SerializeField] bool awakenBoss = false;
    [SerializeField] bool defeatBoss = false;
    [SerializeField] bool startFight = false;
    [SerializeField] bool resetBoss = false;
    
    protected override void Update()
    {
        base.Update();

        DebugMenu();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        fightInProgress.OnValueChanged += OnFightInProgressChanged;
        OnFightInProgressChanged(false, fightInProgress.Value);

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

            if (hasBeenDefeated.Value || !hasBeenAwakened.Value)
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

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        fightInProgress.OnValueChanged -= OnFightInProgressChanged;
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
        PlayerUIManager.instance.popupManager.SendBossDefeatedPopup(defeatedMessage);
        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;
            fightInProgress.Value = false;
            DefeatBoss();

            
            WorldSaveGameManager.instance.SaveGame();
        }

        return base.ProcessDeathEvent(overrideDeathAnimation);
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

            fightInProgress.Value = true;
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

    private void OnFightInProgressChanged(bool oldStatus, bool fightNowInProgress)
    {
        if (fightNowInProgress)
        {
            WorldSoundFXManager.instance.PlayBossTrack(bossIntroClip, bossLoopClip);

            GameObject bossHealthBar = Instantiate(
                PlayerUIManager.instance.playerUIHUDManager.bossHealthBarPrefab,
                PlayerUIManager.instance.playerUIHUDManager.bossHealthBarParent
            );
            UI_BossBar bossHPBar = bossHealthBar.GetComponentInChildren<UI_BossBar>();
            bossHPBar.EnableBossHPBar(this);
        }
        else
        {
            WorldSoundFXManager.instance.StopBossMusic();
        }
    }

    public void PhaseShift()
    {
        if (isDead.Value)
            return;
        StartCoroutine(SwitchPhases());
    }

    private IEnumerator SwitchPhases()
    {
        while (isPerformingAction)
            yield return new WaitForEndOfFrame();

        combatStance = Instantiate(phase2CombatStance);
        currentState = combatStance;
        characterAnimatorManager.PlayTargetActionAnimation(phaseChangeAnimation, true, false, true, false);

        yield return null;
    }
}
