using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BossNetworkManager : CharacterNetworkManager
{
    AIBossManager boss;

    protected override void Awake()
    {
        base.Awake();

        boss = GetComponent<AIBossManager>();
    }

    public override void CheckHP(int oldValue = 0, int newValue = 0)
    {
        base.CheckHP(oldValue, newValue);

        if (boss.IsOwner && currentHealth.Value < maxHealth.Value * boss.phaseChangeHealthThreshold)
        {
            boss.PhaseShift();
        }
    }
    
}
