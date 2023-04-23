using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    
    //Speed is in km/h
    private float _currentSpeed = 0f;

    private float _currentMoveForce = 0;
    private float _currentBreakForce = 0;
    private float _currentTurningAngle = 0;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _currentMoveForce = InputManager.PlayerActions.CarMovement.Gas.ReadValue<float>() * moveForce;
        _currentTurningAngle = InputManager.PlayerActions.CarMovement.Directions.ReadValue<float>() * turningAngle;
        _currentBreakForce = InputManager.PlayerActions.CarMovement.Break.ReadValue<float>() * breakForce;
    }

    private void FixedUpdate()
    {
        StartCoroutine(CalculateVelocity());
        
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
        
        if ((frontLeftWheel.isGrounded || frontRightWheel.isGrounded) && _currentBreakForce == 0)
            _rb.AddRelativeForce(_currentMoveForce * Vector3.forward);
        else if (_currentBreakForce > 0)
            _rb.AddRelativeForce(_currentBreakForce * Vector3.back);
    }

    private void UpdateWheel(WheelCollider collider, Transform trans)
    {
        Vector3 _position;
        Quaternion _rotation;
        
        collider.GetWorldPose(out _position, out _rotation);

        trans.position = _position;
        trans.rotation = _rotation;
    }

    IEnumerator CalculateVelocity()
    {
        //Gets starting position position of the RB
        //then waits for end of FixedUpdate to get the end position
        Vector3 startPos = _rb.position;
        yield return new WaitForFixedUpdate();
        Vector3 endPos = _rb.position;
        
        //calculates the velocity of the car
        //then calculates the speed that it currently moves, in km/h
        Vector3 currentVelocity = (startPos - endPos) / Time.fixedDeltaTime;
        _currentSpeed = currentVelocity.magnitude * 3.6f;
        Debug.Log(_currentSpeed.ToString("#.##") + "km/h");
    }
}