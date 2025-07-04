using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : Singleton<MouseManager>
{
    public Texture2D Cursor_Idle, Cursor_Point, Cursor_Target, Cursor_Attack, Cursor_Doorway;

    private Camera m_Camera;
    private RaycastHit m_HitInfo;

    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnObjectClicked;

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    protected override void Awake()
    {
        base.Awake();
        m_Camera = Camera.main;
    }

    void SetCursorTexture()
    {
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out m_HitInfo))
        {
            switch (m_HitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(Cursor_Target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(Cursor_Attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && m_HitInfo.collider != null)
        {
            GameObject go = m_HitInfo.collider.gameObject;
            if (go.CompareTag("Ground"))
                OnMouseClicked?.Invoke(m_HitInfo.point);
            if(go.CompareTag("Enemy"))
                OnObjectClicked?.Invoke(go);
        }
    }
}