using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private int boidAmount = 100;
    [SerializeField] private float fieldSize = 1f;
    [SerializeField] GameObject boidPrefab;

    [Header("Boids")]
    [SerializeField] private List<Boid> boidList;
    [SerializeField] private List<Transform> transformList;
    [SerializeField] private float distancePerBoid;
    [SerializeField] private float boundingEdgeSize = 10f;
    [SerializeField] private float CohesionValue = 10f;
    [SerializeField] private float limitSpeedValue = 1f;
    [SerializeField] private float AlingmentValue = 8f;

    [Header("Boids")]
    [SerializeField] private float CohesionDebug;
    [SerializeField] private float SeperationDebug;
    [SerializeField] private float AlingmentDebug;

    private void Awake()
    {
        SpawnBoids();
    }

    private void Update()
    {
        UpdateBoidPositions();
    }

    private void SpawnBoids()
    {
        for (int i = 0; i < boidAmount; i++)
        {
            float x = Random.Range(0, fieldSize);
            float z = Random.Range(0, fieldSize);
            float y = Random.Range(0, fieldSize);
            Vector3 BoidSpawnPos = new Vector3(x, y, z);
            Boid newBoid = Instantiate(boidPrefab, BoidSpawnPos, Quaternion.identity).GetComponent<Boid>();
            transformList.Add(newBoid.transform);
            boidList.Add(newBoid);
        }
    }

    private void UpdateBoidPositions()
    {
        Vector3 Rule1 = new Vector3();
        Vector3 Rule2 = new Vector3();
        Vector3 Rule3 = new Vector3();

        foreach (Boid b in boidList)
        {
            b.Velocity += Cohesion(b) * CohesionDebug;
            b.Velocity += Separation(b) * SeperationDebug;
            b.Velocity += Alignment(b) * AlingmentDebug;

            Debug.Log( "RULE1"  + Rule1);
            Debug.Log("RULE2"  + Rule2);
            Debug.Log("RULE3"  + Rule3);

            LimitSpeed(b);
            b.transform.position += b.Velocity;
        }
    }

    private Vector3 Cohesion(Boid _CurrentBoid)
    {
        Vector3 CohesionVector = Vector3.zero;
        foreach (Boid otherBoid in boidList)
        {
            if (otherBoid != _CurrentBoid)
            {
                //Count all of the postions onto one vector;
                CohesionVector += _CurrentBoid.transform.position;
            }
        }
        //devide by all of the boids - the currentone
        CohesionVector /= boidList.Count - 1;
        CohesionVector.Normalize();
        CohesionVector -= _CurrentBoid.transform.position;
        CohesionVector.Normalize();
        return CohesionVector;
    }

    private Vector3 Separation(Boid _CurrentBoid)
    {
        Vector3 SeparateVector = Vector3.zero;

        foreach(Boid otherBoid in boidList)
        {
            if (otherBoid != _CurrentBoid)
            {
                //if a boid is close by then
                if (Vector3.Distance(_CurrentBoid.transform.position, otherBoid.transform.position) < distancePerBoid)
                {
                    SeparateVector += _CurrentBoid.transform.position - otherBoid.transform.position;
                }
            }
        }
        SeparateVector.Normalize();
        return SeparateVector;
    }

    private Vector3 Alignment(Boid _CurrentBoid)
    {
        Vector3 AlignmentVector = Vector3.zero;
        foreach (Boid otherBoid in boidList)
        {
            if (otherBoid != _CurrentBoid)
            {
                AlignmentVector += otherBoid.Velocity;
            }
        }
        AlignmentVector /= boidAmount - 1;
        AlignmentVector.Normalize();
        AlignmentVector = AlignmentVector - _CurrentBoid.Velocity;
        AlignmentVector.Normalize();
        return AlignmentVector;
    }

    private void LimitSpeed(Boid _CurrentBoid)
    {
        float Vlim = limitSpeedValue;
        float boidVelocityMagnitude = _CurrentBoid.Velocity.magnitude;
        if (boidVelocityMagnitude > Vlim)
        {
            _CurrentBoid.Velocity = (_CurrentBoid.Velocity / boidVelocityMagnitude) * Vlim;
        }
    }
}
