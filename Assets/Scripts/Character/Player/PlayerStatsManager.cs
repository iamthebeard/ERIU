using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        // When we have a class select menu, we'll set these there.
        player.playerNetworkManager.maxHealth.Value =
            CalculateMaxHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value);
        PlayerUIManager.instance.playerUIHUDManager.SetMaxHealthValue(player.playerNetworkManager.maxHealth.Value);
        player.playerNetworkManager.currentHealth.Value = player.playerNetworkManager.maxHealth.Value;
        player.playerNetworkManager.maxStamina.Value =
            CalculateMaxStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
        PlayerUIManager.instance.playerUIHUDManager.SetMaxStaminaValue(player.playerNetworkManager.maxStamina.Value);
        player.playerNetworkManager.currentStamina.Value = player.playerNetworkManager.currentStamina.Value;
    } 
}
