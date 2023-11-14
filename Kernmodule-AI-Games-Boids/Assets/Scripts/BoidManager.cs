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
	[SerializeField] private float limitSpeedValue = 1f;

    private void Awake()
    {
        SpawnBoids();
    }

    private void LateUpdate()
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
            Rule1 = Cohesion(b);
            Rule2 = Separation(b);
            Rule3 = Alignment(b);
            Rule4 = BoundToArea(b);
            b.Velocity = b.Velocity + Rule1 + Rule2 + Rule3 + Rule4;

            LimitSpeed(b);

            b.transform.position = b.transform.position + b.Velocity;
        }
    }

    private Vector3 Cohesion(Boid _CurrentBoid)
    {
        Vector3 Center = new Vector3();
        foreach (Boid b  in boidList)
        {
            if (b.transform.position != Center)
            {
                Center += b.transform.position;
            }
            Center /= boidAmount - 1;
        }
        return (Center - _CurrentBoid.transform.position) / 100f;
    }

    private Vector3 Separation(Boid _CurrentBoid)
    {
        Vector3 Center = new Vector3();
        Center = Vector3.zero;

        foreach(Boid b in boidList)
        {
            if (b !=  _CurrentBoid)
            {
                if (Vector3.Distance(b.transform.position, _CurrentBoid.transform.position) < distancePerBoid)
                {
                    Center = Center - (b.transform.position - _CurrentBoid.transform.position);
                }
            }
        }
        return Center;
    }

    private Vector3 Alignment(Boid _CurrentBoid)
    {
        Vector3 pvJ = new Vector3();

        foreach (Boid b in boidList)
        {
            if (b != _CurrentBoid)
            {
                pvJ += b.Velocity;
            }
        }

        pvJ /= boidAmount - 1;
        return (pvJ - _CurrentBoid.Velocity) / 8;
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
