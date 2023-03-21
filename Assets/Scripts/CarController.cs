using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float moveForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float turningAngle;
    
    [Header("Collider")] 
    [SerializeField] private WheelCollider frontLeftWheel;
    [SerializeField] private WheelCollider frontRightWheel;
    [SerializeField] private WheelCollider backLeftWheel;
    [SerializeField] private WheelCollider backRightWheel;

    [Header("Transform")]
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;
    
    private Rigidbody _rb;

    private float _currentMoveForce = 0;
    private float _currentBreakForce = 0;
    private float _currentTurningAngle = 0;

    private void Update()
    {
        _currentMoveForce = InputManager.PlayerActions.CarMovement.Gas.ReadValue<float>() * moveForce;
        _currentTurningAngle = InputManager.PlayerActions.CarMovement.Directions.ReadValue<float>() * turningAngle;
        _currentBreakForce = InputManager.PlayerActions.CarMovement.Break.ReadValue<float>() * breakForce;
    }

    private void FixedUpdate()
    {
        frontLeftWheel.motorTorque = _currentMoveForce;
        frontRightWheel.motorTorque = _currentMoveForce;

        frontLeftWheel.brakeTorque = _currentBreakForce;
        frontRightWheel.brakeTorque = _currentBreakForce;
        backLeftWheel.brakeTorque = _currentBreakForce;
        backRightWheel.brakeTorque = _currentBreakForce;

        frontLeftWheel.steerAngle = _currentTurningAngle;
        frontRightWheel.steerAngle = _currentTurningAngle;

        UpdateWheel(frontRightWheel, frontRightWheelTransform);
        UpdateWheel(frontLeftWheel, frontLeftWheelTransform);
        UpdateWheel(backRightWheel, backRightWheelTransform);
        UpdateWheel(backLeftWheel, backLeftWheelTransform);
    }

    private void UpdateWheel(WheelCollider collider, Transform trans)
    {
        Vector3 _position;
        Quaternion _rotation;
        
        collider.GetWorldPose(out _position, out _rotation);

        trans.position = _position;
        trans.rotation = _rotation;
    }
    
}