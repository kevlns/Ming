using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD,
    PATROL,
    CHASE,
    ATTACK,
    DEAD
}

[RequireComponent(typeof(NavMeshAgent), typeof(BoxCollider))]
public class EnemyController : MonoBehaviour
{
    public EnemyStates m_State;

    private NavMeshAgent m_Agent;

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Agent.speed = 2f;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SwitchStates()
    {
        switch (m_State)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                break;
            case EnemyStates.CHASE:
                break;
            case EnemyStates.ATTACK:
                break;
            case EnemyStates.DEAD:
                break;
        }
    }
}