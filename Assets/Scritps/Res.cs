using UnityEngine;

public class Res : MonoBehaviour, IRes
{
    public enum Resourses { Wood, Stone, OilCristall, None }
    public Resourses ress;
    public Renderer rd;
    
    [SerializeField] private int resourceValue = 10;
    
    void Start()
    {
        rd = GetComponent<Renderer>();
        GenerateRandomResource();
    }
    
    private void GenerateRandomResource()
    {
        int i = Random.Range(0, 3);
        
        switch (i)
        {
            case 0:
                ress = Resourses.Wood;
                rd.material.color = new Color(0.65f, 0.46f, 0.16f);
                break;
            case 1:
                ress = Resourses.Stone;
                rd.material.color = new Color(0.5f, 0.5f, 0.5f);
                break;
   
        }
        
        Color randomVariance = new Color(
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f)
        );
        rd.material.color += randomVariance;
    }
    
    public Resourses GetResourceType()
    {
        return ress;
    }
    
    public int GetResourceValue()
    {
        return resourceValue;
    }
    
    public void SetupResource(Resourses type, int value = 10)
    {
        ress = type;
        resourceValue = value;
        
        if (rd == null) rd = GetComponent<Renderer>();
        
    }
}