using System;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Header("Настройки карты")]
    public int mapWidth = 50;
    public int mapHeight = 50;
    public float tileSize = 2f;
    
    [Header("Префабы тайлов")]
    public GameObject grassPrefab;
    public GameObject dirtPrefab;
    public GameObject stonePrefab;
    
    [Header("Настройки шума")]
    public float noiseScale = 10f;
    public float dirtThreshold = 0.6f;
    public float stoneThreshold = 0.8f;
    


    [Header("Прочее")]

    Camera camera;
    public GameObject mainTower;

    private GameObject levelFolder;
    
    
    private TileData[,] grid;
    
    void Awake()
    {
        levelFolder = new GameObject();
        levelFolder.name = "Level";
        levelFolder.transform.position = Vector3.zero;
        
        camera = Camera.main;
        GenerateMap();
    }


    void SetUpCamera()
    {
        camera.transform.position = new Vector3(mapWidth * tileSize /2, camera.transform.position.y, mapHeight * tileSize /2);
    }

    void PlaceMainTower(Vector3 pos)
    {
        Instantiate(mainTower, pos, Quaternion.identity, transform);
    }
    
    public void GenerateMap()
    {
        ClearMap();
        SetUpCamera();
        grid = new TileData[mapWidth, mapHeight];
        
        float offsetX = Random.Range(0f, 100f);
        float offsetY = Random.Range(0f, 100f);
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float noise = Mathf.PerlinNoise(
                    (x + offsetX) / noiseScale, 
                    (y + offsetY) / noiseScale
                );
                
                TileType type = GetTileType(noise);
                CreateTile(x, y, type, noise);
            }
        }

        PlaceMainTower(new Vector3(mapWidth * tileSize / 2, 0, mapHeight * tileSize / 2));
        
        
        transform.AddComponent<NavMeshSurface>();
        NavMeshSurface ns = transform.GetComponent<NavMeshSurface>();
        ns.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        ns.BuildNavMesh();
    }
    
    void ClearMap()
    {
        if (grid == null) return;
        
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        grid = null;
    }
    
    TileType GetTileType(float noise)
    {
        if (noise > stoneThreshold) return TileType.Stone;
        if (noise > dirtThreshold) return TileType.Dirt;
        return TileType.Grass;
    }
    
    void CreateTile(int x, int y, TileType type, float noise)
    {
        Vector3 pos = new Vector3(x * tileSize, 0, y * tileSize);
        GameObject prefab = GetTilePrefab(type);
        
        if (prefab)
        {
            GameObject tile = Instantiate(prefab, pos, Quaternion.identity, levelFolder.transform);
            
            grid[x, y] = new TileData
            {
                gridPosition = new Vector2Int(x, y),
                type = type,
                currentTileObject = tile,
                noiseValue = noise
            };
        }
    }
    
    GameObject GetTilePrefab(TileType type)
    {
        return type switch
        {
            TileType.Grass => grassPrefab,
            TileType.Dirt => dirtPrefab,
            TileType.Stone => stonePrefab,
            _ => grassPrefab
        };
    }
    
    
    
    public TileData GetTileAtWorldPosition(Vector3 worldPosition)
    {
        if (grid == null) return null;
        
        int x = Mathf.FloorToInt(worldPosition.x / tileSize);
        int y = Mathf.FloorToInt(worldPosition.z / tileSize);
        
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            return grid[x, y];
        
        return null;
    }
}