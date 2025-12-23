using UnityEngine;

public class ResourcesSystem : MonoBehaviour
{
    [SerializeField] private int wood;
    [SerializeField] private int stone;
    [SerializeField] private int oilCristall;
    void Start()
    {
        
    }

    public void AddWood( int w)
    {
        wood += w;
    }
    public void AddStone(int s)
    {
        stone += s;
    }
    public void AddCristall(int oc)
    {
        oilCristall += oc;
    }

    public void SubWood(int w)
    {
        wood -= w;
    }
    public void SubStone(int s)
    {
        stone -= s;
    }
    public void SybCristall(int oc)
    {
        oilCristall -= oc;
    }


    void Update()
    {
        
    }
}
