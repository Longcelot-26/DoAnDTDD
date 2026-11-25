using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform[] spawnpoints;
    [SerializeField] private float spawnTime = 1.5f;
    [SerializeField] private float spawnRadiusMin = 15f;
    [SerializeField] private float spawnRadiusMax = 20f;
    private Player player;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnTime);
            if (enemies == null || enemies.Length == 0) continue;

            GameObject enemyPrefab = enemies[Random.Range(0, enemies.Length)];
            Vector3 spawnPos;

            if (player != null)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                float dist = Random.Range(spawnRadiusMin, spawnRadiusMax);
                spawnPos = player.transform.position + new Vector3(randomDir.x, randomDir.y, 0f) * dist;
            }
            else if (spawnpoints != null && spawnpoints.Length > 0)
            {
                Transform sp = spawnpoints[Random.Range(0, spawnpoints.Length)];
                spawnPos = sp.position;
            }
            else
            {
                spawnPos = transform.position;
            }

            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}
