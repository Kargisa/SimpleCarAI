using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(CarController))]
public class CheckpointManager : MonoBehaviour
{
    private Transform _checkpointRoot;
    
    private List<Checkpoint> _allCheckpoints;
    private int _nextCheckpointIndex;
    
    public event EventHandler OnCheckpointReached;
    public event EventHandler OnWrongCheckpointReached;

    private CarController _carController;

    private void Awake()
    {
        _nextCheckpointIndex = 0;
        _allCheckpoints = new List<Checkpoint>();
        _checkpointRoot = GameObject.FindWithTag("CheckpointManager").transform;
        _carController = GetComponent<CarController>();
        
        foreach (Transform checkpoint in _checkpointRoot)
        {
            Checkpoint thisCheckpoint = checkpoint.GetComponent<Checkpoint>();
            _allCheckpoints.Add(thisCheckpoint);
        }
    }

    public void CheckpointReached(Checkpoint checkpoint)
    {
        if (checkpoint.index == _nextCheckpointIndex)
        {
            _nextCheckpointIndex = (_nextCheckpointIndex + 1) % _allCheckpoints.Count;
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
        return _allCheckpoints[_nextCheckpointIndex].transform;
    }
    
    public void ResetCheckpoints()
    {
        _nextCheckpointIndex = 0;
    }
    
}
