using UnityEngine;

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
    
    [Header("Декорации")]
    public Decoration[] decorations;

    [Header("Прочее")]

    public Camera camera;
    public GameObject mainTower;
    
    
    [System.Serializable]
    public class Decoration
    {
        public GameObject prefab;
        public TileType[] allowedTiles;
        [Range(0f, 1f)] public float minNoise = 0f;
        [Range(0f, 1f)] public float maxNoise = 1f;
        [Range(0f, 1f)] public float density = 0.1f;
    }
    
    private TileData[,] grid;
    
    void Awake()
    {
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

        PlaceMainTower(new Vector3(mapWidth * tileSize / 2, 1, mapHeight * tileSize / 2));
        PlaceDecorations();
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
            GameObject tile = Instantiate(prefab, pos, Quaternion.identity, transform);
            
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
    
    void PlaceDecorations()
    {
        if (decorations == null || decorations.Length == 0) return;
        
        foreach (Decoration decor in decorations)
        {
            if (decor.prefab == null) continue;
            
            float decorOffsetX = Random.Range(0f, 100f);
            float decorOffsetY = Random.Range(0f, 100f);
            
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    TileData tile = grid[x, y];
                    
                    if (!CanPlaceDecoration(tile, decor)) continue;
                    
                    float decorNoise = Mathf.PerlinNoise(
                        (x + decorOffsetX) * 5f, 
                        (y + decorOffsetY) * 5f
                    );
                    
                    if (decorNoise < decor.density)
                    {
                        PlaceSingleDecoration(tile, decor);
                    }
                }
            }
        }
    }
    
    bool CanPlaceDecoration(TileData tile, Decoration decor)
    {
        if (decor.allowedTiles != null && decor.allowedTiles.Length > 0)
        {
            bool allowed = false;
            foreach (TileType allowedType in decor.allowedTiles)
            {
                if (tile.type == allowedType)
                {
                    allowed = true;
                    break;
                }
            }
            if (!allowed) return false;
        }
        
        return tile.noiseValue >= decor.minNoise && tile.noiseValue <= decor.maxNoise;
    }
    
    void PlaceSingleDecoration(TileData tile, Decoration decor)
    {
        Vector3 tileCenter = new Vector3(
            tile.gridPosition.x * tileSize,
            0,
            tile.gridPosition.y * tileSize
        );
        
        float offsetX = Random.Range(-tileSize * 0.3f, tileSize * 0.3f);
        float offsetZ = Random.Range(-tileSize * 0.3f, tileSize * 0.3f);
        
        Vector3 pos = tileCenter + new Vector3(offsetX, 0, offsetZ);
        Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        
        Instantiate(decor.prefab, pos, rot, tile.currentTileObject.transform);
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