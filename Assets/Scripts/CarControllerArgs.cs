using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControllerArgs : EventArgs
{
    public CarController Controller { get; }

    public CarControllerArgs(CarController controller)
    {
        Controller = controller;
    }
}
