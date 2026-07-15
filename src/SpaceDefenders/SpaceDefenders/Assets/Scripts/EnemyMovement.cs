using UnityEngine;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    public List<Transform> waypoints;
    public float speed = 0.2f;
    public float rotationSpeed = 5f;
    public float waypointThreshold = 0.02f;

    [Tooltip("How much HP this enemy removes from the reactor if it reaches the end.")]
    public int damageToReactor = 1;

    private int currentWaypointIndex = 0;

    void Update()
    {
        if (waypoints == null || currentWaypointIndex >= waypoints.Count) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = target.position - transform.position;
        
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target.position) < waypointThreshold)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                BaseHealth baseHealth = Object.FindAnyObjectByType<BaseHealth>();
                if (baseHealth != null) baseHealth.TakeDamage(damageToReactor);
                Destroy(gameObject);
            }
        }
    }

    public void SetWaypoints(List<Transform> newWaypoints)
    {
        waypoints = newWaypoints;
    }
}