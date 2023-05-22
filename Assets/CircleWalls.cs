using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleWalls : MonoBehaviour
{
    [SerializeField]
    private GameObject wallPrefab; // Reference to the wall prefab
    [SerializeField]
    private int totalWalls = 10; // Total number of walls
    [SerializeField]
    private float radius = 5f; // Radius of the circle
    [SerializeField]
    private Vector2 center = Vector2.zero; // Center position of the circle

    private void Start()
    {
        PlaceWalls();
    }

    private void PlaceWalls()
    {
        for (int i = 0; i < totalWalls; i++)
        {
            float angle = i * (2 * Mathf.PI / totalWalls);
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);

            Vector3 position = new Vector3(x, y, 0f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * angle);

            Instantiate(wallPrefab, position, rotation);
        }
    }
}
