using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public interface Injurable
{
    void TakeDamage(Damage damage);
    // int MaxHealth { get; set;}
    // int CurrentHealth { get; set;}
    NetworkVariable<int> maxHealth { get; set;}
    NetworkVariable<int> currentHealth { get; set;}
    CharacterManagerY character{ get; }

    void OnSetupInjurable()
    {
        currentHealth.OnValueChanged += CheckHP;
    }

    void CheckHP(int oldHP, int newHP)
    {
        if (currentHealth.Value <= 0)
        {
            character.StartCoroutine(character.ProcessDeathEvent());
        }

        if (character.IsOwner)
        {
            if (currentHealth.Value > maxHealth.Value)
            {
                currentHealth.Value = maxHealth.Value;
            }
        }
    }   
}

public interface Killable
{
    CharacterManagerY character{ get; }

    public NetworkVariable<bool> isDead {get; set;}

    public virtual IEnumerator ProcessDeathEvent(bool overrideDeathAnimation = false)
    {
        if (character.IsOwner)
        {
            character.currentHealth.Value = 0;
            isDead.Value = true;

            // Reset any flags that need to be reset
            // Nothing yet

            // If we are not grounded, play aerial death animation.

            if (!overrideDeathAnimation)
            {
                // Play regular death animation (or select randomly from the standard set)
                character.characterAnimatorManager.PlayTargetActionAnimation("Standing React Death Forward", true);
                // Would this work better?
                // animator.CrossFade("Standing React Death Forward", 0.5f);
            }
        }

        // Play death SFX (to all players, not just owner)

        yield return new WaitForSeconds(5);

        // Award player with runes and other after-death effecets

        // Disable character
    }
}

public class CharacterManagerY : NetworkBehaviour, Injurable, Killable
{
    public CharacterManagerY character {get; }

    public CharacterAnimatorManager characterAnimatorManager;

    public NetworkVariable<int> maxHealth {get; set;} =
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentHealth {get; set;} =
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isDead {get; set;} = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    
    public void TakeDamage(Damage damage)
    {
        currentHealth.Value -= (int)damage.physical;
    }

    public IEnumerator ProcessDeathEvent()
    {
        yield return null;
    }
}