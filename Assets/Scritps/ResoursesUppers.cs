using UnityEngine;

public class ResoursesUppers : MonoBehaviour,IRes
{
    public enum Resourses { Wood, Stone, OilCristall }
    private ResourcesSystem _resourcesSystem;

    [SerializeField] private int[] resUps;
    void Start()
    {
        _resourcesSystem = FindFirstObjectByType<ResourcesSystem>();
    }

    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        ResoursesUppers.Resourses thisRes = (ResoursesUppers.Resourses)other.GetComponent<Res>().ress;
        if (thisRes == Resourses.Wood)
        {
            _resourcesSystem.AddWood(resUps[0]);
        }
        if (thisRes == Resourses.Stone)
        {
            _resourcesSystem.AddStone(resUps[1]);
        }
        if (thisRes == Resourses.OilCristall)
        {
            _resourcesSystem.AddCristall(resUps[2]);
        }
        Destroy(other.gameObject);
    }
}
