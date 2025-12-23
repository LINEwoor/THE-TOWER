using UnityEngine;

public class ResGeneration : MonoBehaviour
{
    [SerializeField] private GameObject resPrefabs;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float currentTimer = 0f;
    
    [SerializeField] private float minSpawnRadius = 3f;
    [SerializeField] private float maxSpawnRadius = 10f;
    
    [SerializeField] private int maxTotalResources = 20;
    [SerializeField] private int currentResourcesCount = 0;
    
    [SerializeField] private bool spawnAroundPlayer = true;
    [SerializeField] private Transform spawnCenter;
    
    private Transform player;
    private Transform resourcesContainer;

    void Start()
    {
        if (spawnAroundPlayer)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                spawnAroundPlayer = false;
            }
        }
        
        GameObject container = new GameObject("GeneratedResources");
        resourcesContainer = container.transform;
        
        currentTimer = spawnInterval;
    }
    
    void Generate()
    {
        if(currentTimer <= 0 && currentResourcesCount < maxTotalResources)
        {   
            GenerateRes(resPrefabs);
            currentTimer = spawnInterval;
        }
        else
        {
            currentTimer -= Time.deltaTime;
        }
    }

    void GenerateRes(GameObject rp)
    {
        Vector3 centerPoint;
        
        if (spawnAroundPlayer && player != null)
        {
            centerPoint = player.position;
        }
        else if (spawnCenter != null)
        {
            centerPoint = spawnCenter.position;
        }
        else
        {
            centerPoint = transform.position;
        }
        
        Vector3 spawnPosition = GetValidSpawnPosition(centerPoint);
        
        GameObject newResource = Instantiate(rp, spawnPosition + Vector3.up, Quaternion.identity);
        newResource.transform.parent = resourcesContainer;
        
        currentResourcesCount++;
        
        ResourceTracker tracker = newResource.AddComponent<ResourceTracker>();
        tracker.SetGenerator(this);
    }
    
    Vector3 GetValidSpawnPosition(Vector3 center)
    {
        Vector2 randomCircle;
        float distance;
        
        do
        {
            randomCircle = Random.insideUnitCircle.normalized;
            distance = Random.Range(minSpawnRadius, maxSpawnRadius);
        } 
        while (distance < minSpawnRadius);
        
        Vector3 spawnPos = new Vector3(
            center.x + randomCircle.x * distance,
            center.y,
            center.z + randomCircle.y * distance
        );
        
        if (CheckCollisionAtPosition(spawnPos))
        {
            return GetValidSpawnPosition(center);
        }
        
        return spawnPos;
    }
    
    bool CheckCollisionAtPosition(Vector3 position)
    {
        float checkRadius = 1.0f;
        Collider[] colliders = Physics.OverlapSphere(position, checkRadius);
        
        foreach (Collider collider in colliders)
        {
            if (collider.isTrigger) continue;
            if (collider.gameObject.layer == LayerMask.NameToLayer("Ground")) continue;
            
            if (collider.gameObject.CompareTag("Resourses") || 
                collider.gameObject.GetComponent<Res>() != null)
            {
                return true;
            }
        }
        
        return false;
    }
    
    public void ResourceCollectedOrDestroyed()
    {
        if (currentResourcesCount > 0)
        {
            currentResourcesCount--;
        }
    }
    
    public bool TryGenerateResource()
    {
        if (currentResourcesCount < maxTotalResources)
        {
            GenerateRes(resPrefabs);
            return true;
        }
        return false;
    }
    
    public int GetCurrentResourceCount() => currentResourcesCount;
    public int GetMaxResourceCount() => maxTotalResources;
    public float GetSpawnProgress() => (float)currentResourcesCount / maxTotalResources;

    void Update()
    {
        Generate();
    }



    public class ResourceTracker : MonoBehaviour
    {
        private ResGeneration generator;

        public void SetGenerator(ResGeneration gen)
        {
            generator = gen;
        }

        void OnDestroy()
        {
            if (generator != null && gameObject.scene.isLoaded)
            {
                generator.ResourceCollectedOrDestroyed();
            }
        }
    }
}