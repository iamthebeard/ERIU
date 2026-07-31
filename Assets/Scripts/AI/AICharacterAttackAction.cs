using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Attack")]
public class AICharacterAttackAction : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] protected string attackAnimation;

    [Header("Combo Action")]
    public AICharacterAttackAction comboAction; // The action we perform afterward, if we combo.
    // This could be a list

    [Header("Action Values")]
    public int attackWeight = 50;
    [SerializeField] public AttackType attackType;
    // Repeatable (allow twice in a ro)
    public float recoveryTime = 1.5f; // Other than for combos
    public float attackableAngle = 35;
    public float minimumDistance = 1;
    public float maximumDistance = 3;

    public void AttemptToPerformAction(AICharacterManager aiCharacter)
    {
        aiCharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(attackType, attackAnimation, true);
    }
}
