using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// The mouse lock manager is a singleton that manages the state of the mouse lock.
// Sometimes multiple UI elements will want to unlock the mouse, so this script
// keeps track of how many UI elements are using the mouse, and only locks the
// mouse when there are no UI elements using it.
public class MouseLockManager : MonoBehaviour
{
    // Singletons are a useful pattern for manager scripts like this one
    public static MouseLockManager Instance { get; private set; }
    
    // Whether or not, in the end, the mouse is locked
    public static bool IsMouseLocked { get; private set; }

    // Called when the mouse needs to be unlocked (like when a UI element is open)
    public static void TakeMouse()
    {
        _mouseUsers++;
    }
    
    // Called after TakeMouse() when the mouse is no longer needed
    public static void ReleaseMouse()
    {
        _mouseUsers--;
    }
    
    // The number of UI elements that are using the mouse
    private static int _mouseUsers = 0;

    void Awake()
    {
        Instance = this;
    }
    
    void Update()
    {
        // Consider the mouse locked if there are no UI elements using it
        if (_mouseUsers < 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            IsMouseLocked = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;   
            IsMouseLocked = false;
        }
    }
}
