using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private CheckpointManager checkpointManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CarController>(out CarController controller))
        {
            checkpointManager.CheckpointReached(this, controller);
        }
    }

    public void SetCheckpointManager(CheckpointManager manager)
    {
        checkpointManager = manager;
    }
    
}
