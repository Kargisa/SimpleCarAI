using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private List<Checkpoint> _allCheckpoints;
    private int _nextCheckpointIndex;
    
    public event EventHandler OnCheckpointReached;
    public event EventHandler OnWrongCheckpointReached;

    private void Awake()
    {
        _nextCheckpointIndex = 0;
        _allCheckpoints = new List<Checkpoint>();
        
        foreach (Transform checkpoint in transform)
        {
            Checkpoint thisCheckpoint = checkpoint.GetComponent<Checkpoint>();
            thisCheckpoint.SetCheckpointManager(this);
            _allCheckpoints.Add(thisCheckpoint);
        }
    }

    public void CheckpointReached(Checkpoint checkpoint, CarController carController)
    {
        if (_allCheckpoints.IndexOf(checkpoint) == _nextCheckpointIndex)
        {
            _nextCheckpointIndex = (_nextCheckpointIndex + 1) % _allCheckpoints.Count;
            OnCheckpointReached?.Invoke(this, new CarControllerArgs(carController));
            Debug.Log("Correct");
        }
        else
        {
            OnWrongCheckpointReached?.Invoke(this, new CancelEventArgs(carController));
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
