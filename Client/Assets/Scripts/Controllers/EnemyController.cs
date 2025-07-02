using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD,
    PATROL,
    CHASE,
    OBSERVE,
    DEAD
}

[RequireComponent(typeof(NavMeshAgent), typeof(BoxCollider))]
public class EnemyController : MonoBehaviour
{
    private EnemyStates m_InitState;
    private EnemyStates m_State;
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private CharacterStats m_Stats;
    private GameObject m_AttackTarget;
    private Vector3 m_NextPatrolPosition;
    private Vector3 m_InitPosition;
    private float m_RestPatrolTime;
    private float m_RestAttackTime;
    private BoxCollider m_Collider;
    private float m_RestObserveTime;

    private bool isWalk;
    private bool isChase;
    private bool isFollow;

    [Header("Enemy Settings")] public float sightRadius;
    public int health;
    public float patrolRange;
    public float patrolCd;

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Stats = GetComponent<CharacterStats>();
        m_Collider = GetComponent<BoxCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Agent.speed = 3f;
        m_InitState = EnemyStates.PATROL;
        m_State = m_InitState;
        m_InitPosition = transform.position;
        m_RestPatrolTime = 0;
        m_RestAttackTime = 0;
        m_RestObserveTime = 3.0f;
        GenNewPatrolPosition();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        SwitchStates();
        SwitchAnimation();
    }

    void UpdateTime()
    {
        m_RestPatrolTime -= Time.deltaTime;
        m_RestAttackTime -= Time.deltaTime;
        m_RestObserveTime -= Time.deltaTime;
    }

    void SwitchAnimation()
    {
        m_Animator.SetBool("Walk", isWalk);
        m_Animator.SetBool("Chase", isChase);
        m_Animator.SetBool("Follow", isFollow);
        m_Animator.SetBool("Critical", m_Stats.isCritical);

        if (m_Stats.isDead) m_Animator.SetTrigger("Dead");
    }

    void SwitchStates()
    {
        if (m_Stats.isDead)
        {
            m_State = EnemyStates.DEAD;
        }
        else
        {
            if (CanSeePlayer())
                m_State = EnemyStates.CHASE;

            if (m_State != EnemyStates.OBSERVE)
                m_RestObserveTime = 3;
        }

        switch (m_State)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                isChase = false;
                isFollow = false;

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
                m_Agent.isStopped = false;

                if (!CanSeePlayer())
                {
                    isChase = false;
                    isFollow = false;
                    m_Agent.destination = m_Agent.transform.position;
                    m_State = EnemyStates.OBSERVE;
                }
                else
                {
                    isFollow = true;
                    m_Agent.destination = m_AttackTarget.transform.position;
                }

                if (PlayerInAttackRange() || PlayerInSkillRange())
                {
                    isFollow = false;
                    m_Agent.isStopped = true;

                    if (m_RestAttackTime < 0)
                    {
                        m_RestAttackTime = m_Stats.CoolDown;
                        m_Stats.isCritical = Random.value < m_Stats.CriticalChance;
                        DoAttackAnimation();
                    }
                }

                break;
            case EnemyStates.OBSERVE:
                if (m_RestObserveTime < 0)
                {
                    m_State = m_InitState;
                    m_RestObserveTime = 3.0f;
                }

                break;
            case EnemyStates.DEAD:
                isWalk = false;
                isChase = false;
                isFollow = false;
                m_Collider.enabled = false;
                m_Agent.enabled = false;
                Destroy(gameObject, 2.5f);
                break;
        }
    }

    void DoAttackAnimation()
    {
        transform.LookAt(m_AttackTarget.transform);
        if (PlayerInAttackRange())
        {
            m_Animator.SetTrigger("Attack");
        }

        if (PlayerInSkillRange())
        {
            m_Animator.SetTrigger("Skill");
        }
    }

    bool PlayerInSkillRange()
    {
        if (m_AttackTarget != null)
            return Vector3.Distance(transform.position, m_AttackTarget.transform.position) < m_Stats.SkillRange;
        return false;
    }

    bool PlayerInAttackRange()
    {
        if (m_AttackTarget != null)
            return Vector3.Distance(transform.position, m_AttackTarget.transform.position) < m_Stats.AttackRange;
        return false;
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
        m_NextPatrolPosition =
            NavMesh.SamplePosition(randomPosition, out hit, patrolRange, NavMesh.GetAreaFromName("Walkable"))
                ? hit.position
                : randomPosition;
    }

    private void Hit()
    {
        if (m_AttackTarget != null)
            CharacterStats.TakeDamage(m_Stats, m_AttackTarget.GetComponent<CharacterStats>());
    }
}