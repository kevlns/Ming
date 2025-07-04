using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Polybrush;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
    public ParameterData_SO ParameterDataTemplate;
    public AttackData_SO AttackDataTemplate;

    private ParameterData_SO m_ParamsData;
    private AttackData_SO m_AttackData;

    [HideInInspector] public bool isCritical;
    [HideInInspector] public bool isDead;

    private void Awake()
    {
        if (ParameterDataTemplate != null)
            m_ParamsData = Instantiate(ParameterDataTemplate);
        if (AttackDataTemplate != null)
            m_AttackData = Instantiate(AttackDataTemplate);
    }

    #region Data Mapping

    public int Health
    {
        get => m_ParamsData ? m_ParamsData.health : 0;
        set => m_ParamsData.health = value;
    }

    public int MaxHealth
    {
        get => m_ParamsData ? m_ParamsData.maxHealth : 0;
        set => m_ParamsData.maxHealth = value;
    }

    public float AttackRange
    {
        get => m_AttackData ? m_AttackData.attackRange : 0;
        set => m_AttackData.attackRange = value;
    }

    public float SkillRange
    {
        get => m_AttackData ? m_AttackData.skillRange : 0;
        set => m_AttackData.skillRange = value;
    }

    public float CoolDown
    {
        get => m_AttackData ? m_AttackData.coolDown : 0;
        set => m_AttackData.coolDown = value;
    }

    public int MinDamage
    {
        get => m_AttackData ? m_AttackData.minDamage : 0;
        set => m_AttackData.minDamage = value;
    }

    public int MaxDamage
    {
        get => m_AttackData ? m_AttackData.maxDamage : 0;
        set => m_AttackData.maxDamage = value;
    }

    public float CriticalFactor
    {
        get => m_AttackData ? m_AttackData.criticalFactor : 0;
        set => m_AttackData.criticalFactor = value;
    }

    public float CriticalChance
    {
        get => m_AttackData ? m_AttackData.criticalChance : 0;
        set => m_AttackData.criticalChance = value;
    }

    public int Defence
    {
        get => m_ParamsData ? m_ParamsData.defence : 0;
        set => m_ParamsData.defence = value;
    }

    #endregion

    #region Character Combat

    public static void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        int damage = Mathf.Max(CalculateDamage(in attacker) - defender.Defence, 0);
        defender.Health = Mathf.Max(defender.Health - damage, 0);
        defender.isDead = defender.Health == 0;
        Debug.Log(defender.gameObject.name + " HP: " + defender.Health);
        
        if (attacker.isCritical)
            defender.GetComponent<Animator>().SetTrigger("GetHit");
    }

    private static int CalculateDamage(in CharacterStats attacker)
    {
        float damage = Random.Range(attacker.MinDamage, attacker.MaxDamage);

        if (attacker.isCritical)
            damage = attacker.MaxDamage * attacker.CriticalFactor;

        return (int)damage;
    }

    #endregion
}