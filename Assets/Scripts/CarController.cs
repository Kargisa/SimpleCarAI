using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CarController : MonoBehaviour
{
    public bool playing;

    [Header("Movement")] [SerializeField] private float moveForce;
    [SerializeField] private float backForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float turningAngle;

    [Header("Collider")] [SerializeField] private WheelCollider frontLeftWheel;
    [SerializeField] private WheelCollider frontRightWheel;
    [SerializeField] private WheelCollider backLeftWheel;
    [SerializeField] private WheelCollider backRightWheel;

    [Header("Transform")] [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;

    [Header("UI")] //[SerializeField] private TMP_Text speedText;
    private Rigidbody _rb;

    private Vector3 _startPos;

    //Speed is in km/h
    [HideInInspector] public float currentSpeed = 0f;

    private float _currentMoveForce = 0;
    private float _currentBackForce = 0;
    private float _currentBreakForce = 0;
    private float _currentTurningAngle = 0;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _startPos = transform.position;
    }

    private void Update()
    {
        if (playing)
            GetInput();
    }

    private void FixedUpdate()
    {
        frontLeftWheel.motorTorque = (_currentMoveForce - _currentBackForce) * 0.03f;
        frontRightWheel.motorTorque = (_currentMoveForce - _currentBackForce) * 0.03f;

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
        {
            _rb.AddRelativeForce(_currentMoveForce * Vector3.forward);
            _rb.AddRelativeForce(_currentBackForce * Vector3.back);
        }
        else if (_currentBreakForce > 0)
            _rb.AddRelativeForce(_currentBreakForce * Vector3.back);

        StartCoroutine(CalculateVelocity());
        UpdateSpeedUI(currentSpeed);
    }

    private void UpdateWheel(WheelCollider collider, Transform trans)
    {
        Vector3 _position;
        Quaternion _rotation;

        collider.GetWorldPose(out _position, out _rotation);

        trans.position = _position;
        trans.rotation = _rotation;
    }

    public void ResetCar()
    {
        _rb.angularVelocity = Vector3.zero;
        _rb.velocity = Vector3.zero;
        //_rb.position = new Vector3(Random.Range(-9, +9), 0, Random.Range(0, -7));
        _rb.position = _startPos;
        _rb.rotation = Quaternion.Euler(0, 0, 0);
    }

    private IEnumerator CalculateVelocity()
    {
        //Gets starting position position of the RB
        //then waits for end of FixedUpdate to get the end position
        Vector3 startPos = _rb.position;
        yield return new WaitForFixedUpdate();
        Vector3 endPos = _rb.position;

        //calculates the velocity of the car
        //then calculates the speed that it currently moves, in km/h
        Vector3 currentVelocity = (startPos - endPos) / Time.fixedDeltaTime;
        currentSpeed = currentVelocity.magnitude * 3.6f;
    }

    private void GetInput()
    {
        _currentMoveForce = InputManager.PlayerActions.CarMovement.Gas.ReadValue<float>() * moveForce;
        _currentBackForce = InputManager.PlayerActions.CarMovement.ReverseGear.ReadValue<float>() * backForce;
        _currentTurningAngle = InputManager.PlayerActions.CarMovement.Directions.ReadValue<float>() * turningAngle;
        _currentBreakForce = InputManager.PlayerActions.CarMovement.Break.ReadValue<float>() * breakForce;
    }

    public void SetInputs(float moveAmount, float turningAmount, float breakAmount)
    {
        switch (moveAmount)
        {
            case 0:
                _currentMoveForce = 0 * moveForce;
                _currentBackForce = 0 * backForce;
                break;
            case 1:
                _currentMoveForce = 1 * moveForce;
                _currentBackForce = 0 * backForce;
                break;
            case 2:
                _currentBackForce = 1 * backForce;
                _currentMoveForce = 0 * moveForce;
                break;
            default:
                _currentMoveForce = 0 * moveForce;
                _currentBackForce = 0 * backForce;
                break;
        }
        _currentTurningAngle = turningAmount * turningAngle;
        _currentBreakForce = breakAmount * breakForce;
    }

    private void UpdateSpeedUI(float newSpeed = 0f)
    {
        //speedText.text = $"{newSpeed:#} km/h";
    }
}