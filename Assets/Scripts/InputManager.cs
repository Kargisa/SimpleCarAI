using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static PlayerActions PlayerActions { get; set; }

    private void Awake()
    {
        PlayerActions = new PlayerActions();
        PlayerActions.CarMovement.Enable();
    }
}
