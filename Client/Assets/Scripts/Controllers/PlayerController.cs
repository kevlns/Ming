using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private GameObject m_AttackTarget;
    private CharacterStats m_Stats;
    private CapsuleCollider m_Collider;
    private bool m_IsDead;

    private float m_RestTimeToAttack;

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Stats = GetComponent<CharacterStats>();
        m_Collider = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnObjectClicked += OnEnemyClicked;
        m_RestTimeToAttack = 0;
        m_IsDead = false;
        gameObject.tag = "Player";
        gameObject.layer = LayerMask.NameToLayer("Player");

        GameManager.Instance.RegisterPlayer(m_Stats);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsDead) return;

        UpdateTime();
        SwitchAnimation();
    }

    void UpdateTime()
    {
        m_RestTimeToAttack -= Time.deltaTime;
    }

    void SwitchAnimation()
    {
        if (m_Stats.isDead)
        {
            m_IsDead = m_Stats.isDead;
            m_Agent.isStopped = true;
            m_Collider.enabled = false;
            m_Agent.radius = 0;
            m_Animator.SetTrigger("Dead");

            GameManager.Instance.NotifyPlayerDeadAsync();
        }

        m_Animator.SetFloat("Speed", m_Agent.velocity.magnitude);
        m_Animator.SetBool("Critical", m_Stats.isCritical);
    }

    private void MoveToTarget(Vector3 targetPos)
    {
        StopAllCoroutines();
        if (m_IsDead) return;
        m_Agent.isStopped = false;
        m_Agent.destination = targetPos;
    }

    private void OnEnemyClicked(GameObject enemy)
    {
        if (m_IsDead) return;
        if (enemy != null)
        {
            m_AttackTarget = enemy;
            StartCoroutine(DoAttackAnimationEvent());
        }
    }

    IEnumerator DoAttackAnimationEvent()
    {
        while (Vector3.Distance(m_AttackTarget.transform.position, transform.position) > m_Stats.AttackRange)
        {
            transform.LookAt(m_AttackTarget.transform);
            m_Agent.destination = m_AttackTarget.transform.position;
            yield return null;
        }

        m_Agent.isStopped = true;

        if (m_RestTimeToAttack < 0)
        {
            transform.LookAt(m_AttackTarget.transform);
            m_Stats.isCritical = Random.value < m_Stats.CriticalChance;
            m_RestTimeToAttack = m_Stats.CoolDown;
            m_Animator.SetTrigger("Attack_1");
        }
    }

    private void Hit()
    {
        if (m_AttackTarget != null)
            CharacterStats.TakeDamage(m_Stats, m_AttackTarget.GetComponent<CharacterStats>());
    }

    public void GetHit()
    {
        m_Animator.SetTrigger("GetHit");
    }
}