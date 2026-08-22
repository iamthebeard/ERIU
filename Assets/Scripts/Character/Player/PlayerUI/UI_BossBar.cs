using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class UI_BossBar : UI_StatBar
{
    [SerializeField] AIBossManager relatedBoss;
    [SerializeField] TextMeshProUGUI label;
    public void EnableBossHPBar(AIBossManager bossActivated)
    {
        relatedBoss = bossActivated;
        relatedBoss.aiCharacterNetworkManager.currentHealth.OnValueChanged += OnBossHPChanged;
        SetMaxStat(relatedBoss.aiCharacterNetworkManager.maxHealth.Value);
        SetStat(relatedBoss.aiCharacterNetworkManager.currentHealth.Value);

        label.text = bossActivated.aiCharacterName;
    }

    private void OnDestroy()
    {
        relatedBoss.aiCharacterNetworkManager.currentHealth.OnValueChanged -= OnBossHPChanged;
    }

    private void OnBossHPChanged(int oldValue, int newValue)
    {
        SetStat(newValue);

        if (newValue <= 0)
        {
            RemoveBar(2.5f);
        }
    }
}
