using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerBossFight : EventTrigger
{
    [SerializeField] string bossID;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        AIBossManager boss = WorldAIManager.instance.GetBossByID(bossID);
        if (boss != null)
        {
            boss.aiCharacterNetworkManager.currentHealth.OnValueChanged += OnBossHealthChange;
            boss.WakeBoss();
        }
    }

    public void OnBossHealthChange(int oldHealth, int newHealth)
    {
        if (newHealth < 0)
            CloseTrigger();
    }
}
