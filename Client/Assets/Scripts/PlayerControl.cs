using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public CharacterController m_CharacterController;
    public Animator m_Animator;
    private Vector2 m_MoveDirection;
    // private float moveSpeed = 3f;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        m_MoveDirection = new Vector2(horizontal, vertical).normalized;
        
        float speed = m_MoveDirection.magnitude;
        
        m_Animator.SetFloat("HorizontalValue", horizontal);
        m_Animator.SetFloat("VerticalValue", vertical);
        m_Animator.SetFloat("Speed", speed);
        
        if(Input.GetKeyDown(KeyCode.LeftShift))
            m_Animator.SetBool("ShiftPressed", true);
        if(Input.GetKeyUp(KeyCode.LeftShift))
            m_Animator.SetBool("ShiftPressed", false);
    }
}