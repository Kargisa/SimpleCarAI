using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CarController))]
public class CheckpointManager : MonoBehaviour
{
    private Transform _checkpointRoot;

    private List<Checkpoint> _allCheckpoints;
    public int nextCheckpointIndex;
    public int lab = 0;

    public event EventHandler OnCheckpointReached;
    public event EventHandler OnWrongCheckpointReached;

    private CarController _carController;

    private void Awake()
    {
        nextCheckpointIndex = 0;
        _allCheckpoints = new List<Checkpoint>();
        _checkpointRoot = GameObject.Find("Checkpoints").transform;
        _carController = GetComponent<CarController>();
        int index = 0;
        foreach (Transform trans in _checkpointRoot)
        {
            Checkpoint checkpoint = trans.GetComponent<Checkpoint>();
            checkpoint.index = index;
            _allCheckpoints.Add(checkpoint);
            index++;
        }
    }

    public void CheckpointReached(Checkpoint checkpoint)
    {
        if (checkpoint.index == nextCheckpointIndex)
        {
            nextCheckpointIndex = (nextCheckpointIndex + 1) % _allCheckpoints.Count;
            if (nextCheckpointIndex == 0)
                lab++;
            OnCheckpointReached?.Invoke(this, new CarControllerArgs(_carController));
            Debug.Log("Correct");
        }
        else
        {
            OnWrongCheckpointReached?.Invoke(this, new CancelEventArgs(_carController));
            Debug.Log("Wrong");
        }
    }

    public Transform GetNextCheckpoint()
    {
        if (_allCheckpoints.Count <= 0)
            throw new AggregateException("No checkpoints found");
        return _allCheckpoints[nextCheckpointIndex].transform;
    }

    public void ResetCheckpoints()
    {
        nextCheckpointIndex = 0;
        lab = 0;
    }
}