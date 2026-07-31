using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;
    [HideInInspector] public AICharacterNetworkManager aiCharacterNetworkManager;
    [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;

    [Header("Navigation")]
    public NavMeshAgent navMeshAgent;

    [Header("Current State")]
    [SerializeField] AIState currentState;

    [Header("States")]
    [SerializeField] public IdleState idle;
    [SerializeField] public PursueTargetState pursueTarget;
    [SerializeField] public CombatStanceState combatStance;
    [SerializeField] public AttackState attack;
    


    protected override void Awake()
    {
        base.Awake();

        aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        aiCharacterNetworkManager = GetComponent<AICharacterNetworkManager>();
        aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();

        // Immediately make a copy of the ScriptableObjects so we don't modify the originals
        idle = Instantiate(idle);
        pursueTarget = Instantiate(pursueTarget);

        currentState = idle; // Always set initial state to idle
    }

    protected override void Update()
    {
        base.Update();

        aiCharacterCombatManager.HandleActionRecovery(this);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (IsOwner)
            ProcessStateMachine();
    }

    // constantly tick on the same state
    private void ProcessStateMachine()
    {
        AIState nextState = currentState?.Tick(this);

        if (nextState != null)
        {
            currentState = nextState;
        }

        // Constantly reset navMeshAgent's position so it never actually catches up to us.
        //  The aiCharacter will be guided by the navMeshAgent, but will use root motion to actually move.
        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;

        if (navMeshAgent.enabled)
        {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);

            if (remainingDistance > navMeshAgent.stoppingDistance)
            {
                aiCharacterNetworkManager.isMoving.Value = true;
            }
            else
            {
                aiCharacterNetworkManager.isMoving.Value = false; // Close enough to stop moving
            }
        }
        else
        {
            aiCharacterNetworkManager.isMoving.Value = false;
        }
    }
}
