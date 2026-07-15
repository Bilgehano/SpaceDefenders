using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 10f;
    public GameObject chipPrefab;
    public int chipsToDrop = 2;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (chipPrefab != null)
        {
            for (int i = 0; i < chipsToDrop; i++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-0.2f, 0.2f), 0.1f, Random.Range(-0.2f, 0.2f));
                Instantiate(chipPrefab, transform.position + randomOffset, Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }
}