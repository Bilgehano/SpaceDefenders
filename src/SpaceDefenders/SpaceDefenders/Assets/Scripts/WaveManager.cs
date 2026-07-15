using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject prefab;
        public int count;
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public List<EnemySpawnInfo> enemies;
        public float interval;
        public float pauseAfter;
    }

    [System.Serializable]
    public class Wave
    {
        public float speedMultiplier = 1f;
        public List<EnemyGroup> groups;
    }

    public List<Wave> waves;
    public Transform waypointsParent;
    public TextMeshProUGUI waveInfoText;
    public TextMeshProUGUI chipInfoText;
    public TextMeshProUGUI warningText;

    private List<Transform> waypoints = new List<Transform>();
    private int currentWaveIndex = 0;
    private bool isWaveActive = false;

    void Start()
    {
        if (waypointsParent == null)
        {
            GameObject pathObj = GameObject.Find("Path");
            if (pathObj != null)
            {
                waypointsParent = pathObj.transform;
                Debug.Log("WaveManager: Automatically assigned 'Path' as waypointsParent.");
            }
        }
        
        InitializeWaypoints();
        UpdateWaveInfo();
        if (warningText != null) warningText.gameObject.SetActive(false);
    }

    public void InitializeWaypoints()
    {
        waypoints.Clear();
        if (waypointsParent != null)
        {
            foreach (Transform t in waypointsParent)
            {
                waypoints.Add(t);
            }
        }
    }

    public void StartNextWave()
    {
        if (isWaveActive)
        {
            ShowWarning("Wave is not completed");
            return;
        }

        if (currentWaveIndex >= waves.Count)
        {
            ShowWarning("All waves completed!");
            return;
        }

        int waveToStart = currentWaveIndex;
        StartCoroutine(SpawnWave(waveToStart));
        currentWaveIndex++;
        UpdateWaveInfo();
    }

    private IEnumerator SpawnWave(int waveIndex)
    {
        isWaveActive = true;
        Wave wave = waves[waveIndex];
        Debug.Log($"Starting Wave {waveIndex + 1}");

        if (wave.groups == null || wave.groups.Count == 0)
        {
            Debug.LogWarning($"Wave {waveIndex + 1} has no groups defined!");
        }
        else
        {
            foreach (var group in wave.groups)
            {
                if (group.enemies == null || group.enemies.Count == 0)
                {
                    Debug.LogWarning($"Wave {waveIndex + 1} has a group with no enemies!");
                    continue;
                }

                for (int typeIndex = 0; typeIndex < group.enemies.Count; typeIndex++)
                {
                    var enemyInfo = group.enemies[typeIndex];
                    if (enemyInfo.prefab == null || enemyInfo.count <= 0) continue;

                    for (int i = 0; i < enemyInfo.count; i++)
                    {
                        SpawnEnemy(enemyInfo.prefab, wave.speedMultiplier);
                        
                        // Wait unless it's the absolute last enemy of the absolute last type in this group
                        bool isLastType = typeIndex == group.enemies.Count - 1;
                        bool isLastEnemyOfType = i == enemyInfo.count - 1;

                        if (!(isLastType && isLastEnemyOfType))
                        {
                            yield return new WaitForSeconds(group.interval);
                        }
                    }
                }
                yield return new WaitForSeconds(group.pauseAfter);
            }
        }


        while (Object.FindObjectsByType<EnemyMovement>(FindObjectsInactive.Exclude).Length > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        isWaveActive = false;
        Debug.Log($"Wave {waveIndex + 1} Finished");
    }

    private void SpawnEnemy(GameObject prefab, float speedMultiplier)
    {
        if (prefab == null || waypoints.Count == 0) return;

        GameObject enemy = Instantiate(prefab, waypoints[0].position, Quaternion.identity);
        enemy.tag = "Enemy"; // Ensure it's tagged for tower targeting
        
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.speed *= speedMultiplier;
            movement.SetWaypoints(waypoints);
        }
    }

    private void UpdateWaveInfo()
    {
        if (waveInfoText != null)
        {
            // Show 1-based wave number if a wave is running, otherwise show the next wave to start
            waveInfoText.text = "Wave: " + (currentWaveIndex);
        }
    }

    public void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            warningText.gameObject.SetActive(true);
            StopCoroutine("HideWarning");
            StartCoroutine(HideWarning());
        }
    }

    private IEnumerator HideWarning()
    {
        yield return new WaitForSeconds(1f);
        if (warningText != null) warningText.gameObject.SetActive(false);
    }
}