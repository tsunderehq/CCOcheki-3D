using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// define here what callback functions to invoke when a key is pressed
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance = null;

    public UnityEvent onPressEnter;

    /// <summary>
    /// guarantee only one instance of this InputManager
    /// (be careful as it will destroy the second instanced gameobject)
    /// </summary>
    private void Start()
    {
        if (InputManager.Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("pressed enter");
            onPressEnter.Invoke();
        }
    }
}
