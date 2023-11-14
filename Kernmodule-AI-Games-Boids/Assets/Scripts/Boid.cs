using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 Velocity = Vector3.zero;
    public List<Transform> NeighborsInRange;

    public void UpdatePosition(Vector3 _pos, float _distance)
    {
        transform.position = _pos;
        transform.rotation = Quaternion.LookRotation(Vector3.Normalize(Velocity));
        Velocity = SteerSeperation(_distance);
    }

    private Vector3 SteerSeperation(float _distance)
    {
        Vector3 direction = Vector3.zero;
        foreach (Transform t in NeighborsInRange)
        {
            float ratio = Mathf.Clamp01((t.position - transform.position).magnitude / _distance);
            direction -= ratio * (t.position - transform.position);
        }

        return direction.normalized;
    }

    public void UpdateNeighbors(List<Transform> _listBoids, float _distance)
    {
        NeighborsInRange.Clear();

        foreach (Transform t in _listBoids)
        {
            if (transform != t)
            {
                if (Vector3.Distance(t.position, transform.position) < _distance)
                {
                    NeighborsInRange.Add(t);
                }
            }
        }
    }





    // Cohesion, separation, alignment methods...
}
