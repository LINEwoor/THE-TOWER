using System.Collections.Generic;
using UnityEngine;

public class MinerManager : MonoBehaviour
{
    [SerializeField] private GameObject minerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxMiners = 10; 
    [SerializeField] private int startMiners = 3; 
    
    [SerializeField] private List<GameObject> activeMiners = new List<GameObject>();
    [SerializeField] private int currentMinerCount = 0;
    
    void Start()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("Player").transform;
        
        for (int i = 0; i < startMiners; i++)
        {
            SpawnNewMiner();
        }
    }
    
    public bool SpawnNewMiner()
    {
        if (currentMinerCount >= maxMiners)
        {
            return false;
        }
        
        if (minerPrefab == null)
        {
            return false;
        }
        
        GameObject newMiner = Instantiate(minerPrefab, spawnPoint.position  + new Vector3(Random.insideUnitCircle.x * 5,2,Random.insideUnitCircle.y * 5), Quaternion.identity);
        
        activeMiners.Add(newMiner);
        currentMinerCount++;
        
        return true;
    }
    
    public bool RemoveOneMiner()
    {
        if (currentMinerCount <= 0 || activeMiners.Count == 0)
        {
            return false;
        }
        
        GameObject minerToRemove = activeMiners[activeMiners.Count - 1];
        
        activeMiners.RemoveAt(activeMiners.Count - 1);
        currentMinerCount--;
        
        if (minerToRemove != null)
        {
            Destroy(minerToRemove);
        }
        
        return true;
    }
    
    public bool RemoveSpecificMiner(GameObject miner)
    {
        if (miner == null)
        {
            return false;
        }
        
        if (!activeMiners.Contains(miner))
        {
            return false;
        }
        
        activeMiners.Remove(miner);
        currentMinerCount--;
        
        Destroy(miner);
        
        return true;
    }
    
    public void RemoveAllMiners()
    {
        for (int i = activeMiners.Count - 1; i >= 0; i--)
        {
            if (activeMiners[i] != null)
            {
                Destroy(activeMiners[i]);
            }
        }
        
        activeMiners.Clear();
        currentMinerCount = 0;
    }
    
    public int GetCurrentMinerCount()
    {
        return currentMinerCount;
    }
    
    public int GetMaxMinerCount()
    {
        return maxMiners;
    }
    
    public List<GameObject> GetAllMiners()
    {
        return new List<GameObject>(activeMiners);
    }
    
    public bool CanSpawnMore()
    {
        return currentMinerCount < maxMiners;
    }
    
    void Update()
    {
        for (int i = activeMiners.Count - 1; i >= 0; i--)
        {
            if (activeMiners[i] == null)
            {
                activeMiners.RemoveAt(i);
                currentMinerCount--;
            }
        }
    }
    
}