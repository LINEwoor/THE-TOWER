using UnityEngine;

public class ResoursesUppers : MonoBehaviour,IRes
{
    public enum Resourses { Wood, Stone, OilCristall, None }
    private ResourcesSystem _resourcesSystem;
    public ResoursesUppers.Resourses thisRes;

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
        

        if (other.tag == "Resourses")
        {
            thisRes = (ResoursesUppers.Resourses)other.GetComponent<Res>().ress;
            other.tag = "Untagged";
            other.gameObject.SetActive(false);
        }
        
    }
}
