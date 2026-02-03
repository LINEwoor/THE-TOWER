using UnityEngine;
using UnityEngine.EventSystems;

public class PlatformTower : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject panelBuild;

    [SerializeField] private bool ispanelActive = false;
    [SerializeField] private bool isbuild = false;
    [SerializeField] private MeshRenderer mr;

    [SerializeField] private Material act;
    [SerializeField] private Material NoAct;

    private ResourcesSystem rs;
    
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        rs = FindFirstObjectByType<ResourcesSystem>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.CompareTag("Platform"))
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ActiveDesckTowerBuild();
            }
        }
        
    }

    public void ActiveDesckTowerBuild()
    {
        //Debug.Log("p");
        if(!isbuild)
        {
            if (ispanelActive)
            {
                panelBuild.SetActive(false);
                ispanelActive = false;
                mr.material = NoAct;
            }
            else
            {
                panelBuild.SetActive(true);
                ispanelActive = true;
                mr.material = act;
            }
        }
        
    }

    public void BuildTower()
    {
        if (!isbuild && ispanelActive)
        {
            Instantiate(tower,this.transform.position,this.transform.rotation);
            panelBuild.SetActive(false);
            ispanelActive = false;
            isbuild = true;
        }
        
    }
    void Update()
    {
        
    }
}
