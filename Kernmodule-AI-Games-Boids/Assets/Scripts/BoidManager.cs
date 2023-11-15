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
        Vector3 Rule4 = new Vector3();

        foreach (Boid b in boidList)
        {
            b.Velocity += Cohesion(b) * CohesionDebug;
            b.Velocity += Separation(b) * SeperationDebug;
            b.Velocity += Alignment(b) * AlingmentDebug;
            b.Velocity += BoundToArea(b);

            Debug.Log( "RULE1" + this + Rule1);
            Debug.Log("RULE2" + this + Rule2);
            Debug.Log("RULE3" + this + Rule3);

            LimitSpeed(b);
            b.transform.position += b.Velocity;

            b.Velocity = Vector3.zero;
        }
    }

    private Vector3 Cohesion(Boid _CurrentBoid)
    {
        Vector3 CohesionVector = new Vector3();
        foreach (Boid OtherBoid in boidList)
        {
            if (_CurrentBoid.transform.position != CohesionVector)
            {
                //Count all of the postions onto one vector;
                CohesionVector += _CurrentBoid.transform.position;
            }
        }
        //devide by all of the boids - the currentone
        CohesionVector /= boidList.Count - 1;
        return (CohesionVector - _CurrentBoid.transform.position) / CohesionValue;
    }

    private Vector3 Separation(Boid _CurrentBoid)
    {
        Vector3 SeparateVector = new Vector3();
        SeparateVector = Vector3.zero;

        foreach(Boid otherBoid in boidList)
        {
            if (otherBoid.transform.position != _CurrentBoid.transform.position)
            {
                //if a boid is close by then
                if (Vector3.Distance(_CurrentBoid.transform.position, otherBoid.transform.position) < distancePerBoid)
                {
                    SeparateVector += (_CurrentBoid.transform.position - otherBoid.transform.position);
                }
            }
        }
        return SeparateVector;
    }

    private Vector3 Alignment(Boid _CurrentBoid)
    {
        Vector3 AlignmentVector = new Vector3();
        foreach (Boid b in boidList)
        {
            if (b != _CurrentBoid)
            {
                AlignmentVector += b.Velocity;
            }
        }
        AlignmentVector /= boidAmount - 1;
        return (AlignmentVector - _CurrentBoid.Velocity) / AlingmentValue;
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

    private Vector3 BoundToArea(Boid _CurrentBoid)
    {
        float Xmin = 0f;
        float Xmax = fieldSize;
        float Ymin = 0f;
        float Ymax = fieldSize;
        float Zmin = 0f;
        float Zmax = fieldSize;
        Vector3 BoundingDirection = new Vector3();

        if (_CurrentBoid.transform.position.x < Xmin)
        {
            BoundingDirection.x = boundingEdgeSize; 
        } else if (_CurrentBoid.transform.position.x > Xmax)
        {
            BoundingDirection.x = -boundingEdgeSize;
		}

        if (_CurrentBoid.transform.position.y < Ymin)
        {
            BoundingDirection.y = boundingEdgeSize;
        } else if (_CurrentBoid.transform.position.y > Ymax)
        {
            BoundingDirection.y = -boundingEdgeSize;
        }

        if (_CurrentBoid.transform.position.z < Zmin)
        {
            BoundingDirection.z = boundingEdgeSize;
        } else if (_CurrentBoid.transform.position.z  > Zmax)
        {
            BoundingDirection.z = -boundingEdgeSize;
        }

        return BoundingDirection;
    }
}
