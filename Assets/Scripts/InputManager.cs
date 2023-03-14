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

    public UnityEvent onPressN, onPressA, onPressI;

    private ChekiLogic _chekiLogic;

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

        _chekiLogic = FindObjectOfType<ChekiLogic>();
    }

    private void Update()
    {
        // the button to interrupt
        if (Input.GetKeyDown(KeyCode.A))
        {
            onPressA.Invoke();
        }

        // for the buttons below, do not receive any inputs until it finishes all the animations
        if (_chekiLogic.AnimState != ChekiLogic.AnimationState.Finished) return;

        // define below which keys to bind the events
        if (Input.GetKeyDown(KeyCode.N))
        {
            onPressN.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            onPressI.Invoke();
        }
    }
}
