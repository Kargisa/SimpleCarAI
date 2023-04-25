using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform goalTransform;

    private CarController _controller;
    private Rigidbody _rb;

    private void Start()
    {
        _controller = GetComponent<CarController>();
        _rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if (_controller.playing)
            Debug.LogWarning("Manual inputs are active! Make sure to not train the AI while manual inputs are active!");
        _controller.ResetCar();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(goalTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log(actions.DiscreteActions[0]);
        
        float moveAmount = actions.DiscreteActions[0];
        float breakAmount = actions.DiscreteActions[1];
        float turningAmount = actions.DiscreteActions[2] switch
        {
            0 => 0,
            1 => 1,
            2 => -1,
            _ => 0
        };

        _controller.SetInputs(moveAmount, turningAmount, breakAmount);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = (int)InputManager.PlayerActions.CarMovement.Gas.ReadValue<float>();
        discreteActions[1] = (int)InputManager.PlayerActions.CarMovement.Break.ReadValue<float>();
        discreteActions[2] = (int)InputManager.PlayerActions.CarMovement.Directions.ReadValue<float>() switch
        {
            0 => 0,
            1 => 1,
            -1 => 2,
            _ => 0
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            SetReward(1f);
            EndEpisode();
        }
        else if (other.CompareTag("Respawn"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }
}