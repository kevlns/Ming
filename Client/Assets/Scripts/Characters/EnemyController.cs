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
    private EnemyStates m_InitState;
    private EnemyStates m_State;
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private GameObject m_AttackTarget;
    private Vector3 m_NextPatrolPosition;
    private Vector3 m_InitPosition;
    private float m_RestPatrolTime;

    private bool isWalk;
    private bool isChase;
    private bool isFollow;

    [Header("Enemy Settings")]
    public float sightRadius;
    public int health;
    public float patrolRange;
    public float patrolCd;
    
    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Agent.speed = 3f;
        m_InitState = EnemyStates.PATROL;
        m_State = m_InitState;
        m_InitPosition = transform.position;
        m_RestPatrolTime = patrolCd;
        GenNewPatrolPosition();
    }

    // Update is called once per frame
    void Update()
    {
        SwitchStates();
        SwitchAnimation();
    }

    void SwitchAnimation()
    {
        m_Animator.SetBool("Walk", isWalk);
        m_Animator.SetBool("Chase", isChase);
        m_Animator.SetBool("Follow", isFollow);
    }

    void SwitchStates()
    {
        if (CanSeePlayer())
            m_State = EnemyStates.CHASE;

        switch (m_State)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                isChase = false;
                isFollow = false;
                m_RestPatrolTime -= Time.deltaTime;

                if (Vector3.Distance(transform.position, m_NextPatrolPosition) < m_Agent.stoppingDistance)
                {
                    isWalk = false;
                    
                    if (m_RestPatrolTime < 0)
                    {
                        GenNewPatrolPosition();
                        m_RestPatrolTime = patrolCd;
                    }
                }
                else
                {
                    isWalk = true;
                    m_Agent.destination = m_NextPatrolPosition;
                }
                
                break;
            case EnemyStates.CHASE:
                isWalk = false;
                isChase = true;

                if (!CanSeePlayer())
                {
                    isChase = false;
                    isFollow = false;
                    m_Agent.destination = m_Agent.transform.position;
                }
                else
                {
                    isFollow = true;
                    m_Agent.destination = m_AttackTarget.transform.position;
                }
                break;
            case EnemyStates.ATTACK:
                break;
            case EnemyStates.DEAD:
                break;
        }
    }

    bool CanSeePlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                m_AttackTarget = target.gameObject;
                return true;
            }
        }

        m_State = m_InitState;
        m_AttackTarget = null;
        return false;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    void GenNewPatrolPosition()
    {
        float dx = Random.Range(-patrolRange, patrolRange);
        float dz = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPosition = new Vector3(m_InitPosition.x + dx, m_InitPosition.y, m_InitPosition.z + dz);
        NavMeshHit hit;
        m_NextPatrolPosition = NavMesh.SamplePosition(randomPosition, out hit, patrolRange, NavMesh.GetAreaFromName("Walkable")) ? hit.position : randomPosition;
    }
}