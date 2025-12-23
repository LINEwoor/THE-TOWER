using UnityEngine;

public class ResGeneration : MonoBehaviour
{
    [SerializeField] private GameObject resPrefabs;
    [SerializeField] private float Tiner = 0;
    [SerializeField] private float UseTime = 3f;
    [SerializeField] private int RadiosGenerate = 5;

    void Start()
    {
        
    }
    void Generate()
    {
        if(Tiner <= 0)
        {   
            GenerateRes(resPrefabs,RadiosGenerate);
            Tiner = UseTime;
        }
        else
        {
            Tiner -= Time.deltaTime;
        }
    }

    void GenerateRes(GameObject rp, int radios)
    {
        //float x = Random.Range(this.transform.position.x - radios, +radios);
        Vector2 r = Random.insideUnitCircle * radios;
        Vector3 Gen = new Vector3(transform.position.x+r.x,transform.position.y,transform.position.z+r.y);
        GameObject res = Instantiate(rp,Gen,Quaternion.identity);

    }

    
    void Update()
    {
        Generate();
    }
}
