using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        [TextArea(2, 4)]
        public string waveDescription = "Описание волны...";
        public Sprite waveIcon;
        public GameObject[] addictiveObjects;
        public List<GameObject> enemyPrefabs;
        public int enemyCount = 10;
        public float spawnDelay = 2f;
    }
    
    [System.Serializable]
    public class WaveEvent : UnityEvent<string, string, Sprite, GameObject[]> { }
    
    public List<Wave> waves;
    public float spawnRadius = 20f;
    public float deadZoneRadius = 5f;
    public Transform player;
    public WaveEvent onWaveStart;
    public UnityEvent onAllWavesComplete;
    
    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private int enemiesAlive = 0;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        StartCoroutine(SpawnWaveWithDelay(5));
    }

    IEnumerator SpawnWaveWithDelay(int delay)
    {
        yield return new WaitForSeconds(delay);
        StartNextWave();
    }
    
    void StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            onAllWavesComplete?.Invoke();
            return;
        }
        
        Wave currentWave = waves[currentWaveIndex];
        onWaveStart?.Invoke(currentWave.waveName, currentWave.waveDescription, currentWave.waveIcon, currentWave.addictiveObjects);
        StartCoroutine(SpawnWave(currentWave));
    }
    
    IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;
        
        for (int i = 0; i < wave.enemyCount; i++)
        {
            if (wave.enemyPrefabs.Count > 0)
            {
                GameObject randomEnemy = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Count)];
                SpawnEnemy(randomEnemy);
            }
            
            yield return new WaitForSeconds(wave.spawnDelay);
        }
        
        isSpawning = false;
        
        yield return new WaitUntil(() => enemiesAlive <= 0);
        yield return new WaitForSeconds(2f);
        
        currentWaveIndex++;
        
        if (currentWaveIndex < waves.Count)
        {
            StartNextWave();
        }
        else
        {
            onAllWavesComplete?.Invoke();
        }
    }
    
    void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        
        if (spawnPos != Vector3.zero && enemyPrefab != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            enemiesAlive++;
            
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.onDeath.AddListener(() => OnEnemyDeath());
            }
            
            BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();
            if (baseEnemy != null && player != null)
            {
                baseEnemy.MainTarget = player;
            }
        }
    }
    
    void OnEnemyDeath()
    {
        enemiesAlive--;
    }
    
    Vector3 GetRandomSpawnPosition()
    {
        if (player == null)
        {
            return transform.position;
        }
        
        for (int i = 0; i < 10; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(deadZoneRadius, spawnRadius);
            
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;
            
            Vector3 spawnPos = player.position + new Vector3(x, 0, z);
            
            RaycastHit hit;
            if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out hit, 20f))
            {
                spawnPos.y = hit.point.y;
            }
            
            if (Physics.OverlapSphere(spawnPos, 1f).Length == 0)
            {
                return spawnPos;
            }
        }
        
        float angle2 = Random.Range(0f, 360f);
        float distance2 = Random.Range(deadZoneRadius, spawnRadius);
        float x2 = Mathf.Cos(angle2 * Mathf.Deg2Rad) * distance2;
        float z2 = Mathf.Sin(angle2 * Mathf.Deg2Rad) * distance2;
        
        Vector3 finalPos = player.position + new Vector3(x2, 0, z2);
        
        RaycastHit hit2;
        if (Physics.Raycast(finalPos + Vector3.up * 10f, Vector3.down, out hit2, 20f))
        {
            finalPos.y = hit2.point.y;
        }
        
        return finalPos;
    }
}