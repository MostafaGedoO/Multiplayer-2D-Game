using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInput : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    private void Start()
    {
        inputReader.OnFireEvent += InputReader_OnFireEvent;
        inputReader.OnMoveEvent += InputReader_OnMoveEvent;
    }

    private void OnDestroy()
    {
        inputReader.OnFireEvent -= InputReader_OnFireEvent;
        inputReader.OnMoveEvent -= InputReader_OnMoveEvent;
    }

    private void InputReader_OnMoveEvent(Vector2 obj)
    {
        Debug.Log(obj);
    }

    private void InputReader_OnFireEvent(bool obj)
    {
        Debug.Log(obj);
    }
}
