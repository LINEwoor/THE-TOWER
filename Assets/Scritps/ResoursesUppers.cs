using UnityEngine;

public class ResoursesUppers : MonoBehaviour, IRes
{
    public enum Resourses { Wood, Stone, OilCristall, None }
    private ResourcesSystem _resourcesSystem;
    public Resourses thisRes;
    
    [SerializeField] private int resourceAmount = 10;
    [SerializeField] private GameObject carriedResource;

    void Start()
    {
        _resourcesSystem = FindFirstObjectByType<ResourcesSystem>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Resourses")
        {
            Res resComponent = other.GetComponent<Res>();
            if (resComponent != null)
            {
                thisRes = (Resourses)resComponent.ress;
                other.tag = "Untagged";
                other.gameObject.SetActive(false);
                
                if (carriedResource != null)
                {
                    carriedResource.SetActive(true);
                    Renderer rend = carriedResource.GetComponent<Renderer>();
   
                }
            }
        }
        
        if (other.tag == "Player" && thisRes != Resourses.None)
        {
            DeliverResources();
            thisRes = Resourses.None;
            
            if (carriedResource != null)
            {
                carriedResource.SetActive(false);
            }
        }
    }
    
    private void DeliverResources()
    {
        if (_resourcesSystem == null) return;
        
        switch (thisRes)
        {
            case Resourses.Wood:
                _resourcesSystem.AddWood(resourceAmount);
                break;
            case Resourses.Stone:
                _resourcesSystem.AddStone(resourceAmount);
                break;
            case Resourses.OilCristall:
                _resourcesSystem.AddCristall(resourceAmount);
                break;
        }
        
    }
    
    
    public bool IsCarryingResource()
    {
        return thisRes != Resourses.None;
    }
    
    public Resourses GetCarriedResourceType()
    {
        return thisRes;
    }
}