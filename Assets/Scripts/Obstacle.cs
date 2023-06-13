using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType
    {
        Fence,
        Rock,
        Wall,
        Cone,
    }

    public ObstacleType obstacleType;
}
