using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private GameObject m_AttackTarget;
    
    private float m_AttackCd;
    private float m_RestTimeToAttack;

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnObjectClicked += OnEnemyClicked;
        m_AttackCd = 1.5f;
        m_RestTimeToAttack = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.SetFloat("Speed", m_Agent.velocity.magnitude);
        
        m_RestTimeToAttack -= Time.deltaTime;
    }

    private void MoveToTarget(Vector3 targetPos)
    {
        m_Agent.destination = targetPos;
    }

    private void OnEnemyClicked(GameObject enemy)
    {
        if (enemy)
        {
            m_AttackTarget = enemy;
            
        }
    }

    IEnumerator AttackEvent()
    {
        transform.LookAt(m_AttackTarget.transform);

        while (Vector3.Distance(transform.position, m_AttackTarget.transform.position) > 1)
        {
            m_Agent.destination = m_AttackTarget.transform.position;
            yield return null;
        }

        m_Agent.isStopped = true;

        if (m_RestTimeToAttack < 0)
        {
            m_Animator.SetTrigger("Attack_1");
            m_RestTimeToAttack = m_AttackCd;
        }
    }
}