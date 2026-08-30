using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Performs identically to the UI Stat Bar, except it appears and disappears and is in world space (always faces the camera)
public class UI_CharacterBar : UI_StatBar
{
    CharacterManager character;
    AICharacterManager aiCharacter;
    PlayerManager player;

    [SerializeField] bool displayCharacterNameOnDamage = false;
    [SerializeField] float defaultTimeBeforeBarHides = 3;
    [SerializeField] float timer = 0;
    [SerializeField] int currentDamageTaken = 0;

    public int oldHealthValue = 0;
    
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI characterDamage;

    protected override void Awake()
    {
        base.Awake();

        character = GetComponentInParent<CharacterManager>();
        // aiCharacter = GetComponentInParent<AICharacterManager>(); // Will be null for a non-ai character
        // player = GetComponentInParent<PlayerManager>(); // Will be null for non-player

        // Apparently this works:
        if (character != null)
        {
            // `as` returns `null` when the cast doesn't work
            aiCharacter = character as AICharacterManager;
            player = character as PlayerManager;
        }
    }

    protected override void Start()
    {
        base.Start();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);

        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            timer = 0;
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        currentDamageTaken = 0;
    }

    public override void SetStat(float newValue)
    {
        if (displayCharacterNameOnDamage)
        {
            characterName.enabled = true;
            characterName.text = character.name;
            if (aiCharacter != null)
                characterName.text = aiCharacter.aiCharacterName;
            else if (player != null)
                characterName.text = player.playerNetworkManager.characterName.Value.ToString();
        }
        else
            characterName.enabled = false;
        
        SetMaxStat(character.characterNetworkManager.maxHealth.Value); // In case of buffs, etc.

        // To Do: Run secondary bar logic (shows amount of current hit, yellow in Elden Ring)

        // Total the damage taken while the bar is acive
        float oldDamage = currentDamageTaken;
        currentDamageTaken = Mathf.RoundToInt(currentDamageTaken + (oldHealthValue - newValue));

        if (currentDamageTaken < 0)
        {
            // Health *added* (heals?)
            currentDamageTaken = Mathf.Abs(currentDamageTaken);
            characterDamage.text = "+ " + currentDamageTaken.ToString();
        }
        else
        {
            characterDamage.text = "- " + currentDamageTaken.ToString();
        }

        base.SetStat(character.characterNetworkManager.currentHealth.Value);

        if (character.characterNetworkManager.currentHealth.Value != character.characterNetworkManager.maxHealth.Value)
        {
            // We have taken damage, so show the damage bar.
            timer = defaultTimeBeforeBarHides;
            gameObject.SetActive(true);
        }
    }
}
