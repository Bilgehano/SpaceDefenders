using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2.0f;
    public int health = 3;
    
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private BaseHealth baseHealth;

    public void Setup(Transform[] pathWaypoints, BaseHealth targetBase)
    {
        waypoints = pathWaypoints;
        baseHealth = targetBase;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = target.position - transform.position;
        transform.position += direction.normalized * speed * Time.deltaTime;


        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex++;

            // Check if we reached the end
            if (currentWaypointIndex >= waypoints.Length)
            {
                if (baseHealth != null)
                {
                    baseHealth.TakeDamage(1);
                }
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
