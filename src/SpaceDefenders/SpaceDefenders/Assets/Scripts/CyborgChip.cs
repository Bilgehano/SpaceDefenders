using UnityEngine;

public class CyborgChip : MonoBehaviour
{
    public float collectDistance = 1.0f;
    public float moveSpeed = 5.0f;
    private Transform playerTransform;
    private bool isBeingCollected = false;

    public AudioClip collectSound;
    [Range(0f, 1f)] public float collectVolume = 0.6f;

    void Start()
    {
        if (Camera.main != null)
        {
            playerTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance < collectDistance)
        {
            isBeingCollected = true;
        }

        if (isBeingCollected)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, playerTransform.position) < 0.1f)
            {
                if (collectSound != null)
                {
                    AudioSource.PlayClipAtPoint(collectSound, transform.position, collectVolume);
                }

                ResourceManager.Instance.AddChips(1);
                Destroy(gameObject);
            }
        }
    }
}