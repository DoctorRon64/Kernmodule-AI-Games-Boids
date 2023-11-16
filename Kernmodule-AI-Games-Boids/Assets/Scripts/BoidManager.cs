using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private int boidAmount = 100;
    [SerializeField] private float SpawnArea = 100f;
    [SerializeField] GameObject boidPrefab;

    [Header("Boids")]
    [SerializeField] private List<Boid> boidList;
    [SerializeField] private List<Transform> transformList;

    [SerializeField] private float distancePerBoid = .3f;
    [SerializeField] private float limitSpeedValue = 1f;
    [SerializeField] private float AlingmentValue = 8f;
    [SerializeField] private float SphereRadius = 8f;

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
            float x = Random.Range(0, SpawnArea);
            float z = Random.Range(0, SpawnArea);
            float y = Random.Range(0, SpawnArea);
            Vector3 BoidSpawnPos = new Vector3(x, y, z);
            Boid newBoid = Instantiate(boidPrefab, BoidSpawnPos, Quaternion.identity).GetComponent<Boid>();
            transformList.Add(newBoid.transform);
            boidList.Add(newBoid);
        }
    }

    private void UpdateBoidPositions()
    {
        foreach (Boid b in boidList)
        {
            b.Velocity += Cohesion(b) * CohesionDebug;
            b.Velocity += Separation(b) * SeperationDebug;
            b.Velocity += Alignment(b) * AlingmentDebug;

            LimitSpeed(b);
			ConstrainToBounds(b);

			b.transform.position += b.Velocity;
        }
    }

	private void ConstrainToBounds(Boid _CurrentBoid)
	{
        float sphereRadius = SphereRadius;
		Vector3 center = Vector3.zero;

		float distanceToCenter = Vector3.Distance(_CurrentBoid.transform.position, center);

		if (distanceToCenter > sphereRadius)
		{
			Vector3 toCenter = (center - _CurrentBoid.transform.position).normalized;
			_CurrentBoid.Velocity += toCenter;
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
