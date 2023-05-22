using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CheckpointManager>(out CheckpointManager manager))
        {
            manager.CheckpointReached(this);
        }
    }
}
