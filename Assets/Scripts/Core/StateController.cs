using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    public StateSO currentState;
    public EnemyStates enemyStates; 
    public Transform eyes;
    public StateSO remainState;

    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public TankShooting tankShooting;
    [HideInInspector] public List<Transform> wayPointList;
    [HideInInspector] public int nextWayPoint;

    internal void CheckIfCountDownElapsed(object searchingDuration)
    {
        throw new NotImplementedException();
    }

    [HideInInspector] public Transform chaseTarget;
    [HideInInspector] public float stateTimeElapsed;
    private bool aiActive;

    private void Awake()
    {
        tankShooting = GetComponent<TankShooting>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetupAI(bool aiActivationFromTankManager, List<Transform> wayPointsFromTankManager)
    {
        wayPointList = wayPointsFromTankManager;
        aiActive = aiActivationFromTankManager;
        if (aiActive)
        {
            navMeshAgent.enabled = true;
        }
        else
        {
            navMeshAgent.enabled = false;
        }
    }

    private void Update()
    {
        if (!aiActive)
            return;
        currentState.UpdateState(this);
    }

    private void OnDrawGizmos()
    {
        if (currentState != null && eyes != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawSphere(eyes.position, enemyStates.lookSphereCastRadius);
        }
    }

    public void TransitionToState(StateSO nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }
     
    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }
}
