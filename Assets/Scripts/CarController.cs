using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float turningSpeed;
    
    [Header("GroundCheck")] 
    [SerializeField] private Transform groundCheckAnchor;
    [SerializeField] private float groundCheckRadius = .01f;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody _rb;
    
    private float _gas = 0;
    private float _direction = 0;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _gas = InputManager.PlayerActions.CarMovement.Gas.ReadValue<float>();
        _direction = InputManager.PlayerActions.CarMovement.Directions.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            if (_rb.velocity != Vector3.zero)
            {
                transform.Rotate(Vector3.up * _direction * turningSpeed * Time.fixedDeltaTime);
            }
            _rb.AddRelativeForce(new Vector3(0,0, _gas * speed), ForceMode.Force);
        }
    }

    private bool IsGrounded()
    {
        Collider[] overlap = Physics.OverlapSphere(groundCheckAnchor.position, groundCheckRadius, groundMask);
        if (overlap.Length == 0)
            return false;
        return true;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheckAnchor.position, groundCheckRadius);
    }
}
