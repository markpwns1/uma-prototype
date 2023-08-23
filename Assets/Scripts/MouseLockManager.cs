using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseLockManager : MonoBehaviour
{
    public static MouseLockManager Instance { get; private set; }
    
    public static bool IsMouseLocked { get; private set; }

    public static void TakeMouse()
    {
        _mouseUses++;
    }
    
    public static void ReleaseMouse()
    {
        _mouseUses--;
    }
    
    private static int _mouseUses = 0;

    public UnityEvent OnMouseLocked;
    public UnityEvent OnMouseUnlocked;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Update()
    {
        if (_mouseUses < 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            IsMouseLocked = true;
            OnMouseLocked.Invoke();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;   
            IsMouseLocked = false;
            OnMouseUnlocked.Invoke();
        }
    }
}
