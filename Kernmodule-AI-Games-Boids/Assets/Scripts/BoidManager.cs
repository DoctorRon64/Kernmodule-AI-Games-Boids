using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private int BoidAmount = 100;
    [SerializeField] private float FieldSize = 1f;
    [SerializeField] GameObject BoidPrefab;

    [Header("Boids")]
    [SerializeField] private List<Boid> boidList;
    [SerializeField] private List<Transform> transformList;
    [SerializeField] private float DistancePerBoid;

    private void Awake()
    {
        SpawnBoids();
    }
    private void Start()
    {
        UpdateBoidPositions();
        SearchNeighbors();
    }

    private void SpawnBoids()
    {
        for (int i = 0; i < BoidAmount; i++)
        {
            float x = Random.Range(0, FieldSize);
            float z = Random.Range(0, FieldSize);
            float y = Random.Range(0, FieldSize);
            Vector3 BoidSpawnPos = new Vector3(x, y, z);
            Boid newBoid = Instantiate(BoidPrefab, BoidSpawnPos, Quaternion.identity).GetComponent<Boid>();
            transformList.Add(newBoid.transform);
            boidList.Add(newBoid);
        }
    }

    private void UpdateBoidPositions()
    {
        foreach (Boid boid in boidList)
        {
            Vector3 newPos = boid.transform.position + boid.Velocity * Time.deltaTime;
            boid.UpdatePosition(newPos, DistancePerBoid);
        }
    }

    private void SearchNeighbors()
    {
        foreach (Boid boid in boidList)
        {
            boid.UpdateNeighbors(transformList, DistancePerBoid);
        }
    }

    
}
