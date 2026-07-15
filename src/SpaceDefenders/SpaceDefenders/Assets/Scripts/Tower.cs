using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float range = 0.8f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    public float rotationSpeed = 200f;

    public AudioClip shootSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GameObject nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            Vector3 targetDirection = nearestEnemy.transform.position - transform.position;

            if (targetDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            fireCountdown -= Time.deltaTime;
            if (fireCountdown <= 0f)
            {
                Shoot(nearestEnemy);
                fireCountdown = 1f / fireRate;
            }
        }
        else
        {
            fireCountdown = 0f;
        }
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance && distance <= range)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }
        return nearest;
    }

    void Shoot(GameObject target)
    {
        if (projectilePrefab == null) return;

        Vector3 direction = (target.transform.position - shootPoint.position).normalized;
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.right, direction);

        GameObject projectileObj = Instantiate(projectilePrefab, shootPoint.position, spawnRotation);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Seek(target.transform);
        }

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, 0.5f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}