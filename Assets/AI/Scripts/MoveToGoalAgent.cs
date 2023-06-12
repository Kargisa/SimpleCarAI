using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CarController), typeof(CheckpointManager))]
public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform goalTransform;
    [Min(1f)]
    public int labCount = 1;
    
    private CheckpointManager _checkpointManager;
    private CarController _controller;

    private void Awake()
    {
        _checkpointManager = GetComponent<CheckpointManager>();
        _controller = GetComponent<CarController>();
    }

    private void Start()
    {
        _checkpointManager.OnCheckpointReached += OnCheckpointReached;
        _checkpointManager.OnWrongCheckpointReached += OnWrongCheckpoint;
    }

    public override void OnEpisodeBegin()
    {
        if (_controller.playing)
            Debug.LogWarning("Manual inputs are active! Make sure to not train the AI while manual inputs are active!");

        _checkpointManager.ResetCheckpoints();
        _controller.ResetCar();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 nextCheckpointForward = _checkpointManager.GetNextCheckpoint().forward;
        float directionDot = Vector3.Dot(transform.forward, nextCheckpointForward);

        sensor.AddObservation(directionDot);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(goalTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
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

        
        if ((int)InputManager.PlayerActions.CarMovement.Gas.ReadValue<float>() == 1 && (int)InputManager.PlayerActions.CarMovement.ReverseGear.ReadValue<float>() == 1)
            discreteActions[0] = 0;
        else if ((int)InputManager.PlayerActions.CarMovement.Gas.ReadValue<float>() == 1)
            discreteActions[0] = 1;
        else if ((int)InputManager.PlayerActions.CarMovement.ReverseGear.ReadValue<float>() == 1)
            discreteActions[0] = 2;
        
        discreteActions[1] = (int)InputManager.PlayerActions.CarMovement.Break.ReadValue<float>();
        discreteActions[2] = (int)InputManager.PlayerActions.CarMovement.Directions.ReadValue<float>() switch
        {
            0 => 0,
            1 => 1,
            -1 => 2,
            _ => 0
        };
    }

    private void OnCheckpointReached(object sender, EventArgs e)
    {
        if (e is not CarControllerArgs cca)
            return;

        if (cca.Controller.transform == transform)
            AddReward(1f);
    }

    private void OnWrongCheckpoint(object sender, EventArgs e)
    {
        if (e is not CarControllerArgs cca)
            return;

        if (cca.Controller.transform == transform)
            AddReward(-0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            if (_checkpointManager.lab != labCount) 
                return;
            
            AddReward(10f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Respawn"))
        {
            AddReward(-1f);
        }
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.collider.CompareTag("Respawn"))
        {
            AddReward(-0.5f * Time.deltaTime);
        }
    }
}